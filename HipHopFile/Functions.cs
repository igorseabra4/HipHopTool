using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public static class Functions
    {
        public static UInt32 Switch(UInt32 a)
        {
            byte[] b = BitConverter.GetBytes(a);
            return BitConverter.ToUInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static Int32 Switch(Int32 a)
        {
            byte[] b = BitConverter.GetBytes(a);
            return BitConverter.ToInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static Single Switch(Single a)
        {
            byte[] b = BitConverter.GetBytes(a);
            return BitConverter.ToSingle(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }
        
        public static byte[] ReadContainedFile(BinaryReader binaryReader, int position, int lenght)
        {
            long savePosition = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Position = position;
            byte[] data = binaryReader.ReadBytes(lenght);
            binaryReader.BaseStream.Position = savePosition;
            return data;
        }

        public static string ReadString(BinaryReader binaryReader)
        {
            List<char> charList = new List<char>();
            do charList.Add((char)binaryReader.ReadByte());
            while (charList.Last() != '\0');            
            charList.Remove('\0');

            if (charList.Count % 2 == 0) binaryReader.BaseStream.Position += 1;

            return new string(charList.ToArray());
        }

        public static void WriteString(ref List<byte> listBytes, string writeString)
        {
            foreach (char i in writeString)
                listBytes.Add((byte)i);

            if (writeString.Length % 2 == 0) listBytes.AddRange(new byte[] { 0, 0 });
            if (writeString.Length % 2 == 1) listBytes.AddRange(new byte[] { 0 });
        }

        public static int globalRelativeStartOffset;
        public static Game currentGame = Game.Unknown;
        public static Platform currentPlatform = Platform.Unknown;
        
        public static HipSection[] HipFileToHipArray(string fileName)
        {
            List<HipSection> hipFile = new List<HipSection>(4);

            BinaryReader binaryReader = new BinaryReader(new FileStream(fileName, FileMode.Open));

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                string currentSection = new string(binaryReader.ReadChars(4));
                if (currentSection == Section.HIPA.ToString()) hipFile.Add(new Section_HIPA(binaryReader));
                else if (currentSection == Section.PACK.ToString()) hipFile.Add(new Section_PACK(binaryReader));
                else if (currentSection == Section.DICT.ToString()) hipFile.Add(new Section_DICT(binaryReader));
                else if (currentSection == Section.STRM.ToString()) hipFile.Add(new Section_STRM(binaryReader));
                else throw new Exception(currentSection);
            }

            binaryReader.Close();
            
            return hipFile.ToArray();
        }

        public static byte[] HipArrayToFile(HipSection[] hipFile)
        {
            // Create hip as list of bytes...
            List<byte> list = new List<byte>();
            foreach (HipSection i in hipFile)
                i.SetBytes(ref list);

            // Fix AHDR offsets...
            foreach (HipSection h in hipFile)
            {
                if (h is Section_DICT DICT)
                {
                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    {
                        AHDR.fileOffset += globalRelativeStartOffset;
                    }
                }
            }

            // Create hip as list of bytes again, now with correct offsets.
            list = new List<byte>();
            foreach (HipSection i in hipFile)
                i.SetBytes(ref list);

            return list.ToArray();
        }

        public static void HipArrayToIni(HipSection[] LoadedHIP, string unpackFolder, bool multiFolder)
        {
            Directory.CreateDirectory(unpackFolder);
            
            switch (currentGame)
            {
                case Game.Scooby:
                    Console.WriteLine("Game: Scooby-Doo: Night of 100 Frights");
                    break;
                case Game.Incredibles:
                    Console.WriteLine("Game: The Incredibles, The Spongebob Squarepants Movie, or Rise of the Underminer");
                    break;
                case Game.BFBB:
                    Console.WriteLine("Game: Spongebob Squarepants: Battle For Bikini Bottom");
                    break;
                default:
                    Console.WriteLine("Error: Unknown game.");
                    break;
            }

            string fileName = (Path.Combine(unpackFolder, "Settings.ini"));

            StreamWriter INIWriter = new StreamWriter(new FileStream(fileName, FileMode.Create));
            INIWriter.WriteLine("Game=" + currentGame.ToString());

            foreach (HipSection i in LoadedHIP)
            {
                if (i is Section_PACK PACK)
                {
                    INIWriter.WriteLine("PACK.PVER=" + PACK.PVER.subVersion.ToString() + "," + PACK.PVER.clientVersion.ToString() + "," + PACK.PVER.compatible.ToString());
                    INIWriter.WriteLine("PACK.PFLG=" + PACK.PFLG.flags.ToString());
                    INIWriter.WriteLine("PACK.PCRT=" + PACK.PCRT.fileDate.ToString() + "," + PACK.PCRT.dateString);
                    if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                    {
                        INIWriter.WriteLine("PACK.PLAT.Target=" + PACK.PLAT.TargetPlatform);
                        INIWriter.WriteLine("PACK.PLAT.RegionFormat=" + PACK.PLAT.RegionFormat);
                        INIWriter.WriteLine("PACK.PLAT.Language=" + PACK.PLAT.Language);
                        INIWriter.WriteLine("PACK.PLAT.TargetGame=" + PACK.PLAT.TargetGame);
                        if (currentGame == Game.BFBB)
                            INIWriter.WriteLine("PACK.PLAT.TargetPlatformName=" + PACK.PLAT.TargetPlatformName);
                    }
                    else if (currentGame != Game.Scooby) throw new Exception("Unknown game");

                    INIWriter.WriteLine();
                }
                else if (i is Section_DICT DICT)
                {
                    INIWriter.WriteLine("DICT.ATOC.AINF=" + DICT.ATOC.AINF.value.ToString());
                    INIWriter.WriteLine("DICT.LTOC.LINF=" + DICT.LTOC.LINF.value.ToString());
                    INIWriter.WriteLine();

                    Dictionary<int, string> AHDRDictionary = new Dictionary<int, string>();

                    string directoryToUnpack = Path.Combine(unpackFolder, "files");

                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    {
                        if (multiFolder)
                            directoryToUnpack = Path.Combine(unpackFolder, AHDR.assetType.ToString());

                        if (!Directory.Exists(directoryToUnpack))
                            Directory.CreateDirectory(directoryToUnpack);

                        string assetFileName = "[" + AHDR.assetID.ToString("X8") + "] " + AHDR.ADBG.assetName;

                        foreach (char c in Path.GetInvalidFileNameChars())
                        {
                            assetFileName = assetFileName.Replace(c, '_');
                        }

                        File.WriteAllBytes(Path.Combine(directoryToUnpack, assetFileName), AHDR.containedFile);
                        AHDRDictionary.Add(AHDR.assetID, AHDR.assetID.ToString("X8") + "," + AHDR.assetType + "," + AHDR.flags.ToString() + "," + AHDR.ADBG.alignment.ToString() + "," + AHDR.ADBG.assetName + "," + AHDR.ADBG.assetFileName + "," + AHDR.ADBG.checksum.ToString("X8"));
                    }

                    foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    {
                        INIWriter.WriteLine("LayerType=" + (int)LHDR.layerType + " " + LHDR.layerType.ToString());
                        if (LHDR.assetIDlist.Count() == 0)
                            INIWriter.WriteLine("AssetAmount=0");
                        INIWriter.WriteLine("LHDR.LDBG=" + LHDR.LDBG.value.ToString());
                        foreach (int j in LHDR.assetIDlist)
                            INIWriter.WriteLine("Asset=" + AHDRDictionary[j]);
                        INIWriter.WriteLine("EndLayer");

                        INIWriter.WriteLine();
                    }
                }
                else if (i is Section_STRM STRM)
                {
                    INIWriter.WriteLine("STRM.DHDR=" + STRM.DHDR.value.ToString());
                    INIWriter.WriteLine("STRM.Padding=" + STRM.DPAK.firstPadding.ToString());
                    INIWriter.WriteLine();
                }
            }

            INIWriter.Close();
        }

        public static HipSection[] IniToHipArray(string INIFile)
        {
            // Let's load the data from the INI first

            string[] INI = File.ReadAllLines(INIFile);

            Section_HIPA HIPA = new Section_HIPA();

            Section_PACK PACK = new Section_PACK
            {
                PLAT = new Section_PLAT(),
            };

            Section_DICT DICT = new Section_DICT
            {
                ATOC = new Section_ATOC(),
                LTOC = new Section_LTOC()
            };

            Section_STRM STRM = new Section_STRM();

            List<int> assetIDlist = new List<int>();
            Section_LHDR CurrentLHDR = new Section_LHDR();

            foreach (string s in INI)
            {
                if (s.StartsWith("Game="))
                {
                    switch (s.Split('=')[1])
                    {
                        case "Scooby":
                            currentGame = Game.Scooby;
                            break;
                        case "BFBB":
                            currentGame = Game.BFBB;
                            break;
                        case "Incredibles":
                            currentGame = Game.Incredibles;
                            break;
                        default:
                            throw new Exception("Unknown game");
                    }
                }
                else if (s.StartsWith("PACK.PVER"))
                {
                    string[] j = s.Split('=')[1].Split(',');
                    PACK.PVER = new Section_PVER(Convert.ToInt32(j[0]), Convert.ToInt32(j[1]), Convert.ToInt32(j[2]));
                }
                else if (s.StartsWith("PACK.PFLG"))
                {
                    PACK.PFLG = new Section_PFLG(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("PACK.PCRT"))
                {
                    string[] j = s.Split('=')[1].Split(',');
                    PACK.PMOD = new Section_PMOD(Convert.ToInt32(j[0]));
                    PACK.PCRT = new Section_PCRT(Convert.ToInt32(j[0]), j[1]);
                }
                else if (s.StartsWith("PACK.PLAT.Target="))
                {
                    PACK.PLAT.TargetPlatform = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.TargetPlatformName"))
                {
                    PACK.PLAT.TargetPlatformName = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.RegionFormat"))
                {
                    PACK.PLAT.RegionFormat = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.Language"))
                {
                    PACK.PLAT.Language = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.TargetGame"))
                {
                    PACK.PLAT.TargetGame = s.Split('=')[1];
                }
                else if (s.StartsWith("DICT.ATOC.AINF"))
                {
                    DICT.ATOC.AINF = new Section_AINF(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("DICT.LTOC.LINF"))
                {
                    DICT.LTOC.LINF = new Section_LINF(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("STRM.DHDR"))
                {
                    STRM.DHDR = new Section_DHDR(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("STRM.Padding"))
                {
                    STRM.DPAK = new Section_DPAK(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("LayerType"))
                {
                    CurrentLHDR.layerType = (LayerType)Convert.ToInt32(s.Split('=')[1].Split()[0]);
                }
                else if (s.StartsWith("Asset="))
                {
                    string[] j = s.Split('=')[1].Split(',');
                    assetIDlist.Add(Convert.ToInt32(j[0], 16));
                    DICT.ATOC.AHDRList.Add(new Section_AHDR(Convert.ToInt32(j[0], 16), j[1], (AHDRFlags)Convert.ToInt32(j[2]),
                        new Section_ADBG(Convert.ToInt32(j[3]), j[4], j[5], Convert.ToInt32(j[6], 16))));
                }
                else if (s.StartsWith("LHDR.LDBG"))
                {
                    CurrentLHDR.LDBG = new Section_LDBG(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("EndLayer"))
                {
                    CurrentLHDR.assetIDlist = assetIDlist;
                    DICT.LTOC.LHDRList.Add(CurrentLHDR);

                    assetIDlist = new List<int>();
                    CurrentLHDR = new Section_LHDR();
                }
            }

            // Let's get the data from the files now, then add them to the AHDRs

            string[] Folders = Directory.GetDirectories(Path.GetDirectoryName(INIFile));

            Dictionary<int, byte[]> FileDictionary = new Dictionary<int, byte[]>();

            foreach (string f in Folders)
                foreach (string i in Directory.GetFiles(f))
                {
                    byte[] file = File.ReadAllBytes(i);
                    try
                    {
                        FileDictionary.Add(Convert.ToInt32(Path.GetFileName(i).Substring(1, 8), 16), file);
                    }
                    catch
                    {
                        Console.WriteLine("Error: asset " + Path.GetFileName(i) + " already imported. Skipping.");
                    }
                }

            List<byte> newStream = new List<byte>();
            for (int j = 0; j < STRM.DPAK.firstPadding; j++)
                newStream.Add(0x33);

            int LargestSourceFileAsset = 0;
            int LargestLayer = 0;
            int LargestSourceVirtualAsset = 0;

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                if (!FileDictionary.Keys.Contains(AHDR.assetID))
                {
                    Console.WriteLine("Error: asset id [" + AHDR.assetID.ToString("X8") + "] not present. File will be unusable.");
                    continue;
                }

                AHDR.containedFile = FileDictionary[AHDR.assetID];

                AHDR.fileOffset = newStream.Count();
                AHDR.fileSize = AHDR.containedFile.Length;
                newStream.AddRange(AHDR.containedFile);

                AHDR.plusValue = 0;

                int alignment = 16;
                if (currentGame == Game.BFBB)
                {
                    if (AHDR.assetType == AssetType.CSN |
                        AHDR.assetType == AssetType.SND |
                        AHDR.assetType == AssetType.SNDS)
                        alignment = 32;
                    else if (AHDR.assetType == AssetType.CRDT)
                        alignment = 4;
                }

                int value = AHDR.fileSize % alignment;
                if (value != 0)
                    AHDR.plusValue = alignment - value;
                for (int j = 0; j < AHDR.plusValue; j++)
                    newStream.Add(0x33);
            }

            int value2 = (newStream.Count - STRM.DPAK.firstPadding) % 0x20;
            if (value2 != 0)
                for (int j = 0; j < 0x20 - value2; j++)
                    newStream.Add(0x33);

            STRM.DPAK.data = newStream.ToArray();

            PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count, LargestSourceFileAsset, LargestLayer, LargestSourceVirtualAsset);

            return new HipSection[4] { HIPA, PACK, DICT, STRM };
        }
    }
}