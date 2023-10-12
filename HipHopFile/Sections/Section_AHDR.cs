﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_AHDR : HipSection
    {
        public uint assetID;
        public AssetType assetType;
        public int fileOffset;
        public int fileSize;
        public int plusValue;
        public AHDRFlags flags;
        public Section_ADBG ADBG;

        public byte[] data;

        public Section_AHDR() : base(Section.AHDR)
        {
        }

        public Section_AHDR(uint assetID, string assetType, AHDRFlags flags, Section_ADBG ADBG) : base(Section.AHDR)
        {
            this.assetID = assetID;
            this.assetType = AssetType.Null;
            foreach (var at in Enum.GetValues(typeof(AssetType)))
                if (at.ToString().Equals(assetType))
                {
                    this.assetType = (AssetType)at;
                    break;
                }
            if (this.assetType == AssetType.Null)
                throw new Exception("Unknown asset type: " + assetType);

            this.flags = flags;
            this.ADBG = ADBG;
        }

        public Section_AHDR(uint assetID, AssetType assetType, AHDRFlags flags, Section_ADBG ADBG, byte[] data) : base(Section.AHDR)
        {
            this.assetID = assetID;
            this.assetType = assetType;
            this.flags = flags;
            this.ADBG = ADBG;
            this.data = data;
        }

        public Section_AHDR(BinaryReader binaryReader, Platform platform) : base(binaryReader, Section.AHDR)
        {
            assetID = Switch(binaryReader.ReadUInt32());
            var tempAssetType = new string(binaryReader.ReadChars(4));
            fileOffset = Switch(binaryReader.ReadInt32());
            fileSize = Switch(binaryReader.ReadInt32());
            plusValue = Switch(binaryReader.ReadInt32());
            flags = (AHDRFlags)Switch(binaryReader.ReadInt32());

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ADBG.ToString()) throw new Exception();
            ADBG = new Section_ADBG(binaryReader);

            long savePosition = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = fileOffset;
            data = binaryReader.ReadBytes(fileSize);
            binaryReader.BaseStream.Position = savePosition;

            assetType = AssetTypeFromString(tempAssetType, platform);
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.AHDR;

            listBytes.AddBigEndian(assetID);
            foreach (char i in assetType.GetCode().PadRight(4))
                listBytes.Add((byte)i);
            listBytes.AddBigEndian(fileOffset);
            listBytes.AddBigEndian(fileSize);
            listBytes.AddBigEndian(plusValue);
            listBytes.AddBigEndian((int)flags);

            ADBG.SetBytes(game, platform, ref listBytes);
        }

        public override int GetHashCode()
        {
            return (int)assetID;
        }

        public bool IsDyna => assetType.IsDyna();
        
        public AssetType AssetTypeFromString(string type, Platform platform)
        {
            type = type.Trim();
            switch (type)
            {
                case "ALST": return AssetType.AnimationList;
                case "ANIM": return AssetType.Animation;
                case "ATBL": return AssetType.AnimationTable;
                case "ATKT": return AssetType.AttackTable;
                case "BINK": return AssetType.BinkVideo;
                case "BOUL": return AssetType.Boulder;
                case "BSP": return AssetType.BSP;
                case "BUTN": return AssetType.Button;
                case "CAM": return AssetType.Camera;
                case "CCRV": return AssetType.CameraCurve;
                case "CNTR": return AssetType.Counter;
                case "COLL": return AssetType.CollisionTable;
                case "COND": return AssetType.Conditional;
                case "CRDT": return AssetType.Credits;
                case "CSN": return AssetType.Cutscene;
                case "CSNM": return AssetType.CutsceneManager;
                case "CSSS": return AssetType.CutsceneStreamingSound;
                case "CTOC": return AssetType.CutsceneTableOfContents;
                case "DEST": return AssetType.Destructible;
                case "DPAT": return AssetType.Dispatcher;
                case "DSCO": return AssetType.DiscoFloor;
                case "DSTR": return AssetType.DestructibleObject;
                case "DTRK": return AssetType.DashTrack;
                case "DUPC": return AssetType.Duplicator;
                case "DYNA": return GetDynaType(platform);
                case "EGEN": return AssetType.ElectricArcGenerator;
                case "ENV": return AssetType.Environment;
                case "FLY": return AssetType.Flythrough;
                case "FOG": return AssetType.Fog;
                case "GRSM": return AssetType.GrassMesh;
                case "GRUP": return AssetType.Group;
                case "GUST": return AssetType.Gust;
                case "HANG": return AssetType.Hangable;
                case "JAW": return AssetType.JawDataTable;
                case "JSP": return AssetType.JSP;
                case "LITE": return AssetType.Light;
                case "LKIT": return AssetType.LightKit;
                case "LOBM": return AssetType.LobMaster;
                case "LODT": return AssetType.LevelOfDetailTable;
                case "MAPR": return AssetType.SurfaceMapper;
                case "MINF": return AssetType.ModelInfo;
                case "MODL": return AssetType.Model;
                case "MPHT": return AssetType.MorphTarget;
                case "MRKR": return AssetType.Marker;
                case "MVPT": return AssetType.MovePoint;
                case "NGMS": return AssetType.NavigationMesh;
                case "NPC": return AssetType.Villain;
                case "NPCS": return AssetType.NPCSettings;
                case "ONEL": return AssetType.OneLiner;
                case "PARE": return AssetType.ParticleEmitter;
                case "PARP": return AssetType.ParticleProperties;
                case "PARS": return AssetType.ParticleSystem;
                case "PEND": return AssetType.Pendulum;
                case "PGRS": return AssetType.ProgressScript;
                case "PICK": return AssetType.PickupTable;
                case "PIPT": return AssetType.PipeInfoTable;
                case "PKUP": return AssetType.Pickup;
                case "PLAT": return AssetType.Platform;
                case "PLYR": return AssetType.Player;
                case "PORT": return AssetType.Portal;
                case "PRJT": return AssetType.Projectile;
                case "RANM": return AssetType.ReactiveAnimation;
                case "RAW": return AssetType.RawImage;
                case "RWTX": return AssetType.Texture;
                case "SCRP": return AssetType.Script;
                case "SDFX": return AssetType.SDFX;
                case "SFX": return AssetType.SFX;
                case "SGRP": return AssetType.SoundGroup;
                case "SHDW": return AssetType.ShadowTable;
                case "SHRP": return AssetType.Shrapnel;
                case "SIMP": return AssetType.SimpleObject;
                case "SLID": return AssetType.SlideProperty;
                case "SND": return AssetType.Sound;
                case "SNDI": return AssetType.SoundInfo;
                case "SNDS": return AssetType.SoundStream;
                case "SPLN": return AssetType.Spline;
                case "SPLP": return AssetType.SplinePath;
                case "SSET": return AssetType.SceneSettings;
                case "SUBT": return AssetType.Subtitles;
                case "SURF": return AssetType.Surface;
                case "TEXS": return AssetType.TextureStream;
                case "TEXT": return AssetType.Text;
                case "TIMR": return AssetType.Timer;
                case "TPIK": return AssetType.PickupTypes;
                case "TRCK": return AssetType.Track;
                case "TRIG": return AssetType.Trigger;
                case "TRWT": return AssetType.ThrowableTable;
                case "UI": return AssetType.UserInterface;
                case "UIFN": return AssetType.UIFN;
                case "UIFT": return AssetType.UserInterfaceFont;
                case "UIM": return AssetType.UserInterfaceMotion;
                case "VIL": return AssetType.NPC;
                case "VILP": return AssetType.NPCProperties;
                case "VOLU": return AssetType.Volume;
                case "WIRE": return AssetType.WireframeModel;
                case "ZLIN": return AssetType.ZipLine;
            }

            throw new Exception("Unknown asset type: " + type);
        }

        public int GetCompareValue(Game game, Platform platform)
        {
            switch (assetType)
            {
                case AssetType.Texture: return 1;
                case AssetType.TextureStream: return 1;
                case AssetType.BinkVideo: return 2;
                case AssetType.WireframeModel: return 3;
                case AssetType.BSP: return 4;
                case AssetType.JSP: return 5;
                case AssetType.JSPInfo: return 6;
                case AssetType.Model: return 7;
                case AssetType.Player: return 10;
                case AssetType.Villain: return 15;
                case AssetType.NPC: return 20;
                case AssetType.NPCProperties: return 30;
                case AssetType.Duplicator: return 35; // ?
                case AssetType.Pickup: return 40;
                case AssetType.Trigger: return 50;
                case AssetType.Camera:
                    if (ADBG.assetName == "STARTCAM")
                        return 90;
                    return 100;
                case AssetType.CameraCurve: return 101;
                case AssetType.Spline: return 102;
                case AssetType.Environment: return 110;
                case AssetType.Timer: return 120;
                case AssetType.Portal: return 130;
                case AssetType.Text: return 131;
                case AssetType.Subtitles: return 132;
                case AssetType.MovePoint: return 160;
                case AssetType.Marker: return 170;
                case AssetType.Group: return 180;
                case AssetType.RawImage: return 190;
                case AssetType.Counter: return 200;
                case AssetType.Hangable: return 204;
                case AssetType.Pendulum: return 206;
                case AssetType.SFX: return 210;
                case AssetType.SDFX: return 215;
                case AssetType.Platform:
                    switch (TypeFlag(game))
                    {
                        case PlatType.ExtendRetract:
                        case PlatType.Orbit:
                        case PlatType.Spline:
                        case PlatType.MovePoint:
                        case PlatType.FullyManipulable:
                            return 150;

                        case PlatType.ConveyorBelt: return 220;
                        case PlatType.BreakawayPlatform: return 230;
                        case PlatType.Springboard: return 240;
                        case PlatType.TeeterTotter: return 250;
                        case PlatType.Mechanism: return 260;
                        case PlatType.Paddle: return 270;

                        case PlatType.Pendulum:
                        case PlatType.FallingPlatform:
                        case PlatType.FR:
                        default:
                            return 0;
                    }
                case AssetType.Track: return 298;
                case AssetType.SimpleObject: return 299;
                case AssetType.Button: return 300;
                case AssetType.SlideProperty: return 304;
                case AssetType.ZipLine: return 305;
                case AssetType.Surface: return 310;
                case AssetType.DestructibleObject: return 320;
                case AssetType.Gust: return 321;
                case AssetType.Volume: return 322;
                case AssetType.Dispatcher: return 330;
                case AssetType.Conditional: return 340;
                case AssetType.UserInterface: return 350;
                case AssetType.UserInterfaceFont: return 360;
                case AssetType.Projectile: return 361;
                case AssetType.LobMaster: return 362;
                case AssetType.Fog: return 370;
                case AssetType.Light: return 375;
                case AssetType.ParticleProperties: return 380;
                case AssetType.ParticleEmitter: return 390;
                case AssetType.ParticleSystem: return 400;
                case AssetType.CutsceneManager: return 410;
                case AssetType.ElectricArcGenerator: return 420;
                case AssetType.AnimationList: return 430;
                case AssetType.Boulder: return 440;
                case AssetType.LightKit: return 450;
                case AssetType.AttackTable: return 451;
                case AssetType.NPCSettings: return 452;
                case AssetType.OneLiner: return 453;
                case AssetType.UserInterfaceMotion: return 454;
                case AssetType.Script: return 455;
                case AssetType.ProgressScript: return 456;
                case AssetType.SplinePath: return 457;
                case AssetType.Credits: return 460;
                case AssetType.DiscoFloor: return 470;
                case AssetType.AudioConversation: return 480;
                case AssetType.CameraBinaryPoi: return 481;
                case AssetType.CameraPreset: return 482;
                case AssetType.CameraTransitionPath: return 483;
                case AssetType.CameraTransitionTime: return 484;
                case AssetType.AnalogDeflection: return 485;
                case AssetType.AnalogDirection: return 486;
                case AssetType.CarryableObject: return 490;
                case AssetType.CarryablePropertyGeneric: return 491;
                case AssetType.CarryablePropertyAttract: return 492;
                case AssetType.CarryablePropertyRepel: return 493;
                case AssetType.CarryablePropertySwipe: return 494;
                case AssetType.Checkpoint: return 495;
                case AssetType.PoleSwing: return 496;
                case AssetType.Springboard: return 497;
                case AssetType.Tightrope: return 500;
                case AssetType.BossBrain: return 501;
                case AssetType.Flamethrower: return 502;
                case AssetType.Grass: return 503;
                case AssetType.LensFlareElement: return 504;
                case AssetType.LensFlareSource: return 505;
                case AssetType.LightEffect: return 506;
                case AssetType.LightEffectFlicker: return 507;
                case AssetType.LightEffectStrobe: return 508;
                case AssetType.Lightning: return 509;
                case AssetType.ParticleGenerator: return 510;
                case AssetType.Rumble: return 511;
                case AssetType.RumbleBoxEmitter: return 512;
                case AssetType.RumbleSphericalEmitter: return 513;
                case AssetType.ScreenFade: return 514;
                case AssetType.ScreenWarp: return 515;
                case AssetType.SmokeEmitter: return 516;
                case AssetType.SparkEmitter: return 517;
                case AssetType.NPCGate: return 518;
                case AssetType.NPCWalls: return 519;
                case AssetType.EnemySwarmBug: return 520;
                case AssetType.EnemySwarmOwl: return 521;
                case AssetType.EnemyThief: return 522;
                case AssetType.EnemyLeftArm: return 523;
                case AssetType.EnemyRightArm: return 524;
                case AssetType.EnemyWaiter: return 525;
                case AssetType.Splash: return 526;
                case AssetType.Spotlight: return 527;
                case AssetType.UberLaser: return 528;
                case AssetType.WaterHose: return 529;
                case AssetType.WaterBody: return 530;
                case AssetType.Bomber: return 531;
                case AssetType.BossUnderminerDrill: return 532;
                case AssetType.BossUnderminerUM: return 533;
                case AssetType.Chicken: return 534;
                case AssetType.Driller: return 535;
                case AssetType.Enforcer: return 536;
                case AssetType.Humanoid: return 537;
                case AssetType.Rat: return 538;
                case AssetType.RobotTank: return 539;
                case AssetType.Scientist: return 540;
                case AssetType.Shooter: return 541;
                case AssetType.EnemySB: return 542;
                case AssetType.Spawner: return 543;
                case AssetType.CastNCrew: return 544;
                case AssetType.Critter: return 545;
                case AssetType.Dennis: return 546;
                case AssetType.FrogFish: return 547;
                case AssetType.Mindy: return 548;
                case AssetType.Neptune: return 549;
                case AssetType.Enemy: return 550;
                case AssetType.Crate: return 551;
                case AssetType.Turret: return 552;
                case AssetType.BoulderGenerator: return 553;
                case AssetType.BulletMark: return 554;
                case AssetType.BulletTime: return 555;
                case AssetType.BungeeDrop: return 556;
                case AssetType.BungeeHook: return 557;
                case AssetType.BusStop: return 558;
                case AssetType.CameraParamAsset: return 559;
                case AssetType.CameraTweak: return 560;
                case AssetType.DashCameraSpline: return 561;
                case AssetType.FlameEmitter: return 562;
                case AssetType.FlythroughObject: return 563;
                case AssetType.FreezableObject: return 564;
                case AssetType.Grapple: return 565;
                case AssetType.HangableObject: return 566;
                case AssetType.IncrediblesPickup: return 567;
                case AssetType.LaserBeam: return 568;
                case AssetType.NPCSettingsObject: return 570;
                case AssetType.RaceTimer: return 571;
                case AssetType.RbandCameraAsset: return 572;
                case AssetType.Ring: return 573;
                case AssetType.RingControl: return 574;
                case AssetType.RubbleGenerator: return 575;
                case AssetType.TalkBox: return 576;
                case AssetType.TaskBox: return 577;
                case AssetType.Taxi: return 578;
                case AssetType.TeleportBox: return 579;
                case AssetType.TextBox: return 580;
                case AssetType.TrainCar: return 581;
                case AssetType.TrainJunction: return 582;
                case AssetType.TurretObject: return 583;
                case AssetType.Vent: return 584;
                case AssetType.VentType: return 585;
                case AssetType.HUDImage: return 586;
                case AssetType.HUDMeterFont: return 587;
                case AssetType.HUDMeterUnit: return 588;
                case AssetType.HUDModel: return 589;
                case AssetType.HUDText: return 590;
                case AssetType.HUDCompassObject: return 591;
                case AssetType.HUDCompassSystem: return 592;
               
                case AssetType.IncrediblesIcon: return 600;
                case AssetType.InteractionIceBridge: return 601;
                case AssetType.InteractionLaunch: return 602;
                case AssetType.InteractionLift: return 603;
                case AssetType.InteractionSwitchLever: return 604;
                case AssetType.InteractionTurn: return 605;
                case AssetType.InterestPointer: return 606;
                case AssetType.JSPExtraData: return 607;
                case AssetType.LogicFunctionGenerator: return 608;
                case AssetType.LogicMission: return 609;
                case AssetType.LogicReference: return 610;
                case AssetType.LogicTask: return 611;
                case AssetType.NPCCoverPoint: return 612;
                case AssetType.NPCGroup: return 613;
                case AssetType.NPCCustomAV: return 614;
                case AssetType.Pointer: return 615;
                case AssetType.PourWidget: return 616;
                case AssetType.SceneProperties: return 617;
                case AssetType.Twiddler: return 618;
                case AssetType.UserInterfaceBox: return 619;
                case AssetType.UserInterfaceController: return 620;
                case AssetType.UserInterfaceImage: return 621;
                case AssetType.UserInterfaceModel: return 621;
                case AssetType.UserInterfaceText: return 622;
                case AssetType.UserInterfaceTextUserString: return 623;
                case AssetType.Unknown_EBC04E7B: return 624;
                case AssetType.PickupTypes: return 642;
                case AssetType.ThrowableTable: return 643;
                case AssetType.ReactiveAnimation: return 644;
                case AssetType.SceneSettings: return 645;
                case AssetType.Flythrough: return 650;
                case AssetType.NavigationMesh: return 653;
                case AssetType.GrassMesh: return 654;
                case AssetType.MorphTarget: return 655;
                case AssetType.Animation: return 656;
                case AssetType.AnimationTable: return 660;
                case AssetType.Shrapnel: return 670;
                case AssetType.PickupTable: return 680;
                case AssetType.ModelInfo: return 690;
                case AssetType.SoundGroup: return 695;
                case AssetType.Destructible: return 699;
                case AssetType.LevelOfDetailTable: return 700;
                case AssetType.CollisionTable: return 710;
                case AssetType.ShadowTable: return 720;
                case AssetType.PipeInfoTable: return 730;
                case AssetType.JawDataTable: return 740;
                case AssetType.SurfaceMapper: return 750;
                case AssetType.Cutscene: return 755;
                case AssetType.Sound: return 760;
                case AssetType.SoundStream: return 770;
                case AssetType.CutsceneStreamingSound: return 780;
                case AssetType.SoundInfo: return 790;

                case AssetType.CutsceneTableOfContents:
                case AssetType.DashTrack:
                case AssetType.UIFN:
                    return 0;
            }
            return 0;
        }

        private PlatType TypeFlag(Game game)
        {
            if (assetType == AssetType.Platform)
            {
                if (game == Game.BFBB)
                    return (PlatType)data[0x54];
                return (PlatType)data[0x50];
            }
            return 0;
        }

        private AssetType GetDynaType(Platform platform)
        {
            if (platform == Platform.GameCube)
                return ((DynaType)BitConverter.ToUInt32(new byte[] { data[0xB], data[0xA], data[0x9], data[0x8] }, 0)).ToAssetType();
            return ((DynaType)BitConverter.ToUInt32(data, 8)).ToAssetType();
        }
    }
}
