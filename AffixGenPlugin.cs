using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace AffixGen
{

    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(ItemDropAPI), nameof(ResourcesAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class AffixGen : BaseUnityPlugin
    {
        private const string ModVer = "1.0.0";
        private const string ModName = "AffixGen";
        private const string ModGuid = "com.RicoValdezio.AffixGen";

        private void Awake()
        {
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AffixGen.affixgen"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@AffixGen", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            PaleOrbEquip.Init();
            LunarOrbEquip.Init();
            PaleOrbAffliction.Init();
            LunarOrbAffliction.Init();
            OrbEquipmentHooks.Init();
        }
    }

    internal class PaleOrbEquip
    {
        internal static EquipmentIndex PaleOrbEquipmentIndex;

        internal static void Init()
        {
            EquipmentDef PaleOrbEquipmentDef = new EquipmentDef
            {
                name = "PaleOrbEquipment",
                cooldown = 45f,
                isLunar = false,
                pickupModelPath = "@AffixGen:Assets/PaleOrb.prefab",
                pickupIconPath = "@AffixGen:Assets/PaleOrb_Icon.png",
                nameToken = "Pale Orb",
                pickupToken = "Gain <style=cIsUtility>1%</style> chance to capture an element.",
                descriptionToken = "Gain <style=cIsUtility>1% (+1% per use)</style> chance to capture an element.",
                loreToken = "Not so much a Tabula Rasa as it is a Sphaera Mundi./n -Octavius VII",
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
            EquipmentDef LunarOrbEquipmentDef = new EquipmentDef
            {
                name = "LunarOrbEquipment",
                cooldown = 45f,
                isLunar = true,
                pickupModelPath = "@AffixGen:Assets/LunarOrb.prefab",
                pickupIconPath = "@AffixGen:Assets/LunarOrb_Icon.png",
                nameToken = "Soul of a Tempest",
                pickupToken = "Gain <style=cIsUtility>10%</style> chance to capture an element, <style=cDeath>for a cost</style>.",
                descriptionToken = "Gain <style=cIsUtility>10% (+10% per use)</style> chance to capture an element and <style=cDeath>10% (+10% per use) weakness to elemental damage</style>.",
                loreToken = "You just had to go and use that damned thing twice, didn't you?/n -Octavius VII",
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
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex) =>
            {
                CharacterBody Body = self.characterBody;
                Inventory PlayerInventory = Body.inventory;
                if (equipmentIndex == PaleOrbEquip.PaleOrbEquipmentIndex && PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) < 100)
                {
                    PlayerInventory.GiveItem(PaleOrbAffliction.PaleOrbAfflictionIndex);
                    return true;
                }
                else if (equipmentIndex == LunarOrbEquip.LunarOrbEquipmentIndex && PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) < 100)
                {
                    int rep = 0;
                    while (rep < 10 & PlayerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) < 100)
                    {
                        PlayerInventory.GiveItem(PaleOrbAffliction.PaleOrbAfflictionIndex);
                        PlayerInventory.GiveItem(LunarOrbAffliction.LunarOrbAfflictionIndex);
                        rep++;
                    }
                    return true;
                }
                return orig(self, equipmentIndex);
            };

            //On Elite Kill
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                CharacterBody AttackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                CharacterBody VictimBody = victim.GetComponent<CharacterBody>();
                CharacterMaster AttackerMaster = AttackerBody.master;
                CharacterMaster VictimMaster = VictimBody.master;
                Inventory AttackerInventory = AttackerBody.inventory;
                //Determine if Player Gets a Affix
                if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) > 0)
                {
                    int BoonCount = AttackerInventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex);
                    int BoonRoll = new Random().Next(100) + 1;
                    if (BoonCount < BoonRoll)
                    {
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixBlue))
                    {
                        ClearPaleAffliction(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixBlue);
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixRed))
                    {
                        ClearPaleAffliction(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixRed);
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixWhite))
                    {
                        ClearPaleAffliction(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixWhite);
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixHaunted))
                    {
                        ClearPaleAffliction(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixHaunted);
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixPoison))
                    {
                        ClearPaleAffliction(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixPoison);
                        return;
                    }
                }
                //Determine if Player Clears Debuff
                if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerInventory.GetItemCount(LunarOrbAffliction.LunarOrbAfflictionIndex) > 0)
                {
                    if (VictimBody.isElite && AttackerBody.isElite)
                    {
                        AttackerInventory.RemoveItem(LunarOrbAffliction.LunarOrbAfflictionIndex);
                        return;
                    }
                }
                return;
            };
        }

        internal static void ClearPaleAffliction(CharacterBody playerBody)
        {
            Inventory inventory = playerBody.inventory;
            while (inventory.GetItemCount(PaleOrbAffliction.PaleOrbAfflictionIndex) > 0)
            {
                inventory.RemoveItem(PaleOrbAffliction.PaleOrbAfflictionIndex);
            }
        }
    }

    internal class PaleOrbAffliction
    {
        internal static ItemIndex PaleOrbAfflictionIndex;

        internal static void Init()
        {
            ItemDef PaleOrbAfflictionDef = new ItemDef
            {
                name = "PaleOrbAffliction",
                pickupModelPath = null,
                pickupIconPath = "@AffixGen:Assets/PaleOrb_Affliction.png",
                nameToken = "Boon of the Tempest",
                pickupToken = "Grants <style=cIsUtility>1%</style> chance to capture an element.",
                descriptionToken = "Grants <style=cIsUtility>1%</style> chance to capture an element.",
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
            ItemDef LunarOrbAfflictionDef = new ItemDef
            {
                name = "LunarOrbAffliction",
                pickupModelPath = null,
                pickupIconPath = "@AffixGen:Assets/LunarOrb_Affliction.png",
                nameToken = "Curse of the Tempest",
                pickupToken = "<style=cDeath>Grants 1% weakness to elemental damage</style>.",
                descriptionToken = "<style=cDeath>Grants 1% weakness to elemental damage</style>.",
                loreToken = null,
                tier = ItemTier.NoTier
            };

            ItemDisplayRule[] DisplayRules = null;

            CustomItem LunarOrbAfflictionItem = new CustomItem(LunarOrbAfflictionDef, DisplayRules);

            LunarOrbAfflictionIndex = ItemAPI.Add(LunarOrbAfflictionItem);
        }
    }
}
