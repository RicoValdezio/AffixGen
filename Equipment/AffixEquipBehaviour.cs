using RoR2;
using System.Collections.Generic;
using UnityEngine;
using R2API.Networking;

namespace AffixGen
{
    internal class AffixEquipBehaviour : MonoBehaviour
    {
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
            affixTrackers = new List<AffixTracker>();
            foreach(AffixTracker tracker in AffixTrackerLib.affixTrackers)
            {
                affixTrackers.Add((AffixTracker)tracker.Clone());
            }
            ShuffleTrackers();
            trackerMaster = gameObject.GetComponent<CharacterMaster>();
            curseCount = 0;
            currentStageLocks = 0;
            AffixGenPlugin.activeBehaviours.Add(this);
        }

        private void OnDisable()
        {
            AffixGenPlugin.activeBehaviours.Remove(this);
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

        internal float CalculateNewDamage(float damage, GameObject damageDealer)
        {
            damage *= 1 + (curseMultiplier * curseCount);

            //Try and capture the affix type of the attacker/inflictor
            foreach (AffixTracker tracker in affixTrackers)
            {
                CharacterBody testBody  = damageDealer.GetComponent<CharacterBody>();

                //Set the curse flag if its a match
                if (testBody && testBody.isElite && testBody.HasBuff(tracker.buffIndex))
                {
                    mostRecentAttackIndex = tracker.buffIndex;
                    //Chat.AddMessage("Most Recent Damage Type Was : " + tracker.affixNameTag);
                }
            }
            return damage;
        }

        internal bool PerformBaseAction()
        {
            //If there's an affix left to give this stage, give it and destroy the equipment
            foreach (AffixTracker tracker in affixTrackers)
            {
                if (!tracker.isStageLock && tracker.loopsRequired <= Run.instance.loopClearCount && currentStageLocks < maxStageLocks)
                {
                    tracker.isStageLock = true;
                    trackerBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    currentStageLocks++;
                    return false;
                }
            }
            ShuffleTrackers();
            //Else do nothing and keep the equipment
            return true;
        }

        internal bool PerformLunarAction()
        {
            //If the most recent affix was one you don't have yet, add it
            foreach (AffixTracker tracker in affixTrackers)
            {
                if (tracker.buffIndex == mostRecentAttackIndex)
                {
                    tracker.isCurseLock = true;
                    return true;
                }
            }
            //Else do nothing and keep the equipment
            return true;
        }

        internal void UpdateVultures(BuffIndex buffType, float duration)
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

        internal void ResetStage()
        {
            //On a new stage, reset all relevant fields
            foreach (AffixTracker tracker in affixTrackers)
            {
                tracker.isStageLock = false;
                tracker.isVultured = false;
                tracker.vultureTimeLeft = 0f;
            }
            currentStageLocks = 0;
        }

        internal void UpdateEquipment(EquipmentIndex gainedEquipmentIndex, bool wasGained)
        {
            foreach (AffixTracker tracker in affixTrackers)
            {
                //If I have the elite equip, set isHeld to true
                if (gainedEquipmentIndex == tracker.equipmentIndex)
                {
                    tracker.isHeld = wasGained;
                    break;
                }
            }
        }
    }
}
