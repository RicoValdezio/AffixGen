using RoR2;
using UnityEngine;
using R2API;

namespace AffixGen
{
    public class CurseBuff
    {
        public static BuffIndex index;

        internal static void Init()
        {
            BuffDef buffDef = new BuffDef
            {
                name = "AffixCurse",
                canStack = true,
                buffColor = new Color(0.517f, 0f, 0.812f),
                eliteIndex = EliteIndex.None,
                isDebuff = false,
                iconPath = "Textures/BuffIcons/texWarcryBuffIcon"
            };

            CustomBuff customBuff = new CustomBuff(buffDef);

            index = BuffAPI.Add(customBuff);
        }
    }
}