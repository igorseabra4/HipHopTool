using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public byte[] containedFile;
        
        public Section_AHDR(uint assetID, string assetType, AHDRFlags flags, Section_ADBG ADBG)
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

        public Section_AHDR(uint assetID, AssetType assetType, AHDRFlags flags, Section_ADBG ADBG)
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
