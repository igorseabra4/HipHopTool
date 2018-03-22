using System;
using System.Collections.Generic;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PACK : HipSection
    {
        Section_PVER PVER;
        Section_PFLG PFLG;
        Section_PCNT PCNT;
        Section_PCRT PCRT;
        Section_PMOD PMOD;
        Section_PLAT PLAT;

        public Section_PACK Read(BinaryReader binaryReader)
        {
            sectionName = Section.PACK;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PVER.ToString()) throw new Exception();
            PVER = new Section_PVER().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PFLG.ToString()) throw new Exception();
            PFLG = new Section_PFLG().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCNT.ToString()) throw new Exception();
            PCNT = new Section_PCNT().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PCRT.ToString()) throw new Exception();
            PCRT = new Section_PCRT().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PMOD.ToString()) throw new Exception();
            PMOD = new Section_PMOD().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.PLAT.ToString()) throw new Exception();
            PLAT = new Section_PLAT().Read(binaryReader);
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PACK;

            listBytes.AddRange(PVER.GetBytes());
            listBytes.AddRange(PFLG.GetBytes());
            listBytes.AddRange(PCNT.GetBytes());
            listBytes.AddRange(PCRT.GetBytes());
            listBytes.AddRange(PMOD.GetBytes());
            listBytes.AddRange(PLAT.GetBytes());
        }
    }
}
