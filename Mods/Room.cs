using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace VioletTemplate.Mods
{
    internal class Room
    {
        public static void JoinRoom(string roomName)
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, GorillaNetworking.JoinType.Solo);
        }
    }
}
