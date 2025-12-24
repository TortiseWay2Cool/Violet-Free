using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VioletTemplate.Main.Extentions
{
    public class Addons : MonoBehaviour
    {
        public static GameObject outlineObj;
        public static void AddOutline(GameObject obj, Color clr)
        {
            var outlineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(outlineObj.GetComponent<Rigidbody>());
            Destroy(outlineObj.GetComponent<BoxCollider>());
            outlineObj.transform.parent = obj.transform;
            outlineObj.transform.rotation = Quaternion.identity;
            outlineObj.transform.localPosition = obj.transform.localPosition;
            outlineObj.transform.localScale = obj.transform.localScale + new Vector3(-0.01f, 0.0215f, 0.0215f);
            outlineObj.GetComponent<MeshRenderer>().material.color = clr;

        }

        public static void ChangeBoardMaterial(string parentPath, int targetIndex, Material newMaterial, ref Material originalMat)
        {
            GameObject parent = GameObject.Find(parentPath);
            int currentIndex = 0;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject childObj = parent.transform.GetChild(i).gameObject;
                if (childObj.name.Contains("UnityTempFile"))
                {
                    currentIndex++;
                    if (currentIndex == targetIndex)
                    {
                        Renderer renderer = childObj.GetComponent<Renderer>();
                        if (originalMat == null)
                            originalMat = renderer.material;

                        renderer.material = newMaterial;
                        break;
                    }
                }
            }
        }

        public static bool TargetPlrTagger;

        public static string PlayerInfo
        {
            get
            {
                var info = new StringBuilder();
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    var rig = RigManager.GetVRRigFromPlayer(player);
                    string master = player.IsMasterClient ? " (Master)" : "";
                    info.AppendLine($"{player.NickName} (Actor: {player.ActorNumber}){master} (FPS: {rig?.fps ?? 0})");
                }
                return info.ToString();
            }
        }

        private static Material originalMat1;
        private static Material originalMat2;
        private static Material originalMat3;
        private static Color32 _cachedRgbColor;

        public static void Board()
        {
            try
            {
                VertexGradient rgbGradient = GetSlidingRGBGradient();

                Shader shader = Shader.Find("GorillaTag/UberShader");
                if (!shader) return;

                Material colorMaterial = new Material(shader);
                colorMaterial.color = Color.black;
                colorMaterial.SetFloat("_Mode", 2f);

                Material mat = new Material(shader);
                mat.color = Color.black;
                mat.SetFloat("_Mode", 2f);

                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg")
                    ?.GetComponent<Renderer>().material = mat;

                string motdBodyPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText";
                string motdHeadingPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText";
                string cocHeadingPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText";
                string cocBodyPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData";
                string cocTextPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/COC Text";
                string gameModePath = "Environment Objects/LocalObjects_Prefab/TreeRoom/GameModes Title Text";

                TextMeshPro motdBody = GameObject.Find(motdBodyPath)?.GetComponent<TextMeshPro>();
                MeshRenderer motdRenderer = GameObject.Find(motdBodyPath)?.GetComponent<MeshRenderer>();
                TextMeshPro motdHeading = GameObject.Find(motdHeadingPath)?.GetComponent<TextMeshPro>();
                TextMeshPro cocHeading = GameObject.Find(cocHeadingPath)?.GetComponent<TextMeshPro>();

                motdRenderer.material = mat;

                if (PhotonNetwork.InRoom)
                {
                    motdBody.text =
                        $"Hey guys its tortise if you want to support me give me money.\n\n" +
                        $"Is Master: {PhotonNetwork.IsMasterClient}\n" +
                        $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / 10\n" +
                        $"Region: {PhotonNetwork.CloudRegion}";
                }
                else
                {
                    motdBody.text = "not in room join one....";
                    if (motdHeading)
                        motdHeading.text = Core.MenuName;
                }

                if (cocHeading)
                    cocHeading.text = "Violet Paid Made By: Tortise";

                GameObject gameMode = GameObject.Find(gameModePath);
                if (gameMode)
                {
                    gameMode.GetComponent<TextMeshPro>().text = Core.MenuName;
                    gameMode.GetComponent<MeshRenderer>().material = mat;
                }

                ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/TreeRoom", 5, colorMaterial, ref originalMat1);
                ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/Forest", 13, colorMaterial, ref originalMat2);
                ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/Forest/Terrain", 11, colorMaterial, ref originalMat3);

                GameObject monitor = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
                if (monitor)
                    monitor.GetComponent<MeshRenderer>().material = mat;

                TextMeshPro cocBody = GameObject.Find(cocBodyPath)?.GetComponent<TextMeshPro>();
                TextMeshPro cocText = GameObject.Find(cocTextPath)?.GetComponent<TextMeshPro>();
                motdHeading.fontStyle = FontStyles.Bold;
                motdHeading.alignment = TextAlignmentOptions.Top;   


                ApplyGradient(motdBody, rgbGradient);
                ApplyGradient(motdHeading, rgbGradient);
                ApplyGradient(cocHeading, rgbGradient);
                ApplyGradient(cocBody, rgbGradient);
                ApplyGradient(cocText, rgbGradient);
                ApplyGradient(gameMode.GetComponent<TextMeshPro>(), rgbGradient);

                if (cocBody)
                {
                    cocBody.fontStyle = FontStyles.Bold;
                    cocBody.alignment = TextAlignmentOptions.Top;
                    cocBody.fontSize = 75;
                    cocBody.text = PhotonNetwork.InRoom ? "\n\n" + PlayerInfo : "\nNOT CONNECTED TO A ROOM\n";
                }

                if (cocText)
                {
                    cocText.alignment = TextAlignmentOptions.Top;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }



        public static void ApplyGradient(TMP_Text text, VertexGradient gradient)
        {
            if (!text) return;
            text.enableVertexGradient = true;
            text.colorGradient = gradient;
        }


        public static Color32 SmoothRGBColor()
        {
            float time = Time.time * 0.7f;
            Color32 rbg = new Color32
            {
                r = (byte)((Mathf.Sin(time) * 0.5f + 0.5f) * 255),
                g = (byte)((Mathf.Sin(time + 2.0943952f) * 0.5f + 0.5f) * 255),
                b = (byte)((Mathf.Sin(time + 4.1887903f) * 0.5f + 0.5f) * 255),
                a = 255
            };
            return rbg;
        }

        private static VertexGradient GetSlidingRGBGradient(float speed = 0.7f)
        {
            float t = Time.time * speed;

            Color32 c1 = new Color32(
                (byte)((Mathf.Sin(t) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 2.0943952f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 4.1887903f) + 1f) * 127.5f),
                255
            );

            Color32 c2 = new Color32(
                (byte)((Mathf.Sin(t + 1.5f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 3.5f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 5.5f) + 1f) * 127.5f),
                255
            );

            Color32 c3 = new Color32(
                (byte)((Mathf.Sin(t + 3.0f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 5.0f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 1.0f) + 1f) * 127.5f),
                255
            );

            Color32 c4 = new Color32(
                (byte)((Mathf.Sin(t + 4.5f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 1.5f) + 1f) * 127.5f),
                (byte)((Mathf.Sin(t + 3.5f) + 1f) * 127.5f),
                255
            );

            return new VertexGradient(c1, c2, c3, c4);
        }

        public static void ApplyGradient(TMP_Text text, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
        {
            text.enableVertexGradient = true;
            text.colorGradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
        }



        public static Vector3 Orbit()
        {
            return new Vector3(Mathf.Cos(240f + ((float)Time.frameCount / 30)), 1, Mathf.Sin(240f + ((float)Time.frameCount / 30)));
        }
    }
    public class MovingGradient : MonoBehaviour
    {
        public TMP_Text text;

        public Color topLeftStart = Color.red;
        public Color topRightStart = Color.blue;
        public Color bottomLeftStart = Color.green;
        public Color bottomRightStart = Color.yellow;

        public Color topLeftEnd = Color.blue;
        public Color topRightEnd = Color.green;
        public Color bottomLeftEnd = Color.yellow;
        public Color bottomRightEnd = Color.red;

        public float duration = 2f;

        private float timer = 0f;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            text.enableVertexGradient = true;
        }

        private void Update()
        {
            timer += Time.deltaTime / duration;

            float t = Mathf.PingPong(timer, 1f);

            Color topLeft = Color.Lerp(topLeftStart, topLeftEnd, t);
            Color topRight = Color.Lerp(topRightStart, topRightEnd, t);
            Color bottomLeft = Color.Lerp(bottomLeftStart, bottomLeftEnd, t);
            Color bottomRight = Color.Lerp(bottomRightStart, bottomRightEnd, t);

            text.colorGradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
        }
    }
}
