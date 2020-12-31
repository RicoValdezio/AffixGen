using RoR2;
using System;

namespace AffixGen
{
    public class AffixTracker : ICloneable
    {
        public BuffIndex buffIndex;
        public EquipmentIndex equipmentIndex;
        public bool isStageLock, isCurseLock, isHeld, isVultured;
        public float vultureTimeLeft;
        public Func<bool> isViable;
        public string affixNameTag;

        /// <summary>
        /// Creates a AffixTracker for a specific elite type
        /// </summary>
        /// <param name="buffIdx">The BuffIndex used to determine target elite type, and is granted to user on activation.</param>
        /// <param name="equipIdx">The EquipmentIndex that passively grants the BuffIndex, used to offset the curse.</param>
        /// <param name="viablityFunction">The boolean function used to determine if the buff can be granted by the basic equipment. If you use ESO, this should match the isAvailable function.</param>
        /// <param name="name">The display name used to notify the user/log of which Tracker was activated.</param>
        public AffixTracker(BuffIndex buffIdx, EquipmentIndex? equipIdx, Func<bool> viablityFunction, string name)
        {
            buffIndex = buffIdx;
            equipmentIndex = equipIdx ?? EquipmentIndex.None; //Eventually stand up a null Equipment to be used if not given.
            isViable = viablityFunction;
            affixNameTag = name;

            isStageLock = false;
            isCurseLock = false;
            isHeld = false;
            isVultured = false;
            vultureTimeLeft = 0f;
        }

        /// <summary>
        /// Creates a AffixTracker for a specific elite type using ESO tier pattern
        /// </summary>
        /// <param name="buffIdx">The BuffIndex used to determine target elite type, and is granted to user on activation.</param>
        /// <param name="equipIdx">The EquipmentIndex that passively grants the BuffIndex, used to offset the curse.</param>
        /// <param name="tier">The ESO tier of the elite type, automatically creates viabilityFunction like ESO.</param>
        /// <param name="name">The display name used to notify the user/log of which Tracker was activated.</param>
        public AffixTracker(BuffIndex buffIdx, EquipmentIndex? equipIdx, int tier, string name)
        {
            buffIndex = buffIdx;
            equipmentIndex = equipIdx ?? EquipmentIndex.None; //Eventually stand up a null Equipment to be used if not given.
            isViable = () => Run.instance.loopClearCount > (tier - 1);
            affixNameTag = name;

            isStageLock = false;
            isCurseLock = false;
            isHeld = false;
            isVultured = false;
            vultureTimeLeft = 0f;
        }

        public object Clone()
        {
            return new AffixTracker(buffIndex, equipmentIndex, isViable, affixNameTag); ;
        }
    }
}
