using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public static class Functions
    {
        public static void SendMessage()
        {
            Console.WriteLine();
        }

        public static void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static int Switch(int value) => BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
        public static uint Switch(uint value) => BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

        public static uint BKDRHash(string str)
        {
            str = str.ToUpper();
            uint seed = 131;
            uint hash = 0;
            int length = str.Length;

            //if (length > 31)
            //    length = 31;

            for (int i = 0; i < length; i++)
                hash = (hash * seed) + str[i];

            return hash;
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            List<char> charList = new List<char>();
            do charList.Add((char)binaryReader.ReadByte());
            while (charList.Last() != '\0');
            charList.Remove('\0');

            if (charList.Count % 2 == 0) binaryReader.BaseStream.Position += 1;

            return new string(charList.ToArray());
        }

        public static bool IsDyna(this AssetType assetType) => assetType.GetCode().Equals("DYNA");

        public static string GetCode(this AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Null: return "NULL";
                case AssetType.AnimationList: return "ALST";
                case AssetType.Animation: return "ANIM";
                case AssetType.AnimationTable: return "ATBL";
                case AssetType.AttackTable: return "ATKT";
                case AssetType.BinkVideo: return "BINK";
                case AssetType.Boulder: return "BOUL";
                case AssetType.BSP: return "BSP";
                case AssetType.Button: return "BUTN";
                case AssetType.Camera: return "CAM";
                case AssetType.CameraCurve: return "CCRV";
                case AssetType.Counter: return "CNTR";
                case AssetType.CollisionTable: return "COLL";
                case AssetType.Conditional: return "COND";
                case AssetType.Credits: return "CRDT";
                case AssetType.Cutscene: return "CSN";
                case AssetType.CutsceneManager: return "CSNM";
                case AssetType.CutsceneStreamingSound: return "CSSS";
                case AssetType.CutsceneTableOfContents: return "CTOC";
                case AssetType.Destructible: return "DEST";
                case AssetType.Dispatcher: return "DPAT";
                case AssetType.DiscoFloor: return "DSCO";
                case AssetType.DestructibleObject: return "DSTR";
                case AssetType.DashTrack: return "DTRK";
                case AssetType.Duplicator: return "DUPC";
                case AssetType.ElectricArcGenerator: return "EGEN";
                case AssetType.Environment: return "ENV";
                case AssetType.Flythrough: return "FLY";
                case AssetType.Fog: return "FOG";
                case AssetType.GrassMesh: return "GRSM";
                case AssetType.Group: return "GRUP";
                case AssetType.Gust: return "GUST";
                case AssetType.Hangable: return "HANG";
                case AssetType.JawDataTable: return "JAW";
                case AssetType.JSP: return "JSP";
                case AssetType.Light: return "LITE";
                case AssetType.LightKit: return "LKIT";
                case AssetType.LobMaster: return "LOBM";
                case AssetType.LevelOfDetailTable: return "LODT";
                case AssetType.SurfaceMapper: return "MAPR";
                case AssetType.ModelInfo: return "MINF";
                case AssetType.Model: return "MODL";
                case AssetType.MorphTarget: return "MPHT";
                case AssetType.Marker: return "MRKR";
                case AssetType.MovePoint: return "MVPT";
                case AssetType.NavigationMesh: return "NGMS";
                case AssetType.NPC: return "NPC";
                case AssetType.NPCSettings: return "NPCS";
                case AssetType.OneLiner: return "ONEL";
                case AssetType.ParticleEmitter: return "PARE";
                case AssetType.ParticleProperties: return "PARP";
                case AssetType.ParticleSystem: return "PARS";
                case AssetType.Pendulum: return "PEND";
                case AssetType.ProgressScript: return "PGRS";
                case AssetType.PickupTable: return "PICK";
                case AssetType.PipeInfoTable: return "PIPT";
                case AssetType.Pickup: return "PKUP";
                case AssetType.Platform: return "PLAT";
                case AssetType.Player: return "PLYR";
                case AssetType.Portal: return "PORT";
                case AssetType.Projectile: return "PRJT";
                case AssetType.ReactiveAnimation: return "RANM";
                case AssetType.RawImage: return "RAW";
                case AssetType.Texture: return "RWTX";
                case AssetType.Script: return "SCRP";
                case AssetType.SDFX: return "SDFX";
                case AssetType.SFX: return "SFX";
                case AssetType.SoundGroup: return "SGRP";
                case AssetType.ShadowTable: return "SHDW";
                case AssetType.Shrapnel: return "SHRP";
                case AssetType.SimpleObject: return "SIMP";
                case AssetType.SlideProperty: return "SLID";
                case AssetType.Sound: return "SND";
                case AssetType.SoundInfo: return "SNDI";
                case AssetType.StreamingSound: return "SNDS";
                case AssetType.Spline: return "SPLN";
                case AssetType.SplinePath: return "SPLP";
                case AssetType.SceneSettings: return "SSET";
                case AssetType.Subtitles: return "SUBT";
                case AssetType.Surface: return "SURF";
                case AssetType.TEXS: return "TEXS";
                case AssetType.Text: return "TEXT";
                case AssetType.Timer: return "TIMR";
                case AssetType.Track: return "TRCK";
                case AssetType.PickupTypes: return "TPIK";
                case AssetType.Trigger: return "TRIG";
                case AssetType.ThrowableTable: return "TRWT";
                case AssetType.UserInterface: return "UI";
                case AssetType.UIFN: return "UIFN";
                case AssetType.UserInterfaceFont: return "UIFT";
                case AssetType.UserInterfaceMotion: return "UIM";
                case AssetType.VIL: return "VIL";
                case AssetType.VILProperties: return "VILP";
                case AssetType.Volume: return "VOLU";
                case AssetType.WireframeModel: return "WIRE";
                case AssetType.ZipLine: return "ZLIN";
                case AssetType.Checkpoint:
                case AssetType.ParticleGenerator:
                case AssetType.Bomber:
                case AssetType.BossUnderminerDrill:
                case AssetType.BossUnderminerUM:
                case AssetType.Chicken:
                case AssetType.Driller:
                case AssetType.Enforcer:
                case AssetType.Humanoid:
                case AssetType.Rat:
                case AssetType.RobotTank:
                case AssetType.Scientist:
                case AssetType.Shooter:
                case AssetType.EnemySB:
                case AssetType.Spawner:
                case AssetType.CastNCrew:
                case AssetType.Critter:
                case AssetType.Dennis:
                case AssetType.FrogFish:
                case AssetType.Mindy:
                case AssetType.Neptune:
                case AssetType.Enemy:
                case AssetType.Crate:
                case AssetType.Turret:
                case AssetType.IncrediblesIcon:
                case AssetType.InterestPointer:
                case AssetType.JSPExtraData:
                case AssetType.SceneProperties:
                case AssetType.Unknown_EBC04E7B:
                case AssetType.Unknown_LensFlare:
                case AssetType.AudioConversation:
                case AssetType.CameraBinaryPoi:
                case AssetType.CameraPreset:
                case AssetType.CameraTransitionPath:
                case AssetType.CameraTransitionTime:
                case AssetType.BossBrain:
                case AssetType.Flamethrower:
                case AssetType.LensFlareElement:
                case AssetType.LightEffectFlicker:
                case AssetType.LightEffectStrobe:
                case AssetType.Lightning:
                case AssetType.Rumble:
                case AssetType.RumbleBoxEmitter:
                case AssetType.RumbleSphericalEmitter:
                case AssetType.ScreenFade:
                case AssetType.ScreenWarp:
                case AssetType.Splash:
                case AssetType.WaterHose:
                case AssetType.Grass:
                case AssetType.LightEffect:
                case AssetType.SmokeEmitter:
                case AssetType.SparkEmitter:
                case AssetType.Spotlight:
                case AssetType.UberLaser:
                case AssetType.WaterBody:
                case AssetType.BoulderGenerator:
                case AssetType.BusStop:
                case AssetType.CameraTweak:
                case AssetType.FlythroughObject:
                case AssetType.FreezableObject:
                case AssetType.Grapple:
                case AssetType.HangableObject:
                case AssetType.IncrediblesPickup:
                case AssetType.NPCSettingsObject:
                case AssetType.RaceTimer:
                case AssetType.Ring:
                case AssetType.RingControl:
                case AssetType.RubbleGenerator:
                case AssetType.Taxi:
                case AssetType.TeleportBox:
                case AssetType.TurretObject:
                case AssetType.Vent:
                case AssetType.VentType:
                case AssetType.BulletMark:
                case AssetType.BulletTime:
                case AssetType.BungeeDrop:
                case AssetType.BungeeHook:
                case AssetType.CameraParamAsset:
                case AssetType.DashCameraSpline:
                case AssetType.FlameEmitter:
                case AssetType.LaserBeam:
                case AssetType.RbandCameraAsset:
                case AssetType.TalkBox:
                case AssetType.TaskBox:
                case AssetType.TextBox:
                case AssetType.TrainCar:
                case AssetType.TrainJunction:
                case AssetType.HudImage:
                case AssetType.HudMeterFont:
                case AssetType.HudMeterUnit:
                case AssetType.HudModel:
                case AssetType.HudText:
                case AssetType.InteractionIceBridge:
                case AssetType.InteractionLaunch:
                case AssetType.InteractionLift:
                case AssetType.InteractionSwitchLever:
                case AssetType.InteractionTurn:
                case AssetType.FunctionGenerator:
                case AssetType.LogicReference:
                case AssetType.NPCCoverPoint:
                case AssetType.NPCCustomAV:
                case AssetType.NPCGroup:
                case AssetType.Pointer:
                case AssetType.UserInterfaceBox:
                case AssetType.UserInterfaceController:
                case AssetType.UserInterfaceImage:
                case AssetType.UserInterfaceModel:
                case AssetType.UserInterfaceText:
                case AssetType.UserInterfaceTextUserString:
                    return "DYNA";
            }

            throw new Exception("Unknown asset type: " + assetType);
        }

        public static void AddString(this List<byte> listBytes, string writeString)
        {
            foreach (char i in writeString)
                listBytes.Add((byte)i);

            if (writeString.Length % 2 == 0) listBytes.AddRange(new byte[] { 0, 0 });
            if (writeString.Length % 2 == 1) listBytes.AddRange(new byte[] { 0 });
        }

        public static void AddBigEndian(this List<byte> listBytes, float value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, int value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, uint value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, short value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, ushort value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, byte value)
        {
            listBytes.Add(value);
        }

        public static AssetType ToAssetType(this DynaType dynaType)
        {
            switch (dynaType)
            {
                case DynaType.audio__conversation: return AssetType.AudioConversation;
                case DynaType.camera__binary_poi: return AssetType.CameraBinaryPoi;
                case DynaType.camera__preset: return AssetType.CameraPreset;
                case DynaType.camera__transition_path: return AssetType.CameraTransitionPath;
                case DynaType.camera__transition_time: return AssetType.CameraTransitionTime;
                case DynaType.Checkpoint: return AssetType.Checkpoint;
                case DynaType.effect__BossBrain: return AssetType.BossBrain;
                case DynaType.effect__Flamethrower: return AssetType.Flamethrower;
                case DynaType.effect__grass: return AssetType.Grass;
                case DynaType.effect__LensFlareElement: return AssetType.LensFlareElement;
                case DynaType.effect__light: return AssetType.LightEffect;
                case DynaType.effect__LightEffectFlicker: return AssetType.LightEffectFlicker;
                case DynaType.effect__LightEffectStrobe: return AssetType.LightEffectStrobe;
                case DynaType.effect__Lightning: return AssetType.Lightning;
                case DynaType.Effect__particle_generator: return AssetType.ParticleGenerator;
                case DynaType.effect__Rumble: return AssetType.Rumble;
                case DynaType.effect__RumbleBoxEmitter: return AssetType.RumbleBoxEmitter;
                case DynaType.effect__RumbleSphericalEmitter: return AssetType.RumbleSphericalEmitter;
                case DynaType.effect__ScreenFade: return AssetType.ScreenFade;
                case DynaType.effect__ScreenWarp: return AssetType.ScreenWarp;
                case DynaType.effect__smoke_emitter: return AssetType.SmokeEmitter;
                case DynaType.effect__spark_emitter: return AssetType.SparkEmitter;
                case DynaType.effect__Splash: return AssetType.Splash;
                case DynaType.effect__spotlight: return AssetType.Spotlight;
                case DynaType.effect__uber_laser: return AssetType.UberLaser;
                case DynaType.effect__Waterhose: return AssetType.WaterHose;
                case DynaType.effect__water_body: return AssetType.WaterBody;
                case DynaType.Enemy__IN2__Bomber: return AssetType.Bomber;
                case DynaType.Enemy__IN2__BossUnderminerDrill: return AssetType.BossUnderminerDrill;
                case DynaType.Enemy__IN2__BossUnderminerUM: return AssetType.BossUnderminerUM;
                case DynaType.Enemy__IN2__Chicken: return AssetType.Chicken;
                case DynaType.Enemy__IN2__Driller: return AssetType.Driller;
                case DynaType.Enemy__IN2__Enforcer: return AssetType.Enforcer;
                case DynaType.Enemy__IN2__Humanoid: return AssetType.Humanoid;
                case DynaType.Enemy__IN2__Rat: return AssetType.Rat;
                case DynaType.Enemy__IN2__RobotTank: return AssetType.RobotTank;
                case DynaType.Enemy__IN2__Scientist: return AssetType.Scientist;
                case DynaType.Enemy__IN2__Shooter: return AssetType.Shooter;
                case DynaType.Enemy__SB: return AssetType.EnemySB;
                case DynaType.Enemy__SB__BucketOTron: return AssetType.Spawner;
                case DynaType.Enemy__SB__CastNCrew: return AssetType.CastNCrew;
                case DynaType.Enemy__SB__Critter: return AssetType.Critter;
                case DynaType.Enemy__SB__Dennis: return AssetType.Dennis;
                case DynaType.Enemy__SB__FrogFish: return AssetType.FrogFish;
                case DynaType.Enemy__SB__Mindy: return AssetType.Mindy;
                case DynaType.Enemy__SB__Neptune: return AssetType.Neptune;
                case DynaType.Enemy__SB__Standard: return AssetType.Enemy;
                case DynaType.Enemy__SB__SupplyCrate: return AssetType.Crate;
                case DynaType.Enemy__SB__Turret: return AssetType.Turret;
                case DynaType.game_object__BoulderGenerator: return AssetType.BoulderGenerator;
                case DynaType.game_object__bullet_mark: return AssetType.BulletMark;
                case DynaType.game_object__bullet_time: return AssetType.BulletTime;
                case DynaType.game_object__bungee_drop: return AssetType.BungeeDrop;
                case DynaType.game_object__bungee_hook: return AssetType.BungeeHook;
                case DynaType.game_object__BusStop: return AssetType.BusStop;
                case DynaType.game_object__camera_param_asset: return AssetType.CameraParamAsset;
                case DynaType.game_object__Camera_Tweak: return AssetType.CameraTweak;
                case DynaType.game_object__dash_camera_spline: return AssetType.DashCameraSpline;
                case DynaType.game_object__flame_emitter: return AssetType.FlameEmitter;
                case DynaType.game_object__Flythrough: return AssetType.FlythroughObject;
                case DynaType.game_object__FreezableObject: return AssetType.FreezableObject;
                case DynaType.game_object__Grapple: return AssetType.Grapple;
                case DynaType.game_object__Hangable: return AssetType.HangableObject;
                case DynaType.game_object__IN_Pickup: return AssetType.IncrediblesPickup;
                case DynaType.game_object__laser_beam: return AssetType.LaserBeam;
                case DynaType.game_object__NPCSettings: return AssetType.NPCSettingsObject;
                case DynaType.game_object__RaceTimer: return AssetType.RaceTimer;
                case DynaType.game_object__rband_camera_asset: return AssetType.RbandCameraAsset;
                case DynaType.game_object__Ring: return AssetType.Ring;
                case DynaType.game_object__RingControl: return AssetType.RingControl;
                case DynaType.game_object__RubbleGenerator: return AssetType.RubbleGenerator;
                case DynaType.game_object__talk_box: return AssetType.TalkBox;
                case DynaType.game_object__task_box: return AssetType.TaskBox;
                case DynaType.game_object__Taxi: return AssetType.Taxi;
                case DynaType.game_object__Teleport: return AssetType.TeleportBox;
                case DynaType.game_object__text_box: return AssetType.TextBox;
                case DynaType.game_object__train_car: return AssetType.TrainCar;
                case DynaType.game_object__train_junction: return AssetType.TrainJunction;
                case DynaType.game_object__Turret: return AssetType.TurretObject;
                case DynaType.game_object__Vent: return AssetType.Vent;
                case DynaType.game_object__VentType: return AssetType.VentType;
                case DynaType.hud__image: return AssetType.HudImage;
                case DynaType.hud__meter__font: return AssetType.HudMeterFont;
                case DynaType.hud__meter__unit: return AssetType.HudMeterUnit;
                case DynaType.hud__model: return AssetType.HudModel;
                case DynaType.hud__text: return AssetType.HudText;
                case DynaType.Incredibles__Icon: return AssetType.IncrediblesIcon;
                case DynaType.interaction__IceBridge: return AssetType.InteractionIceBridge;
                case DynaType.interaction__Launch: return AssetType.InteractionLaunch;
                case DynaType.interaction__Lift: return AssetType.InteractionLift;
                case DynaType.interaction__SwitchLever: return AssetType.InteractionSwitchLever;
                case DynaType.interaction__Turn: return AssetType.InteractionTurn;
                case DynaType.Interest_Pointer: return AssetType.InterestPointer;
                case DynaType.JSPExtraData: return AssetType.JSPExtraData;
                case DynaType.logic__FunctionGenerator: return AssetType.FunctionGenerator;
                case DynaType.logic__reference: return AssetType.LogicReference;
                case DynaType.npc__CoverPoint: return AssetType.NPCCoverPoint;
                case DynaType.npc__group: return AssetType.NPCGroup;
                case DynaType.npc__NPC_Custom_AV: return AssetType.NPCCustomAV;
                case DynaType.pointer: return AssetType.Pointer;
                case DynaType.SceneProperties: return AssetType.SceneProperties;
                case DynaType.ui__box: return AssetType.UserInterfaceBox;
                case DynaType.ui__controller: return AssetType.UserInterfaceController;
                case DynaType.ui__image: return AssetType.UserInterfaceImage;
                case DynaType.ui__model: return AssetType.UserInterfaceModel;
                case DynaType.ui__text: return AssetType.UserInterfaceText;
                case DynaType.ui__text__userstring: return AssetType.UserInterfaceTextUserString;
                case DynaType.Unknown_EBC04E7B: return AssetType.Unknown_EBC04E7B;
                case DynaType.Unknown_LensFlareSomething: return AssetType.Unknown_LensFlare;
            }

            throw new Exception("Unknown DYNA type: " + dynaType.ToString("X8"));
        }
    }
}