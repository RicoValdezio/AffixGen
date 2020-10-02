using BepInEx;
using R2API;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AffixGen
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ItemAPI", "ResourcesAPI", "LanguageAPI", "NetworkingAPI" })]
    [BepInPlugin(modGuid, modName, modVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AffixGenPlugin : BaseUnityPlugin
    {
        private const string modVer = "2.1.0";
        private const string modName = "AffixGen";
        private const string modGuid = "com.RicoValdezio.AffixGen";
        public static AffixGenPlugin instance;
        internal static List<AffixEquipBehaviour> activeBehaviours;

        private void Awake()
        {
            if (instance == null) instance = this;
            activeBehaviours = new List<AffixEquipBehaviour>();

            RegisterAssetBundleProvider();
            ConfigMaster.Init();
            BaseAffixEquip.Init();
            LunarAffixEquip.Init();
            AffixTrackerLib.Init();
            HookMaster.Init();
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