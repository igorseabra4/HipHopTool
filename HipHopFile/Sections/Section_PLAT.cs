using System;

namespace HipHopFile
{
    public class Section_PLAT : HipSection
    {
        public string targetPlatform;
        public string targetPlatformName;
        public string regionFormat;
        public string language;
        public string targetGame;

        public Section_PLAT() : base(Section.PLAT) { }

        public Section_PLAT(BigEndianBinaryReader reader, Game game, out Platform platform) : base(reader, Section.PLAT)
        {
            if (game == Game.BFBB)
            {
                targetPlatform = reader.ReadString();
                targetPlatformName = reader.ReadString();
                regionFormat = reader.ReadString();
                language = reader.ReadString();
                targetGame = reader.ReadString();
            }
            else if (game >= Game.Incredibles)
            {
                targetPlatform = reader.ReadString();
                language = reader.ReadString();
                regionFormat = reader.ReadString();
                targetGame = reader.ReadString();
            }
            else
                throw new Exception("PLAT reading error: unsupported PLAT version");

            platform = GetPlatform();
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PLAT;

            if (game == Game.BFBB)
            {
                writer.Write(targetPlatform);
                writer.Write(targetPlatformName);
                writer.Write(regionFormat);
                writer.Write(language);
                writer.Write(targetGame);
            }
            else if (game >= Game.Incredibles)
            {
                writer.Write(targetPlatform);
                writer.Write(language);
                writer.Write(regionFormat);
                writer.Write(targetGame);
            }
            else
                throw new Exception("PLAT writing error");
        }

        public Platform GetPlatform()
        {
            if (targetPlatform == "XB" || targetPlatformName == "Xbox" || targetPlatform == "BX")
                return Platform.Xbox;
            if (targetPlatform == "GC" || targetPlatformName == "GameCube")
                return Platform.GameCube;
            if (targetPlatform == "P2" || targetPlatform == "PS2" || targetPlatformName == "PlayStation 2")
                return Platform.PS2;
            throw new Exception("PLAT reading error: unknown platform: " + targetPlatform);
        }
    }
}
