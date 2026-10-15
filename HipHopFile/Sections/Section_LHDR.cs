using System;
using System.Collections.Generic;

namespace HipHopFile
{
    public class Section_LHDR : HipSection
    {
        public int layerType;
        public List<uint> assetIDlist;
        public Section_LDBG LDBG;

        public Section_LHDR() : base(Section.LHDR) { }

        public Section_LHDR(BigEndianBinaryReader reader) : base(reader, Section.LHDR)
        {
            layerType = reader.ReadInt32();
            int assetAmount = reader.ReadInt32();

            assetIDlist = new List<uint>(assetAmount);
            for (int i = 0; i < assetAmount; i++)
                assetIDlist.Add(reader.ReadUInt32());

            string currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.LDBG.ToString())
                throw new Exception();
            LDBG = new Section_LDBG(reader);
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.LHDR;

            writer.Write(layerType);
            writer.Write(assetIDlist.Count);

            foreach (var u in assetIDlist)
                writer.Write(u);

            LDBG.SetBytes(game, platform, writer);
        }
    }
}
