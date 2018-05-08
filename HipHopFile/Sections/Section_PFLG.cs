using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PFLG : HipSection
    {
        public int flags;

        public Section_PFLG(int flag)
        {
            sectionName = Section.PFLG;
            flags = flag;
        }

        public Section_PFLG(BinaryReader binaryReader)
        {
            sectionName = Section.PFLG;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            flags = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            if (flags != 0x2E & currentGame == Game.Incredibles)
            {
                currentGame = Game.BFBB;
            }
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PFLG;
            listBytes.AddRange(BitConverter.GetBytes(flags).Reverse());
        }
    }
}
