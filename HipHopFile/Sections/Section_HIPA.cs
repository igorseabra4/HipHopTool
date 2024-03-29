﻿using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_HIPA : HipSection
    {
        public Section_HIPA() : base(Section.HIPA) { }

        public Section_HIPA(BinaryReader binaryReader) : base(binaryReader, Section.HIPA) { }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.HIPA;
        }
    }
}
