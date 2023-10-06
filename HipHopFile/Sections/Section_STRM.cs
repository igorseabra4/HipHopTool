using System;
using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_STRM : HipSection
    {
        public Section_DHDR DHDR;
        public Section_DPAK DPAK;

        public Section_STRM() : base(Section.STRM)
        {
            DHDR = new Section_DHDR(-1);
            DPAK = new Section_DPAK();
        }

        public Section_STRM(BinaryReader binaryReader) : base(binaryReader, Section.STRM)
        {
            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DHDR.ToString())
                throw new Exception();
            DHDR = new Section_DHDR(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.DPAK.ToString())
                throw new Exception();
            DPAK = new Section_DPAK(binaryReader);
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.STRM;
            DHDR.SetBytes(game, platform, ref listBytes);
            DPAK.SetBytes(game, platform, ref listBytes);
        }
    }
}
