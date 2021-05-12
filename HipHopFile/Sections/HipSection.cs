using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public abstract class HipSection
    {
        public Section sectionType;
        public int sectionSize;

        public HipSection() { }

        public HipSection(Section sectionName)
        {
            this.sectionType = sectionName;
        }

        public HipSection(BinaryReader binaryReader, Section sectionType)
        {
            this.sectionType = sectionType;
            sectionSize = Switch(binaryReader.ReadInt32());
        }

        public void SetBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            int position = listBytes.Count();
            listBytes.AddRange(new byte[] {
                0, 0, 0, 0,
                0, 0, 0, 0,
            });

            SetListBytes(game, platform, ref listBytes);

            sectionSize = listBytes.Count() - position - 0x8;

            var sectionNameString = sectionType.ToString();
            var sectionSizeBytes = BitConverter.GetBytes(sectionSize);

            listBytes[position + 0] = (byte)sectionNameString[0];
            listBytes[position + 1] = (byte)sectionNameString[1];
            listBytes[position + 2] = (byte)sectionNameString[2];
            listBytes[position + 3] = (byte)sectionNameString[3];
            listBytes[position + 4] = sectionSizeBytes[3];
            listBytes[position + 5] = sectionSizeBytes[2];
            listBytes[position + 6] = sectionSizeBytes[1];
            listBytes[position + 7] = sectionSizeBytes[0];
        }

        public abstract void SetListBytes(Game game, Platform platform, ref List<byte> listBytes);
    }
}
