using BepInEx.Configuration;

namespace AffixGen
{
    internal class ConfigMaster
    {
        internal static void Init()
        {
            AffixEquipBehaviour.curseMultiplier = AffixGenPlugin.instance.Config.Bind(new ConfigDefinition("General Settings", "Curse Damage Multiplier"), 0.1f, new ConfigDescription("The decimal form of the percent increase in damage that each curse give the player")).Value;
            AffixEquipBehaviour.maxStageLocks = AffixGenPlugin.instance.Config.Bind(new ConfigDefinition("General Settings", "Max Stage Lock"), 1, new ConfigDescription("The maximum number of elite effects the basic equipment can give per stage")).Value;
        }
    }
}