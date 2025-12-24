using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static GorillaNetworking.CosmeticsController;
using static TransferrableObject;
using static VioletTemplate.Main.Extentions.GunLib;
using static VioletTemplate.Main.Extentions.PlayerManager;
using static VioletTemplate.Main.Extentions.Serilization;
using static VioletTemplate.Mods.Competitive;
using VioletTemplate.Main;
using VioletTemplate.Main.Extentions;

namespace VioletTemplate.Mods
{
    public class Projectiles : MonoBehaviour
    {
        #region Projectiles
        public static int ProjectileNumber = 2;
        public static float delay;
        public static Color ProjectileColor = Color.white;

        private static int CurrentProjectileCycle = 1;

        private static readonly List<ProjectileInfo> ProjectileList = new List<ProjectileInfo>
        {
            new ProjectileInfo( 0, "GrowingSnowballRightAnchor(Clone)",              "LMACF. RIGHT.", "Snowball"),

            new ProjectileInfo( 2, "WaterBalloonRightAnchor(Clone)",                 "LMAEY. RIGHT.", "Water Balloon"),
            new ProjectileInfo( 3, "VotingRockAnchor_RIGHT(Clone)",                  "LMAMT. RIGHT.", "Voting Rock"),
            new ProjectileInfo( 4, "BucketGiftFunctionalAnchor_Right(Clone)",        "LMAHR. RIGHT.", "Gift"),
            new ProjectileInfo( 5, "ScienceCandyRightAnchor(Clone)",                 "LMAIF. RIGHT.", "Mento"),
            new ProjectileInfo( 6, "FishFoodRightAnchor(Clone)",                     "LMAIP. RIGHT.", "FishFood"),
            new ProjectileInfo( 7, "TrickTreatFunctionalAnchorRIGHT Variant(Clone)", "LMAMO. RIGHT.", "Candy"),
            new ProjectileInfo( 8, "LavaRockAnchor(Clone)",                          "LMAGE. RIGHT.", "Lava Rock"),
            new ProjectileInfo( 9, "AppleRightAnchor(Clone)",                        "LMAMV.",        "Apple"),
            new ProjectileInfo(10, "BookRightAnchor(Clone)",                         "LMAQA. RIGHT.", "Book"),
            new ProjectileInfo(11, "CoinRightAnchor(Clone)",                         "LMAQC.",        "Coin"),
            new ProjectileInfo(12, "EggRightHand_Anchor Variant(Clone)",             "LMAPS. RIGHT.", "Egg"),
            new ProjectileInfo(13, "IceCreamRightAnchor(Clone)",                     "LMARA. LEFT.",  "Ice Cream"),
            new ProjectileInfo(14, "HotDogRightAnchor(Clone)",                       "LMARC.",        "HotDog"),
            new ProjectileInfo(15, "Fireworks_Anchor Variant_Right Hand(Clone)",     "LMAQU. LEFT.",  "Firework"),

            new ProjectileInfo(17, "TurkeyLegRightAnchor(Clone)",                    "LMAUR. RIGHT.", "Turkey Leg"),
            new ProjectileInfo(18, "GrowingStuffingRightAnchor(Clone)",              "LMAUP. RIGHT.", "Growing Stuffing"),

            new ProjectileInfo(23, "GrowingMashedPotatoRightAnchor(Clone)",          "LMAUH. RIGHT.", "Mashed Potato"),
            new ProjectileInfo(24, "LayerDipRightAnchor(Clone)",                     "LMAUF. RIGHT.", "Layer Dip"),

            new ProjectileInfo(26, "CornRightAnchor(Clone)",                         "LMAUT. RIGHT.", "Corn"),
            new ProjectileInfo(27, "ChipsRightAnchor(Clone)",                        "LMAUC. RIGHT.", "Chips"),
            new ProjectileInfo(28, "BerryPieRightAnchor(Clone)",                     "LMAUL. RIGHT.", "Berry Pie"),
            new ProjectileInfo(29, "ApplePieRightAnchor(Clone)",                     "LMAUJ. RIGHT.", "Apple Pie"),

            new ProjectileInfo(30, "Papers_Anchor Variant_Right Hand(Clone)",        "LMASG. RIGHT.", "Papers"),
            new ProjectileInfo(31, "IceCreamScoopRightAnchor(Clone)",                "LMASD. RIGHT.", "Ice Cream Scoop"),

            new ProjectileInfo(32, "FireworkMortarRightAnchor(Clone)",               "LMAEW. RIGHT.", "Firework Mortar"),
            new ProjectileInfo(33, "SalsaRightAnchor(Clone)",                        "LMAUD. RIGHT.", "Salsa"),
            new ProjectileInfo(34, "PumpkinPieRightAnchor(Clone)",                  "LMAUN.",        "Pumpkin Pie"),
            new ProjectileInfo(35, "GoalpostFootball_Anchor_RightHand(Clone)",      "LMATL.",        "Goalpost Football"),
            new ProjectileInfo(36, "PopcornBall_Anchor_Right(Clone)",               "LMATP.",        "Popcorn Ball"),
            new ProjectileInfo(37, "CrackedPlate_Lump_Projectile_Anchor_RIGHT(Clone)","LMAUA.",       "Cracked Plate"),
            new ProjectileInfo(38, "PortableBonfire_Sticks_Anchor_RightHand(Clone)","LMATY.",        "Portable Bonfire")
        };


        public class ProjectileInfo
        {
            public int Number { get; }
            public string ObjectName { get; }
            public string Code { get; }
            public string DisplayName { get; }

            public ProjectileInfo(int number, string objectName, string code, string displayName)
            {
                Number = number;
                ObjectName = objectName;
                Code = code;
                DisplayName = displayName;
            }
        }

        public static ProjectileInfo CurrentProjectile => ProjectileList[CurrentProjectileCycle];

        public static void ChangeProjectile()
        {
            CurrentProjectileCycle = (CurrentProjectileCycle + 1) % ProjectileList.Count;
            Core.ChangeButtonText("Current Projectile", "Current Projectile: " + CurrentProjectile.DisplayName);
            var proj = CurrentProjectile;
            ProjectileNumber = proj.Number;

        }

        private const string BaseHandPath = "Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/";

        public static SnowballThrowable Snowball()
        {
            return GameObject.Find(BaseHandPath + CurrentProjectile.ObjectName).transform.Find(CurrentProjectile.Code).GetComponent<SnowballThrowable>();
        }

        public static void LaunchProjectile(Vector3 velocity, Vector3 position, bool Rhand)
        {
            SnowballThrowable projectile = Snowball();
            if (!projectile.gameObject.activeSelf) projectile.SetSnowballActiveLocal(true); projectile.transform.SetPositionAndRotation(position, new Quaternion());
            if (Rhand) GorillaTagger.Instance.offlineVRRig.RightThrowableProjectileIndex = CurrentProjectile.Number;
            else GorillaTagger.Instance.offlineVRRig.LeftThrowableProjectileIndex = CurrentProjectile.Number;
            if (Time.time > delay)
            {
                delay = Time.time + 0.32f;
                object[] parameters = new object[]
                {
                    position,
                    velocity,
                    Rhand ? RoomSystem.ProjectileSource.RightHand : RoomSystem.ProjectileSource.LeftHand,
                    projectile.LaunchSnowballLocal(position, velocity, 1, true, ProjectileColor).myProjectileCount,
                    true,
                    ProjectileColor.r,
                    ProjectileColor.g,
                    ProjectileColor.b,
                    ProjectileColor.a
                };
                RoomSystem.SendEvent(0, parameters, new NetEventOptions { Reciever = NetEventOptions.RecieverTarget.others, }, false);
            }
        }

        public static void LaunchProjectile()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Overpowered.projModsEnabled)
                {
                    LaunchProjectile(GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 10, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, true);
                }
                else
                {
                    Overpowered.EnableAllProjs();
                }
            }
            else
            {
                Snowball().SetSnowballActiveLocal(false);
            }
        }

        public static void GrabProjectile()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Overpowered.projModsEnabled)
                {
                    LaunchProjectile(GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 0, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, true);
                }
                else
                {
                    Overpowered.EnableAllProjs();
                }
            }
            else
            {
                Snowball().SetSnowballActiveLocal(false);
            }
        }
        #endregion

        #region SplashMods
        public static void SendSplash(Vector3 position, bool Big_Splash, NetPlayer plr)
        {
            SmoothSerilize(new PlayerTransformData
            {
                BodyPos = position + new Vector3(0,-3,0),
            }, new int[]
            {
                plr.ActorNumber,
            });
             GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[] { position, Quaternion.identity, 1f, 0.5f, true, false });
        }

        public static void SplashGun()
        {
            StartBothGuns(() =>
            {
                if (Time.time > Competitive.Delay)
                {
                    Competitive.Delay = Time.time + 0.2f;
                    SendSplash(TargetRig.transform.position, true, TargetPlayer);
                }
            });
            if (TargetRig == null || TargetPlayer == null)
            {
                if (Time.time > Competitive.Delay)
                {
                    Competitive.Delay = Time.time + 0.1f;
                    ResetSerilization();
                }
            }
        }


        public static void SendSplash(Vector3 position, RpcTarget Reciver)
        {
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", Reciver, new object[] { position, Quaternion.identity, 1f, 0.5f, true, false });
        }
        #endregion

        public enum CosmeticID
        {
            YellowHandBootsLeft = 0,
            YellowHandBootsRight = 1,
            CloudHandBootsLeft = 2,
            CloudHandBootsRight = 3,
            GoldenHandBootsLeft = 4,
            GoldenHandBootsRight = 5,
            ElectricGuitar = 6,
            GoldenEletricGuitar = 8,
            BlackUmbrella = 10,
            GoldForkRight = 11,
            GoldKnife = 12,
            RegularFork = 13,
            RegularKnife = 14,
            ColorfulUmbrella = 15,
            GoldenUmbrella = 16,
            RubberDuck = 17,
            REGULARWRENCH = 18,
            TurkeyLeg = 19,
            Sparkler = 20,
            Icicle = 21,
            GoldRose = 22,
            GoldWrench = 23,
            PinkRose = 24,
            RedRose = 25,
            FourLeafClover = 26,
            BlackRose = 27,
            ModStick = 28,
            GoldFourLeafClover = 29,
            StarPrincessWand = 30,
            CherryBlossom = 31,
            CherryBlossomRoseGold = 32,
            Bubbler = 33,
            Popsicle = 34,
            CandyCane = 35,
            StarBalloon = 37,
            DiamondBalloon = 38,
            ChocolateDonutBalloon = 39,
            PinkDonutBalloon = 40,
            HeartBalloon = 41,
            SpiderWebUmbrella_Prefab = 42,
            UnicornStaff = 43,
            GhostBalloon = 44,
            GiantCandyBar = 45,
            CandyBarFunSize = 46,
            Yorick = 47,
            TurkeyToy = 48,
            CranberryCan = 49,
            CandyApple = 50,
            CaramelApple = 51,
            BalloonTurkey = 52,
            FryingPan = 53,
            Ladle = 54,
            TurkeyLeg2022 = 55,
            PieSlice = 56,
            CornOnTheCob = 57,
            PartyHorn = 58,
            GoldGorillaIdol = 59,
            ToyFrog = 60,
            GorillaTagFlag = 61,
            HappySadFlag = 62,
            Pinwheel = 63,
            GorillaBalloon = 64,
            CloudUmbrella = 65,
            GingerbreadMan = 66,
            HolidayPudding = 67,
            UglyTree = 68,
            ToyGorillaElf = 69,
            ChristmasUmbrella22 = 70,
            GiantCandyCane = 71,
            SnowShovel = 72,
            SnowflakeWand = 73,
            SqueezyPenguin = 74,
            CocoaMug = 75,
            Anniv2Cupcake = 76,
            HeartCandyBalloon = 77,
            MintHeartCandyBalloon = 78,
            TeddyBear = 79,
            SqueezyDogToy = 80,
            HotSnack = 81,
            PolarTeddyBear = 82,
            WinterUmbrella23 = 83,
            SnowBlower = 84,
            HockeyStickA = 85,
            HockeyStickB = 86,
            PotOGold = 87,
            SqueezyDragon = 88,
            DragonBalloon = 89,
            D20 = 90,
            BardLute = 91,
            SqueezyMonkeye = 92,
            WizardStaff = 93,
            Spatula = 94,
            DaisyBalloon = 95,
            SqueezyBumblebee = 96,
            Duster = 97,
            Broom = 98,
            CheeseWedge = 99,
            PotLid = 100,
            Sponge = 101,
            Dustpan = 102,
            RollingPin = 103,
            HandVacuum = 104,
            Banjo = 105,
            SqueezyCoyote = 106,
            HorseStick = 107,
            PickAxe = 108,
            SqueezyShark = 109,
            HotDog = 110,
            WaffleConeIceCream = 111,
            CoconutDrink = 112,
            SmilingSunBalloon = 113,
            BeachBallUmbrella = 114,
            SqueezyDolphin = 115,
            BugSpray = 116,
            CardboardBinoculars = 117,
            Compass = 118,
            MonkeIceCream = 119,
            MarshmallowSkewer = 120,
            Milkshake = 121,
            Suitcase = 122,
            MonkeCandle = 123,
            BatBalloon = 124,
            CrystalUmbrella = 125,
            CrystalWhistle = 126,
            HandDrill = 127,
            EdibleMushroom = 128,
            Geode = 129,
            SqueezyEel = 130,
            Flare = 131,
            EdibleAppleRed = 132,
            CoachWhistle = 133,
            BalloonSchoolBus = 134,
            SqueezyBottleWater = 135,
            SpitballStraw = 136,
            NoveltyPencil = 137,
            Backpack = 138,
            LavaConchShell = 140,
            EdibleHotHotPepper = 141,
            FireFan = 142,
            MagmaMug = 143,
            SqueezyPhoenix = 144,
            MagmaUmbrella = 146,
            VolcanoHammer = 147,
            GuitarSkull_Functional = 148,
            SqueezyCatToy = 149,
            EdibleGummySpider = 150,
            CupCauldron = 151,
            PitchforkPlastic = 152,
            ApplePieSlice = 153,
            SqueezySquirrel = 154,
            GravyBoat = 155,
            BalloonMapleLeaf = 156,
            ConfettiPopper = 157,
            Dreidel = 158,
            HandBell = 159,
            HandSaw = 160,
            IceStaff = 161,
            MonkeCrackerToy = 162,
            SnowGlobe = 163,
            StuffedMonke = 164,
            TreeCookie = 165,
            WoodTrainWhistle = 166,
            LightningRod = 167,
            RayGun = 168,
            Stopwatch = 169,
            HorseShoeMagnet = 171,
            TuningFork = 172,
            PaperAirplane = 173,
            MagnifyingGlass = 174,
            Flamethrower = 175,
            FireExtinguisher = 176,
            SqueezyHeart = 177,
            HeartStaff = 178,
            Anniv3Cake = 179,
            EdibleChocolatePepper = 180,
            BeekeeperSmoker = 181,
            PersianRugUmbrella = 182,
            GragerToy = 183,
            ChocolateBunny = 184,
            BalloonHoneyBee = 185,
            StuffedBunny = 186,
            WateringCan = 187,
            StuffedBunnyYellow = 188,
            StuffedBunnyPink = 189,
            GTMonkePlush = 190,
            LeafNinjaStarFunctional = 191,
            StormingUmbrella = 192,
            BalloonDiamondKite = 193,
            CheckeredFlag = 194,
            FlagFlames = 195,
            LeafNinjaStarColor1 = 196,
            LeafNinjaStarColor2 = 197,
            MonkeHandBanner = 198,
            CosmicD20 = 199,
            D6 = 200,
            CosmicD6 = 201,
            Fireball = 202,
            FlagPrideStandard = 203,
            FlagPrideTrans = 204,
            GlitterGun = 205,
            LionPlushy = 206,
            RainbowLionPlushy = 207,
            RainbowUmbrella = 208,
            RainbowUmbrellaVar1 = 209,
            RainbowUmbrellaVar2 = 210,
            Keytar = 211,
            DropZone212 = 212,
            DropZone214 = 214,
            DropZone215 = 215,
            DropZone216 = 216,
            DropZone217 = 217,
            DropZone218 = 218,
            DropZone219 = 219,
            DropZone220 = 220,
            DropZone221 = 221,
            DropZone222 = 222,
            DropZone223 = 223,
            AcousticGuitar = 232,
            GoldenAcousticGuitar = 233,
            BagelCreamCheese = 487,
            BalloonPretzel = 488,
            Dumbbell = 489,
            MetroCoffee = 491,
            StatueLibertyTorch = 492,
            RCRemoteBlimpVariant = 493,
            MonkeCandlePurple = 494,
            MonkeCandleGreen = 495,
            SparklerV2_RWB = 496,
            SparklerV2_RED = 497,
            SparklerV2_WHITE = 498,
            SparklerV2_BLUE = 499,
            ConchShell = 500,
            LifeGuardFloater = 501,
            OrcaBalloon = 502,
            WitchBloodBook = 503,
            RCPlane_SharkRemote = 506,
            LightShowToy = 507,
            PlainMilkCarton = 509,
            ChocolateMilkCarton = 510,
            StrawberryMilkCarton = 511,
            SpinachCan = 513,
            Guitar80sV_Functional = 514,
            GuitarBass_Functional = 515,
            WAMSqueezie = 516,
            CyberNinjaStar = 517,
            RaccoonPromo = 518,
            CrawfishPlushy = 519,
            LucysParasol = 520,
            SnakeCane = 521,
            PrimalTorch = 522,
            SkeletonMarionette = 524,
            PlayingCard = 525,
            MonkeKola = 526,
            RottenPumpkin = 528,
            CoconutMystic = 529,
            TrickOrTreatBucket = 530,
            EdibleSugarSkull = 531,
            MonkePlushGT = 532,
            BackpackGT = 533,
            BallCupToy = 534,
            BatCane = 535,
            NightmareHorsey = 536,
            TrickyTreatBucket = 538,
            PunchingBag = 544,
            MayanRattle = 545,
            MayanGoldenStaff = 546,
            RCRemoteDragonVariant = 553,
            DropZone558 = 558,
            ToyWoodenSword = 560,
            ToyWoodenShield = 561,
            PlumbBob = 563,
            LeafblowerGun = 564,
            ElfLauncher = 566,
            BananaNoiseMaker = 567,
            ClackerToy = 568,
            SparklerV2_GB = 569,
            SparklerV2_O = 570,
            SparklerV2_P = 571,
            PartyBox = 572,
            CandleNewYearsGold = 575,
            CandleNewYearsMaroon = 576,
            CandleNewYearsPurple = 577,
            RubberbandCarHotRod = 579,
            GreenWindUpCar = 582,
            RedWindUpCar = 583,
            ChartreuseWindUpCar = 584,
            FlamesWindUpCar = 585,
            ChineseLanternBalloon_Remake = 586,
            FireCrackers = 587,
            WoodenSnake = 588,
            CanSnake_Functional = 591,
            EdibleLoveCookie = 592,
            VenusFlyTrap = 593,
            BirthdayCupcake4 = 594,
            HeartRayBubbler = 595,
            GiantHeartLollipop = 596,
            PaperAirplaneSquare = 597,
            BirthdayNoiseMaker4 = 599,
            SmokeBomb = 600,
            SqueezyLemming = 601,
            LemmingWindUpToy = 602,
            ThrowableHeart = 603,
            SqueezyLemmingAngry = 607,
            MarionetteMonke = 608,
            NekoAtsumeCatPlushV1 = 609,
            NekoAtsumeCatPlushV2 = 610,
            RobotFistCannon = 611,
            RobotShield = 612,
            MonkePlushInfected = 614,
            MysteryCube = 616,
            BarrelLeprechaun = 618,
            MushroomUmbrella = 619,
            RCHoverboardRemote = 620,
            Barrel = 621,
            ShirtBackPack = 622,
            ButterflyNet = 623,
            GiantTweezers = 624,
            SqueezyFrogSticky = 625,
            WhoopeeCushion = 626,
            ForestBranch = 627,
            GlowBugsInJar = 628,
            SprayCanSunScreen = 629,
            CartonOfEggs = 630,
            ShadeRevealer = 633,
            FlamingBanner = 636,
            ForestGuideStick = 638,
            SandPirateTelescope = 643,
            HoldableLance = 644,
            SwitchBladeComb = 645,
            PartyBalloonMonkeFace = 646,
            MedusaEye = 647,
            TalkingSkull = 648,
            AccessoryOnStick_Emoji = 649,
            Toothbrush = 650,
            RubberChickenSword = 651,
            ShirtGiantKey = 652,
            Slate = 654,
            AccessoryOnStick_Glasses = 655,
            AccessoryOnStick_Mustache = 656,
            AccessoryOnStick_MonkeFace = 657,
            PartyPayloadBackpack = 658,
            ShirtPackDougBug = 661,
            DigiCoolDougToy = 662,
            SparklerV2_NeonPink = 671,
            SparklerV2_Green = 672,
            MonkeCandleSkyBlue = 673,
            MonkeCandleWhite = 674,
            MonkeCandleYellow = 675,
            LighterA = 676,
            LighterB = 677,
            LighterC = 678,
            MarshmallowShooter = 679,
            GiantStrongmanBarbell = 681,
            UmbrellaJellyfish = 682,
            SqueezyHippo = 683,
            ChickenHorseStick = 684,
            StaffMonkeKing = 686,
            GreenMonkeLauncher = 687,
            SwordLightningRod = 690,
            PitCrewGun = 691,
            CreepyDoll = 692,
            FlamingTrashCan = 696,
            SteeringWheel = 699,
            MushroomSeedPacket = 700,
            DropZone701 = 701,
            AxolotlPlushieV1 = 702,
            AxolotlPlushieV2 = 703,
            AxolotlPlushieV3 = 704,
            HookHand = 705,
            ShipSteamPunkRemote = 707,
            ShipToyDama = 708,
            SpittingDinoUmbrella = 709
        }

        #region Cosmetics
        public static int index = 0;
        public static void SpawnCosmetic(CosmeticID type, int Type)
        {
            TransferrableObject transferrableObject = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.allObjects[(int)type];
            VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, (int)type);
            transferrableObject.gameObject.SetActive(true);
            transferrableObject.storedZone = BodyDockPositions.DropPositions.RightArm;
            transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;
            if (ControllerInputPoller.instance.rightGrab)
            {
               if (Time.time > Delay)
                {
                    Delay = Time.time + 0.4f;
                    if (Type == 1)
                    {
                        ThrowableHoldableCosmetic projectile = transferrableObject.gameObject.GetComponent<ThrowableHoldableCosmetic>();
                        projectile._events.Activate.RaiseAll(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, Quaternion.identity, new Vector3(), 1f);
                    }
                    else if (Type == 2)
                    {
                        DeployableObject projectile = transferrableObject.gameObject.GetComponent<DeployableObject>();
                        PhotonSignal<long, int, long> deploySignal = (PhotonSignal<long, int, long>)Traverse.Create(projectile).Field("_deploySignal").GetValue();
                        deploySignal.Raise(ReceiverGroup.All, BitPackUtils.PackWorldPosForNetwork(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position), BitPackUtils.PackQuaternionForNetwork(Quaternion.identity), BitPackUtils.PackWorldPosForNetwork(Vector3.zero));
                    }

                    else if (Type == 3)
                    {
                        PaperPlaneThrowable projectile = transferrableObject.gameObject.GetComponent<PaperPlaneThrowable>();
                        PaperPlaneThrowable.FetchViewID(projectile);
                        projectile.GetThrowableId();
                        projectile.LaunchProjectileLocal(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation, GorillaTagger.Instance.offlineVRRig.rightHandTransform.up *8);
                        Hashtable table = new Hashtable();
                        table[0] = PaperPlaneThrowable.kProjectileEvent;
                        table[1] = PositionState.InRightHand;
                        table[2] = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
                        table[3] = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;
                        table[4] = GorillaTagger.Instance.offlineVRRig.rightHandTransform.up * 8;
                        PhotonNetwork.RaiseEvent(176, table, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                     }

                }
            }
        }

        public static void EquipCosmetic(int type)
        {
            TransferrableObject transferrableObject = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.allObjects[(int)type];
            if (Time.time > Delay)
            {
                Delay = Time.time + 0.8f;
                if (!transferrableObject.gameObject.activeSelf)
                {
                    VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, (int)type);
                    VRRig.LocalRig.myBodyDockPositions.allObjects[(int)type].gameObject.SetActive(true);
                    transferrableObject.storedZone = BodyDockPositions.DropPositions.RightArm;
                    transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;
                    string name = VRRig.LocalRig.myBodyDockPositions.allObjects[(int)type].gameObject.transform.parent.name;

                    if (name.EndsWith("(Clone)"))
                        name = name.Substring(0, name.Length - "(Clone)".Length);
                    Notifications.Show(name, "");
                }

            }
        }
        #endregion
    }
}