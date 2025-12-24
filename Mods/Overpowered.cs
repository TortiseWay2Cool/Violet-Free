using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Technie.PhysicsCreator;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Valve.VR.InteractionSystem;
using static Fusion.Sockets.NetBitBuffer;
using static Photon.Realtime.Player;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.PlayerManager;
using static VioletTemplate.Main.Extentions.Serilization;
using static VioletTemplate.Mods.Competitive;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using JoinType = GorillaNetworking.JoinType;
using Player = Photon.Realtime.Player;
using Random = UnityEngine.Random;
using VioletTemplate.Main.Extentions;
namespace VioletTemplate.Mods
{
    internal class Overpowered : MonoBehaviour
    {
        #region Kick
        public static void KickGun(byte Code)
        {
            StartBothGuns(() =>
             {
                 if (NetworkSystem.Instance.SessionIsPrivate)
                 {
                     GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
                     GorillaTagger.Instance.offlineVRRig.StartCoroutine(Rejoin());
                 }
             }, null, true);
        }

        public static void KickAll(byte Code)
        {
            foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerList)
            {
                if (NetworkSystem.Instance.SessionIsPrivate)
                {
                    LastRoom = PhotonNetwork.CurrentRoom.Name;
                    PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                    PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                    object[] groupJoinSendData = new object[2];
                    groupJoinSendData[0] = PhotonNetworkController.Instance.shuffler;
                    groupJoinSendData[1] = PhotonNetworkController.Instance.keyStr;

                    GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Add(plr.UserId);
                    RoomSystem.SendEvent(Code, groupJoinSendData, new NetEventOptions { TargetActors = new int[] { plr.actorNumber } }, true);
                    PhotonNetwork.SendAllOutgoingCommands();
                    if (Code == 4) PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaNetworking.GorillaComputer.instance.GetSelectedMapJoinTrigger(), JoinType.JoinWithNearby);
                    GorillaTagger.Instance.offlineVRRig.StartCoroutine(Rejoin());
                }
            }

        }



        public static string LastRoom = "";
        public static IEnumerator Rejoin()
        {
            yield return new WaitForSeconds(4f);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(LastRoom, GorillaNetworking.JoinType.Solo);
        }
        #endregion

        #region Laggers / Stutters
        public static void LagGun()
        {
            StartBothGuns(() =>
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.5f;
                    for (int i = 0; i < 230; i++)
                    {
                        Hashtable table = new Hashtable();
                        table[float.NaN] = float.NaN;
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(186, table,
                        new RaiseEventOptions { TargetActors = new int[] { TargetPlayer.ActorNumber } }, SendOptions.SendUnreliable);
                    }
                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }, null, true);
        }

        public static void LagAll()
        {
            if (Time.time > Delay)
            {
                Delay = Time.time + 0.5f;
                for (int i = 0; i < 230; i++)
                {
                    PhotonNetwork.RPC(FriendshipGroupDetection.Instance.photonView, "AddPartyMembers", RpcTarget.Others, false, new object[3]);
                }
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }

        public static void StutterGun()
        {
            StartBothGuns(() =>
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 3.5f;
                    for (int i = 0; i < 1000; i++)
                    {
                        Hashtable table = new Hashtable();
                        table[float.NaN] = float.NaN;
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(186, table,
                        new RaiseEventOptions { TargetActors = new int[] { TargetPlayer.ActorNumber } }, SendOptions.SendUnreliable);
                    }
                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }, null, true);
        }

        public static void StutterAll()
        {
            if (Time.time > Delay)
            {
                Delay = Time.time + 3.5f;
                for (int i = 0; i < 1000; i++)
                {
                    Hashtable table = new Hashtable();
                    table[float.NaN] = float.NaN;
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(186, table,
                    new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendUnreliable);
                }
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
        #endregion

        #region Misc 
        public static void InvisJoinGun()
        {
            StartBothGuns(() =>
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(TargetPlayer.actorNumber);
            }, null, true);
        }

        public static void InvisJoinAll()
        {
            foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerList)
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(plr.actorNumber);
            }
        }

        
        public static void FreezeServer()
        {
            if (PhotonNetwork.InRoom)
            {
                var gorillaNotInstance = GorillaNot.instance;
                var photonView = PunExtensions.GetPhotonView(gorillaNotInstance.gameObject);
                var localPlayer = NetworkSystem.Instance.LocalPlayer;

                var gorillaNotType = GorillaNot.instance;

                var lowestActorNumberField = GorillaNot.instance.lowestActorNumber;
                var refreshRPCsMethod = GorillaNot.instance.RefreshRPCs;
                var sendReportField = GorillaNot.instance._sendReport;
                var suspiciousPlayerIdField = GorillaNot.instance._suspiciousPlayerId;
                var suspiciousPlayerNameField = GorillaNot.instance._suspiciousPlayerName;

                lowestActorNumberField = localPlayer.ActorNumber;
                refreshRPCsMethod.Invoke();
                sendReportField = false;

                var randomPlayer = RigManager.GetRandomPlayer(false);

                suspiciousPlayerIdField = randomPlayer.UserId;
                suspiciousPlayerNameField = randomPlayer.NickName;

                gorillaNotInstance.testAssault = false;

                var webFlags = new WebFlags(1);
                var eventOptions = new NetEventOptions
                {
                    TargetActors = new[] { localPlayer.ActorNumber },
                    Reciever = NetEventOptions.RecieverTarget.master,
                    Flags = webFlags
                };

                var cachedPlayers = gorillaNotInstance.cachedPlayerList;
                string[] cachedUserIds = new string[cachedPlayers.Length];
                for (int i = 0; i < cachedPlayers.Length; i++)
                {
                    cachedUserIds[i] = cachedPlayers[i].UserId;
                }

                object[] eventData = new object[7]
                {
                    NetworkSystem.Instance.RoomStringStripped(),
                    cachedUserIds,
                    PhotonNetwork.MasterClient.UserId,
                    randomPlayer.UserId,
                    randomPlayer.NickName,
                    "",
                    NetworkSystemConfig.AppVersion
                };

                NetworkSystemRaiseEvent.RaiseEvent(51, eventData, eventOptions, reliable: true);
            }


        }

        #endregion

        #region SnowBalls  

        public static GrowingSnowballThrowable GetGrowingSnowballThrowable()
        {
            return GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingStuffingRightAnchor(Clone)").transform.Find("LMAUP. RIGHT.").GetComponent<GrowingSnowballThrowable>();
        }

        public static async void SpawnSnowball(Vector3 velocity, Vector3 position, int size, RaiseEventOptions options)
        {
            if (!projModsEnabled) EnableAllProjs();
            GrowingSnowballThrowable SnowballThrowable = GetGrowingSnowballThrowable();
            if (!SnowballThrowable.gameObject.activeSelf)
            {
                SnowballThrowable.SetSnowballActiveLocal(true);
                SnowballThrowable.transform.SetPositionAndRotation(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, new Quaternion());
            }

            PhotonNetwork.RaiseEvent(176, new object[] { SnowballThrowable.changeSizeEvent._eventId, size }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = false, });
            PhotonNetwork.RaiseEvent(176, new object[] { SnowballThrowable.snowballThrowEvent._eventId, position, velocity, SnowballThrowable.LaunchSnowballLocal(position, velocity, 0).myProjectileCount }, options, new SendOptions { Reliability = false, });
        }

        public static void SnowballLauncher(int Size)
        {
            if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.16f;
                    SpawnSnowball(-GorillaTagger.Instance.rightHandTransform.forward * -38, GorillaTagger.Instance.rightHandTransform.position, Size, new RaiseEventOptions { Receivers = ReceiverGroup.All });
                }
            }

            else if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.16f;
                    SpawnSnowball(-GorillaTagger.Instance.leftHandTransform.forward * -38, GorillaTagger.Instance.leftHandTransform.position, Size, new RaiseEventOptions { Receivers = ReceiverGroup.All });
                }
            }
            else
            {
                GetGrowingSnowballThrowable().SetSnowballActiveLocal(false);
            }
        }

        public static void SnowballFlingGun(Vector3 Offset, int Size)
        {
            StartBothGuns(() =>
            {
                if (Time.time > Competitive.Delay)
                {
                    Competitive.Delay = Time.time + 0.4f;
                    Serilization.SmoothSerilize(new Serilization.PlayerTransformData
                    {
                        BodyPos = TargetRig.transform.position,
                        LeftHandPosition = new Vector3(0, -999, 0),
                        RightHandPosition = new Vector3(0, -999, 0),
                        RightHandRotation = TargetRig.rightHandTransform.transform.rotation,
                        LeftHandRotation = TargetRig.leftHandTransform.transform.rotation,
                    }, new int[] { TargetPlayer.ActorNumber });
                    SpawnSnowball(new Vector3(0, -49, 0), TargetRig.transform.position + Offset, Size, new RaiseEventOptions { TargetActors = new int[] { TargetPlayer.ActorNumber } });
                }
            }, null, true);
            if (TargetRig == null || TargetPlayer == null)
            {
                GetGrowingSnowballThrowable().SetSnowballActiveLocal(false);
                if (Time.time > Competitive.Delay)
                {
                    Competitive.Delay = Time.time + 0.1f;
                    ResetSerilization();
                }
            }
        }


        public static void SnowballParticleGun()
        {
            StartBothGuns(() =>
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.1f;
                    RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, TargetPlayer);
                }
            }, null, true);
        }


        public static bool projModsEnabled = false;
        public static readonly List<GameObject> enabledHoldables = new List<GameObject>();

        public static readonly string[] holdableNames =
        {
            "GrowingSnowballRightAnchor(Clone)/LMACF. RIGHT.",
            "GrowingSnowballLeftAnchor(Clone)/LMACF. LEFT.",
            "GrowingSnowballRightAnchor(Clone)",
            "WaterBalloonRightAnchor(Clone)/LMAEY. RIGHT.",
            "VotingRockAnchor_RIGHT(Clone)/LMAMT. RIGHT.",
            "BucketGiftFunctionalAnchor_Right(Clone)/LMAHR. RIGHT.",
            "ScienceCandyRightAnchor(Clone)/LMAIF. RIGHT.",
            "FishFoodRightAnchor(Clone)/LMAIP. RIGHT.",
            "TrickTreatFunctionalAnchorRIGHT Variant(Clone)/LMAMO. RIGHT.",
            "LavaRockAnchor(Clone)/LMAGE. RIGHT.",
            "AppleRightAnchor(Clone)/LMAMV.",
            "BookRightAnchor(Clone)/LMAQA. RIGHT.",
            "CoinRightAnchor(Clone)/LMAQC.",
            "EggRightHand_Anchor Variant(Clone)/LMAPS. RIGHT.",
            "IceCreamRightAnchor(Clone)/LMARA. LEFT.",
            "HotDogRightAnchor(Clone)/LMARC.",
            "Fireworks_Anchor Variant_Right Hand(Clone)/LMAQU. LEFT.",
            "TurkeyLegRightAnchor(Clone)/LMAUR. RIGHT.",
            "GrowingStuffingRightAnchor(Clone)/LMAUP. RIGHT.",
            "GrowingMashedPotatoRightAnchor(Clone)/LMAUH. RIGHT.",
            "LayerDipRightAnchor(Clone)/LMAUF. RIGHT.",
            "CornRightAnchor(Clone)/LMAUT. RIGHT.",
            "ChipsRightAnchor(Clone)/LMAUC. RIGHT.",
            "BerryPieRightAnchor(Clone)/LMAUL. RIGHT.",
            "ApplePieRightAnchor(Clone)/LMAUJ. RIGHT.",
            "Papers_Anchor Variant_Right Hand(Clone)/LMASG. RIGHT.",
            "IceCreamScoopRightAnchor(Clone)/LMASD. RIGHT.",

            "FireworkMortarRightAnchor(Clone)/LMAEW. RIGHT.",
            "SalsaRightAnchor(Clone)/LMAUD. RIGHT.",
            "PumpkinPieRightAnchor(Clone)/LMAUN.",
            "GoalpostFootball_Anchor_RightHand(Clone)/LMATL.",
            "PopcornBall_Anchor_Right(Clone)/LMATP.",
            "CrackedPlate_Lump_Projectile_Anchor_RIGHT(Clone)/LMAUA.",
            "PortableBonfire_Sticks_Anchor_RightHand(Clone)/LMATY."
        };


        public const string holdablesPath = "Player Objects/Local VRRig/Local Gorilla Player/Holdables/";
        public const string rightHandPath = "Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/";

        public static void EnableAllProjs()
        {
            enabledHoldables.Clear();

            foreach (string name in holdableNames)
            {
                GameObject obj = GameObject.Find(holdablesPath + name);
                if (obj != null)
                {
                    obj.SetActive(true);
                    enabledHoldables.Add(obj);
                }
            }

            foreach (string name in holdableNames)
            {
                GameObject obj = GameObject.Find(rightHandPath + name);
                if (obj != null) obj.SetActive(false);
            }

            projModsEnabled = true;
        }
        #endregion

        #region HoverBoard

        public static bool Hover = false;
        public static void SpawnBoard(Vector3 pos, Quaternion rot, Vector3 vel, Color col)
        {
            
            FreeHoverboardManager.instance.photonView.RPC("DropBoard_RPC", RpcTarget.All, new object[]
                {
                    Hover,
                    BitPackUtils.PackWorldPosForNetwork(pos),
                    BitPackUtils.PackQuaternionForNetwork(rot),
                    BitPackUtils.PackWorldPosForNetwork(vel),
                    BitPackUtils.PackWorldPosForNetwork(Vector3.zero),
                    BitPackUtils.PackColorForNetwork(col)
                });
            Hover = !Hover;
        }

        public static void GrabRainbowHoverBoard()
        {
            if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.2f;
                    SpawnBoard(GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, Addons.SmoothRGBColor());
                }
            }
            else if (GetInput(InputType.LGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.2f;
                    SpawnBoard(GorillaTagger.Instance.leftHandTransform.position, Quaternion.identity, Vector3.zero, Addons.SmoothRGBColor());
                }
            }
        }

        public static void GrabHoverBoard()
        {
            if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.2f;
                    SpawnBoard(GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, GorillaTagger.Instance.offlineVRRig.playerColor);
                }
            }
            else if (GetInput(InputType.LGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.2f;
                    SpawnBoard(GorillaTagger.Instance.leftHandTransform.position, Quaternion.identity, Vector3.zero, GorillaTagger.Instance.offlineVRRig.playerColor);
                }
            }
        }

        public static void LaunchHoverBoard()
        {
            if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.5f;
                    SpawnBoard(GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, GorillaTagger.Instance.rightHandTransform.forward * 10, GorillaTagger.Instance.offlineVRRig.playerColor);
                }
            }
            else if (GetInput(InputType.LGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.5f;
                    SpawnBoard(GorillaTagger.Instance.leftHandTransform.position, Quaternion.identity, GorillaTagger.Instance.leftHandTransform.forward * 10, GorillaTagger.Instance.offlineVRRig.playerColor);
                }
            }
        }

        public static void LaunchRainbowHoverBoard()
        {
            if (GetInput(InputType.RGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.5f;
                    SpawnBoard(GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, GorillaTagger.Instance.rightHandTransform.forward * 10, Addons.SmoothRGBColor());
                }
            }
            else if (GetInput(InputType.LGrip))
            {
                if (Time.time > Delay)
                {
                    Delay = Time.time + 0.5f;
                    SpawnBoard(GorillaTagger.Instance.leftHandTransform.position, Quaternion.identity, GorillaTagger.Instance.leftHandTransform.forward * 10, Addons.SmoothRGBColor());
                }
            }
        }

        public static void OrbitHoverBoard()
        {
            if (Time.time > Delay)
            {
                Delay = Time.time + 0.2f;
                SpawnBoard(GorillaTagger.Instance.leftHandTransform.position + Addons.Orbit(), Quaternion.identity, new Vector3(), GorillaTagger.Instance.offlineVRRig.playerColor);
            }
        }
        public static void OrbitRainbowHoverBoard()
        {
            if (Time.time > Delay)
            {
                Delay = Time.time + 0.2f;
                SpawnBoard(GorillaTagger.Instance.leftHandTransform.position + Addons.Orbit(), Quaternion.identity, new Vector3(), Addons.SmoothRGBColor());
            }
        }
        #endregion

    }
}

