using R2API;
using RoR2;
using System;

namespace AffixGen
{
    class BaseAffixEquip
    {
        internal static EquipmentIndex index;

        internal static void Init()
        {
            RegisterTokens();
            RegisterEquipment();
        }
        private static void RegisterTokens()
        {
            LanguageAPI.Add("BASE_AFFIX_NAME", "Untapped Potential");
            LanguageAPI.Add("BASE_AFFIX_PICK", "Gain a random affix for the rest of the stage");
            LanguageAPI.Add("BASE_AFFIX_DESC", "Gain a random affix for the rest of the stage. Available affixes are determined by the stage count.");
            string longLore = "Fill this in later";
            LanguageAPI.Add("BASE_AFFIX_LORE", longLore);
        }

        private static void RegisterEquipment()
        {
            EquipmentDef equipmentDef = new EquipmentDef
            {
                name = "UntappedPotential",
                nameToken = "BASE_AFFIX_NAME",
                pickupToken = "BASE_AFFIX_PICK",
                descriptionToken = "BASE_AFFIX_DESC",
                loreToken = "BASE_AFFIX_LORE",
                isBoss = false,
                isLunar = false,
                enigmaCompatible = false,
                canDrop = true,
                cooldown = 30f,
                pickupIconPath = "@AffixGen:Assets/AffixGen/AffixBase.png",
                pickupModelPath = "@AffixGen:Assets/AffixGen/BaseAffix.prefab"
            };

            ItemDisplayRuleDict ruleDict = new ItemDisplayRuleDict();

            CustomEquipment equipment = new CustomEquipment(equipmentDef, ruleDict);
            index = ItemAPI.Add(equipment);
        }
    }
}
