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

        public Section_PACK()
        {
            sectionName = Section.PACK;
        }

        public Section_PACK(BinaryReader binaryReader)
        {
            sectionName = Section.PACK;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PVER.ToString()) throw new Exception();
            PVER = new Section_PVER(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PFLG.ToString()) throw new Exception();
            PFLG = new Section_PFLG(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCNT.ToString()) throw new Exception();
            PCNT = new Section_PCNT(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCRT.ToString()) throw new Exception();
            PCRT = new Section_PCRT(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PMOD.ToString()) throw new Exception();
            PMOD = new Section_PMOD(binaryReader);

            if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.PLAT.ToString()) throw new Exception();
                PLAT = new Section_PLAT(binaryReader);
            }
            else if (currentGame == Game.Scooby)
            {
                PLAT = null;
            }
            else throw new Exception("PACK reading error");
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PACK;
            
            PVER.SetBytes(ref listBytes);
            PFLG.SetBytes(ref listBytes);
            PCNT.SetBytes(ref listBytes);
            PCRT.SetBytes(ref listBytes);
            PMOD.SetBytes(ref listBytes);
            if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                PLAT.SetBytes(ref listBytes);
        }
    }
}
