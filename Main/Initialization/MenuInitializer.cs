using BepInEx;
using GorillaLocomotion;
using HarmonyLib;
using System;
using UnityEngine;
using VioletTemplate.Main.Extentions;

namespace VioletTemplate.Main.Initialization
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Initializer : MonoBehaviour
    {

        private static GameObject menuObject;

        private static void Postfix()
        {
            try
            {
                if (menuObject != null) return;
                    menuObject = new GameObject("Violet Template");
                    menuObject.AddComponent<Core>();
                    menuObject.AddComponent<Notifications>();
                    menuObject.AddComponent<RoomLogic>();
                    GameObject.DontDestroyOnLoad(menuObject);
                
            }
            catch (Exception ex) { }
        }
    }

    [BepInPlugin("com.Violet.Violetpaid.org", "Violet Paid", "7.0")]
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class BepInExInitializer : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource LoggerInstance;

        void Awake()
        {
            LoggerInstance = Logger;
            new Harmony("com.Violet.Violetpaid.org").PatchAll();
        }
    }
}