using CG.Network;
using CG.Ship.Modules;
using HarmonyLib;
using System;

namespace ImprovedScoop
{
    [HarmonyPatch(typeof(CarryableAttractorLink), "TryInsertIntoSocket")]
    internal class CarryableAttractorLinkPatch
    {
        static bool Prefix(CarryableAttractorLink __instance, Action<CarryableAttractorLink> OnInsertCompleted)
        {
            string name = __instance.carryable.name;
            if (ScoopConfig.ProcessAlloys.Value && name.Contains("Item_Alloy"))
            {
                int.TryParse(name.Substring(name.Length - 1), out int tier);
                OnInsertCompleted(__instance);
                GameSessionProgressTracker.Instance.ModifyAlloyCount(tier*3, ResourceChangeAlloy.RECYCLE, __instance.carryable.assetGuid);
                ObjectFactory.DestroyCloneStarObject(__instance.carryable);
                return false;
            }

            if (ScoopConfig.ProcessBiomass.Value && name.Contains("Item_Biomass"))
            {
                int.TryParse(name.Substring(name.Length - 1), out int tier);
                int amount = tier switch { 1 => 25, 2 => 75, 3 => 150, _ => 0 };
                OnInsertCompleted(__instance);
                GameSessionProgressTracker.Instance.ModifyBiomassCount(amount, ResourceChangeBiomass.BIOMASS_DISPENSOR, __instance.carryable.assetGuid);
                ObjectFactory.DestroyCloneStarObject(__instance.carryable);
                return false;
            }

            return true;
        }
    }
}
