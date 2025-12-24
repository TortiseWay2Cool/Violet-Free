using BepInEx;
using g3;
using Photon.Realtime;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using static VioletTemplate.Main.Extentions.PlayerManager;

namespace VioletTemplate.Main.Extentions
{
    public class GunLib : MonoBehaviour
    {
        public static readonly Color Violet = new Color(0.56f, 0f, 1f);
        public static Color PointerColor = Violet;
        public static Color HitColor = Violet;
        public static Color LineColor = Violet;

        public static GameObject pointerObject;
        public static GameObject lineObject;
        public static LineRenderer lineRenderer;
        public static RaycastHit rayHit;
        public static VRRig TargetRig { get; private set; }
        public static Player TargetPlayer { get; private set; }
        public static Vector3 TargetPosition { get; private set; }

        private static void UpdateLineCurve(Vector3 start, Vector3 end)
        {
            lineRenderer.positionCount = 64;
            float[] widths = new float[64];
            for (int i = 0; i < 64; i++)
            {
                float t = (float)i / 63f;
                lineRenderer.SetPosition(i, Vector3.Lerp(start, end, t));
                float pulse = Mathf.Sin(i * Mathf.PI * 8f / 64f + Time.time * 5f) * 0.02f + 0.04f;
                widths[i] = pulse;
            }
            lineRenderer.widthCurve = new AnimationCurve(widths.Select((w, i) => new Keyframe((float)i / 63f, w)).ToArray());
        }

        public static void CreateGunlib(Action startAction, Action endAction, bool lockOn)
        {
            if (!GetInput(InputType.RGrip))
            {
                if (pointerObject) Destroy(pointerObject);
                if (lineObject) Destroy(lineObject);
                pointerObject = lineObject = null; lineRenderer = null;
                TargetRig = null; TargetPlayer = null; TargetPosition = Vector3.zero;
                endAction?.Invoke();
                return;
            }

            if (!pointerObject)
            {
                pointerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointerObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Destroy(pointerObject.GetComponent<SphereCollider>());
                pointerObject.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                pointerObject.GetComponent<Renderer>().material.color = PointerColor;
            }

            if (!lineObject)
            {
                lineObject = new GameObject("GunLine");
                lineRenderer = lineObject.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                lineRenderer.material.color = LineColor;
                lineRenderer.useWorldSpace = true;
            }

            if (!GetInput(InputType.RTrigger)) { TargetRig = null; TargetPlayer = null; TargetPosition = Vector3.zero; }

            if (TargetRig && GetInput(InputType.RTrigger))
            {
                pointerObject.transform.position = TargetRig.transform.position;
                pointerObject.GetComponent<Renderer>().material.color = HitColor;
                TargetPosition = TargetRig.transform.position;
                UpdateLineCurve(GorillaTagger.Instance.rightHandTransform.position, TargetRig.transform.position);
                startAction?.Invoke();
                return;
            }

            if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out rayHit, float.MaxValue, NoInvisLayerMask()))
            {
                pointerObject.transform.position = rayHit.point;
                VRRig rig = rayHit.collider.GetComponentInParent<VRRig>();
                pointerObject.GetComponent<Renderer>().material.color = rig ? HitColor : PointerColor;

                if (GetInput(InputType.RTrigger) && lockOn)
                {
                    TargetRig = rig;
                    TargetPlayer = rig ? RigManager.GetPlayerFromVRRig(rig) : null;
                    TargetPosition = rig ? rig.transform.position : rayHit.point;
                    pointerObject.transform.position = TargetPosition;
                    UpdateLineCurve(GorillaTagger.Instance.rightHandTransform.position, TargetPosition);
                    startAction?.Invoke();
                }
                else
                {
                    TargetPosition = rayHit.point;
                    UpdateLineCurve(GorillaTagger.Instance.rightHandTransform.position, rayHit.point);
                    if (GetInput(InputType.RTrigger)) startAction?.Invoke();
                }
            }
            else
            {
                Vector3 far = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.rightHandTransform.up * 100f;
                pointerObject.transform.position = far;
                pointerObject.GetComponent<Renderer>().material.color = PointerColor;
                UpdateLineCurve(GorillaTagger.Instance.rightHandTransform.position, far);
            }
        }

        public static int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static int Zone = LayerMask.NameToLayer("Zone");
        public static int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");
        public static int NoInvisLayerMask() => ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);

        public static void CreatePcGunlib(Action startAction, Action endAction, bool lockOn)
        {
            if (!Mouse.current.rightButton.isPressed)
            {
                if (pointerObject) Destroy(pointerObject);
                if (lineObject) { if (lineRenderer) Destroy(lineObject); }
                pointerObject = lineObject = null; lineRenderer = null;
                TargetRig = null; TargetPlayer = null; TargetPosition = Vector3.zero;
                endAction?.Invoke();
                return;
            }

            if (!pointerObject)
            {
                pointerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointerObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Destroy(pointerObject.GetComponent<SphereCollider>());
                pointerObject.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                pointerObject.GetComponent<Renderer>().material.color = PointerColor;
            }

            if (!lineObject)
            {
                lineObject = new GameObject("GunLine");
                lineRenderer = lineObject.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                lineRenderer.material.color = LineColor;
                lineRenderer.useWorldSpace = true;
            }

            Ray ray = GameObject.Find("Shoulder Camera").activeSelf
                ? GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition)
                : GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition);

            if (!Mouse.current.leftButton.isPressed) { TargetRig = null; TargetPlayer = null; TargetPosition = Vector3.zero; }

            if (TargetRig && Mouse.current.leftButton.isPressed)
            {
                pointerObject.transform.position = TargetRig.transform.position;
                pointerObject.GetComponent<Renderer>().material.color = HitColor;
                TargetPosition = TargetRig.transform.position;
                UpdateLineCurve(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, TargetRig.transform.position);
                startAction?.Invoke();
                return;
            }

            if (Physics.Raycast(ray.origin, ray.direction, out rayHit, float.MaxValue, NoInvisLayerMask()))
            {
                pointerObject.transform.position = rayHit.point;
                VRRig rig = rayHit.collider.GetComponentInParent<VRRig>();
                pointerObject.GetComponent<Renderer>().material.color = rig ? HitColor : PointerColor;

                if (Mouse.current.leftButton.isPressed && lockOn)
                {
                    TargetRig = rig;
                    TargetPlayer = rig ? RigManager.GetPlayerFromVRRig(rig) : null;
                    TargetPosition = rig ? rig.transform.position : rayHit.point;
                    pointerObject.transform.position = TargetPosition;
                    UpdateLineCurve(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, TargetPosition);
                    startAction?.Invoke();
                }
                else
                {
                    TargetPosition = rayHit.point;
                    UpdateLineCurve(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, rayHit.point);
                    if (Mouse.current.leftButton.isPressed) startAction?.Invoke();
                }
            }
            else
            {
                Vector3 far = ray.origin + ray.direction * 100f;
                pointerObject.transform.position = far;
                pointerObject.GetComponent<Renderer>().material.color = PointerColor;
                UpdateLineCurve(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, far);
            }
        }

        public static void StartBothGuns(Action action, Action disable = null, bool locko = true)
        {
            if (XRSettings.isDeviceActive) CreateGunlib(action, disable, locko);
            else CreatePcGunlib(action, disable, locko);
        }
    }
}