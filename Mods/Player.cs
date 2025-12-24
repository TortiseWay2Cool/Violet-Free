using GorillaLocomotion.Swimming;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.PlayerManager;
using static VioletTemplate.Mods.Competitive;
using static VioletTemplate.Main.Extentions.Serilization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using VioletTemplate.Main.Extentions;
namespace VioletTemplate.Mods
{
    internal class Player
    {
        public static void GhostMonkey()
        {
            if (GetInput(InputType.RTrigger))
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void InvisMonkey()
        {
            if (GetInput(InputType.RTrigger))
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(0, 1000, 0);
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        private static bool ghostMonkeyEnabled = false;
        private static bool invisMonkeyEnabled = false;
        private static bool lastRSecondaryState = false;

        public static void ToggleGhostMonkey()
        {
            bool current = GetInput(InputType.RSecondary);
            if (current && !lastRSecondaryState) ghostMonkeyEnabled = !ghostMonkeyEnabled;
            GorillaTagger.Instance.offlineVRRig.enabled = !ghostMonkeyEnabled;
            lastRSecondaryState = current;
        }

        public static void ToggleInvisMonkey()
        {
            bool current = GetInput(InputType.RSecondary);
            if (current && !lastRSecondaryState) invisMonkeyEnabled = !invisMonkeyEnabled;
            GorillaTagger.Instance.offlineVRRig.enabled = !invisMonkeyEnabled;
            if (!GorillaTagger.Instance.offlineVRRig.enabled)
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(0, 1000, 0);
            lastRSecondaryState = current;
        }


        public static void GrabRig()
        {
            if (GetInput(InputType.RGrip))
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
            }
            else if (GetInput(InputType.LGrip))
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void ScareGun()
        {
            StartBothGuns(()=>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = TargetRig.transform.position + new Vector3(Random.Range(1, -1), Random.Range(1, -1), Random.Range(1, -1));
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }, true);
            
        }

        public static void ScareClosest()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig rig = RigManager.GetClosestVRRig();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
            }
            else
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        public static void FollowGun()
        {
            StartBothGuns(() =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = TargetRig.transform.position;
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }, true);
        }

        public static void FollowClosest()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig rig = RigManager.GetClosestVRRig();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
            }
            else
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        public static void OrbitGun()
        {
            StartBothGuns(() =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = TargetRig.transform.position + Addons.Orbit();
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }, true);
        }

        public static void OrbitClosest()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig rig = RigManager.GetClosestVRRig();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position + Addons.Orbit();
            }
            else
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        public static void SpazRig()
        {
            if (GetInput(InputType.RGrip))
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position + new Vector3(UnityEngine.Random.Range(1,-1), UnityEngine.Random.Range(1, -1), UnityEngine.Random.Range(1, -1));
            }
            else { GorillaTagger.Instance.offlineVRRig.enabled = true; }
        }

        public static async void StutterRig()
        {
            if (GetInput(InputType.RGrip))
            {
                Serilization.SendPlayerFast(GorillaTagger.Instance.offlineVRRig, GorillaTagger.Instance.transform.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)), null);
                await Task.Delay(1000);
                Serilization.ResetSerilization();
            }
            else { GorillaTagger.Instance.offlineVRRig.enabled = true; }
        }

        public static async void StutterRigGun()
        {
            StartBothGuns(async () =>
            {
                Serilization.SendUpdateData(true, new Serilization.PlayerTransformData
                {
                    BodyPos = GorillaTagger.Instance.transform.position + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100))
                }, new int[] { TargetPlayer.actorNumber });
                await Task.Delay(1000);
                Serilization.ResetSerilization();

            }, null, true);
        }

        public static void SpinHead(float x, float y, float z)
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset += new Vector3(x,y,z);
        }

        public static void ResetHeadSpin()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset = Vector3.zero;
        }

        public static void ChangeName(string name)
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.NetworkingClient.NickName = name;
            PhotonNetwork.NickName = name;
            GorillaComputer.instance.currentName = name;
            GorillaComputer.instance.savedName = name   ;
            PlayerPrefs.SetString("GorillaLocomotion.PlayerName", name);
        }

        public static void MaxQuestScore()
        {
            GorillaTagger.Instance.offlineVRRig.SetQuestScore(int.MaxValue);
        }

        public static void SpazQuestScore()
        {
            GorillaTagger.Instance.offlineVRRig.SetQuestScore(UnityEngine.Random.Range(0, 99999));
        }

        public static void SpoofColor()
        {
            for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
            {
                if (GorillaParent.instance.vrrigs[i] != GorillaTagger.Instance.offlineVRRig)
                {
                    int current = UnityEngine.Random.Range(0, colors.Count);
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RigManager.GetPlayerFromVRRig(GorillaParent.instance.vrrigs[i]), new object[] {
                    colors[current].r,
                    colors[current].g,
                    colors[current].b,
                });
                }
            }
        }

        public static void StumpRGB()
        {
            Color32 color = Addons.SmoothRGBColor();
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));
            GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
            PlayerPrefs.Save();
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b });
        }

        private static List<Color> colors = new List<Color>()
        {
            Color.red,Color.yellow,Color.green,Color.blue,Color.magenta,Color.cyan
        };

        public static async void Strobe()
        {
            int current = UnityEngine.Random.Range(0, colors.Count);
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(colors[current].r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(colors[current].g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(colors[current].b, 0f, 1f));
            GorillaTagger.Instance.UpdateColor(colors[current].r, colors[current].g, colors[current].b);
            PlayerPrefs.Save();
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { colors[current].r, colors[current].g, colors[current].b });
            await Task.Delay(40);
        }

        public static async void WalkOnWater(bool Enabled)
        {
            foreach (WaterVolume wv in Object.FindObjectsOfType(typeof(WaterVolume)))
            {
                if (Enabled)
                    wv.gameObject.layer = LayerMask.NameToLayer("Default");
                else
                    wv.gameObject.layer = LayerMask.NameToLayer("Water");
            }
        }

        #region Serilization
        public static void FollowAll()
        {
            if (Time.time > Competitive.Delay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != GorillaTagger.Instance.offlineVRRig)
                    {
                        Competitive.Delay = Time.time + Serilization.ReturnNotKickableAmount();

                        Serilization.SmoothSerilize(new Serilization.PlayerTransformData
                        {
                            BodyPos = rig.transform.position,
                            LeftHandPosition = new Vector3(0, -999, 0),
                            RightHandPosition = new Vector3(0, -999, 0),
                            RightHandRotation = rig.rightHandTransform.transform.rotation,
                            LeftHandRotation = rig.leftHandTransform.transform.rotation,

                        }, new int[] { rig.creator.ActorNumber });

                    }
                }


            }
        }

        public static void ScareAll()
        {
            if (Time.time > Competitive.Delay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != GorillaTagger.Instance.offlineVRRig)
                    {
                        Competitive.Delay = Time.time + Serilization.ReturnNotKickableAmount();

                        Serilization.SmoothSerilize(new Serilization.PlayerTransformData
                        {
                            BodyPos = rig.transform.position + new Vector3(Random.Range(1,-1), Random.Range(1, -1), Random.Range(1, -1)),
                            LeftHandPosition = new Vector3(0, -999, 0),
                            RightHandPosition = new Vector3(0, -999, 0),
                            RightHandRotation = rig.rightHandTransform.transform.rotation,
                            LeftHandRotation = rig.leftHandTransform.transform.rotation,

                        }, new int[] { rig.creator.ActorNumber });

                    }
                }


            }
        }

        public static void OrbitAll()
        {
            if (Time.time > Competitive.Delay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (rig != GorillaTagger.Instance.offlineVRRig)
                    {
                        Competitive.Delay = Time.time + Serilization.ReturnNotKickableAmount();

                        Serilization.SmoothSerilize(new Serilization.PlayerTransformData
                        {
                            BodyPos = rig.transform.position + new Vector3(Mathf.Cos(240f + ((float)Time.frameCount / 30)), 1, Mathf.Sin(240f + ((float)Time.frameCount / 30))),
                            LeftHandPosition = new Vector3(0, -999, 0),
                            RightHandPosition = new Vector3(0, -999, 0),
                            RightHandRotation = rig.rightHandTransform.transform.rotation,
                            LeftHandRotation = rig.leftHandTransform.transform.rotation,

                        }, new int[] { rig.creator.ActorNumber });

                    }
                }


            }
        }
        #endregion
    }
}
