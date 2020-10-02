using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
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
        private const string modVer = "2.0.5";
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

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
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

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self.teamComponent.teamIndex == TeamIndex.Player && !self.masterObject.GetComponent<AffixEquipBehaviour>() && self.master.playerCharacterMasterController)
            {
                //Chat.AddMessage("Adding Component to : " + self.baseNameToken);
                self.masterObject.AddComponent<AffixEquipBehaviour>();
            }
            orig(self);
        }
    }
}