using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using static RoomSystem;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.RigManager;
using Random = UnityEngine.Random;
namespace VioletTemplate.Mods
{
    public class Master
    {
        #region Slow
        public static void SlowGun()
        {
            StartBothGuns(() =>
            {
                SendStatusEffectToPlayer(StatusEffects.SetSlowedTime, TargetPlayer);
            }, null, true);
        }

        public static void SlowAll()
        {
            SendStatusEffectAll(StatusEffects.SetSlowedTime);
        }

        #endregion

        #region Vibrate
        public static void VibrateGun()
        {
            StartBothGuns(() =>
            {
                SendStatusEffectToPlayer(StatusEffects.JoinedTaggedTime, TargetPlayer);
            }, null, true);
        }

        public static void VibrateAll()
        {
            SendStatusEffectAll(StatusEffects.JoinedTaggedTime);
        }
        #endregion

        #region Sounds
        public static void RoomSystemSoundGun(int type)
        {
            StartBothGuns(() =>
            {
                SendSoundEffectToPlayer(type, 0.25f, TargetPlayer, true);
            }, null, true);
        }

        public static void RoomSystemSoundAll(int type)
        {
            SendSoundEffectAll(type, 0.25f, true);
        }
        #endregion

        #region GameMode
        public static void EndGamemode()
        {
            GorillaGameManager GameModeManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaGameManager>();
            GameModeManager.StopPlaying();
        }

        public static void StartGamemode()
        {
            GorillaGameManager GameModeManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaGameManager>();
            GameModeManager.StartPlaying();
        }

        public static void RestartGamemode()
        {
            GorillaGameManager GameModeManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaGameManager>();
            GameModeManager.StopPlaying();
            GameModeManager.StartPlaying();
        }

        public static async void SpazmGamemode()
        {
            GorillaGameManager GameModeManager = GorillaGameManager.instance.gameObject.GetComponent<GorillaGameManager>();
            GameModeManager.StopPlaying();
            GameModeManager.StartPlaying();
            await Task.Delay(300);
        }

        public static void SetInfectionThreshold(int amount)
        {
            GorillaTagManager GtagManager = GorillaTagManager.instance.gameObject.GetComponent<GorillaTagManager>();
            GtagManager.infectedModeThreshold = amount;
        }
        #endregion

        #region Infection
        public static async void Mat(Photon.Realtime.Player plr)
        {
            GorillaTagManager GtagManager = GorillaTagManager.instance.gameObject.GetComponent<GorillaTagManager>();
            if (GtagManager.currentInfected.Contains(plr))
            {
                GtagManager.currentInfected.Remove(plr);
            }
            else
            {
                GtagManager.currentInfected.Add(plr);
            }
            await Task.Delay(100);
        }

        public static void Tag(Photon.Realtime.Player plr)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaTagManager GtagManager = GorillaTagManager.instance.gameObject.GetComponent<GorillaTagManager>();
                GtagManager.currentInfected.Add(plr);
            }
        }

        public static void UnTag(Photon.Realtime.Player plr)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaTagManager GtagManager = GorillaTagManager.instance.gameObject.GetComponent<GorillaTagManager>();
                GtagManager.currentInfected.Remove(plr);
            }
        }
        #endregion

        #region SuperInfection / Ghost Reactor
        public static void CreateItem(SuperInfectionItems createhash, Vector3 Position, Quaternion Rotation)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameEntityManager EnitityManager = SuperInfectionManager.activeSuperInfectionManager.gameEntityManager;

                object[] Data = { new[] { EnitityManager.CreateNetId(), }, new[] { (int)createhash }, new[] { BitPackUtils.PackWorldPosForNetwork(Position) }, new[] { BitPackUtils.PackQuaternionForNetwork(Rotation) }, new[] { 0L } };

                EnitityManager.photonView.RPC("CreateItemRPC", RpcTarget.All, Data);
            }
        }

        public static void Create(SuperInfectionItems hash)
        {
            if (ControllerInputPoller.instance.rightGrab)
            {

                if (Time.time > Competitive.Delay)
                {
                    Competitive.Delay = Time.time + 0.1f;
                    CreateItem(hash, GorillaTagger.Instance.rightHandTransform.transform.position, Quaternion.identity);
                }
            }
        }

        public enum SuperInfectionItems
        {
            LongArms = 1428761418,
            Cogwheel = 1573124711,
            propeller = -1912435955,
            Sword = -894667703,
            Spring = 1447779317,
            PinkButton = 1618940484,
            GreenButton = 1657474495,
            Spinner = 1799386883,
            Sand = -1111610567,
            Shrine = 1551901997,
            BluPrint = 1880272606,
            SpiralGrabber = -1409076879,
        }
        #endregion
    }
}
