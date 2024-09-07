using CG.Game;
using CG.Game.Missions;
using CG.Network;
using CG.Objects;
using CG.Ship.Hull;
using CG.Ship.Modules;
using Gameplay.Tags;
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
