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
            internal bool isActive, isStageLock, isCurseLock, isHeld;
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
                    buffIndex = BuffIndex.AffixRed
                },
                new AffixTracker //Lightning
                {
                    eliteIndex = EliteIndex.Lightning,
                    buffIndex = BuffIndex.AffixBlue
                },
                new AffixTracker //Ice
                {
                    eliteIndex = EliteIndex.Ice,
                    buffIndex = BuffIndex.AffixWhite
                },
                new AffixTracker //Poison
                {
                    eliteIndex = EliteIndex.Poison,
                    buffIndex = BuffIndex.AffixPoison
                },
                new AffixTracker //Ghost
                {
                    eliteIndex = EliteIndex.Haunted,
                    buffIndex = BuffIndex.AffixHaunted
                }
            };
            trackerBody = gameObject.GetComponent<CharacterBody>();
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeCurseDamage;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private void Update()
        {
            int tempCurseCount = 0;
            foreach (AffixTracker tracker in affixTrackers)
            {
                //Check to see if the buff is active
                if(trackerBody.HasBuff(tracker.buffIndex))
                {
                    tracker.isActive = true;
                }

                //Calculate the current curse count
                if (tracker.isCurseLock)
                {
                    //If its cursed and neither staged nor held, add a curse
                    if(!tracker.isHeld && !tracker.isStageLock)
                    {
                        tempCurseCount++;
                    }
                }

                //Update curse count if its changed
                if(tempCurseCount != curseCount)
                {
                    curseCount = tempCurseCount;
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
            if(self.characterBody == trackerBody)
            {
                //BaseEquip
                //LunarEquip
            }
            return orig(self, equipmentIndex);
        }
    }
}
