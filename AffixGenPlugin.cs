using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace AffixGen
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
        "AssetPlus",
    })]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class AffixGen : BaseUnityPlugin
    {
        private const string ModVer = "1.0.0";
        private const string ModName = "AffixGen";
        private const string ModGuid = "com.RicoValdezio.AffixGen";

        internal static ConfigEntry<float> BoonValue;
        internal static ConfigEntry<float> CurseValue;
        internal static ConfigEntry<int> MaxStacks;
        public static AffixGen Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AffixGen.affixgen"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@AffixGen", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            AffixGenConfig.Init();
            PaleOrbEquip.Init();
            LunarOrbEquip.Init();
            PaleOrbAffliction.Init();
            LunarOrbAffliction.Init();
            OrbEquipmentHooks.Init();
        }
    }

    internal class AffixGenConfig
    {
        internal static void Init()
        {
            AffixGen.BoonValue = AffixGen.Instance.Config.Bind<float>("MainSettings", "BoonValuePercent", 1f, "% Chance Gain from each Boon");
            AffixGen.CurseValue = AffixGen.Instance.Config.Bind<float>("MainSettings", "CurseValuePercent", 1f, "% Chance Gain from each Curse");
            AffixGen.MaxStacks = AffixGen.Instance.Config.Bind<int>("MainSettings", "MaxStacks", 100, "Maximum Stacks of Both Boons and Curses");
        }
    }

    internal class PaleOrbEquip
    {
        internal static EquipmentIndex PaleOrbEquipmentIndex;

        internal static void Init()
        {
            R2API.AssetPlus.Languages.AddToken("PALE_ORB_NAME_TOKEN", "Pale Orb");
            R2API.AssetPlus.Languages.AddToken("PALE_ORB_PICKUP_TOKEN", "Gain <style=cIsUtility>1 Boon of the Tempest</style> on use.");
            R2API.AssetPlus.Languages.AddToken("PALE_ORB_DESCRIPITON_TOKEN", "Gain <style=cIsUtility>1 Boon of the Tempest</style> on use.");
            R2API.AssetPlus.Languages.AddToken("PALE_ORB_LORE_TOKEN", "Not so much a Tabula Rasa as it is a Sphaera Mundi./n -Octavius VII");

            EquipmentDef PaleOrbEquipmentDef = new EquipmentDef
            {
                name = "PALE_ORB_NAME_TOKEN",
                cooldown = 45f,
                isLunar = false,
                pickupModelPath = "@AffixGen:Assets/PaleOrb.prefab",
                pickupIconPath = "@AffixGen:Assets/PaleOrb_Icon.png",
                nameToken = "PALE_ORB_NAME_TOKEN",
                pickupToken = "PALE_ORB_PICKUP_TOKEN",
                descriptionToken = "PALE_ORB_DESCRIPITON_TOKEN",
                loreToken = "PALE_ORB_LORE_TOKEN",
                canDrop = true,
                enigmaCompatible = true
            };

            ItemDisplayRule[] PaleOrbDisplayRules = null;

            CustomEquipment PaleOrbEquipment = new CustomEquipment(PaleOrbEquipmentDef, PaleOrbDisplayRules);

            PaleOrbEquipmentIndex = ItemAPI.Add(PaleOrbEquipment);
        }
    }

    internal class LunarOrbEquip
    {
        internal static EquipmentIndex LunarOrbEquipmentIndex;

        internal static void Init()
        {
            R2API.AssetPlus.Languages.AddToken("LUNAR_ORB_NAME_TOKEN", "Soul of a Tempest");
            R2API.AssetPlus.Languages.AddToken("LUNAR_ORB_PICKUP_TOKEN", "Gain <style=cIsUtility>10 Boon of the Tempest</style> and <style=cDeath>10 Curse of the Tempest</style> on use.");
            R2API.AssetPlus.Languages.AddToken("LUNAR_ORB_DESCRIPITON_TOKEN", "Gain <style=cIsUtility>10 Boon of the Tempest</style> and <style=cDeath>10 Curse of the Tempest</style> on use.");
            R2API.AssetPlus.Languages.AddToken("LUNAR_ORB_LORE_TOKEN", "You just had to go and use that damned thing ten times, didn't you?/n -Octavius VII");

            EquipmentDef LunarOrbEquipmentDef = new EquipmentDef
            {
                name = "LUNAR_ORB_NAME_TOKEN",
                cooldown = 45f,
                isLunar = false,
                pickupModelPath = "@AffixGen:Assets/LunarOrb.prefab",
                pickupIconPath = "@AffixGen:Assets/LunarOrb_Icon.png",
                nameToken = "LUNAR_ORB_NAME_TOKEN",
                pickupToken = "LUNAR_ORB_PICKUP_TOKEN",
                descriptionToken = "LUNAR_ORB_DESCRIPITON_TOKEN",
                loreToken = "LUNAR_ORB_LORE_TOKEN",
                canDrop = true,
                enigmaCompatible = true
            };

            ItemDisplayRule[] LunarOrbDisplayRules = null;

            CustomEquipment LunarOrbEquipment = new CustomEquipment(LunarOrbEquipmentDef, LunarOrbDisplayRules);

            LunarOrbEquipmentIndex = ItemAPI.Add(LunarOrbEquipment);
        }
    }

    internal class OrbEquipmentHooks
    {
        internal static void Init()
        {
            //On Equipment Usage
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;

            //On Take Damage
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            CharacterBody Body = self.characterBody;
            Inventory PlayerInventory = Body.inventory;
            if (equipmentIndex == PaleOrbEquip.PaleOrbEquipmentIndex && PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) < AffixGen.MaxStacks.Value)
            {
                PlayerInventory.GiveItem(PaleOrbAffliction.PaleOrbAfflictionIndex, 1);
                return true;
            }
            else if (equipmentIndex == LunarOrbEquip.LunarOrbEquipmentIndex && PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) < AffixGen.MaxStacks.Value)
            {
                int GiveAmount = Math.Min(AffixGen.MaxStacks.Value - PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex), 10);
                PlayerInventory.GiveItem(PaleOrbAffliction.PaleOrbAfflictionIndex, GiveAmount);
                PlayerInventory.GiveItem(LunarOrbAffliction.LunarOrbAfflictionIndex, GiveAmount);
                return true;
            }
            return orig(self, equipmentIndex);
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                if (damageInfo.attacker.GetComponent<CharacterBody>().inventory)
                {
                    CharacterBody AttackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    CharacterBody VictimBody = self.GetComponent<CharacterBody>();
                    CharacterMaster AttackerMaster = AttackerBody.master;
                    CharacterMaster VictimMaster = VictimBody.master;
                    Inventory AttackerInventory = AttackerBody.inventory;
                    //Determine if Player Gets a Affix
                    if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) > 0)
                    {
                        int BoonCount = AttackerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex);
                        float BoonCountValue = BoonCount * AffixGen.BoonValue.Value;
                        float BoonRoll = (float)UnityEngine.Random.Range(1, 100);
                        if (BoonCountValue < BoonRoll) { }
                        else if (VictimBody.HasBuff(BuffIndex.AffixBlue))
                        {
                            ClearPaleAffliction(AttackerBody);
                            AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixBlue);
                        }
                        else if (VictimBody.HasBuff(BuffIndex.AffixRed))
                        {
                            ClearPaleAffliction(AttackerBody);
                            AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixRed);
                        }
                        else if (VictimBody.HasBuff(BuffIndex.AffixWhite))
                        {
                            ClearPaleAffliction(AttackerBody);
                            AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixWhite);
                        }
                        else if (VictimBody.HasBuff(BuffIndex.AffixHaunted))
                        {
                            ClearPaleAffliction(AttackerBody);
                            AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixHaunted);
                        }
                        else if (VictimBody.HasBuff(BuffIndex.AffixPoison))
                        {
                            ClearPaleAffliction(AttackerBody);
                            AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixPoison);
                        }
                    }
                    //Determine if Player Clears Debuff
                    if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerInventory.GetItemCount(LunarOrbAffliction.LunarOrbAfflictionIndex) > 0)
                    {
                        if (VictimBody.isElite && AttackerBody.isElite)
                        {
                            AttackerInventory.RemoveItem(LunarOrbAffliction.LunarOrbAfflictionIndex, 1);
                        }
                    }
                }

                if (self.GetComponent<CharacterBody>().inventory)
                {
                    CharacterBody AttackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    CharacterBody VictimBody = self.GetComponent<CharacterBody>();
                    Inventory AttackerInventory = AttackerBody.inventory;
                    Inventory VictimInventory = VictimBody.inventory;
                    //Determine if Player Takes Curse Damage
                    if (VictimInventory.GetItemCount(LunarOrbAffliction.LunarOrbAfflictionIndex) > 0 && AttackerBody.isElite)
                    {
                        int CurseCount = AttackerInventory.GetItemCount(LunarOrbAffliction.LunarOrbAfflictionIndex);
                        float CurseCountValue = CurseCount * AffixGen.CurseValue.Value;
                        float CursePercent = CurseCountValue / 100;
                        DamageInfo EliteBonusDamage = new DamageInfo
                        {
                            damage = damageInfo.damage * CursePercent,
                            damageColorIndex = DamageColorIndex.DeathMark,
                            damageType = DamageType.Generic,
                            attacker = null,
                            crit = damageInfo.crit,
                            force = Vector3.zero,
                            inflictor = null,
                            position = damageInfo.position,
                            procChainMask = damageInfo.procChainMask,
                            procCoefficient = 0f
                        };
                        self.TakeDamage(EliteBonusDamage);
                    }
                }
            }
            orig(self, damageInfo);
        }

        private static void ClearPaleAffliction(CharacterBody playerBody)
        {
            Inventory inventory = playerBody.inventory;
            inventory.RemoveItem(PaleOrbAffliction.PaleOrbAfflictionIndex, inventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex));
        }
    }

    internal class PaleOrbAffliction
    {
        internal static ItemIndex PaleOrbAfflictionIndex;

        internal static void Init()
        {
            R2API.AssetPlus.Languages.AddToken("PALE_BOON_NAME_TOKEN", "Boon of the Tempest");
            R2API.AssetPlus.Languages.AddToken("PALE_BOON_PICKUP_TOKEN", "Grants <style=cIsUtility>" + (AffixGen.BoonValue.Value).ToString() + "%</style> chance to capture an element.");
            R2API.AssetPlus.Languages.AddToken("PALE_BOON_DESCRIPITON_TOKEN", "Grants <style=cIsUtility>" + (AffixGen.BoonValue.Value).ToString() + "%</style> chance to capture an element.");

            ItemDef PaleOrbAfflictionDef = new ItemDef
            {
                name = "PALE_BOON_NAME_TOKEN",
                pickupModelPath = null,
                pickupIconPath = "@AffixGen:Assets/PaleOrb_Affliction.png",
                nameToken = "PALE_BOON_NAME_TOKEN",
                pickupToken = "PALE_BOON_PICKUP_TOKEN",
                descriptionToken = "PALE_BOON_DESCRIPITON_TOKEN",
                loreToken = null,
                tier = ItemTier.NoTier
            };

            ItemDisplayRule[] DisplayRules = null;

            CustomItem PaleOrbAfflictionItem = new CustomItem(PaleOrbAfflictionDef, DisplayRules);

            PaleOrbAfflictionIndex = ItemAPI.Add(PaleOrbAfflictionItem);
        }
    }

    internal class LunarOrbAffliction
    {
        internal static ItemIndex LunarOrbAfflictionIndex;

        internal static void Init()
        {
            R2API.AssetPlus.Languages.AddToken("PALE_CURSE_NAME_TOKEN", "Curse of the Tempest");
            R2API.AssetPlus.Languages.AddToken("PALE_CURSE_PICKUP_TOKEN", "Grants <style=cDeath>" + (AffixGen.CurseValue.Value).ToString() + "% weakness to elites</style>.");
            R2API.AssetPlus.Languages.AddToken("PALE_CURSE_DESCRIPITON_TOKEN", "Grants <style=cDeath>" + (AffixGen.CurseValue.Value).ToString() + "% weakness to elites</style>.");

            ItemDef LunarOrbAfflictionDef = new ItemDef
            {
                name = "PALE_CURSE_NAME_TOKEN",
                pickupModelPath = null,
                pickupIconPath = "@AffixGen:Assets/LunarOrb_Affliction.png",
                nameToken = "PALE_CURSE_NAME_TOKEN",
                pickupToken = "PALE_CURSE_PICKUP_TOKEN",
                descriptionToken = "PALE_CURSE_DESCRIPITON_TOKEN",
                loreToken = null,
                tier = ItemTier.NoTier
            };

            ItemDisplayRule[] DisplayRules = null;

            CustomItem LunarOrbAfflictionItem = new CustomItem(LunarOrbAfflictionDef, DisplayRules);

            LunarOrbAfflictionIndex = ItemAPI.Add(LunarOrbAfflictionItem);
        }
    }
}
