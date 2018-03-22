using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
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
        public string fileType;
        public int fileOffset;
        public int fileSize;
        public int plusValue;
        public int flags;
        public Section_ADBG ADBG;

        public byte[] containedFile;

        public Section_AHDR Read(BinaryReader binaryReader)
        {
            sectionName = Section.AHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            assetID = Switch(binaryReader.ReadInt32());
            fileType = new string(binaryReader.ReadChars(4));
            fileOffset = Switch(binaryReader.ReadInt32());
            fileSize = Switch(binaryReader.ReadInt32());
            plusValue = Switch(binaryReader.ReadInt32());
            flags = Switch(binaryReader.ReadInt32());

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ADBG.ToString()) throw new Exception();
            ADBG = new Section_ADBG().Read(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            containedFile = ReadContainedFile(fileOffset, fileSize);

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AHDR;

            listBytes.AddRange(BitConverter.GetBytes(assetID).Reverse());
            foreach (char i in fileType)
                listBytes.Add((byte)i);
            listBytes.AddRange(BitConverter.GetBytes(fileOffset).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(fileSize).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(plusValue).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(flags).Reverse());

            listBytes.AddRange(ADBG.GetBytes());
        }
    }
}
