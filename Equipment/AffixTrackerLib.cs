using RoR2;
using System.Collections.Generic;

namespace AffixGen
{
    public class AffixTrackerLib
    {
        public static List<AffixTracker> affixTrackers;

        public static void Init()
        {
            affixTrackers = new List<AffixTracker>
            {
                new AffixTracker(BuffIndex.AffixRed, EquipmentIndex.AffixRed, () => true, "Fire"),
                new AffixTracker(BuffIndex.AffixBlue, EquipmentIndex.AffixBlue, () => true, "Lightning"),
                new AffixTracker(BuffIndex.AffixWhite, EquipmentIndex.AffixWhite, () => true, "Ice"),
                new AffixTracker(BuffIndex.AffixPoison, EquipmentIndex.AffixPoison, () => Run.instance.loopClearCount > 0, "Poison"),
                new AffixTracker(BuffIndex.AffixHaunted, EquipmentIndex.AffixHaunted, () => Run.instance.loopClearCount > 0, "Haunting")
            };
        }
    }
}
