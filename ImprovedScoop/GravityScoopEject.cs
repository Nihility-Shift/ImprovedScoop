using CG.Objects;
using CG.Ship.Hull;
using CG.Ship.Modules;
using Gameplay.Carryables;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImprovedScoop
{
    internal class GravityScoopEject
    {
        internal static List<GUIDUnion> eject = ScoopConfig.HexToGUIDs(ScoopConfig.ItemEjectlist.Value);

        private static Dictionary<CarryablesSocket, DateTime> toEject = new();

        private const long EjectWaitTimeMs = 1000;
        private const float distance = 4;
        private const float speed = 100;

        private static readonly FieldInfo attractPointField = AccessTools.Field(typeof(CarryableAttractor), "_attractPoint");

        internal static void CheckAndEject(object sender, EventArgs e)
        {
            for (int i = toEject.Count - 1; i >= 0; i--)
            {
                KeyValuePair<CarryablesSocket, DateTime> pair = toEject.ElementAt(i);
                CarryablesSocket socket = pair.Key;
                DateTime time = pair.Value;
                if (socket.Payload == null)
                {
                    toEject.Remove(socket);
                    continue;
                }
                if ((DateTime.Now - time).TotalMilliseconds > EjectWaitTimeMs)
                {
                    toEject.Remove(socket);
                    Transform attractPoint = (Transform)attractPointField.GetValue(socket.GameObject.GetComponentInParent<CarryableAttractor>());
                    socket.Payload.transform.position += attractPoint.rotation * (Vector3.back * distance);
                    socket.CarryableHandler.TryEjectCarryable(attractPoint.rotation * (Vector3.back * speed));
                }
            }

            if (toEject.Count == 0)
            {
                VoidManager.Events.Instance.LateUpdate -= CheckAndEject;
            }
        }

        internal static void SocketItemInserted(ICarrier carrier, CarryableObject carryable, ICarrier previousCarrier)
        {
            if (!PhotonNetwork.IsMasterClient || !eject.Contains(carryable.assetGuid) || carrier is not CarryablesSocket) return;

            if (toEject.Count == 0)
            {
                VoidManager.Events.Instance.LateUpdate += CheckAndEject;
            }
            toEject.Add((CarryablesSocket)carrier, DateTime.Now);
        }
    }
}
