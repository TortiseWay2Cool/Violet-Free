using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using GorillaLocomotion;
using GorillaTagScripts;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OVR.OpenVR;
using Photon.Pun;
using Photon.Pun;
using Photon.Realtime;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;
using static Liv.Lck.NativeMicrophone.LckNativeMicrophone;
using static VioletTemplate.Main.Extentions.Notifications;
using static VioletTemplate.Mods.Overpowered;
using Player = Photon.Realtime.Player;
namespace VioletTemplate.Main.Extentions
{
    internal class RoomLogic : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Show("Room", "You Are Master Client", UnityEngine.Color.green);
            }
            else
            {
                Show("Room", "You Are Not Master Client", UnityEngine.Color.red);
            }
        }


        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Show($"Failed To Join Room", $"Code {returnCode}, Message {message}", UnityEngine.Color.red);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Show(newPlayer.nickName, $"Has Joined the room", UnityEngine.Color.violet);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Show(otherPlayer.nickName, $"Has Left the room", UnityEngine.Color.violet);
        }
    }
}
