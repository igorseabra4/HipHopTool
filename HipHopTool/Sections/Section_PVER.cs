using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PVER : HipSection
    {
        public int subVersion;
        public int clientVersion;
        public int compatible;

        public Section_PVER Read(BinaryReader binaryReader)
        {
            sectionName = Section.PVER;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            subVersion = Switch(binaryReader.ReadInt32());
            clientVersion = Switch(binaryReader.ReadInt32());
            compatible = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PVER;

            listBytes.AddRange(BitConverter.GetBytes(subVersion).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(clientVersion).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(compatible).Reverse());
        }
    }
}
