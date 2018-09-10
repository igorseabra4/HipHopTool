using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public abstract class HipSection
    {
        public Section sectionName;
        public int sectionSize;

        public HipSection() { }

        public HipSection(Section sectionName)
        {
            this.sectionName = sectionName;
        }

        public HipSection(BinaryReader binaryReader, Section sectionName)
        {
            this.sectionName = sectionName;
            sectionSize = Switch(binaryReader.ReadInt32());
        }

        public void SetBytes(ref List<byte> listBytes)
        {
            int position = listBytes.Count();
            listBytes.AddRange(new byte[] {
                0, 0, 0, 0,
                0, 0, 0, 0,
            });

            SetListBytes(ref listBytes);

            sectionSize = listBytes.Count() - position - 0x8;

            listBytes[position + 0] = (byte)sectionName.ToString()[0];
            listBytes[position + 1] = (byte)sectionName.ToString()[1];
            listBytes[position + 2] = (byte)sectionName.ToString()[2];
            listBytes[position + 3] = (byte)sectionName.ToString()[3];
            listBytes[position + 4] = BitConverter.GetBytes(sectionSize)[3];
            listBytes[position + 5] = BitConverter.GetBytes(sectionSize)[2];
            listBytes[position + 6] = BitConverter.GetBytes(sectionSize)[1];
            listBytes[position + 7] = BitConverter.GetBytes(sectionSize)[0];
        }

        public abstract void SetListBytes(ref List<byte> listBytes);
    }
}
