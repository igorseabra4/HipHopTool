using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public enum LayerType
    {
        DEFAULT = 0, // Spongebob uses only this on HIP but the others on HOP
        TEXTURE = 1,
        BSP = 2,
        MODEL = 3,
        ANIMATION = 4,
        SRAM = 6,
        SNDTOC = 7,
        CUTSCENE = 8,
        JSPINFO = 10,
    }

    public class Section_LHDR : HipSection
    {
        public LayerType layerType;
        public int amountOfUnknown;
        public int[] unknowns;
        public Section_LDBG LDBG;
        
        public Section_LHDR Read(BinaryReader binaryReader)
        {
            sectionName = Section.LHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            layerType = (LayerType)Switch(binaryReader.ReadInt32());
            amountOfUnknown = Switch(binaryReader.ReadInt32());

            unknowns = new int[amountOfUnknown];
            for (int i = 0; i < amountOfUnknown; i++)
                unknowns[i] = Switch(binaryReader.ReadInt32());
            
            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LDBG.ToString()) throw new Exception();
            LDBG = new Section_LDBG().Read(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LHDR;

            listBytes.AddRange(BitConverter.GetBytes((int)layerType).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(amountOfUnknown).Reverse());

            foreach (int i in unknowns)
                listBytes.AddRange(BitConverter.GetBytes(i).Reverse());

            listBytes.AddRange(LDBG.GetBytes());
        }
    }
}
