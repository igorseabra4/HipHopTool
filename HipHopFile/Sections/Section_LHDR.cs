using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public enum LayerType
    {
        DEFAULT = 0,
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
        public List<uint> assetIDlist;
        public Section_LDBG LDBG;

        public Section_LHDR()
        {
            sectionName = Section.LHDR;
        }

        public Section_LHDR(BinaryReader binaryReader)
        {
            sectionName = Section.LHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            layerType = (LayerType)Switch(binaryReader.ReadInt32());
            int assetAmount = Switch(binaryReader.ReadInt32());

            assetIDlist = new List<uint>(assetAmount);
            for (int i = 0; i < assetAmount; i++)
                assetIDlist.Add(Switch(binaryReader.ReadUInt32()));
            
            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LDBG.ToString()) throw new Exception();
            LDBG = new Section_LDBG(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LHDR;

            listBytes.AddRange(BitConverter.GetBytes((int)layerType).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(assetIDlist.Count()).Reverse());

            foreach (int i in assetIDlist)
                listBytes.AddRange(BitConverter.GetBytes(i).Reverse());

            LDBG.SetBytes(ref listBytes);
        }
    }
}
