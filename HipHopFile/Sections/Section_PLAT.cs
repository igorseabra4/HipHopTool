using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

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

        public Section_PLAT(BinaryReader binaryReader) : base(binaryReader, Section.PLAT)
        {
            if (currentGame == Game.BFBB)
            {
                targetPlatform = ReadString(binaryReader);
                targetPlatformName = ReadString(binaryReader);
                regionFormat = ReadString(binaryReader);
                language = ReadString(binaryReader);
                targetGame = ReadString(binaryReader);
            }
            else if (currentGame == Game.Incredibles)
            {
                targetPlatform = ReadString(binaryReader);
                language = ReadString(binaryReader);
                regionFormat = ReadString(binaryReader);
                targetGame = ReadString(binaryReader);
            }
            else throw new Exception("PLAT reading error: unsupported PLAT version");

            if (targetPlatform == "XB" | targetPlatformName == "Xbox" | targetPlatform == "BX") currentPlatform = Platform.Xbox;
            else if (targetPlatform == "GC" | targetPlatformName == "GameCube") currentPlatform = Platform.GameCube;
            else if (targetPlatform == "P2" | targetPlatformName == "PlayStation 2") currentPlatform = Platform.PS2;
            else throw new Exception("PLAT reading error: unknown platform");
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PLAT;

            if (currentGame == Game.BFBB)
            {
                listBytes.AddString(targetPlatform);
                listBytes.AddString(targetPlatformName);
                listBytes.AddString(regionFormat);
                listBytes.AddString(language);
                listBytes.AddString(targetGame);
            }
            else if (currentGame == Game.Incredibles)
            {
                listBytes.AddString(targetPlatform);
                listBytes.AddString(language);
                listBytes.AddString(regionFormat);
                listBytes.AddString(targetGame);
            }
            else throw new Exception("PLAT writing error");

            if (targetPlatform == "XB" | targetPlatformName == "Xbox" | targetPlatform == "BX") currentPlatform = Platform.Xbox;
            else if (targetPlatform == "GC" | targetPlatformName == "GameCube") currentPlatform = Platform.GameCube;
            else if (targetPlatform == "P2" | targetPlatformName == "PlayStation 2") currentPlatform = Platform.PS2;
            else throw new Exception("PLAT writing error: unknown platform");
        }
    }
}
