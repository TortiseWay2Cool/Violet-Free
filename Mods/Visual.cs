using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VioletTemplate.Main.Extentions;
using static Fusion.Sockets.NetBitBuffer;
using Random = UnityEngine.Random;

namespace VioletTemplate.Mods
{
    internal class Visual : MonoBehaviour
    {
        public static void NameTags()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject textObject = new GameObject("NameTags");
                    textObject.transform.parent = rig.transform;
                    TextMeshPro tmpText = textObject.AddComponent<TextMeshPro>();
                    tmpText.text = RigManager.GetPlayerFromVRRig(rig).nickName;
                    tmpText.fontSize = 0.16f;
                    tmpText.fontStyle = FontStyles.Bold;
                    Addons.ApplyGradient(
                         tmpText,
                         Color.red,
                         Color.green,
                         Color.green,
                         Color.red
                     );
                    tmpText.alignment = TextAlignmentOptions.Center;
                    tmpText.enableAutoSizing = true;
                    textObject.transform.localPosition = new Vector3(0, 0.5f, 0);
                    textObject.transform.localRotation = Quaternion.Euler(180, rig.transform.rotation.x, 180f);
                    textObject.transform.localScale = new Vector3(0.09f, 0.1f, 0.75f);

                    Destroy(textObject, Time.deltaTime);
                }
            }
        }

        public static void FPSTags()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject textObject = new GameObject("FPSTags");
                    textObject.transform.parent = rig.transform;
                    TextMeshPro tmpText = textObject.AddComponent<TextMeshPro>();
                    tmpText.text = "FPS: " + rig.fps.ToString();
                    tmpText.fontSize = 0.16f;
                    tmpText.fontStyle = FontStyles.Bold;
                    Addons.ApplyGradient(
                        tmpText,
                        Color.red,
                        Color.green,
                        Color.green,
                        Color.red
                    );
                    tmpText.alignment = TextAlignmentOptions.Center;
                    tmpText.enableAutoSizing = true;
                    textObject.transform.localPosition = new Vector3(0, 0.5f, 0);
                    textObject.transform.localRotation = Quaternion.Euler(180, rig.transform.rotation.x, 180f);
                    textObject.transform.localScale = new Vector3(0.09f, 0.1f, 0.75f);

                    Destroy(textObject, Time.deltaTime);
                }
            }
        }
        public static void PositionTags()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject textObject = new GameObject("FPSTags");
                    textObject.transform.parent = rig.transform;

                    TextMeshPro tmpText = textObject.AddComponent<TextMeshPro>();
                    tmpText.text = "Position: " + rig.transform.position.ToString();
                    tmpText.fontSize = 0.16f;
                    tmpText.fontStyle = FontStyles.Bold;
                    tmpText.alignment = TextAlignmentOptions.Center;
                    tmpText.enableAutoSizing = true;

                    Addons.ApplyGradient(
                        tmpText,
                        Color.red,
                        Color.green,
                        Color.green,
                        Color.red
                    );

                    textObject.transform.localPosition = new Vector3(0, 0.5f, 0);
                    textObject.transform.localRotation = Quaternion.Euler(180, rig.transform.rotation.x, 180f);
                    textObject.transform.localScale = new Vector3(0.09f, 0.1f, 0.75f);

                    Destroy(textObject, Time.deltaTime);
                }
            }
        }


        public static void Tracers()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject LineObject = new GameObject("Tracer");
                    LineObject.transform.parent = GorillaTagger.Instance.offlineVRRig.rightHandTransform;
                    LineRenderer Line = LineObject.AddComponent<LineRenderer>();
                    Line.positionCount = 2;
                    Line.startColor = Color.black;
                    Line.endColor = Color.black;
                    Color c = Line.material.color;
                    c.a = 0.8f;
                    Line.material.color = c;


                    Line.startWidth = 0.03f;
                    Line.endWidth = 0.03f;
                    Line.material.shader = Shader.Find("GUI/Text Shader");
                    Line.SetPosition(0, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position);
                    Line.SetPosition(1, rig.headMesh.transform.position);

                    GameObject LineObjectOut = new GameObject("Tracer");
                    LineObjectOut.transform.parent = GorillaTagger.Instance.offlineVRRig.rightHandTransform;
                    LineRenderer LineOut = LineObjectOut.AddComponent<LineRenderer>();
                    LineOut.positionCount = 2;
                    LineOut.startColor = Color.violet;
                    LineOut.endColor = Color.violet;


                    LineOut.startWidth = 0.035f;
                    LineOut.endWidth = 0.035f;
                    LineOut.material.shader = Shader.Find("UI/Default");
                    LineOut.SetPosition(0, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position);
                    LineOut.SetPosition(1, rig.headMesh.transform.position);

                    Destroy(LineObject, Time.deltaTime);
                    Destroy(LineObjectOut, Time.deltaTime);
                }
            }
        }

        public static void ESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.mainSkin.material.color = vrrig.playerColor;
            }
        }

        public static void RainbowESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.mainSkin.material.color = Addons.SmoothRGBColor();
            }
        }

        public static void InvisMonkes()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                vrrig.mainSkin.material.color = new Color32((byte)Random.Range(0,1), (byte)Random.Range(0, 1), (byte)Random.Range(0, 1), (byte)Random.Range(0, 1));
            }
        }

        public static void ThinMonkes()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.transform.localScale = new Vector3(1f, 1, 0.02f);
            }
        }

        public static void StubbyMonkes()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.transform.localScale = new Vector3(1f, 0.5f, 1);
            }
        }

        public static void BigMonkes()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.transform.localScale = new Vector3(2, 2, 2);
            }
        }

        public static void ResetMonkes()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public static void ResetESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig == GorillaTagger.Instance.offlineVRRig) continue;
                vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                vrrig.mainSkin.material.color = vrrig.playerColor;
            }
        }
    }
}

