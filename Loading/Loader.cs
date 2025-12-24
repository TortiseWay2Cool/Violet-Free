using System;
using UnityEngine;
using VioletTemplate.Main.Initialization;
namespace VioletTemplate.Loading
{
    public class Loader : MonoBehaviour
    {
        public static void Load()
        {
            Loadobj = new GameObject("violet");
            Loadobj.AddComponent<Initializer>();
            Loadobj.AddComponent<BepInExInitializer>();
            DontDestroyOnLoad(Loadobj);
        }

        private static GameObject Loadobj;
    }
}
