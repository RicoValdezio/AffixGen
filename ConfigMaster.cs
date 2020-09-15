using BepInEx.Configuration;

namespace AffixGen
{
    internal class ConfigMaster
    {
        internal static void Init()
        {
            AffixEquipBehaviour.curseMultiplier = AffixGenPlugin.Instance.Config.Bind(new ConfigDefinition("General Settings", "Curse Damage Multiplier"), 0.1f, new ConfigDescription("The decimal form of the percent increase in damage that each curse give the player")).Value;
        }
    }
}