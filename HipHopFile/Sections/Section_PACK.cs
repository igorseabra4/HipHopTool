using System;

namespace HipHopFile
{
    public class Section_PACK : HipSection
    {
        public Section_PVER PVER;
        public Section_PFLG PFLG;
        public Section_PCNT PCNT;
        public Section_PCRT PCRT;
        public Section_PMOD PMOD;
        public Section_PLAT PLAT;

        public Section_PACK() : base(Section.PACK) { }

        public Section_PACK(BigEndianBinaryReader reader, out Game game, out Platform platform) : base(reader, Section.PACK)
        {
            long startSectionPosition = reader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PVER.ToString())
                throw new Exception();
            PVER = new Section_PVER(reader, out game);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PFLG.ToString())
                throw new Exception();
            PFLG = new Section_PFLG(reader, game, out game);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PCNT.ToString())
                throw new Exception();
            PCNT = new Section_PCNT(reader);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PCRT.ToString())
                throw new Exception();
            PCRT = new Section_PCRT(reader);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PMOD.ToString())
                throw new Exception();
            PMOD = new Section_PMOD(reader);

            if (reader.BaseStream.Position == startSectionPosition + sectionSize)
            {
                platform = Platform.Unknown;
                return;
            }

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.PLAT.ToString())
                throw new Exception();
            PLAT = new Section_PLAT(reader, game, out platform);
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PACK;

            PVER.SetBytes(game, platform, writer);
            PFLG.SetBytes(game, platform, writer);
            PCNT.SetBytes(game, platform, writer);
            PCRT.SetBytes(game, platform, writer);
            PMOD.SetBytes(game, platform, writer);
            if (PLAT != null)
                PLAT.SetBytes(game, platform, writer);
        }
    }
}
