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
            string longLore = "Order: Untapped Potential" + Environment.NewLine +
                              "Tracking Number: 14******" + Environment.NewLine +
                              "Estimated Delivery: December 27, 2376" + Environment.NewLine +
                              "Shipping Method: Fragile" + Environment.NewLine +
                              "Shipping Address: 354 Strong St, Hell MI, 48169" + Environment.NewLine +
                              "Shipping Notes:" + Environment.NewLine + Environment.NewLine +
                              "We're not entirely sure what this thing is supposed to be. It doesn't move, smell, have any real colour, or even make noise, but it does shine and has incredibly hydrophobic properties. It actually seems to repel almost all biological matter, so maybe it'll make for a good solvent." + Environment.NewLine + Environment.NewLine +
                              "This thing is incredibly fragile though, and I wouldn't be surprised if the damn postman breaks it before you get a chance to. Be careful though, when this thing breaks I'll bet that super-solvent stuff will burn a whole straight through the ground.";
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
                cooldown = 60f,
                pickupIconPath = "@AffixGen:Assets/AffixGen/AffixBase.png",
                pickupModelPath = "@AffixGen:Assets/AffixGen/BaseAffix.prefab"
            };

            ItemDisplayRuleDict ruleDict = new ItemDisplayRuleDict();

            CustomEquipment equipment = new CustomEquipment(equipmentDef, ruleDict);
            index = ItemAPI.Add(equipment);
        }
    }
}
