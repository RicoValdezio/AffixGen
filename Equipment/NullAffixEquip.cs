using R2API;
using RoR2;
using System;

namespace AffixGen
{
    public class NullAffixEquip
    {
        public static EquipmentIndex index;

        internal static void Init()
        {
            RegisterTokens();
            RegisterEquipment();
        }
        private static void RegisterTokens()
        {
            LanguageAPI.Add("NULL_AFFIX_NAME", "AffixGen Null Equipment");
            LanguageAPI.Add("NULL_AFFIX_PICK", "Do nothing, this is a null equipment for AffixGen");
            LanguageAPI.Add("NULL_AFFIX_DESC", "Do nothing, this is a null equipment for AffixGen");
            LanguageAPI.Add("NULL_AFFIX_LORE", "This equipment does nothing and should be impossible to acquire or use.");
        }

        private static void RegisterEquipment()
        {
            EquipmentDef equipmentDef = new EquipmentDef
            {
                name = "AffixGenNullEquip",
                nameToken = "NULL_AFFIX_NAME",
                pickupToken = "NULL_AFFIX_PICK",
                descriptionToken = "NULL_AFFIX_DESC",
                loreToken = "NULL_AFFIX_LORE",
                isBoss = false,
                isLunar = false,
                enigmaCompatible = false,
                canDrop = false,
                cooldown = 60f,
                pickupIconPath = null,
                pickupModelPath = null
            };

            ItemDisplayRuleDict ruleDict = new ItemDisplayRuleDict();

            CustomEquipment equipment = new CustomEquipment(equipmentDef, ruleDict);
            index = ItemAPI.Add(equipment);
        }
    }
}
