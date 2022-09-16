using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_HIPB : HipSection
    {
        public int Version = 1;
        public int HasNoLayers;

        public Section_HIPB() : base(Section.HIPA)
        {
            HasNoLayers = 0;
        }

        public Section_HIPB(BinaryReader binaryReader) : base(binaryReader, Section.HIPB)
        {
            Version = binaryReader.ReadInt32();
            if (Version >= 1)
                HasNoLayers = binaryReader.ReadInt32();
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.HIPB;

            listBytes.AddBigEndian(Version);
            if (Version >= 1)
                listBytes.AddBigEndian(HasNoLayers);
        }
    }
}
