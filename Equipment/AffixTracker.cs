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
        public int loopsRequired;
        public string affixNameTag;

        /// <summary>
        /// Creates a AffixTracker for a specific elite type
        /// </summary>
        /// <param name="buffIdx">The BuffIndex used to determine target elite type, and is granted to user on activation.</param>
        /// <param name="equipIdx">The EquipmentIndex that passively grants the BuffIndex, used to offset the curse.</param>
        /// <param name="cyclesNeeded">The number of times the user must clear the 5th stage before allowing the buff to be picked.</param>
        /// <param name="name">The display name used to notify the user/log of which Tracker was activated.</param>
        public AffixTracker(BuffIndex buffIdx, EquipmentIndex? equipIdx, int cyclesNeeded, string name)
        {
            buffIndex = buffIdx;
            equipmentIndex = equipIdx ?? EquipmentIndex.None; //Eventually stand up a null Equipment to be used if not given.
            loopsRequired = cyclesNeeded;
            affixNameTag = name;

            isStageLock = false;
            isCurseLock = false;
            isHeld = false;
            isVultured = false;
            vultureTimeLeft = 0f;
        }

        public object Clone()
        {
            return new AffixTracker(buffIndex, equipmentIndex, loopsRequired, affixNameTag); ;
        }
    }
}
