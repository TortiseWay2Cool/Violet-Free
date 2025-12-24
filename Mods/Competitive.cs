using Fusion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
using VioletTemplate.Main.Extentions;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.Serilization;
namespace VioletTemplate.Mods
{
    internal class Competitive
    {
        public static float AuraRange = 2f;
        public static float Delay;  
        public static int Tagindex = 0;  
        public static string LastRoom;
        public static void TagAll()
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Master.Tag(player);
                }
                else
                {
                    if (player != PhotonNetwork.LocalPlayer)
                    {
                        GorillaTagger.Instance.StartCoroutine(DelayedTagAll(player));
                    }
                }
            }
        }

        public static IEnumerator DelayedTagAll(Photon.Realtime.Player plr)
        {
            yield return new WaitWhile(() => RigManager.IsWholeLobbyTagged());
            VRRig targetRig = RigManager.GetVRRigFromPlayer(plr);
            PhotonView v = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
            SendPlayerFast(targetRig, new Vector3(0, 0, 0), () =>
            {
                v.RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { plr.ActorNumber });
            });
        }

        public static void TagSelf()
        {
            VRRig localRig = RigManager.GetVRRigFromPlayer(PhotonNetwork.LocalPlayer);
            if (!RigManager.RigIsInfected(localRig))
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Master.Tag(PhotonNetwork.LocalPlayer);
                    }
                    else
                    {
                        if (Time.time > Delay)
                        {
                            Delay = Time.time + 0.5f;
                            if (player != PhotonNetwork.LocalPlayer && RigManager.RigIsInfected(RigManager.GetVRRigFromPlayer(player)))
                            {
                                SmoothSerilize(new PlayerTransformData
                                {
                                    BodyPos = RigManager.GetVRRigFromPlayer(player).transform.position
                                }, new int[] { player.ActorNumber });
                            }
                        }
                    }
                }
            }
        }

        public static void NoTagOnJoin()
        {
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
        }


        public static void UnTagSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Master.UnTag(PhotonNetwork.LocalPlayer);
            }
            else
            {
                if (PhotonNetwork.InRoom)
                {
                    LastRoom = PhotonNetwork.CurrentRoom.Name;
                    PhotonNetwork.Disconnect();
                }
                else
                {

                    NoTagOnJoin();
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(LastRoom, GorillaNetworking.JoinType.Solo);
                }

            }
        }


        public static void AntiTag()
        {
            foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerListOthers)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Master.UnTag(PhotonNetwork.LocalPlayer);
                }
                else
                {
                    VRRig rig = RigManager.GetVRRigFromPlayer(plr);

                    if (RigManager.RigIsInfected(rig))
                    {
                        if (Time.time > Delay)
                        {
                            Delay = Time.time + 0.1f;
                            if (rig != GorillaTagger.Instance.offlineVRRig && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 2.25f))
                            {
                                if (!RigManager.RigIsInfected(GorillaTagger.Instance.offlineVRRig))
                                {
                                    SmoothSerilize(new PlayerTransformData { BodyPos = GorillaTagger.Instance.transform.position + new Vector3(0, -8, 0) }, new int[] { plr.ActorNumber });
                                }
                                else
                                {
                                    if (Time.time > Delay)
                                    {
                                        Delay = Time.time + 0.5f;
                                        ResetSerilization();
                                    }
                                }
                            }
                            else
                            {
                                if (Time.time > Delay)
                                {
                                    Delay = Time.time + 0.5f;
                                    ResetSerilization();
                                }
                            }
                        }
                        else
                        {
                            if (Time.time > Delay)
                            {
                                Delay = Time.time + 0.5f;
                                ResetSerilization();
                            }
                        }
                    }
                }
            }
        }


        public static void TagGun()
        {
            StartBothGuns(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Master.Tag(TargetPlayer);
                }
                else
                {
                    if (!RigManager.RigIsInfected(GunLib.TargetRig))
                    {
                        PhotonView v = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
                        SendPlayerFast(TargetRig, new Vector3(0, 0, 0), () =>
                        {
                            v.RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { TargetPlayer.ActorNumber });
                        });
                    }
                }
            }, () => { }, true);
        }

        public static void TagAura()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != GorillaTagger.Instance.offlineVRRig && Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < AuraRange)
                {
                    if (rig != GorillaTagger.Instance.offlineVRRig && !RigManager.RigIsInfected(rig))
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Master.Tag(RigManager.GetPlayerFromVRRig(rig));
                        }
                        else
                        {
                            PhotonView v = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
                            v.RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { RigManager.GetPlayerFromVRRig(rig).ActorNumber });
                        }
                    }
                }
            }
        }

        public static void InfectionESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.mainSkin.material.color = RigManager.RigIsInfected(vrrig) ? Color.red : Color.green;
            }
        }

        public static void InfectionTracers()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                GameObject line = new GameObject("Line");
                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                lineRenderer.SetPosition(1, vrrig.transform.position);
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                if (RigManager.RigIsInfected(vrrig))
                {
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                else
                {
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                }
                lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }
        }
    }
}