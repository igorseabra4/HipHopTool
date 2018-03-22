using System;
using System.Collections.Generic;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_LTOC : HipSection
    {
        public Section_LINF LINF;
        public List<Section_LHDR> LHDRList;

        public Section_LTOC Read(BinaryReader binaryReader)
        {
            sectionName = Section.LTOC;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LINF.ToString()) throw new Exception();
            LINF = new Section_LINF().Read(binaryReader);

            LHDRList = new List<Section_LHDR>();

            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.LHDR.ToString()) throw new Exception();
                LHDRList.Add(new Section_LHDR().Read(binaryReader));
            }

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LTOC;

            listBytes.AddRange(LINF.GetBytes());
            foreach (Section_LHDR i in LHDRList)
                listBytes.AddRange(i.GetBytes());
        }
    }
}