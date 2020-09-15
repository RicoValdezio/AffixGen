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
        }

        private List<AffixTracker> affixTrackers;
        internal int curseCount;
        internal float curseMultiplier;
        private CharacterBody trackerBody;

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
                    equipmentIndex = EquipmentIndex.AffixPoison
                },
                new AffixTracker //Ghost
                {
                    eliteIndex = EliteIndex.Haunted,
                    buffIndex = BuffIndex.AffixHaunted,
                    equipmentIndex = EquipmentIndex.AffixHaunted
                }
            };
            trackerBody = gameObject.GetComponent<CharacterBody>();
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeCurseDamage;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
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

        private void HealthComponent_TakeCurseDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            //Inject the extra damage if there's curses
            if (self.body == trackerBody)
            {
                damageInfo.damage *= 1 + (curseMultiplier * curseCount);
            }
            orig(self, damageInfo);
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            if (self.characterBody == trackerBody)
            {
                //BaseEquip
                //LunarEquip
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
    }
}
