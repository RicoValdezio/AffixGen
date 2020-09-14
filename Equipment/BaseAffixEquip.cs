using R2API;
using RoR2;

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
            LanguageAPI.Add("BASE_AFFIX_PICK", "Gain a random affix for 12 seconds");
            LanguageAPI.Add("BASE_AFFIX_DESC", "Gain a random affix for 12 seconds");
            LanguageAPI.Add("BASE_AFFIX_DESC", "Fill this in later");
        }

        private static void RegisterEquipment()
        {

        }
    }
}
