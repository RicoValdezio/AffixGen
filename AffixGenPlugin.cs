using BepInEx;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace AffixGen
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ItemAPI", "ResourcesAPI", "LanguageAPI" })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AffixGen : BaseUnityPlugin
    {
        private const string ModVer = "2.0.0";
        private const string ModName = "AffixGen";
        private const string ModGuid = "com.RicoValdezio.AffixGen";
        public static AffixGen Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            //RegisterAssetBundleProvider();
            ConfigMaster.Init();
            BaseAffixEquip.Init();
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
    }
}