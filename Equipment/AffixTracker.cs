using RoR2;
using System;

namespace AffixGen
{
    public class AffixTracker : ICloneable
    {
        internal EliteIndex eliteIndex;
        internal BuffIndex buffIndex;
        internal EquipmentIndex equipmentIndex;
        internal bool isStageLock, isCurseLock, isHeld, isVultured;
        internal float vultureTimeLeft;
        internal int loopsRequired;
        internal string affixNameTag;

        public object Clone()
        {
            AffixTracker cloneTracker = new AffixTracker
            {
                eliteIndex = eliteIndex,
                buffIndex = buffIndex,
                equipmentIndex = equipmentIndex,
                loopsRequired = loopsRequired,
                affixNameTag = affixNameTag
            };
            return cloneTracker;
        }
    }
}
