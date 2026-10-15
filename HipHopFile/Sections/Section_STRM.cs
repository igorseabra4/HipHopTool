using System;

namespace HipHopFile
{
    public class Section_STRM : HipSection
    {
        public Section_DHDR DHDR;
        public Section_DPAK DPAK;

        public Section_STRM() : base(Section.STRM)
        {
            DHDR = new Section_DHDR(-1);
            DPAK = new Section_DPAK();
        }

        public Section_STRM(BigEndianBinaryReader reader) : base(reader, Section.STRM)
        {
            string currentSectionName;

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.DHDR.ToString())
                throw new Exception();
            DHDR = new Section_DHDR(reader);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.DPAK.ToString())
                throw new Exception();
            DPAK = new Section_DPAK(reader);
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.STRM;
            DHDR.SetBytes(game, platform, writer);
            DPAK.SetBytes(game, platform, writer);
        }
    }
}
