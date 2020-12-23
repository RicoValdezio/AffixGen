using RoR2;

namespace AffixGen
{
    public class HookMaster
    {
        public static void Init()
        {
            On.RoR2.Run.BeginStage += Run_BeginStage;

            On.RoR2.CharacterBody.OnEquipmentGained += CharacterBody_OnEquipmentGained;
            On.RoR2.CharacterBody.OnEquipmentLost += CharacterBody_OnEquipmentLost;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            On.RoR2.CharacterBody.AddTimedBuff += CharacterBody_AddTimedBuff;
        }

        public static void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            foreach (AffixEquipBehaviour behaviour in AffixGenPlugin.activeBehaviours)
            {
                behaviour.ResetStage();
            }
            orig(self);
        }

        public static void CharacterBody_OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (!self.masterObject.GetComponent<AffixEquipBehaviour>())
            {
                //If I don't have a tracker behaviour and I should, give me one
                if (equipmentDef.equipmentIndex == BaseAffixEquip.index || equipmentDef.equipmentIndex == LunarAffixEquip.index)
                {
                    self.masterObject.AddComponent<AffixEquipBehaviour>();
                }
            }
            else if (self.masterObject && self.masterObject.GetComponent<AffixEquipBehaviour>() is AffixEquipBehaviour behaviour)
            {
                //If I have one, run the modified pickup hook in the behaviour
                behaviour.UpdateEquipment(equipmentDef.equipmentIndex, true);
            }
            orig(self, equipmentDef);
        }

        public static void CharacterBody_OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (self.masterObject && self.masterObject.GetComponent<AffixEquipBehaviour>() is AffixEquipBehaviour behaviour)
            {
                //If I have one, run the modified pickup hook in the behaviour
                behaviour.UpdateEquipment(equipmentDef.equipmentIndex, false);
            }
            orig(self, equipmentDef);
        }

        public static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            if (self.characterBody && self.characterBody.masterObject && self.characterBody.masterObject.GetComponent<AffixEquipBehaviour>() is AffixEquipBehaviour behaviour)
            {
                if (equipmentIndex == BaseAffixEquip.index)
                {
                    return behaviour.PerformBaseAction();
                }
                else if (equipmentIndex == LunarAffixEquip.index)
                {
                    return behaviour.PerformLunarAction();
                }
            }
            return orig(self, equipmentIndex);
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.masterObject && self.body.masterObject.GetComponent<AffixEquipBehaviour>() is AffixEquipBehaviour behaviour)
            {
                damageInfo = behaviour.CalculateNewDamage(damageInfo);
            }
            orig(self, damageInfo);
        }

        public static void CharacterBody_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            if (self.masterObject && self.masterObject.GetComponent<AffixEquipBehaviour>() is AffixEquipBehaviour behaviour)
            {
                behaviour.UpdateVultures(buffType, duration);
            }
            orig(self, buffType, duration);
        }
    }
}
