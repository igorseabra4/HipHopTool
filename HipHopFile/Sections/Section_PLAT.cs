using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PLAT : HipSection
    {
        public string TargetPlatform;
        public string TargetPlatformName;
        public string RegionFormat;
        public string Language;
        public string TargetGame;

        public Section_PLAT()
        {
            sectionName = Section.PLAT;
        }

        public Section_PLAT(BinaryReader binaryReader)
        {
            sectionName = Section.PLAT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            if (currentGame == Game.BFBB)
            {
                TargetPlatform = ReadString(binaryReader);
                TargetPlatformName = ReadString(binaryReader);
                RegionFormat = ReadString(binaryReader);
                Language = ReadString(binaryReader);
                TargetGame = ReadString(binaryReader);
            }
            else if (currentGame == Game.Incredibles)
            {
                TargetPlatform = ReadString(binaryReader);
                Language = ReadString(binaryReader);
                RegionFormat = ReadString(binaryReader);
                TargetGame = ReadString(binaryReader);
            }
            else throw new Exception("PLAT reading error");

            if (TargetPlatform == "XB" | TargetPlatformName == "Xbox" | TargetPlatform == "BX") currentPlatform = Platform.Xbox;
            else if (TargetPlatform == "GC" | TargetPlatformName == "GameCube") currentPlatform = Platform.GameCube;
            else if (TargetPlatform == "P2" | TargetPlatformName == "PlayStation 2") currentPlatform = Platform.PS2;
            else throw new Exception("PLAT reading error: unknown platform");

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PLAT;

            if (currentGame == Game.BFBB)
            {
                WriteString(ref listBytes, TargetPlatform);
                WriteString(ref listBytes, TargetPlatformName);
                WriteString(ref listBytes, RegionFormat);
                WriteString(ref listBytes, Language);
                WriteString(ref listBytes, TargetGame);
            }
            else if (currentGame == Game.Incredibles)
            {
                WriteString(ref listBytes, TargetPlatform);
                WriteString(ref listBytes, Language);
                WriteString(ref listBytes, RegionFormat);
                WriteString(ref listBytes, TargetGame);
            }
            else throw new Exception("PLAT writing error");

            if (TargetPlatform == "XB" | TargetPlatformName == "Xbox") currentPlatform = Platform.Xbox;
            else if (TargetPlatform == "GC" | TargetPlatformName == "GameCube") currentPlatform = Platform.GameCube;
            else if (TargetPlatform == "P2" | TargetPlatformName == "PlayStation 2") currentPlatform = Platform.PS2;
            else throw new Exception("PLAT writing error");
        }
    }
}
