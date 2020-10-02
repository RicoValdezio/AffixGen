using RoR2;

namespace AffixGen
{
    internal class HookMaster
    {
        internal static void Init()
        {
            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
        }

        private static void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (!self.masterObject.GetComponent<AffixEquipBehaviour>())
            {
                //If I don't have a tracker behaviour and I should, give me one
                if (equipmentDef.equipmentIndex == BaseAffixEquip.index || equipmentDef.equipmentIndex == LunarAffixEquip.index)
                {
                    self.masterObject.AddComponent<AffixEquipBehaviour>();
                }
            }
            else
            {
                //If I have one, run the modified pickup hook in the behaviour
                self.masterObject.GetComponent<AffixEquipBehaviour>().UpdateEquipment(equipmentDef.equipmentIndex, true);
            }
            orig(self, equipmentDef);
        }

        private static void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (self.masterObject.GetComponent<AffixEquipBehaviour>())
            {
                //If I have one, run the modified pickup hook in the behaviour
                self.masterObject.GetComponent<AffixEquipBehaviour>().UpdateEquipment(equipmentDef.equipmentIndex, false);
            }
            orig(self, equipmentDef);
        }
    }
}
