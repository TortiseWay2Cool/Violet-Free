using BepInEx;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VioletTemplate.Main.Extentions.PlayerManager;
using Object = UnityEngine.Object;
namespace VioletTemplate.Mods
{
    internal class Movement : MonoBehaviour
    {
        #region Variables
        public static GameObject Rplatform;
        public static GameObject Lplatform;
        public static bool Rplat = true;
        public static bool Lplat = true;

        private static Vector3 oldMousePos;
        private static float upwardTimer = 0f;
        private const float upwardInterval = 1f;
        private const float upwardVelocityAmount = 0.44f;
        private static bool spacePressedLastFrame = false;
        #endregion
        public static void Fly(InputType type)
        {
            if (GetInput(type))
            {
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.transform.position += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * 10 * Time.deltaTime;
            }
        }

        public static void SlingShot(InputType type)
        {
            if (GetInput(type))
            {
                GorillaTagger.Instance.rigidbody.velocity += GorillaTagger.Instance.offlineVRRig.headMesh.transform.forward * 10 * Time.deltaTime;
            }
        }


        public static void NoClipFly(InputType type)
        {
            if (GetInput(type))
            {
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.transform.position += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * 10 * Time.deltaTime;

                foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    collider.enabled = false;
                }
            }
            else
            {
                foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    collider.enabled = true;
                }
            }
        }

        public static void Platforms(InputType RplatInput, InputType LplatInput, Color color)
        {
            if (GetInput(RplatInput))
            {
                if (Rplatform == null && Rplat)
                {
                    Rplatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Rplatform.transform.position = GetRightHand().position + new Vector3(0, -0.08f, 0);
                    Rplatform.transform.localScale = new Vector3(0.3f, 0.05f, 0.3f);
                    Rplatform.GetComponent<MeshRenderer>().material.color = color;
                    Rplat = false;
                }

            }
            else
            {
                Rplat = true;
                GorillaTagger.Instance.offlineVRRig.StartCoroutine(DelayedDestroy(false));
            }
            if (GetInput(LplatInput))
            {
                if (Lplatform == null && Lplat)
                {
                    Lplatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Lplatform.transform.position = GetLeftHand().position + new Vector3(0, -0.08f, 0);
                    Lplatform.transform.localScale = new Vector3(0.3f, 0.05f, 0.3f);
                    Lplatform.GetComponent<MeshRenderer>().material.color = color;
                    Lplat = false;
                }

            }
            else
            {
                Lplat = true;
                GorillaTagger.Instance.offlineVRRig.StartCoroutine(DelayedDestroy(true));
            }
        }

        public static IEnumerator DelayedDestroy(bool L)
        {
            if (!L)
            {
                if (Rplatform.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody RightRb = Rplatform.AddComponent<Rigidbody>();
                    RightRb.useGravity = true;
                }
            }
            else
            {
                if (Lplatform.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody LeftRb = Lplatform.AddComponent<Rigidbody>();
                    LeftRb.useGravity = true;
                }
            }

            yield return new WaitForSeconds(0.1f);
            if (L)
                Destroy(Lplatform);
            else
                Destroy(Rplatform);
        }

        public static void SpeedBoost(float multiplier)
        {
            GTPlayer.Instance.maxJumpSpeed = GTPlayer.Instance.maxJumpSpeed * multiplier;
            GTPlayer.Instance.jumpMultiplier = GTPlayer.Instance.maxJumpSpeed * multiplier;
        }

        private static Vector3 wallContactPoint;
        private static Vector3 wallContactNormal;

        public static void WallWalk()
        {
            if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
            {
                var hitInfo = GTPlayer.Instance.lastHitInfoHand;
                wallContactPoint = hitInfo.point;
                wallContactNormal = hitInfo.normal;
            }

            if (wallContactPoint != Vector3.zero && ControllerInputPoller.instance.rightGrab || ControllerInputPoller.instance.leftGrab)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-wallContactNormal * 4.6f, ForceMode.Acceleration);
                Gravity(9.81f);
            }
        }

        public static void CompWallWalk() // Thanks to ii for letting me use his code :)
        {
            float range = 0.2f;
            float power = -2f;

            if (ControllerInputPoller.instance.leftGrab)
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;

                if (Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, -ray.normal, out var Ray, range, GTPlayer.Instance.locomotionEnabledLayers))
                    GorillaTagger.Instance.rigidbody.AddForce(Ray.normal * power, ForceMode.Acceleration);
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;

                if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -ray.normal, out var Ray, range, GTPlayer.Instance.locomotionEnabledLayers))
                    GorillaTagger.Instance.rigidbody.AddForce(Ray.normal * power, ForceMode.Acceleration);
            }
        }


        public static void Gravity(float Power)
        {
            GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (Power / Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void ToggleGravity(InputType type, float Power)
        {
            if (GetInput(type))
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (Power / Time.deltaTime)), ForceMode.Acceleration);
            }
        }

        public static void WASDFly() // Cha554s Code cus im lazy :)
        {
            float speed = 11f;
            Transform camTransform = Camera.main.transform;
            var rb = GorillaTagger.Instance.rigidbody;

            rb.useGravity = false;

            if (UnityInput.Current.GetKey(KeyCode.LeftShift))
                speed *= 2.5f;

            Vector3 horizontalMove = Vector3.zero;
            Vector3 verticalMove = Vector3.zero;

            if (UnityInput.Current.GetKey(KeyCode.W) || UnityInput.Current.GetKey(KeyCode.UpArrow))
                horizontalMove += camTransform.forward;
            if (UnityInput.Current.GetKey(KeyCode.S) || UnityInput.Current.GetKey(KeyCode.DownArrow))
                horizontalMove -= camTransform.forward;
            if (UnityInput.Current.GetKey(KeyCode.A) || UnityInput.Current.GetKey(KeyCode.LeftArrow))
                horizontalMove -= camTransform.right;
            if (UnityInput.Current.GetKey(KeyCode.D) || UnityInput.Current.GetKey(KeyCode.RightArrow))
                horizontalMove += camTransform.right;

            if (UnityInput.Current.GetKey(KeyCode.LeftControl))
                verticalMove -= camTransform.up;

            if (horizontalMove != Vector3.zero)
                horizontalMove = horizontalMove.normalized * speed;

            verticalMove = verticalMove * speed;

            Vector3 currentVel = rb.velocity;
            Vector3 newVelocity = horizontalMove + verticalMove;

            newVelocity.y = currentVel.y;

            upwardTimer += Time.deltaTime;

            if (upwardTimer >= upwardInterval)
            {
                newVelocity.y += upwardVelocityAmount;
                upwardTimer = 0f;
            }

            bool spacePressed = UnityInput.Current.GetKey(KeyCode.Space);
            if (spacePressed && !spacePressedLastFrame)
            {
                newVelocity.y = 6f;
            }
            spacePressedLastFrame = spacePressed;

            rb.velocity = newVelocity;

            if (UnityInput.Current.GetMouseButton(1))
            {
                Vector3 mouseDelta = UnityInput.Current.mousePosition - oldMousePos;
                float pitch = camTransform.localEulerAngles.x - mouseDelta.y * 0.3f;
                float yaw = camTransform.localEulerAngles.y + mouseDelta.x * 0.3f;
                camTransform.localEulerAngles = new Vector3(pitch, yaw, 0f);
            }

            oldMousePos = UnityInput.Current.mousePosition;
        }

        #region Slide Controls

        public static void SlideControl()
        {
            GorillaLocomotion.GTPlayer.Instance.slideControl = 1f;
        }
        public static void QuestSlideControl()
        {
            GorillaLocomotion.GTPlayer.Instance.slideControl = 0.00725f;
        }

        public static void NoSlideControl()
        {
            GorillaLocomotion.GTPlayer.Instance.slideControl = 0f;
        }

        public static void ToggleSlideControl()
        {
            if (GetInput(InputType.RGrip)) { GorillaLocomotion.GTPlayer.Instance.slideControl = 1f; }
        }

        #endregion
    }
}
