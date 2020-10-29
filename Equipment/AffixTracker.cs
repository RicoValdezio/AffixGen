using RoR2;
using System;

namespace AffixGen
{
    public class AffixTracker : ICloneable
    {
        public EliteIndex eliteIndex;
        public BuffIndex buffIndex;
        public EquipmentIndex equipmentIndex;
        internal bool isStageLock, isCurseLock, isHeld, isVultured;
        internal float vultureTimeLeft;
        public int loopsRequired;
        public string affixNameTag;

        public object Clone()
        {
            AffixTracker cloneTracker = new AffixTracker
            {
                eliteIndex = eliteIndex,
                buffIndex = buffIndex,
                equipmentIndex = equipmentIndex,
                loopsRequired = loopsRequired,
                affixNameTag = affixNameTag,
            };
            return cloneTracker;
        }
    }
}
