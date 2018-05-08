using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_STRM : HipSection
    {
        public Section_DHDR DHDR;
        public Section_DPAK DPAK;

        public Section_STRM()
        {
            sectionName = Section.STRM;
        }

        public Section_STRM(BinaryReader binaryReader)
        {
            sectionName = Section.STRM;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DHDR.ToString()) throw new Exception();
            DHDR = new Section_DHDR(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DPAK.ToString()) throw new Exception();
            DPAK = new Section_DPAK(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.STRM;
            DHDR.SetBytes(ref listBytes);
            DPAK.SetBytes(ref listBytes);
        }
    }
}
