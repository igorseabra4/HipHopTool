using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_ATOC : HipSection
    {
        public Section_AINF AINF;
        public List<Section_AHDR> AHDRList;

        public Section_ATOC()
        {
            sectionName = Section.ATOC;
            AHDRList = new List<Section_AHDR>();
        }

        public Section_ATOC(BinaryReader binaryReader)
        {
            sectionName = Section.ATOC;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.AINF.ToString()) throw new Exception();
            AINF = new Section_AINF(binaryReader);

            AHDRList = new List<Section_AHDR>();

            int currentAHDR = 0;

            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.AHDR.ToString()) throw new Exception();
                AHDRList.Add(new Section_AHDR(binaryReader, currentAHDR));

                currentAHDR++;
            }
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ATOC;

            AINF.SetBytes(ref listBytes);
            foreach (Section_AHDR i in AHDRList)
                i.SetBytes(ref listBytes);
        }
    }
}