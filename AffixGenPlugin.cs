using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AffixGen
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[] { "ItemAPI", "ResourcesAPI", "LanguageAPI", "NetworkingAPI", "BuffAPI" })]
    [BepInPlugin(modGuid, modName, modVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class AffixGenPlugin : BaseUnityPlugin
    {
        private const string modVer = "2.2.0";
        private const string modName = "AffixGen";
        private const string modGuid = "com.RicoValdezio.AffixGen";
        public static AffixGenPlugin instance;
        internal static List<AffixEquipBehaviour> activeBehaviours;
        internal static ManualLogSource logSource;

        public void Awake()
        {
            if (instance == null) instance = this;
            logSource = instance.Logger;
            activeBehaviours = new List<AffixEquipBehaviour>();

            RegisterAssetBundleProvider();
            ConfigMaster.Init();

            BaseAffixEquip.Init();
            LunarAffixEquip.Init();
            NullAffixEquip.Init();
            CurseBuff.Init();

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