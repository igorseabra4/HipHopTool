using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PFLG : HipSection
    {
        public int flags;

        public Section_PFLG(int flags) : base(Section.PFLG)
        {
            sectionName = Section.PFLG;
            this.flags = flags;
        }

        public Section_PFLG(BinaryReader binaryReader) : base(binaryReader, Section.PFLG)
        {
            flags = Switch(binaryReader.ReadInt32());
            
            if (flags != 0x2E & currentGame == Game.Incredibles)
                currentGame = Game.BFBB;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PFLG;
            listBytes.AddBigEndian(flags);
        }
    }
}
