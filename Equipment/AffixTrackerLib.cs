using RoR2;
using System.Collections.Generic;

namespace AffixGen
{
    public class AffixTrackerLib
    {
        public static List<AffixTracker> affixTrackers;

        internal static void Init()
        {
            affixTrackers = new List<AffixTracker>
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
        }
    }
}
