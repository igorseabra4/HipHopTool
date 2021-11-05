using System;
using System.Collections.Generic;
using System.IO;
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
            this.assetType = AssetTypeFromString(assetType);
            this.flags = flags;
            this.ADBG = ADBG;
        }

        public Section_AHDR(uint assetID, AssetType assetType, AHDRFlags flags, Section_ADBG ADBG) : base(Section.AHDR)
        {
            this.assetID = assetID;
            this.assetType = assetType;
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

        public Section_AHDR(BinaryReader binaryReader) : base(binaryReader, Section.AHDR)
        {
            assetID = Switch(binaryReader.ReadUInt32());
            assetType = AssetTypeFromString(new string(binaryReader.ReadChars(4)));
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
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.AHDR;

            listBytes.AddBigEndian(assetID);
            foreach (char i in assetType.ToString().PadRight(4))
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

        public int GetCompareValue(Game game, Platform platform)
        {
            switch (assetType)
            {
                case AssetType.RWTX: return 1;
                case AssetType.BSP:  return 2;
                case AssetType.JSP:  return 3;
                case AssetType.MODL: return 4;
                case AssetType.PLYR: return 10;
                case AssetType.NPC:  return 15;
                case AssetType.VIL:  return 20;
                case AssetType.VILP: return 30;
                case AssetType.DUPC: return 35; // ?
                case AssetType.PKUP: return 40;
                case AssetType.TRIG: return 50;
                case AssetType.CAM:
                    if (ADBG.assetName == "STARTCAM")
                        return 90;
                    return 100;
                case AssetType.ENV:  return 110;
                case AssetType.TIMR: return 120;
                case AssetType.PORT: return 130;
                case AssetType.TEXT: return 131;
                case AssetType.SUBT: return 132;
                case AssetType.MVPT: return 160;
                case AssetType.MRKR: return 170;
                case AssetType.GRUP: return 180;
                case AssetType.RAW:  return 190;
                case AssetType.CNTR: return 200;
                case AssetType.HANG: return 204;
                case AssetType.PEND: return 206;
                case AssetType.SFX:  return 210;
                case AssetType.SDFX: return 215;
                case AssetType.PLAT:
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
                case AssetType.TRCK: return 298;
                case AssetType.SIMP: return 299;
                case AssetType.BUTN: return 300;
                case AssetType.SLID: return 304;
                case AssetType.ZLIN: return 305;
                case AssetType.SURF: return 310;
                case AssetType.DSTR: return 320;
                case AssetType.GUST: return 321;
                case AssetType.VOLU: return 322;
                case AssetType.DPAT: return 330;
                case AssetType.COND: return 340;
                case AssetType.UI:   return 350;
                case AssetType.UIFT: return 360;
                case AssetType.PRJT: return 361;
                case AssetType.LOBM: return 362;
                case AssetType.FOG:  return 370;
                case AssetType.LITE: return 375;
                case AssetType.PARP: return 380;
                case AssetType.PARE: return 390;
                case AssetType.PARS: return 400;
                case AssetType.CSNM: return 410;
                case AssetType.EGEN: return 420;
                case AssetType.ALST: return 430;
                case AssetType.BOUL: return 440;
                case AssetType.LKIT: return 450;
                case AssetType.ATKT: return 451;
                case AssetType.NPCS: return 452;
                case AssetType.ONEL: return 453;
                case AssetType.SCRP: return 454;
                case AssetType.PGRS: return 455;
                case AssetType.SPLP: return 456;
                case AssetType.CRDT: return 460;
                case AssetType.DSCO: return 470;
                case AssetType.UIM:  return 471;
                case AssetType.DYNA:
                    switch (GetDynaType(platform))
                    {
                        case DynaType.audio__conversation: return 480;
                        case DynaType.camera__binary_poi: return 481;
                        case DynaType.camera__preset: return 482;
                        case DynaType.camera__transition_path: return 483;
                        case DynaType.camera__transition_time: return 484;
                        case DynaType.Checkpoint: return 485;
                        case DynaType.effect__BossBrain: return 486;
                        case DynaType.effect__Flamethrower: return 487;
                        case DynaType.effect__grass: return 488;
                        case DynaType.Unknown_2CD29541: return 489;
                        case DynaType.Unknown_A072A4DA: return 490;
                        case DynaType.effect__light: return 491;
                        case DynaType.effect__LightEffectFlicker: return 492;
                        case DynaType.effect__LightEffectStrobe: return 493;
                        case DynaType.effect__Lightning: return 494;
                        case DynaType.Effect__particle_generator: return 495;
                        case DynaType.effect__Rumble: return 496;
                        case DynaType.effect__RumbleBoxEmitter: return 500;
                        case DynaType.effect__RumbleSphericalEmitter: return 501;
                        case DynaType.effect__ScreenFade: return 502;
                        case DynaType.effect__ScreenWarp: return 503;
                        case DynaType.effect__smoke_emitter: return 504;
                        case DynaType.effect__spark_emitter: return 505;
                        case DynaType.effect__Splash: return 506;
                        case DynaType.effect__spotlight: return 507;
                        case DynaType.effect__uber_laser: return 508;
                        case DynaType.effect__Waterhose: return 509;
                        case DynaType.effect__water_body: return 510;
                        case DynaType.Enemy__SB: return 511;
                        case DynaType.Enemy__SB__BucketOTron: return 512;
                        case DynaType.Enemy__SB__CastNCrew: return 513;
                        case DynaType.Enemy__SB__Critter: return 514;
                        case DynaType.Enemy__SB__Dennis: return 515;
                        case DynaType.Enemy__SB__FrogFish: return 516;
                        case DynaType.Enemy__SB__Mindy: return 517;
                        case DynaType.Enemy__SB__Neptune: return 518;
                        case DynaType.Enemy__SB__Standard: return 519;
                        case DynaType.Enemy__SB__SupplyCrate: return 520;
                        case DynaType.Enemy__SB__Turret: return 521;
                        case DynaType.game_object__BoulderGenerator: return 522;
                        case DynaType.game_object__bullet_mark: return 523;
                        case DynaType.game_object__bullet_time: return 524;
                        case DynaType.game_object__bungee_drop: return 525;
                        case DynaType.game_object__bungee_hook: return 526;
                        case DynaType.game_object__BusStop: return 527;
                        case DynaType.game_object__camera_param_asset: return 528;
                        case DynaType.game_object__Camera_Tweak: return 529;
                        case DynaType.game_object__dash_camera_spline: return 530;
                        case DynaType.game_object__flame_emitter: return 531;
                        case DynaType.game_object__Flythrough: return 532;
                        case DynaType.game_object__FreezableObject: return 533;
                        case DynaType.game_object__Grapple: return 534;
                        case DynaType.game_object__Hangable: return 535;
                        case DynaType.game_object__IN_Pickup: return 536;
                        case DynaType.game_object__laser_beam: return 537;
                        case DynaType.game_object__NPCSettings: return 538;
                        case DynaType.game_object__RaceTimer: return 539;
                        case DynaType.game_object__rband_camera_asset: return 540;
                        case DynaType.game_object__Ring: return 541;
                        case DynaType.game_object__RingControl: return 542;
                        case DynaType.game_object__RubbleGenerator: return 543;
                        case DynaType.game_object__talk_box: return 544;
                        case DynaType.game_object__task_box: return 545;
                        case DynaType.game_object__Taxi: return 546;
                        case DynaType.game_object__Teleport: return 547;
                        case DynaType.game_object__text_box: return 548;
                        case DynaType.game_object__train_car: return 549;
                        case DynaType.game_object__train_junction: return 550;
                        case DynaType.game_object__Turret: return 551;
                        case DynaType.game_object__Vent: return 552;
                        case DynaType.game_object__VentType: return 553;
                        case DynaType.hud__image: return 554;
                        case DynaType.hud__meter__font: return 555;
                        case DynaType.hud__meter__unit: return 556;
                        case DynaType.hud__model: return 557;
                        case DynaType.hud__text: return 558;
                        case DynaType.Incredibles__Icon: return 559;
                        case DynaType.interaction__IceBridge: return 560;
                        case DynaType.interaction__Launch: return 561;
                        case DynaType.interaction__Lift: return 562;
                        case DynaType.interaction__SwitchLever: return 563;
                        case DynaType.interaction__Turn: return 564;
                        case DynaType.Interest_Pointer: return 565;
                        case DynaType.JSPExtraData: return 566;
                        case DynaType.logic__FunctionGenerator: return 567;
                        case DynaType.logic__reference: return 568;
                        case DynaType.npc__CoverPoint: return 569;
                        case DynaType.npc__group: return 570;
                        case DynaType.npc__NPC_Custom_AV: return 571;
                        case DynaType.pointer: return 572;
                        case DynaType.SceneProperties: return 573;
                        case DynaType.ui__box: return 574;
                        case DynaType.ui__controller: return 575;
                        case DynaType.ui__image: return 576;
                        case DynaType.ui__model: return 577;
                        case DynaType.ui__text: return 578;
                        case DynaType.ui__text__userstring: return 579;
                        case DynaType.Unknown_4EE03B24: return 581;
                        case DynaType.Unknown_9F234F8E: return 582;
                        case DynaType.Unknown_460F4FB2: return 583;
                        case DynaType.Unknown_2743B85C: return 584;
                        case DynaType.Unknown_AD7CB421: return 586;
                        case DynaType.Unknown_C6C76EEE: return 587;
                        case DynaType.Unknown_CDB57387: return 588;
                        case DynaType.Unknown_CF21DB89: return 589;
                        case DynaType.Unknown_E5D82D97: return 590;
                        case DynaType.Unknown_E2301EA9: return 591;
                        case DynaType.Unknown_EBC04E7B: return 592;
                        case DynaType.Unknown_FC2951C1: return 593;

                        default: return 0;
                    }
                case AssetType.TPIK: return 642;
                case AssetType.TRWT: return 643;
                case AssetType.RANM: return 644;
                case AssetType.SSET: return 645;
                case AssetType.FLY:  return 650;
                case AssetType.NGMS: return 653;
                case AssetType.GRSM: return 654;
                case AssetType.MPHT: return 655;
                case AssetType.ANIM: return 656;
                case AssetType.ATBL: return 660;
                case AssetType.SHRP: return 670;
                case AssetType.PICK: return 680;
                case AssetType.MINF: return 690;
                case AssetType.SGRP: return 695;
                case AssetType.DEST: return 699;
                case AssetType.LODT: return 700;
                case AssetType.COLL: return 710;
                case AssetType.SHDW: return 720;
                case AssetType.PIPT: return 730;
                case AssetType.JAW:  return 740;
                case AssetType.MAPR: return 750;
                case AssetType.CSN:  return 755;
                case AssetType.SND:  return 760;
                case AssetType.SNDS: return 770;
                case AssetType.CSSS: return 780;
                case AssetType.SNDI: return 790;

                case AssetType.BINK:
                case AssetType.CCRV:
                case AssetType.CTOC:
                case AssetType.DTRK:
                case AssetType.SPLN:
                case AssetType.TEXS:
                case AssetType.UIFN:
                case AssetType.WIRE:
                    return 0;
            }
            return 0;
        }

        private PlatType TypeFlag(Game game)
        {
            if (assetType == AssetType.PLAT)
            {
                if (game == Game.BFBB)
                    return (PlatType)data[0x54];
                return (PlatType)data[0x50];
            }
            return 0;
        }

        private DynaType GetDynaType(Platform platform)
        {
            if (assetType == AssetType.DYNA)
            {
                if (platform == Platform.GameCube)
                    return (DynaType)BitConverter.ToUInt32(new byte[] { data[0xB], data[0xA], data[0x9], data[0x8] }, 0);
                return (DynaType)BitConverter.ToUInt32(data, 8);
            }
            return 0;
        }
    }
}
