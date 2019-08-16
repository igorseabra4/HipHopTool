using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PACK : HipSection
    {
        public Section_PVER PVER;
        public Section_PFLG PFLG;
        public Section_PCNT PCNT;
        public Section_PCRT PCRT;
        public Section_PMOD PMOD;
        public Section_PLAT PLAT;

        public Section_PACK() : base(Section.PACK) { }

        public Section_PACK(BinaryReader binaryReader, out Game game, out Platform platform) : base(binaryReader, Section.PACK)
        {            
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PVER.ToString()) throw new Exception();
            PVER = new Section_PVER(binaryReader, out game);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PFLG.ToString()) throw new Exception();
            PFLG = new Section_PFLG(binaryReader, game, out game);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCNT.ToString()) throw new Exception();
            PCNT = new Section_PCNT(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCRT.ToString()) throw new Exception();
            PCRT = new Section_PCRT(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PMOD.ToString()) throw new Exception();
            PMOD = new Section_PMOD(binaryReader);

            if (binaryReader.BaseStream.Position == startSectionPosition + sectionSize)
            {
                platform = Platform.Unknown;
                return;
            }

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PLAT.ToString()) throw new Exception();
            PLAT = new Section_PLAT(binaryReader, game, out platform);
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionName = Section.PACK;
            
            PVER.SetBytes(game, platform, ref listBytes);
            PFLG.SetBytes(game, platform, ref listBytes);
            PCNT.SetBytes(game, platform, ref listBytes);
            PCRT.SetBytes(game, platform, ref listBytes);
            PMOD.SetBytes(game, platform, ref listBytes);
            if (PLAT != null)
                PLAT.SetBytes(game, platform, ref listBytes);
        }
    }
}
