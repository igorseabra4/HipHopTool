using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public enum AssetType
    {
        Null,
        ALST,
        ANIM,
        ATBL,
        BOUL,
        BSP,
        BUTN,
        CAM,
        CNTR,
        COLL,
        COND,
        CRDT,
        CSN,
        CSNM,
        CSSS,
        CTOC,
        DEST,
        DPAT,
        DSTR,
        DYNA,
        EGEN,
        ENV,
        FLY,
        FOG,
        GRUP,
        JAW,
        JSP,
        LKIT,
        LOBM,
        LODT,
        MAPR,
        MINF,
        MODL,
        MRKR,
        MVPT,
        NPC,
        PARE,
        PARP,
        PARS,
        PICK,
        PIPT,
        PKUP,
        PLAT,
        PLYR,
        PORT,
        PRJT,
        RANM,
        RAW,
        RWTX,
        SCRP,
        SDFX,
        SFX,
        SGRP,
        SHDW,
        SHRP,
        SIMP,
        SND,
        SNDI,
        SNDS,
        SURF,
        TEXT,
        TIMR,
        TRIG,
        UI,
        UIFN,
        UIFT,
        VIL,
        VILP
    }

    public enum AHDRFlags
    {
        SOURCE_FILE = 1,
        SOURCE_VIRTUAL = 2,
        READ_TRANSFORM = 4,
        WRITE_TRANSFORM = 8
    }

    public class Section_AHDR : HipSection
    {
        public int assetID;
        public AssetType assetType;
        public int fileOffset;
        public int fileSize;
        public int plusValue;
        public AHDRFlags flags;
        public Section_ADBG ADBG;

        public byte[] containedFile;
        
        public Section_AHDR(int assetID, string assetType, AHDRFlags flags, Section_ADBG ADBG)
        {
            this.assetID = assetID;

            this.assetType = AssetType.Null;
            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString().PadRight(4) == assetType)
                {
                    this.assetType = o;
                    break;
                }
            }
            if (this.assetType == AssetType.Null) throw new Exception("Unknown asset type");

            this.flags = flags;
            this.ADBG = ADBG;
        }

        public Section_AHDR(int assetID, AssetType assetType, AHDRFlags flags, Section_ADBG ADBG)
        {
            this.assetID = assetID;
            this.assetType = assetType;
            this.flags = flags;
            this.ADBG = ADBG;
        }

        public Section_AHDR(BinaryReader binaryReader)
        {
            sectionName = Section.AHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            assetID = Switch(binaryReader.ReadInt32());
            string type = new string(binaryReader.ReadChars(4));

            assetType = AssetType.Null;
            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString().PadRight(4) == type)
                {
                    assetType = o;
                    break;
                }
            }
            if (assetType == AssetType.Null) throw new Exception("Unknown asset type: " + type);

            fileOffset = Switch(binaryReader.ReadInt32());
            fileSize = Switch(binaryReader.ReadInt32());
            plusValue = Switch(binaryReader.ReadInt32());
            flags = (AHDRFlags)Switch(binaryReader.ReadInt32());

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ADBG.ToString()) throw new Exception();
            ADBG = new Section_ADBG(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            containedFile = ReadContainedFile(binaryReader, fileOffset, fileSize);
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AHDR;

            listBytes.AddRange(BitConverter.GetBytes(assetID).Reverse());
            foreach (char i in assetType.ToString().PadRight(4))
                listBytes.Add((byte)i);
            listBytes.AddRange(BitConverter.GetBytes(fileOffset).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(fileSize).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(plusValue).Reverse());
            listBytes.AddRange(BitConverter.GetBytes((int)flags).Reverse());

            ADBG.SetBytes(ref listBytes);
        }
    }
}
