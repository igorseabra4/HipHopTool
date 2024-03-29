﻿using System;
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

        public Section_PLAT(BinaryReader binaryReader, Game game, out Platform platform) : base(binaryReader, Section.PLAT)
        {
            if (game == Game.BFBB)
            {
                targetPlatform = ReadString(binaryReader);
                targetPlatformName = ReadString(binaryReader);
                regionFormat = ReadString(binaryReader);
                language = ReadString(binaryReader);
                targetGame = ReadString(binaryReader);
            }
            else if (game >= Game.Incredibles)
            {
                targetPlatform = ReadString(binaryReader);
                language = ReadString(binaryReader);
                regionFormat = ReadString(binaryReader);
                targetGame = ReadString(binaryReader);
            }
            else
                throw new Exception("PLAT reading error: unsupported PLAT version");

            platform = GetPlatform();
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.PLAT;

            if (game == Game.BFBB)
            {
                listBytes.AddString(targetPlatform);
                listBytes.AddString(targetPlatformName);
                listBytes.AddString(regionFormat);
                listBytes.AddString(language);
                listBytes.AddString(targetGame);
            }
            else if (game >= Game.Incredibles)
            {
                listBytes.AddString(targetPlatform);
                listBytes.AddString(language);
                listBytes.AddString(regionFormat);
                listBytes.AddString(targetGame);
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
