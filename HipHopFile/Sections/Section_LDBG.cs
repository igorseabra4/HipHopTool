﻿using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LDBG : HipSection
    {
        public int value;

        public Section_LDBG(int value) : base(Section.LDBG)
        {
            this.value = value;
        }

        public Section_LDBG(BinaryReader binaryReader) : base(binaryReader, Section.LDBG)
        {
            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.LDBG;

            listBytes.AddBigEndian(value);
        }
    }
}