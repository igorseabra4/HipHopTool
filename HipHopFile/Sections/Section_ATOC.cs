using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public class Section_ATOC : HipSection
    {
        public Section_AINF AINF;
        public List<Section_AHDR> AHDRList;

        public static bool noAHDR;

        public Section_ATOC() : base(Section.ATOC)
        {
            AINF = new Section_AINF(0);
            AHDRList = new List<Section_AHDR>();
        }

        public Section_ATOC(BinaryReader binaryReader, Platform platform) : base(binaryReader, Section.ATOC)
        {
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.AINF.ToString())
                throw new Exception();
            AINF = new Section_AINF(binaryReader);

            AHDRList = new List<Section_AHDR>();
            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.AHDR.ToString())
                    throw new Exception();
                AHDRList.Add(new Section_AHDR(binaryReader, platform));
            }

            noAHDR = AHDRList.Count == 0;
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.ATOC;

            AINF.SetBytes(game, platform, ref listBytes);

            foreach (Section_AHDR i in AHDRList)
                i.SetBytes(game, platform, ref listBytes);

            noAHDR = AHDRList.Count == 0;
        }

        public Section_AHDR GetFromAssetID(uint assetID)
        {
            foreach (var AHDR in AHDRList)
                if (AHDR.assetID == assetID)
                    return AHDR;
            return null;
        }

        public void SortAHDRList()
        {
            AHDRList = AHDRList.OrderBy(ahdr => ahdr.assetID).ToList();
        }
    }
}