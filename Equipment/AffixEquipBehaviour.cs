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
            internal bool isActive, isStageLock, isCurseLock;
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
                    isActive = false,
                    isStageLock = false,
                    isCurseLock = false
                },
                new AffixTracker //Lightning
                {
                    eliteIndex = EliteIndex.Lightning,
                    buffIndex = BuffIndex.AffixBlue,
                    isActive = false,
                    isStageLock = false,
                    isCurseLock = false
                },
                new AffixTracker //Ice
                {
                    eliteIndex = EliteIndex.Ice,
                    buffIndex = BuffIndex.AffixWhite,
                    isActive = false,
                    isStageLock = false,
                    isCurseLock = false
                },
                new AffixTracker //Poison
                {
                    eliteIndex = EliteIndex.Poison,
                    buffIndex = BuffIndex.AffixPoison,
                    isActive = false,
                    isStageLock = false,
                    isCurseLock = false
                },
                new AffixTracker //Ghost
                {
                    eliteIndex = EliteIndex.Haunted,
                    buffIndex = BuffIndex.AffixHaunted,
                    isActive = false,
                    isStageLock = false,
                    isCurseLock = false
                }
            };
            trackerBody = gameObject.GetComponent<CharacterBody>();
        }

        private void Update()
        {
            foreach (AffixTracker tracker in affixTrackers)
            {
                //Check to see if the buff is active
                if(tracker.isCurseLock || tracker.isStageLock || trackerBody.HasBuff(tracker.buffIndex))
                {
                    tracker.isActive = true;
                }
            }
        }
    }
}
