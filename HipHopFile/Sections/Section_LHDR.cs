﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LHDR : HipSection
    {
        public int layerType;
        public List<uint> assetIDlist;
        public Section_LDBG LDBG;

        public Section_LHDR() : base(Section.LHDR) { }

        public Section_LHDR(BinaryReader binaryReader) : base(binaryReader, Section.LHDR)
        {
            layerType = Switch(binaryReader.ReadInt32());
            int assetAmount = Switch(binaryReader.ReadInt32());

            assetIDlist = new List<uint>(assetAmount);
            for (int i = 0; i < assetAmount; i++)
                assetIDlist.Add(Switch(binaryReader.ReadUInt32()));

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LDBG.ToString())
                throw new Exception();
            LDBG = new Section_LDBG(binaryReader);
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.LHDR;

            listBytes.AddBigEndian(layerType);
            listBytes.AddBigEndian(assetIDlist.Count());

            foreach (int i in assetIDlist)
                listBytes.AddBigEndian(i);

            LDBG.SetBytes(game, platform, ref listBytes);
        }
    }
}
