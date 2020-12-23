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
                new AffixTracker(BuffIndex.AffixRed, EquipmentIndex.AffixRed, 0, "Fire"),
                new AffixTracker(BuffIndex.AffixBlue, EquipmentIndex.AffixBlue, 0, "Lightning"),
                new AffixTracker(BuffIndex.AffixWhite, EquipmentIndex.AffixWhite, 0, "Ice"),
                new AffixTracker(BuffIndex.AffixPoison, EquipmentIndex.AffixPoison, 1, "Poison"),
                new AffixTracker(BuffIndex.AffixHaunted, EquipmentIndex.AffixHaunted, 1, "Haunting")
            };
        }
    }
}
