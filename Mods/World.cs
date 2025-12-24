using GorillaLocomotion.Gameplay;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.PlayerManager;
using static VioletTemplate.Main.Extentions.Serilization;
using static VioletTemplate.Mods.Competitive;

namespace VioletTemplate.Mods
{
    public class World : MonoBehaviour
    {
        public static void FlingRopes()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (Time.time > Delay + 0.1f)
                {
                    Delay = Time.time;
                    foreach (GorillaRopeSwing ropes in GameObject.FindObjectsOfType<GorillaRopeSwing>())
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, ropes.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true);
                    }
                }
            }
        }

        public static void UpRopes()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (Time.time > Delay + 0.1f)
                {
                    Delay = Time.time;
                    foreach (GorillaRopeSwing ropes in GameObject.FindObjectsOfType<GorillaRopeSwing>())
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, ropes.ropeId, 1, new Vector3(0, 100, 0), true);
                    }
                }
            }
        }

        public static void DownRopes()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (Time.time > Delay + 0.1f)
                {
                    Delay = Time.time;
                    foreach (GorillaRopeSwing ropes in GameObject.FindObjectsOfType<GorillaRopeSwing>())
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, ropes.ropeId, 1, new Vector3(0, -100, 0), true);
                    }
                }
            }
        }

        public static void FlingRopeGun()
        {
           StartBothGuns(() =>
            {
                if (Time.time > Delay + 0.1f)
                {
                    Delay = Time.time;
                    GorillaRopeSwing ropes = rayHit.collider.GetComponentInParent<GorillaRopeSwing>();
                    RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, ropes.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true);
                }
            }, null, false);
        }

    }
}
