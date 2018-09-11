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
        
        public Section_AHDR(uint assetID, string assetType, AHDRFlags flags, Section_ADBG ADBG) : base(Section.AHDR)
        {
            this.assetID = assetID;
            this.assetType = AssetType.Null;

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString() == assetType.Trim())
                {
                    this.assetType = o;
                    break;
                }
            }
            if (this.assetType == AssetType.Null) throw new Exception("Unknown asset type: " + assetType);

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


            long savePosition = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = fileOffset;
            data = binaryReader.ReadBytes(fileSize);
            binaryReader.BaseStream.Position = savePosition;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AHDR;

            listBytes.AddBigEndian(assetID);
            foreach (char i in assetType.ToString().PadRight(4))
                listBytes.Add((byte)i);
            listBytes.AddBigEndian(fileOffset);
            listBytes.AddBigEndian(fileSize);
            listBytes.AddBigEndian(plusValue);
            listBytes.AddBigEndian((int)flags);

            ADBG.SetBytes(ref listBytes);
        }

        public override int GetHashCode()
        {
            return (int)assetID;
        }
    }
}
