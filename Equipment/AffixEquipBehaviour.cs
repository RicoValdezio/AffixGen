using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace AffixGen
{
    class AffixEquipBehaviour : MonoBehaviour
    {
        private class AffixTracker
        {
            internal EliteIndex eliteIndex;
            internal BuffIndex buffIndex;
            internal EquipmentIndex equipmentIndex;
            internal bool isActive, isStageLock, isCurseLock, isHeld, isVultured;
            internal float vultureTimeLeft;
            internal int loopsRequired;
        }

        private List<AffixTracker> affixTrackers;
        internal int curseCount;
        internal static float curseMultiplier;
        private CharacterBody trackerBody;
        private BuffIndex mostRecentAttackIndex;

        private void OnEnable()
        {
            affixTrackers = new List<AffixTracker>()
            {
                new AffixTracker //Fire
                {
                    eliteIndex = EliteIndex.Fire,
                    buffIndex = BuffIndex.AffixRed,
                    equipmentIndex = EquipmentIndex.AffixRed
                },
                new AffixTracker //Lightning
                {
                    eliteIndex = EliteIndex.Lightning,
                    buffIndex = BuffIndex.AffixBlue,
                    equipmentIndex = EquipmentIndex.AffixBlue
                },
                new AffixTracker //Ice
                {
                    eliteIndex = EliteIndex.Ice,
                    buffIndex = BuffIndex.AffixWhite,
                    equipmentIndex = EquipmentIndex.AffixWhite
                },
                new AffixTracker //Poison
                {
                    eliteIndex = EliteIndex.Poison,
                    buffIndex = BuffIndex.AffixPoison,
                    equipmentIndex = EquipmentIndex.AffixPoison,
                    loopsRequired = 1
                },
                new AffixTracker //Ghost
                {
                    eliteIndex = EliteIndex.Haunted,
                    buffIndex = BuffIndex.AffixHaunted,
                    equipmentIndex = EquipmentIndex.AffixHaunted,
                    loopsRequired = 1
                }
            };
            trackerBody = gameObject.GetComponent<CharacterBody>();
            curseCount = 0;

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Update()
        {
            int tempCurseCount = 0;
            foreach (AffixTracker tracker in affixTrackers)
            {
                //Check to see if the buff is active
                if (trackerBody.HasBuff(tracker.buffIndex))
                {
                    tracker.isActive = true;
                }
                //If it's not active, see if it should be and set it (This is the ONLY place that buffs are given!)
                else if((tracker.isCurseLock || tracker.isStageLock || tracker.isVultured || tracker.isHeld) && !tracker.isActive)
                {
                    trackerBody.AddBuff(tracker.buffIndex);
                }
                //If it shouldn't be active and isn't, set Active to false
                else
                {
                    tracker.isActive = false;
                }

                //Check if the buff is currently from held item
                if (trackerBody.equipmentSlot.equipmentIndex == tracker.equipmentIndex)
                {
                    tracker.isHeld = true;
                }
                else
                {
                    tracker.isHeld = false;
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

                //Update curse count if its changed
                if (tempCurseCount != curseCount)
                {
                    curseCount = tempCurseCount;
                    //Post the curse level to chat (will be removed/replaced with a item/buff)
                    Chat.AddMessage("Current Curse Level is: " + curseCount.ToString());
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            //Inject the extra damage if there's curses
            if (self.body == trackerBody)
            {
                damageInfo.damage *= 1 + (curseMultiplier * curseCount);
            }

            //Capture the most recent affix if it was an elite (only the first, just in case)
            if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>())
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody.isElite)
                {
                    foreach(AffixTracker tracker in affixTrackers)
                    {
                        if (attackerBody.HasBuff(tracker.buffIndex))
                        {
                            mostRecentAttackIndex = tracker.buffIndex;
                            break;
                        }
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
                if(equipmentIndex == BaseAffixEquip.index)
                {
                    //If there's an affix left to give this stage, give it and destroy the equipment
                    foreach(AffixTracker tracker in affixTrackers)
                    {
                        if (!tracker.isStageLock && tracker.loopsRequired <= Run.instance.loopClearCount)
                        {
                            tracker.isStageLock = true;
                            trackerBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
                            return true;
                        }
                    }
                    //Else do nothing and keep the equipment
                    return true;
                }
                //LunarEquip
                if(equipmentIndex == LunarAffixEquip.index)
                {
                    //If the most recent affix was one you don't have yet, add it
                    foreach (AffixTracker tracker in affixTrackers)
                    {
                        if (!tracker.isCurseLock && tracker.buffIndex == mostRecentAttackIndex)
                        {
                            tracker.isCurseLock = true;
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
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            foreach (AffixTracker tracker in affixTrackers)
            {
                tracker.isStageLock = false;
            }
        }
    }
}
