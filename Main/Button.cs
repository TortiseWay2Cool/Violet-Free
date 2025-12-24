using ExitGames.Client.Photon.StructWrapping;
using GorillaGameModes;
using GorillaTag.Cosmetics;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Mono.Security.X509.X520;
using static VioletTemplate.Main.Button;
using static VioletTemplate.Main.Core;
using static VioletTemplate.Main.CreateButtons;
using static VioletTemplate.Main.Extentions.PlayerManager;
using static VioletTemplate.Mods.Competitive;
using static VioletTemplate.Mods.Master;
using static VioletTemplate.Mods.Movement;
using static VioletTemplate.Mods.Overpowered;
using static VioletTemplate.Mods.Player;
using static VioletTemplate.Mods.Projectiles;
using static VioletTemplate.Mods.Room;
using static VioletTemplate.Mods.Visual;
using VioletTemplate.Main.Extentions;
namespace VioletTemplate.Main
{
    public class Button
    {
        public string Name;
        public string Description;
        public Catagorys Catagory;
        public Action OnClick;
        public Action OnDisable;
        public bool MasterClient;
        public bool Toggle;
        public bool Enabled;

        public Button(string name, string description, Catagorys catagory, Action onClick, Action onDisable, bool toggle, bool enabled, bool Master)
        {
            Name = name;
            Description = description;
            Catagory = catagory;
            OnClick = onClick;
            OnDisable = onDisable;
            Toggle = toggle;
            Enabled = enabled;
            MasterClient = Master;
        }
        public void SetText(string newText)
        {
            Name = newText;
        }
    }

    public class CreateButtons
    {
        public enum Catagorys
        {
            Home,
            Settings,
            Room,
            Movement,
            Comp,
            Player,
            World,
            Visuals,
            SoundBoard,
            Guardian,
            Overpowered,
            Master,
            Projectiles,
            Cosmetics,
            CsCosmetics,
            CitySpammers,
            Spammers
        }
        public static void CreateAllButtons()
        {
            CreateButton("Room", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Room));
            CreateButton("Movement", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Movement));
            CreateButton("Comp", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Comp));
            CreateButton("Player", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Player));
            CreateButton("World", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.World));
            CreateButton("Visuals", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Visuals));
            CreateButton("Master", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Master));
            CreateButton("Overpowered", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Overpowered));
            CreateButton("Projectiles", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Projectiles));
            CreateButton("Cosmetics", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Cosmetics));

            CreateButton("Spammers", "", Catagorys.Home, false, false, () => ChangePage(Catagorys.Spammers));

            #region Create Catagorys
            RoomButtons();
            MovementButtons();
            VisualButtons();
            CompButtons();
            PlayerButtons();
            OverpoweredButtons();
            MasterButtons();
            PojectilesButtons();
            CosmeticsButtons();
            SpammersButtons();
            #endregion
        }

        private static void RoomButtons()
        {
            CreateButton("Join Code Mod", "joins the private room MOD ", Catagorys.Room, false, false, () => JoinRoom("MOD"));
            CreateButton("Join Code Mods", "joins the private room MODS ", Catagorys.Room, false, false, () => JoinRoom("MODS"));
            CreateButton("Join Code VIOLET", "joins the private room VIOLET ", Catagorys.Room, false, false, () => JoinRoom("VIOLET"));
            CreateButton("Join Code CHA", "joins the private room CHA ", Catagorys.Room, false, false, () => JoinRoom("CHA"));
        }

        private static void MovementButtons()
        {
            CreateButton("Wasd Fly", "Fly using WASD + mouse look (PC only)", Catagorys.Movement, true, false, () => WASDFly());
            CreateButton("Fly", "Hold Right Secondary And You Fly Forward", Catagorys.Movement, true, false, () => Fly(InputType.RSecondary));
            CreateButton("Grip Fly", "Hold right grip button to fly", Catagorys.Movement, true, false, () => Fly(InputType.RGrip));
            CreateButton("Trigger Fly", "Hold right trigger to fly forward", Catagorys.Movement, true, false, () => Fly(InputType.RTrigger));
            CreateButton("SlingShot", "Hold Right Secondary to sling shot forward", Catagorys.Movement, true, false, () => SlingShot(InputType.RSecondary));
            CreateButton("Trigger SlingShot", "Hold Right Trigger to sling shot forward", Catagorys.Movement, true, false, () => SlingShot(InputType.RTrigger));
            CreateButton("NoClip Fly", "Hold Right Secondary to fly through walls", Catagorys.Movement, true, false, () => NoClipFly(InputType.RSecondary));
            CreateButton("NoClip Trigger Fly", "Hold right trigger to noclip fly through everything", Catagorys.Movement, true, false, () => NoClipFly(InputType.RTrigger));
            CreateButton("Platforms", "Hold both grips to spawn platforms under your feet", Catagorys.Movement, true, false, () => Platforms(InputType.RGrip, InputType.LGrip, Color.violet));
            CreateButton("Trigger Platforms", "Hold both triggers to spawn platforms", Catagorys.Movement, true, false, () => Platforms(InputType.RTrigger, InputType.LTrigger, Color.violet));
            CreateButton("Invis Platforms", "Hold grips to spawn completely invisible platforms", Catagorys.Movement, true, false, () => Platforms(InputType.RGrip, InputType.LGrip, Color.clear));
            CreateButton("Trigger Invis Platforms", "Hold triggers to spawn invisible platforms", Catagorys.Movement, true, false, () => Platforms(InputType.RTrigger, InputType.LTrigger, Color.clear));
            CreateButton("Speed Boost", "Greatly increases your walking/running speed", Catagorys.Movement, true, false, () => SpeedBoost(2));
            CreateButton("Mosa Boost", "Moderate speed boost", Catagorys.Movement, true, false, () => SpeedBoost(1.5f));
            CreateButton("Low Gravity", "Makes you jump higher and fall slower", Catagorys.Movement, true, false, () => Gravity(6.66f));
            CreateButton("High Gravity", "Makes Harder To Move", Catagorys.Movement, true, false, () => Gravity(7.77f));
            CreateButton("No Gravity", "You Dont have Gravity", Catagorys.Movement, true, false, () => Gravity(9.81f));
            CreateButton("Toggle Low Gravity", "Press right trigger to toggle low gravity on/off", Catagorys.Movement, true, false, () => ToggleGravity(InputType.RTrigger, 6.66f));
            CreateButton("Toggle High Gravity", "Press right trigger to toggle high gravity", Catagorys.Movement, true, false, () => ToggleGravity(InputType.RTrigger, 7.77f));
            CreateButton("Toggle No Gravity", "Press right trigger to toggle zero gravity", Catagorys.Movement, true, false, () => ToggleGravity(InputType.RTrigger, 9.81f));
            CreateButton("Wall Walk", "Lets you walk on walls", Catagorys.Movement, true, false, () => WallWalk());
            CreateButton("Comp Wall Walk", "Lets you walk on walls", Catagorys.Movement, true, false, () => CompWallWalk());
            CreateButton("Slide Control", "When on ice you control where you go", Catagorys.Movement, true, false, () => SlideControl());
            CreateButton("Toggle Slide Control", "Toggle When on ice you control where you go", Catagorys.Movement, true, false, () => ToggleSlideControl());
            CreateButton("Quest Slide Control", "When on ice you slide like on quest", Catagorys.Movement, true, false, () => QuestSlideControl());
            CreateButton("No Slide Control", "Disables sliding completely", Catagorys.Movement, true, false, () => NoSlideControl());
        }

        private static void VisualButtons()
        {
            CreateButton("Name Tags", "Shows name tags above players heads", Catagorys.Visuals, true, false, () => NameTags());
            CreateButton("FPS Tags", "Shows name tags above players heads", Catagorys.Visuals, true, false, () => FPSTags());
            CreateButton("Position Tags", "Shows name tags above players heads", Catagorys.Visuals, true, false, () => PositionTags());
            CreateButton("Infection ESP", "Shows names/box ESP around infected players", Catagorys.Visuals, true, false, () => InfectionESP(), ()=> ResetESP());
            CreateButton("Infection Tracers", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => InfectionTracers());
            CreateButton("Tracers", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => Tracers());
            CreateButton("Rainbow ESP", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => RainbowESP(), ()=> ResetESP());
            CreateButton("ESP", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => ESP(), () => ResetESP());
            CreateButton("Big Monkeys", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => BigMonkes(), () => ResetMonkes());
            CreateButton("Thin Monkeys", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => ThinMonkes(), () => ResetMonkes());
            CreateButton("Small Monkeys", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => StubbyMonkes(), () => ResetMonkes());
            CreateButton("Invis Monkes", "Shows bright lines to all infected players", Catagorys.Visuals, true, false, () => InvisMonkes(), () => ResetESP());
        }

        private static void CompButtons()
        {
            CreateButton("Anti Tag", "Prevents you from ever being tagged (infection)", Catagorys.Comp, true, false, () => AntiTag(), () => Serilization.ResetSerilization());
            CreateButton("Tag Self", "Force tags yourself", Catagorys.Comp, true, false, () => TagSelf());
            CreateButton("UnTag Self", "Removes tag from yourself instantly ", Catagorys.Comp, false, false, () => UnTagSelf());
            CreateButton("No Tag On Join", "You will never be tagged when entering a room", Catagorys.Comp, true, false, () => NoTagOnJoin());
            CreateButton("Tag All", "Instantly tags every player in the lobby", Catagorys.Comp, false, false, () => TagAll(), () => Serilization.ResetSerilization());
            CreateButton("Tag Gun", "Tags Any Player Shot", Catagorys.Comp, true, false, () => TagGun(), () => Serilization.ResetSerilization());
            CreateButton("Tag Aura", "Automatically tags anyone who comes near you", Catagorys.Comp, true, false, () => TagAura());
            CreateButton("Infection Tracers", "Shows bright lines to all infected players", Catagorys.Comp, true, false, () => InfectionTracers());
            CreateButton("Infection ESP", "Shows names/box ESP around infected players", Catagorys.Comp, true, false, () => InfectionESP(), () => ResetESP());
        }

        public static void PlayerButtons()
        {
            CreateButton("Ghost", "Makes your body stay at one spot", Catagorys.Player, true, false, () => GhostMonkey());
            CreateButton("Invis", "Makes your body invisible", Catagorys.Player, true, false, () => InvisMonkey());
            CreateButton("Toggle Ghost", "Toggle full ghost mode on/off", Catagorys.Player, true, false, () => ToggleGhostMonkey());
            CreateButton("Toggle Invis", "Toggle Invis on/off", Catagorys.Player, true, false, () => ToggleInvisMonkey());
            CreateButton("Grab Rig", "Lets you grab and move your rig", Catagorys.Player, true, false, () => GrabRig());
            CreateButton("Spaz Rig", "Moves rig violently", Catagorys.Player, true, false, () => SpazRig());
            CreateButton("Stutter Rig", "Spazzes room for everyone else", Catagorys.Player, true, false, () => GrabRig());
            CreateButton("Scare Gun", "Teleport to target with gun", Catagorys.Player, true, false, () => ScareGun());
            CreateButton("Scare Closest", "Teleport to closest player", Catagorys.Player, true, false, () => ScareClosest());
            CreateButton("Follow Gun", "Follow target with gun", Catagorys.Player, true, false, () => FollowGun());
            CreateButton("Follow Closest", "Follow closest player", Catagorys.Player, true, false, () => FollowClosest());
            CreateButton("Orbit Gun", "Orbit target with gun", Catagorys.Player, true, false, () => OrbitGun());
            CreateButton("Orbit Closest", "Orbit closest player", Catagorys.Player, true, false, () => OrbitClosest());
            CreateButton("Stutter Rig Gun", "Stutter Rig For Gun Target", Catagorys.Player, true, false, () => StutterRigGun());
            CreateButton("Spin Head X", "Spins your head rapidly on X axis", Catagorys.Player, true, false, () => SpinHead(10, 0, 0), () => ResetHeadSpin());
            CreateButton("Spin Head Y", "Spins your head rapidly on Y axis", Catagorys.Player, true, false, () => SpinHead(0, 10, 0), () => ResetHeadSpin());
            CreateButton("Spin Head Z", "Spins your head rapidly on Z axis", Catagorys.Player, true, false, () => SpinHead(0, 0, 10), () => ResetHeadSpin());
            CreateButton("Walk On Water", "Allows you to walk on water surfaces", Catagorys.Player, false, false, () => WalkOnWater(true));
            CreateButton("Stop Walking On Water", "Disables walk on water", Catagorys.Player, false, false, () => WalkOnWater(false));
            CreateButton("No Name", "Hides your name completely", Catagorys.Player, true, false, () => ChangeName("_"));
            CreateButton("Change Name N Word", "Changes your name to the hard R ", Catagorys.Player, false, false, () => ChangeName("NIGGER"));
            CreateButton("Change Name Tortise", "Changes name to Tortise", Catagorys.Player, false, false, () => ChangeName("TORTISE"));
            CreateButton("Change Name PBBV", "Changes name to PBBV", Catagorys.Player, true, false, () => ChangeName("PBBV"));
            CreateButton("Change Name Daisy 09", "Changes name to Daisy09 ", Catagorys.Player, false, false, () => ChangeName("DASIY O9"));
            CreateButton("Change Name RUN", "Changes name to RUN", Catagorys.Player, false, false, () => ChangeName("RUN"));

            CreateButton("Max Quest Score", "Sets your quest score to the maximum possible value", Catagorys.Player, false, false, () => MaxQuestScore());
            CreateButton("Random Quest Score", "Sets your quest score to a random number (0-99999)", Catagorys.Player, true, false, () => SpazQuestScore());
            CreateButton("Spoof Players self", "Randomly changes your color stump only", Catagorys.Player, false, false, () => SpoofColor());
            CreateButton("RGB Stump", "Smooth RGB cycling on your player", Catagorys.Player, true, false, () => StumpRGB());
            CreateButton("Color Strobe", "Rapidly flashes bright colors on your player", Catagorys.Player, true, false, () => Strobe());
            CreateButton("Follow All", "Moves Rig To Everyone in Lobby", Catagorys.Player, true, false, () => FollowAll(), () => Serilization.ResetSerilization());
            CreateButton("Scare All", "Spazzes Rig By Everyone", Catagorys.Player, true, false, () => ScareAll(), () => Serilization.ResetSerilization());
            CreateButton("Orbit All", "Orbits around everyone", Catagorys.Player, true, false, () => OrbitAll(), () => Serilization.ResetSerilization());
        }

        public static void OverpoweredButtons()
        {
            CreateButton("Auto Flush RPCS", "", Catagorys.Overpowered, true, true, () => PlayerManager.AutoFlushRPCS());
            CreateButton("Anti Report", "Prevents you from being reported", Catagorys.Overpowered, true, true, () => PlayerManager.AntiReport());

            CreateButton("Freeze All", "Slow", Catagorys.Overpowered, true, false, () => FreezeServer());

            CreateButton("Lag Gun", "shot players lag", Catagorys.Overpowered, true, false, () => LagGun());
            CreateButton("Lag All", "Everyone Lags", Catagorys.Overpowered, true, false, () => LagAll());
            CreateButton("Stutter Gun", "Shot Player Stutter / lags", Catagorys.Overpowered, true, false, () => StutterGun());
            CreateButton("Stutter All", "Everyone Stutters / Lags", Catagorys.Overpowered, true, false, () => StutterAll());

            CreateButton("Stump Kick Gun", "Shoot to kick player In stump ", Catagorys.Overpowered, true, false, () => KickGun(4));
            CreateButton("Stump Kick All", "Kicks entire lobby In stump", Catagorys.Overpowered, false, false, () => KickAll(4));


            CreateButton("Large Stuffing Launcher", "Shoots massive snowballs", Catagorys.Overpowered, true, false, () => SnowballLauncher(99));
            CreateButton("Small Stuffing Launcher", "Shoots tiny snowballs", Catagorys.Overpowered, true, false, () => SnowballLauncher(0));
            CreateButton("Stuffing Fling Gun", "Flings Target Player (Little Broken)", Catagorys.Overpowered, true, false, () => SnowballFlingGun(new Vector3(0, -0.5f, 0), 99), () => Serilization.ResetSerilization());
            CreateButton("Stuffing Annoy Gun", "Slightly moves player (Little Broken)", Catagorys.Overpowered, true, false, () => SnowballFlingGun(new Vector3(0, -0.5f, 0), 1), () => Serilization.ResetSerilization());

            CreateButton("Snowball Particle Gun", "Puts snowball particles on player", Catagorys.Overpowered, true, false, () => SnowballParticleGun());

            CreateButton("Grab Rainbow HoverBoard", "", Catagorys.Overpowered, true, false, () => GrabRainbowHoverBoard());
            CreateButton("Grab HoverBoard", "", Catagorys.Overpowered, true, false, () => GrabHoverBoard());
            CreateButton("Launch HoverBoard", "", Catagorys.Overpowered, true, false, () => LaunchHoverBoard());
            CreateButton("Launch Rainbow HoverBoard", "", Catagorys.Overpowered, true, false, () => LaunchRainbowHoverBoard());
            CreateButton("Orbit HoverBoard", "", Catagorys.Overpowered, true, false, () => OrbitHoverBoard());
            CreateButton("Orbit Rainbow HoverBoard", "", Catagorys.Overpowered, true, false, () => OrbitRainbowHoverBoard());


        }

        public static void MasterButtons()
        {
            CreateButton("Slow Gun", "adds tag freeze to the target", Catagorys.Master, true, false, () => SlowGun(), doesNeedMaster: true);
            CreateButton("Slow All", "adds tag freeze to Everyone", Catagorys.Master, true, false, () => SlowAll(), doesNeedMaster: true);
            CreateButton("Vibrate Gun", "Vibrations to the target", Catagorys.Master, true, false, () => VibrateGun(), doesNeedMaster: true);
            CreateButton("Vibrate All", "Vibrations to Everyone", Catagorys.Master, true, false, () => VibrateAll(), doesNeedMaster: true);
            CreateButton("Tag Sound Gun", "Plays the tag sound only to the target", Catagorys.Master, true, false, () => RoomSystemSoundGun(0), doesNeedMaster: true);
            CreateButton("Tag Sound All", "Plays the tag sound to everyone", Catagorys.Master, true, false, () => RoomSystemSoundAll(0), doesNeedMaster: true);
            CreateButton("Game End Sound Gun", "Plays Game End Sound to target", Catagorys.Master, true, false, () => RoomSystemSoundGun(2), doesNeedMaster: true);
            CreateButton("Game End Sound All", "Plays Game End Sound", Catagorys.Master, true, false, () => RoomSystemSoundAll(2), doesNeedMaster: true);

            CreateButton("Start Gamemode", "", Catagorys.Master, false, false, () => StartGamemode(), doesNeedMaster: true);
            CreateButton("Stop Gamemode", "", Catagorys.Master, false, false, () => EndGamemode(), doesNeedMaster: true);
            CreateButton("Restart Gamemode", "", Catagorys.Master, false, false, () => RestartGamemode(), doesNeedMaster: true);
            CreateButton("Spaz Gamemode", "", Catagorys.Master, true, false, () => SpazmGamemode(), doesNeedMaster: true);

            CreateButton("Spawn Long Arms", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.LongArms), doesNeedMaster: true);
            CreateButton("Spawn Cogwheel", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Cogwheel), doesNeedMaster: true);
            CreateButton("Spawn Propeller", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.propeller), doesNeedMaster: true);
            CreateButton("Spawn Sword", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Sword), doesNeedMaster: true);
            CreateButton("Spawn Spring", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Spring), doesNeedMaster: true);
            CreateButton("Spawn Pink Button", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.PinkButton), doesNeedMaster: true);
            CreateButton("Spawn Green Button", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.GreenButton), doesNeedMaster: true);
            CreateButton("Spawn Spinner", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Spinner), doesNeedMaster: true);
            CreateButton("Spawn Sand", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Sand), doesNeedMaster: true);
            CreateButton("Spawn Shrine", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.Shrine), doesNeedMaster: true);
            CreateButton("Spawn Blueprint", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.BluPrint), doesNeedMaster: true);
            CreateButton("Spawn Spiral Grabber", "", Catagorys.Master, true, false, () => Create(SuperInfectionItems.SpiralGrabber), doesNeedMaster: true);
        }
        public static void PojectilesButtons()
        {
            CreateButton("Current Projectile: ", "", Catagorys.Projectiles, false, false, () => ChangeProjectile());
            CreateButton("Launch Projectile", "", Catagorys.Projectiles, true, false, () => LaunchProjectile());
            CreateButton("Grab Projectile", "", Catagorys.Projectiles, false, false, () => GrabProjectile());
        }

        public static void CosmeticsButtons()
        {
            CreateButton("CSCosmetics", "", Catagorys.Cosmetics, false, false, () => 
            {
                for (int i = 0; i < GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.allObjects.Length; i++)
                {
                    string displayName = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.allObjects[i].transform.parent.gameObject.name.ToString();
                    CreateButton($"Equip {displayName}", "", Catagorys.CsCosmetics, true, false, () => EquipCosmetic(i));
                }
                ChangePage(Catagorys.CsCosmetics);
            });
            CreateButton("City Spammers", "", Catagorys.Cosmetics, false, false, () =>
            {
                ChangePage(Catagorys.CitySpammers);
                CreateCityStuff();
            });

            
        }

        public static void CreateCityStuff()
        {
            foreach (CosmeticID cosmetic in Enum.GetValues(typeof(CosmeticID)))
            {
                TransferrableObject transferrableObject = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.allObjects[(int)cosmetic];
                if (transferrableObject.GetComponent<ThrowableHoldableCosmetic>() != null)
                {
                    string displayName = cosmetic.ToString();
                    CreateButton($"Spawm {displayName}", "", Catagorys.CitySpammers, true, false, () => SpawnCosmetic(cosmetic,1));
                }

                if (transferrableObject.GetComponent<DeployableObject>() != null)
                {
                    string displayName = cosmetic.ToString();
                    CreateButton($"Spawm {displayName}", "", Catagorys.CitySpammers, true, false, () => SpawnCosmetic(cosmetic, 2));
                }

                if (transferrableObject.GetComponent<PaperPlaneThrowable>() != null)
                {
                    string displayName = cosmetic.ToString();
                    CreateButton($"Spawm {displayName}", "", Catagorys.CitySpammers, true, false, () => SpawnCosmetic(cosmetic, 3));
                }

            }

            
        }

        public static void SpammersButtons()
        {
            CreateButton("Splash Gun", "Shoots water splashes rapidly at whoever you aim at", Catagorys.Spammers, true, false, () => SplashGun(), () => Serilization.ResetSerilization());
        }


        private static void CreateButton(string label, string Desc, Catagorys page, bool isToggle, bool isActive, Action onClick, Action onDisable = null, bool doesNeedMaster = false)
        {
            try
            {
                var button = new Button(label, Desc, page, onClick, onDisable, isToggle, isActive, doesNeedMaster);
                var tempList = new List<Button>(buttons);
                tempList.Add(button);
                buttons = tempList;
            }
            catch (Exception ex)
            {
                Debug.LogError($"CreateButton Error: {ex}");
            }
        }

    }
}
