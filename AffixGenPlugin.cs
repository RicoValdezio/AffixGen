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
            CaptureBuff.Init();
            CaptureDebuff.Init();
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
                if (equipmentIndex == PaleOrbEquip.PaleOrbEquipmentIndex && Body.GetBuffCount(CaptureBuff.CaptureBuffIndex) < 100)
                {
                    Body.AddBuff(CaptureBuff.CaptureBuffIndex);
                    return true;
                }
                else if (equipmentIndex == LunarOrbEquip.LunarOrbEquipmentIndex && Body.GetBuffCount(CaptureBuff.CaptureBuffIndex) < 100)
                {
                    int rep = 0;
                    while (rep < 10 & Body.GetBuffCount(CaptureBuff.CaptureBuffIndex) < 100)
                    {
                        Body.AddBuff(CaptureBuff.CaptureBuffIndex);
                        Body.AddBuff(CaptureDebuff.CaptureDebuffIndex);
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
                //Determine if Player Gets a Affix
                if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerBody.HasBuff(CaptureBuff.CaptureBuffIndex))
                {
                    int BuffCount = AttackerBody.GetBuffCount(CaptureBuff.CaptureBuffIndex);
                    int BuffRoll = new Random().Next(100) + 1;
                    if(BuffCount < BuffRoll)
                    {
                        return;
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixBlue))
                    {
                        ClearCaptureBuffs(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixBlue);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixRed))
                    {
                        ClearCaptureBuffs(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixRed);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixWhite))
                    {
                        ClearCaptureBuffs(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixWhite);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixHaunted))
                    {
                        ClearCaptureBuffs(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixHaunted);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixPoison))
                    {
                        ClearCaptureBuffs(AttackerBody);
                        AttackerMaster.inventory.SetEquipmentIndex(EquipmentIndex.AffixPoison);
                    }
                }
                //Determine if Player Clears Debuff
                if (VictimMaster.IsDeadAndOutOfLivesServer() && AttackerBody.HasBuff(CaptureDebuff.CaptureDebuffIndex))
                {
                    if (VictimBody.HasBuff(BuffIndex.AffixBlue) && AttackerBody.HasBuff(BuffIndex.AffixBlue))
                    {
                        AttackerBody.RemoveBuff(CaptureDebuff.CaptureDebuffIndex);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixRed) && AttackerBody.HasBuff(BuffIndex.AffixRed))
                    {
                        AttackerBody.RemoveBuff(CaptureDebuff.CaptureDebuffIndex);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixWhite) && AttackerBody.HasBuff(BuffIndex.AffixWhite))
                    {
                        AttackerBody.RemoveBuff(CaptureDebuff.CaptureDebuffIndex);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixHaunted) && AttackerBody.HasBuff(BuffIndex.AffixHaunted))
                    {
                        AttackerBody.RemoveBuff(CaptureDebuff.CaptureDebuffIndex);
                    }
                    else if (VictimBody.HasBuff(BuffIndex.AffixPoison) && AttackerBody.HasBuff(BuffIndex.AffixPoison))
                    {
                        AttackerBody.RemoveBuff(CaptureDebuff.CaptureDebuffIndex);
                    }
                }
                return;
            };
        }

        internal static void ClearCaptureBuffs(CharacterBody playerBody)
        {
            while (playerBody.HasBuff(CaptureBuff.CaptureBuffIndex))
            {
                playerBody.RemoveBuff(CaptureBuff.CaptureBuffIndex);
            }
        }
    }

    internal class CaptureBuff
    {
        internal static BuffIndex CaptureBuffIndex;

        internal static void Init()
        {
            BuffDef CaptureBuffDef = new BuffDef
            {
                name = "AffixCaptureBuff",
                iconPath = "@AffixGen:Assets/Buff_Icon.png",
                buffColor = new Color(0.7764f, 0.9686f, 0.7921f),
                isDebuff = false,
                canStack = true
            };

            CustomBuff CaptureBuffItem = new CustomBuff("Affix Capture Chance", CaptureBuffDef);

            CaptureBuffIndex = ItemAPI.Add(CaptureBuffItem);
        }
    }

    internal class CaptureDebuff
    {
        internal static BuffIndex CaptureDebuffIndex;

        internal static void Init()
        {
            BuffDef CaptureDebuffDef = new BuffDef
            {
                name = "AffixCaptureDebuff",
                iconPath = "@AffixGen:Assets/Buff_Icon.png",
                buffColor = new Color(0.8235f, 0.5882f, 0.4470f),
                isDebuff = true,
                canStack = true,
            };

            CustomBuff CaptureDebuffItem = new CustomBuff("Elemental Weakness", CaptureDebuffDef);

            CaptureDebuffIndex = ItemAPI.Add(CaptureDebuffItem);
        }
    }
}
