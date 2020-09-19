using RoR2;
using System.Collections.Generic;
using UnityEngine;
using R2API.Networking;

namespace AffixGen
{
    class AffixEquipBehaviour : MonoBehaviour
    {
        private class AffixTracker
        {
            internal EliteIndex eliteIndex;
            internal BuffIndex buffIndex;
            internal EquipmentIndex equipmentIndex;
            internal bool isStageLock, isCurseLock, isHeld, isVultured;
            internal float vultureTimeLeft;
            internal int loopsRequired;
            internal string affixNameTag;
        }

        private List<AffixTracker> affixTrackers;
        internal int curseCount;
        internal static float curseMultiplier;
        private CharacterMaster trackerMaster;
        private CharacterBody trackerBody;
        private BuffIndex mostRecentAttackIndex;
        internal static int maxStageLocks;
        internal int currentStageLocks;

        private void OnEnable()
        {
            affixTrackers = new List<AffixTracker>()
            {
                new AffixTracker //Fire
                {
                    eliteIndex = EliteIndex.Fire,
                    buffIndex = BuffIndex.AffixRed,
                    equipmentIndex = EquipmentIndex.AffixRed,
                    affixNameTag = "Fire"
                },
                new AffixTracker //Lightning
                {
                    eliteIndex = EliteIndex.Lightning,
                    buffIndex = BuffIndex.AffixBlue,
                    equipmentIndex = EquipmentIndex.AffixBlue,
                    affixNameTag = "Lightning"
                },
                new AffixTracker //Ice
                {
                    eliteIndex = EliteIndex.Ice,
                    buffIndex = BuffIndex.AffixWhite,
                    equipmentIndex = EquipmentIndex.AffixWhite,
                    affixNameTag = "Ice"
                },
                new AffixTracker //Poison
                {
                    eliteIndex = EliteIndex.Poison,
                    buffIndex = BuffIndex.AffixPoison,
                    equipmentIndex = EquipmentIndex.AffixPoison,
                    loopsRequired = 1,
                    affixNameTag = "Poison"
                },
                new AffixTracker //Ghost
                {
                    eliteIndex = EliteIndex.Haunted,
                    buffIndex = BuffIndex.AffixHaunted,
                    equipmentIndex = EquipmentIndex.AffixHaunted,
                    loopsRequired = 1,
                    affixNameTag = "Haunting"
                }
            };
            ShuffleTrackers();
            trackerMaster = gameObject.GetComponent<CharacterMaster>();
            curseCount = 0;
            currentStageLocks = 0;

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
        }

        private void Update()
        {
            if (gameObject.GetComponent<CharacterMaster>())
            {
                trackerMaster = gameObject.GetComponent<CharacterMaster>();
                if (trackerMaster.GetBody() != null)
                {
                    trackerBody = trackerMaster.GetBody();
                    int tempCurseCount = 0;

                    foreach (AffixTracker tracker in affixTrackers)
                    {
                        //If the buff is StageLock or CurseLock, give it to me
                        if (tracker.isCurseLock || tracker.isStageLock)
                        {
                            //trackerBody.AddBuff(tracker.buffIndex);
                            trackerBody.ApplyBuff(tracker.buffIndex, 1);
                        }
                        //If neither Lock, and also not Held nor Vulture, take it away
                        else if (!tracker.isHeld && !tracker.isVultured)
                        {
                            //trackerBody.RemoveBuff(tracker.buffIndex);
                            trackerBody.ApplyBuff(tracker.buffIndex, 0);
                        }

                        //Check is the buff is currently from Wake of Vultures
                        if (tracker.isVultured)
                        {
                            //Reduce the amount of time and remove flag if no time left
                            tracker.vultureTimeLeft -= Time.deltaTime;
                            if (tracker.vultureTimeLeft <= 0)
                            {
                                tracker.isVultured = false;
                            }
                        }

                        //Calculate the current curse count
                        if (tracker.isCurseLock)
                        {
                            //If its cursed and neither staged nor held nor vultured, add a curse
                            if (!tracker.isHeld && !tracker.isStageLock && !tracker.isVultured)
                            {
                                tempCurseCount++;
                            }
                        }
                    }
                    //Update curse count if its changed
                    if (tempCurseCount != curseCount)
                    {
                        curseCount = tempCurseCount;
                        //Chat.AddMessage("Current Curse Level is: " + curseCount.ToString());
                    }
                }
            }
        }

        private void ShuffleTrackers()
        {
            for (int i = 0; i < affixTrackers.Count; i++)
            {
                AffixTracker temp = affixTrackers[i];
                int randomIndex = Random.Range(i, affixTrackers.Count);
                affixTrackers[i] = affixTrackers[randomIndex];
                affixTrackers[randomIndex] = temp;
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            //Inject the extra damage if there's curses
            if (self.body == trackerBody)
            {
                damageInfo.damage *= 1 + (curseMultiplier * curseCount);

                //Try and capture the affix type of the attacker/inflictor
                foreach (AffixTracker tracker in affixTrackers)
                {
                    CharacterBody testBody = null;
                    if (damageInfo.attacker)
                    {
                        testBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    }
                    else if (damageInfo.inflictor)
                    {
                        testBody = damageInfo.inflictor.GetComponent<CharacterBody>();
                    }

                    //Set the curse flag if its a match
                    if (testBody && testBody.isElite && testBody.HasBuff(tracker.buffIndex))
                    {
                        mostRecentAttackIndex = tracker.buffIndex;
                        //Chat.AddMessage("Most Recent Damage Type Was : " + tracker.affixNameTag);
                    }
                }
            }
            orig(self, damageInfo);
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            if (self.characterBody == trackerBody)
            {
                //BaseEquip
                if (equipmentIndex == BaseAffixEquip.index)
                {
                    //If there's an affix left to give this stage, give it and destroy the equipment
                    foreach (AffixTracker tracker in affixTrackers)
                    {
                        if (!tracker.isStageLock && tracker.loopsRequired <= Run.instance.loopClearCount && currentStageLocks < maxStageLocks)
                        {
                            tracker.isStageLock = true;
                            trackerBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
                            //trackerBody.AddBuff(tracker.buffIndex);
                            //trackerBody.ApplyBuff(tracker.buffIndex, 1);
                            currentStageLocks++;
                            return false;
                        }
                    }
                    ShuffleTrackers();
                    //Else do nothing and keep the equipment
                    return true;
                }
                //LunarEquip
                if (equipmentIndex == LunarAffixEquip.index)
                {
                    //If the most recent affix was one you don't have yet, add it
                    foreach (AffixTracker tracker in affixTrackers)
                    {
                        if (tracker.buffIndex == mostRecentAttackIndex)
                        {
                            tracker.isCurseLock = true;
                            //trackerBody.AddBuff(tracker.buffIndex);
                            //trackerBody.ApplyBuff(tracker.buffIndex, 1);
                            return true;
                        }
                    }
                    //Else do nothing and keep the equipment
                    return true;
                }
            }
            return orig(self, equipmentIndex);
        }

        private void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            if (self == trackerBody)
            {
                foreach (AffixTracker tracker in affixTrackers)
                {
                    //If the timed buff is an affix, add to the vultureTimeLeft
                    if (buffType == tracker.buffIndex)
                    {
                        tracker.vultureTimeLeft += duration;
                        tracker.isVultured = true;
                    }
                }
            }
            orig(self, buffType, duration);
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            //On a new stage, reset all relevant fields
            foreach (AffixTracker tracker in affixTrackers)
            {
                tracker.isStageLock = false;
                tracker.isVultured = false;
                tracker.vultureTimeLeft = 0f;
            }
            currentStageLocks = 0;
            orig(self);
        }

        private void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (self == trackerBody)
            {
                foreach (AffixTracker tracker in affixTrackers)
                {
                    //If I have the elite equip, set isHeld to true
                    if (equipmentDef.equipmentIndex == tracker.equipmentIndex)
                    {
                        tracker.isHeld = true;
                    }
                }
            }
            orig(self, equipmentDef);
        }

        private void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (self == trackerBody)
            {
                foreach (AffixTracker tracker in affixTrackers)
                {
                    //If I had the elite equip, set isHeld to false
                    if (equipmentDef.equipmentIndex == tracker.equipmentIndex)
                    {
                        tracker.isHeld = false;
                    }
                }
            }
            orig(self, equipmentDef);
        }
    }
}
