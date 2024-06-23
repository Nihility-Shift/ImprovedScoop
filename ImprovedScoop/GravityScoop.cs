using CG.Objects;
using CG.Ship.Hull;
using CG.Ship.Modules;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImprovedScoop
{
    internal class GravityScoop
    {
        internal static List<GUIDUnion> eject = ScoopConfig.HexToGUIDs(ScoopConfig.ItemEjectlist.Value);

        internal static Dictionary<CarryableAttractor, List<CarryablesSocket>> gravityScoops = new();

        internal static long EjectWaitTimeMs = 1000;
        private static Dictionary<CarryablesSocket, DateTime> toEject = new();

        private const float distance = 4;
        private const float speed = 100;

        private static readonly FieldInfo attractPointField = AccessTools.Field(typeof(CarryableAttractor), "_attractPoint");

        internal static void CheckAndEject(object sender, EventArgs e)
        {
            foreach(KeyValuePair<CarryableAttractor, List<CarryablesSocket>> pair in gravityScoops)
            {
                foreach (CarryablesSocket socket in pair.Value)
                {
                    if (socket.Payload != null && socket.Payload.AmOwner && eject.Contains(socket.Payload.assetGuid))
                    {
                        if (toEject.ContainsKey(socket))
                        {
                            if ((DateTime.Now - toEject[socket]).TotalMilliseconds > EjectWaitTimeMs)
                            {
                                toEject.Remove(socket);
                                Transform attractPoint = (Transform)attractPointField.GetValue(pair.Key);
                                socket.Payload.transform.position += attractPoint.rotation * (Vector3.back*distance);
                                socket.CarryableHandler.TryEjectCarryable(attractPoint.rotation * (Vector3.back*speed));
                            }
                        }
                        else
                        {
                            toEject.Add(socket, DateTime.Now);
                            continue;
                        }
                    }
                }
            }

            //Remove any empty sockets with a running countdown timer
            if (toEject.Count > 0 && toEject.Keys.Any(socket => socket.Payload == null))
            {
                List<CarryablesSocket> toRemove = new();
                foreach (CarryablesSocket socket in toEject.Keys)
                {
                    if (socket.Payload == null)
                        toRemove.Add(socket);
                }
                foreach (CarryablesSocket socket in toRemove)
                {
                    toEject.Remove(socket);
                }
            }
        }
    }
}
