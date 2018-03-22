using System;
using System.Collections.Generic;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_STRM : HipSection
    {
        public Section_DHDR DHDR;
        public Section_DPAK DPAK;

        public Section_STRM Read(BinaryReader binaryReader)
        {
            sectionName = Section.STRM;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DHDR.ToString()) throw new Exception();
            DHDR = new Section_DHDR().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DPAK.ToString()) throw new Exception();
            DPAK = new Section_DPAK().Read(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.STRM;

            listBytes.AddRange(DHDR.GetBytes());
            listBytes.AddRange(DPAK.GetBytes());
        }
    }
}
