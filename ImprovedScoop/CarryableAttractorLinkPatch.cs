using CG.Game;
using CG.Network;
using CG.Objects;
using CG.Ship.Hull;
using CG.Ship.Modules;
using HarmonyLib;
using RSG;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImprovedScoop
{
    [HarmonyPatch(typeof(CarryableAttractorLink), "TryInsertIntoSocket")]
    internal class CarryableAttractorLinkPatch
    {
        private static List<AbstractCarryableObject> shardQueue = new();

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

            if (ScoopConfig.ProcessShards.Value)
            {
                bool shard = false;
                bool summonShard = false;
                foreach (CsTag tag in __instance.carryable.CsTags)
                {
                    if (tag.name == "Tag_DataShard_Generic") shard = true;
                    if (tag.name == "Tag_DataShard_Escort" || tag.name == "Tag_DataShard_Minefield") summonShard = true;
                }

                if (shard && (!summonShard || ScoopConfig.ProcessSummonShards.Value))
                {
                    CarryablesSocket socket = ClientGame.Current.PlayerShip.GetComponentsInChildren<CarryablesSocketProvider>().FirstOrDefault(provider => provider.name == "Module_Ship_Terminal_AstralMap").Sockets[0];
                    if (TryInsertShard(__instance.carryable))
                    {
                        return false;
                    }
                    else
                    {
                        if (shardQueue.Count == 0) VoidManager.Events.Instance.LateUpdate += TryShards;
                        shardQueue.Add(__instance.carryable);
                    }
                }
            }

            if (ScoopConfig.MoveToShelf.Value)
            {
                CarryablesShelf[] shelves = ClientGame.Current.PlayerShip.GetComponentsInChildren<CarryablesShelf>();

                foreach (CarryablesShelf shelf in shelves)
                {
                    foreach (CarryablesSocket socket in shelf.CarryablesSockets)
                    {
                        if (!socket.IsFull && socket.DoesAccept(__instance.carryable))
                        {
                            OnInsertCompleted(__instance);
                            socket.TryInsertCarryable(__instance.carryable);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void TryShards(object sender, EventArgs e)
        {
            if (TryInsertShard(shardQueue.First()))
            {
                shardQueue.RemoveAt(0);
            }

            if (shardQueue.Count == 0)
            {
                VoidManager.Events.Instance.LateUpdate -= TryShards;
            }
        }

        private static bool TryInsertShard(AbstractCarryableObject shard)
        {
            //FIXME cache
            CarryablesSocket socket = ClientGame.Current.PlayerShip.GetComponentsInChildren<CarryablesSocketProvider>().FirstOrDefault(provider => provider.name == "Module_Ship_Terminal_AstralMap").Sockets[0];
            if (socket.CurrentState == Gameplay.Carryables.SocketState.Closed || socket.IsFull || !socket.IsInput || socket.Payload != null) return false;
            Promise promise = (Promise)socket.TryInsertCarryable(shard);
            if (promise.CurState != PromiseState.Rejected) return true;
            return false;
        }
    }
}
