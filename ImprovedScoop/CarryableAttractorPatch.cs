using CG.Objects;
using CG.Ship.Modules;
using Gameplay.Utilities;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;

namespace ImprovedScoop
{
    [HarmonyPatch(typeof(CarryableAttractor))]
    internal class CarryableAttractorPatch
    {
        public static List<GUIDUnion> dotNotAttract = ScoopConfig.HexToGUIDs(ScoopConfig.ItemBlacklist.Value);

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void Awake(ref float ____catchRadius, ref ModifiableFloat ___MaxRange, ref ModifiableFloat ____pullVelocity)
        {
            ____catchRadius = ScoopConfig.catchRadiusBase * ScoopConfig.CatchRadiusMultiplier.Value;
            ___MaxRange = ScoopConfig.maxRangeBase * ScoopConfig.MaxRangeMultiplier.Value;
            ____pullVelocity = ScoopConfig.pullVelocityBase * ScoopConfig.PullVelocityMultiplier.Value;
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetPossibleItemsToAttrack")]
        static void GetPossibleItemsToAttrack(ref List<AbstractCarryableObject> __result)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            __result.RemoveAll(item => dotNotAttract.Contains(item.assetGuid));
        }
    }
}
