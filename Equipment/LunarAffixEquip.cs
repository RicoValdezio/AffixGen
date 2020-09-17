using R2API;
using RoR2;
using System;

namespace AffixGen
{
    class LunarAffixEquip
    {
        internal static EquipmentIndex index;

        internal static void Init()
        {
            RegisterTokens();
            RegisterEquipment();
        }

        private static void RegisterTokens()
        {
            LanguageAPI.Add("LUNAR_AFFIX_NAME", "Tempestuous Rage");
            LanguageAPI.Add("LUNAR_AFFIX_PICK", "Capture the most recent affix you were hit by, but soften your senses");
            LanguageAPI.Add("LUNAR_AFFIX_DESC", "Capture the most recent affix you were hit by for the rest of your life, but suffer a 10% increase to all damage you take per capture." + Environment.NewLine +
                                                "The severity of the curse can be temporarily reduced by obtaining the affix via Untapped Potential, Wake of Vultures, or the Elite Equipment.");
            string longLore = "There's a storm brewing..." + Environment.NewLine +
                              "Five weeks we've been lost at seas and I'm telling you something," + Environment.NewLine +
                              "There's a storm brewing..." + Environment.NewLine +
                              "Five days since we've run out of food and the men are starving," + Environment.NewLine +
                              "There's a storm brewing..." + Environment.NewLine +
                              "Five hours since the birds all flew away and the men are worried," + Environment.NewLine +
                              "There's a storm brewing." + Environment.NewLine +
                              "Five minutes since the first clap of thunder and the men a crying out," + Environment.NewLine +
                              "There's a storm brewing!" + Environment.NewLine +
                              "Five seconds since the hull was ripped to shreds and now I realize," + Environment.NewLine +
                              "There's a storm brewing...";
            LanguageAPI.Add("LUNAR_AFFIX_LORE", longLore);
        }

        private static void RegisterEquipment()
        {
            EquipmentDef equipmentDef = new EquipmentDef
            {
                name = "TempestuousRage",
                nameToken = "LUNAR_AFFIX_NAME",
                pickupToken = "LUNAR_AFFIX_PICK",
                descriptionToken = "LUNAR_AFFIX_DESC",
                loreToken = "LUNAR_AFFIX_LORE",
                isBoss = false,
                isLunar = true,
                enigmaCompatible = false,
                canDrop = true,
                colorIndex = ColorCatalog.ColorIndex.LunarItem,
                cooldown = 120f,
                pickupIconPath = "@AffixGen:Assets/AffixGen/AffixLunar.png",
                pickupModelPath = "@AffixGen:Assets/AffixGen/LunarAffix.prefab"
            };

            ItemDisplayRuleDict ruleDict = new ItemDisplayRuleDict();

            CustomEquipment equipment = new CustomEquipment(equipmentDef, ruleDict);
            index = ItemAPI.Add(equipment);
        }
    }
}