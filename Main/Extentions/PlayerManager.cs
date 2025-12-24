using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VioletTemplate.Mods.Competitive;

namespace VioletTemplate.Main.Extentions
{
    public class PlayerManager
    {
        public enum InputType
        {
            RPrimary,
            LPrimary,

            RSecondary,
            LSecondary,

            RGrip,
            LGrip,

            RTrigger,
            LTrigger
        }

        public static bool GetInput(InputType input)
        {
            switch (input)
            {
                case InputType.RPrimary:
                    return ControllerInputPoller.instance.rightControllerPrimaryButton;
                case InputType.LPrimary:
                    return ControllerInputPoller.instance.leftControllerPrimaryButton;
                case InputType.RSecondary:
                    return ControllerInputPoller.instance.rightControllerSecondaryButton;
                case InputType.LSecondary:
                    return ControllerInputPoller.instance.leftControllerSecondaryButton;
                case InputType.RGrip:
                    return ControllerInputPoller.instance.rightGrab;
                case InputType.LGrip:
                    return ControllerInputPoller.instance.leftGrab;
                case InputType.RTrigger:
                    return ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f;
                case InputType.LTrigger:
                    return ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f;
                default:
                    return false;
            }
        }

        public static Transform GetRightHand()
        {
            return GorillaTagger.Instance.rightHandTransform;
        }

        public static Transform GetLeftHand()
        {
            return GorillaTagger.Instance.leftHandTransform;
        }

        public static void KawaiiRPC()
        {
            Hashtable RpcHashtables = new Hashtable();
            RpcHashtables[0] = GorillaTagger.Instance.myVRRig.ViewID;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(200, RpcHashtables, new RaiseEventOptions { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { PhotonNetwork.LocalPlayer.ActorNumber } }, SendOptions.SendReliable);
        }

        public static float delay;
        public static void AutoFlushRPCS()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Time.time > delay)
                {
                    delay = Time.time + 0.5f;
                    KawaiiRPC();
                }
            }
        }

        public static void AntiReport()
        {
            if (PhotonNetwork.InRoom)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        Vector3 rHand = vrrig.rightHandTransform.position;
                        Vector3 lHand = vrrig.leftHandTransform.position;
                        rHand = vrrig.rightHandTransform.position + vrrig.rightHandTransform.forward * 0.125f;
                        lHand = vrrig.leftHandTransform.position + vrrig.leftHandTransform.forward * 0.125f;
                        float range = 0.6f;
                        foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (gorillaPlayerScoreboardLine.linePlayer == NetworkSystem.Instance.LocalPlayer)
                            {
                                Vector3 reportButton = gorillaPlayerScoreboardLine.reportButton.gameObject.transform.position + new Vector3(0f, 0.001f, 0.0004f);
                                if (Vector3.Distance(reportButton, lHand) < range)
                                {
                                    PhotonNetwork.Disconnect();
                                }
                                if (Vector3.Distance(reportButton, rHand) < range)
                                {
                                    PhotonNetwork.Disconnect();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
