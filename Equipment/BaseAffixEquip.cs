using R2API;
using RoR2;
using System;

namespace AffixGen
{
    class BaseAffixEquip
    {
        internal static ItemIndex ItemIndex;

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

        }
    }
}
