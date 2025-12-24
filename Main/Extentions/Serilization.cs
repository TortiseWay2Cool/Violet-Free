using ExitGames.Client.Photon;
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
using UnityEngine;

namespace VioletTemplate.Main.Extentions
{
    internal class Serilization
    {
        public struct PlayerTransformData
        {
            public Vector3 BodyPos;
            public Vector3 RightHandPosition;
            public Vector3 LeftHandPosition;
            public Quaternion RightHandRotation;
            public Quaternion LeftHandRotation;

            public PlayerTransformData(Vector3 bodyPos, Vector3 rightHandPos, Vector3 leftHandPos, Quaternion rightHandRotation, Quaternion leftHandRotation)
            {
                BodyPos = bodyPos;
                RightHandPosition = rightHandPos;
                LeftHandPosition = leftHandPos;
                RightHandRotation = rightHandRotation;
                LeftHandRotation = leftHandRotation;
            }
        }

        public static void SendUpdateData(bool ChangePosition, PlayerTransformData Data, int[] TargetActors, int customTimestamp = -1)
        {
            foreach (var targetActor in TargetActors)
            {
                foreach (var kvp in PhotonNetwork.photonViewList)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        var view = kvp.Value;
                        if (view.Synchronization == ViewSynchronization.Off || !view.IsMine || !view.isActiveAndEnabled || PhotonNetwork.blockedSendingGroups.Contains(view.Group))
                            continue;

                        var dataList = PhotonNetwork.OnSerializeWrite(view);

                        var batch = new PhotonNetwork.RaiseEventBatch
                        {
                            Reliable = view.Synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable,
                            Group = view.Group
                        };

                        var viewBatch = new PhotonNetwork.SerializeViewBatch(batch, 2);
                        viewBatch.Add(dataList);

                        viewBatch.ObjectUpdates[0] = customTimestamp == -1 ? PhotonNetwork.ServerTimestamp : customTimestamp;
                        viewBatch.ObjectUpdates[1] = PhotonNetwork.currentLevelPrefix != 0 ? (byte?)PhotonNetwork.currentLevelPrefix : null;


                        if (ChangePosition)
                        {
                            long packedLeft = BitPackUtils.PackHandPosRotForNetwork(Data.LeftHandPosition, Data.LeftHandRotation);
                            long packedRight = BitPackUtils.PackHandPosRotForNetwork(Data.RightHandPosition, Data.RightHandRotation);
                            long Body = BitPackUtils.PackWorldPosForNetwork(Data.BodyPos);

                            void Replace(object obj, ref int positionIndex)
                            {
                                if (obj is IDictionary dict)
                                {
                                    foreach (var key in dict.Keys.Cast<object>().ToList())
                                    {
                                        var val = dict[key];
                                        if (val is long)
                                        {
                                            if (positionIndex == 0) dict[key] = packedLeft;
                                            else if (positionIndex == 1) dict[key] = packedRight;
                                            else if (positionIndex == 2) dict[key] = Body;
                                            positionIndex++;
                                        }
                                        else
                                            Replace(val, ref positionIndex);
                                    }
                                }
                                else if (obj is IList list)
                                {
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        var val = list[i];
                                        if (val is long)
                                        {
                                            if (positionIndex == 0) list[i] = packedLeft;
                                            else if (positionIndex == 1) list[i] = packedRight;
                                            else if (positionIndex == 2) list[i] = Body;
                                            positionIndex++;
                                        }
                                        else
                                            Replace(val, ref positionIndex);
                                    }
                                }
                            }

                            int positionIndex = 0;
                            Replace(viewBatch.ObjectUpdates, ref positionIndex);

                        }

                        var raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { targetActor } };
                        PhotonNetwork.RaiseEventInternal((byte)(viewBatch.Batch.Reliable ? 206 : 201), viewBatch.ObjectUpdates.ToArray(), raiseEventOptions, viewBatch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
                        PhotonNetwork.SendAllOutgoingCommands();
                        viewBatch.Clear();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PhotonNetwork), "SendSerializeViewBatch")]
        public class SerilizePatch
        {
            public static bool enabled = false;

            public static bool Prefix(PhotonNetwork.SerializeViewBatch batch)
            {
                if (enabled || batch == null)
                {
                    return true;
                }
                if (batch.ObjectUpdates != null && batch.ObjectUpdates.Count > 2)
                {

                    RaiseEventOptions options = PhotonNetwork.serializeRaiseEvOptions;
                    options.InterestGroup = batch.Batch.Group;

                    batch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp;
                    batch.ObjectUpdates[1] = PhotonNetwork.currentLevelPrefix != 0 ? (object)PhotonNetwork.currentLevelPrefix : null;

                    PhotonNetwork.NetworkingClient.OpRaiseEvent(
                        (byte)(batch.Batch.Reliable ? 206 : 201),
                        batch.ObjectUpdates,
                        options,
                        batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable
                    );

                    batch.Clear();
                    return false;
                }
                return true;
            }
        }

        public static void SmoothSerilize(PlayerTransformData transformData, int[] targetActors)
        {
            SerilizePatch.enabled = true;
            SetTick(9999);
            PhotonNetwork.SendAllOutgoingCommands();
            photonView = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
            SendUpdateData(true, transformData, targetActors);
            PhotonNetwork.SendAllOutgoingCommands();
            typeof(PhotonView).GetMethod("OnSerialize", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(photonView, new object[] { null, null });
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendAcksOnly();
        }


        public static PhotonView photonView;
        public static void SendPlayerFast(VRRig plr, Vector3 position, Action action, bool SendToMaster = true)
        {
            if (SendToMaster)
            {
                
                SendUpdateData(true,
                    new PlayerTransformData(
                        plr.transform.position,
                        plr.rightHandTransform.position,
                        plr.leftHandTransform.position,
                        plr.rightHandTransform.rotation,
                        plr.leftHandTransform.rotation),
                    new int[] { PhotonNetwork.MasterClient.ActorNumber });
                PhotonNetwork.SendAllOutgoingCommands();

                photonView = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
                action?.Invoke();
                PhotonNetwork.SendAllOutgoingCommands();
                PhotonNetwork.RunViewUpdate();
                typeof(PhotonView).GetMethod("OnSerialize", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(photonView, new object[] { null, null });
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendAcksOnly();
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = position;
                PhotonNetwork.SendAllOutgoingCommands();

                photonView = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView();
                action?.Invoke();
                PhotonNetwork.SendAllOutgoingCommands();
                PhotonNetwork.RunViewUpdate();

                typeof(PhotonView).GetMethod("OnSerialize", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(photonView, new object[] { null, null });
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendAcksOnly();
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void SetTick(float tick)
        {
            Traverse.Create(GameObject.Find("PhotonMono").GetComponent<PhotonHandler>()).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * tick));
            PhotonHandler.SendAsap = true;
            PhotonHandler.instance.nextSendTickCountOnSerialize = (int)(Time.realtimeSinceStartup * tick);
        }

        public static float ReturnNotKickableAmount()
        {
            if (PhotonNetwork.InRoom)
            {
                return 0.007f * PhotonNetwork.CurrentRoom.PlayerCount;
            }
            else
            {
                return 0;
            }
        }


        public static void ResetSerilization()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                SerilizePatch.enabled = false;
                SendUpdateData(false, new PlayerTransformData { }, new int[] { rig.creator.ActorNumber });
                SendPlayerFast(rig, GorillaTagger.Instance.offlineVRRig.transform.position, () => { }, true);
            }

        }

    }
}
