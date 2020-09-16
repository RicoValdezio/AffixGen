using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace AffixGen
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ItemAPI", "ResourcesAPI", "LanguageAPI" })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AffixGenPlugin : BaseUnityPlugin
    {
        private const string ModVer = "2.0.0";
        private const string ModName = "AffixGen";
        private const string ModGuid = "com.RicoValdezio.AffixGen";
        public static AffixGenPlugin Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            //RegisterAssetBundleProvider();
            ConfigMaster.Init();
            BaseAffixEquip.Init();
            LunarAffixEquip.Init();

            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        private static void RegisterAssetBundleProvider()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AffixGen.affixgen"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@AffixGen", bundle);
                ResourcesAPI.AddProvider(provider);
            }
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            //Give all bodies a tracker component on start, even if they don't have the equip
            if (!self.gameObject.GetComponent<AffixEquipBehaviour>())
            {
                self.gameObject.AddComponent<AffixEquipBehaviour>();
            }
        }
    }
}