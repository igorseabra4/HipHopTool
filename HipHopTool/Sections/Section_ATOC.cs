using System;
using System.Collections.Generic;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_ATOC : HipSection
    {
        public Section_AINF AINF;
        public List<Section_AHDR> AHDRList;

        public Section_ATOC Read(BinaryReader binaryReader)
        {
            sectionName = Section.ATOC;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.AINF.ToString()) throw new Exception();
            AINF = new Section_AINF().Read(binaryReader);

            AHDRList = new List<Section_AHDR>();

            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.AHDR.ToString()) throw new Exception();
                AHDRList.Add(new Section_AHDR().Read(binaryReader));
            }
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ATOC;

            listBytes.AddRange(AINF.GetBytes());
            foreach (Section_AHDR i in AHDRList)
                listBytes.AddRange(i.GetBytes());
        }
    }
}