using CG.Objects;
using CG.Ship.Hull;
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
        internal static List<GUIDUnion> dotNotAttract = ScoopConfig.HexToGUIDs(ScoopConfig.ItemBlacklist.Value);

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void Awake(CarryableAttractor __instance, ref float ____catchRadius, ref ModifiableFloat ___MaxRange, ref ModifiableFloat ____pullVelocity)
        {
            if (VoidManager.Configs.IsDebugMode)
                BepinPlugin.Log.LogInfo($"Patching awake for {__instance.name}. Catch Radius: {____catchRadius}, Range: {___MaxRange.Value}:{___MaxRange.BaseValue}, Velocity: {____pullVelocity.Value}:{____pullVelocity.BaseValue}");

            //Assign multiplied values
            ____catchRadius *= ScoopConfig.CatchRadiusMultiplier.Value;
            ___MaxRange.SetBaseValue(___MaxRange.BaseValue * ScoopConfig.MaxRangeMultiplier.Value);
            ____pullVelocity.SetBaseValue(____pullVelocity.BaseValue * ScoopConfig.PullVelocityMultiplier.Value);

            if (VoidManager.Configs.IsDebugMode)
                BepinPlugin.Log.LogInfo($"Patched awake for {__instance.name}. Catch Radius: {____catchRadius}, Range: {___MaxRange.Value}:{___MaxRange.BaseValue}, Velocity: {____pullVelocity.Value}:{____pullVelocity.BaseValue}");

            foreach (CarryablesSocket socket in __instance._carryablesSocketProvider.Sockets)
            {
                socket.OnAcquireCarryable += GravityScoopEject.SocketItemInserted;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnDestroy")]
        static void OnDestroy(CarryableAttractor __instance)
        {
            foreach (CarryablesSocket socket in __instance._carryablesSocketProvider.Sockets)
            {
                socket.OnAcquireCarryable -= GravityScoopEject.SocketItemInserted;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetPossibleItemsToAttrack")]
        static void GetPossibleItemsToAttrack(ref List<CarryableObject> __result)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            __result.RemoveAll(item => dotNotAttract.Contains(item.assetGuid));
        }
    }
}
