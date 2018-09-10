using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public static class Functions
    {
        public static void SendMessage()
        {
            Console.WriteLine();
        }

        public static void SendMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static int Switch(int value)
        {
            byte[] b = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static uint Switch(uint value)
        {
            byte[] b = BitConverter.GetBytes(value);
            return BitConverter.ToUInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static float Switch(float value)
        {
            byte[] b = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static uint BKDRHash(string str)
        {
            str = str.ToUpper();
            uint seed = 131;
            uint hash = 0;
            int length = str.Length;

            if (length > 31)
                length = 31;

            for (int i = 0; i < length; i++)
                hash = (hash * seed) + str[i];

            return hash;
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

        public static void AddString(this List<byte> listBytes, string writeString)
        {
            foreach (char i in writeString)
                listBytes.Add((byte)i);

            if (writeString.Length % 2 == 0) listBytes.AddRange(new byte[] { 0, 0 });
            if (writeString.Length % 2 == 1) listBytes.AddRange(new byte[] { 0 });
        }

        public static void AddBigEndian(this List<byte> listBytes, float value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, int value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, uint value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, short value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, ushort value)
        {
            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }

        public static void AddBigEndian(this List<byte> listBytes, byte value)
        {
            listBytes.Add(value);
        }

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
            List<byte> list = new List<byte>();
            foreach (HipSection i in hipFile)
                i.SetBytes(ref list);

            return list.ToArray();
        }

        public static void HipArrayToIni(HipSection[] hipFile, string unpackFolder, bool multiFolder)
        {
            Directory.CreateDirectory(unpackFolder);
            
            switch (currentGame)
            {
                case Game.Scooby:
                    SendMessage("Game: Scooby-Doo: Night of 100 Frights");
                    break;
                case Game.Incredibles:
                    SendMessage("Game: The Incredibles, The Spongebob Squarepants Movie, or Rise of the Underminer");
                    break;
                case Game.BFBB:
                    SendMessage("Game: Spongebob Squarepants: Battle For Bikini Bottom");
                    break;
                default:
                    SendMessage("Error: Unknown game.");
                    break;
            }

            string fileName = Path.Combine(unpackFolder, "Settings.ini");

            StreamWriter INIWriter = new StreamWriter(new FileStream(fileName, FileMode.Create));
            INIWriter.WriteLine("Game=" + currentGame.ToString());
            INIWriter.WriteLine("IniVersion=1");

            foreach (HipSection i in hipFile)
            {
                if (i is Section_PACK PACK)
                {
                    INIWriter.WriteLine("PACK.PVER=" + PACK.PVER.subVersion.ToString() + v1s + PACK.PVER.clientVersion.ToString() + v1s + PACK.PVER.compatible.ToString());
                    INIWriter.WriteLine("PACK.PFLG=" + PACK.PFLG.flags.ToString());
                    INIWriter.WriteLine("PACK.PCRT=" + PACK.PCRT.fileDate.ToString() + v1s + PACK.PCRT.dateString);
                    if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                    {
                        INIWriter.WriteLine("PACK.PLAT.Target=" + PACK.PLAT.targetPlatform);
                        INIWriter.WriteLine("PACK.PLAT.RegionFormat=" + PACK.PLAT.regionFormat);
                        INIWriter.WriteLine("PACK.PLAT.Language=" + PACK.PLAT.language);
                        INIWriter.WriteLine("PACK.PLAT.TargetGame=" + PACK.PLAT.targetGame);
                        if (currentGame == Game.BFBB)
                            INIWriter.WriteLine("PACK.PLAT.TargetPlatformName=" + PACK.PLAT.targetPlatformName);
                    }
                    else if (currentGame != Game.Scooby) throw new Exception("Unknown game");

                    INIWriter.WriteLine();
                }
                else if (i is Section_DICT DICT)
                {
                    ExtractAssetsToFolders(unpackFolder, ref DICT, multiFolder);

                    INIWriter.WriteLine("DICT.ATOC.AINF=" + DICT.ATOC.AINF.value.ToString());
                    INIWriter.WriteLine("DICT.LTOC.LINF=" + DICT.LTOC.LINF.value.ToString());
                    INIWriter.WriteLine();

                    Dictionary<uint, string> AHDRDictionary = new Dictionary<uint, string>();
                    
                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                        AHDRDictionary.Add(AHDR.assetID, AHDR.assetID.ToString("X8") + v1s + AHDR.assetType + v1s + ((uint)AHDR.flags).ToString() + v1s + AHDR.ADBG.alignment.ToString() + v1s + AHDR.ADBG.assetName + v1s + AHDR.ADBG.assetFileName + v1s + AHDR.ADBG.checksum.ToString("X8"));
                    
                    foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    {
                        INIWriter.WriteLine("LayerType=" + (int)LHDR.layerType + " " + LHDR.layerType.ToString());
                        if (LHDR.assetIDlist.Count() == 0)
                            INIWriter.WriteLine("AssetAmount=0");
                        INIWriter.WriteLine("LHDR.LDBG=" + LHDR.LDBG.value.ToString());
                        foreach (uint j in LHDR.assetIDlist)
                            INIWriter.WriteLine("Asset=" + AHDRDictionary[j]);
                        INIWriter.WriteLine("EndLayer");

                        INIWriter.WriteLine();
                    }
                }
                else if (i is Section_STRM STRM)
                {
                    INIWriter.WriteLine("STRM.DHDR=" + STRM.DHDR.value.ToString());
                    INIWriter.WriteLine();
                }
            }

            INIWriter.Close();
        }

        public static void HipArrayToJson(HipSection[] hipFile, string unpackFolder, bool multiFolder)
        {
            Directory.CreateDirectory(unpackFolder);

            switch (currentGame)
            {
                case Game.Scooby:
                    SendMessage("Game: Scooby-Doo: Night of 100 Frights");
                    break;
                case Game.Incredibles:
                    SendMessage("Game: The Incredibles, The Spongebob Squarepants Movie, or Rise of the Underminer");
                    break;
                case Game.BFBB:
                    SendMessage("Game: Spongebob Squarepants: Battle For Bikini Bottom");
                    break;
                default:
                    SendMessage("Error: Unknown game.");
                    break;
            }
            
            HipSerializer serializer = new HipSerializer()
            {
                currentGame = currentGame
            };
            
            foreach (HipSection i in hipFile)
            {
                if (i is Section_PACK PACK)
                {
                    serializer.PACK_PVER_subVersion = PACK.PVER.subVersion;
                    serializer.PACK_PVER_clientVersion = PACK.PVER.clientVersion;
                    serializer.PACK_PVER_compatible = PACK.PVER.compatible;
                    serializer.PACK_PFLG_flags = PACK.PFLG.flags;
                    serializer.PACK_PCRT_fileDate = PACK.PCRT.fileDate;
                    serializer.PACK_PCRT_dateString = PACK.PCRT.dateString;

                    if (currentGame == Game.BFBB | currentGame == Game.Incredibles)
                    {
                        serializer.PACK_PLAT_TargetPlatform = PACK.PLAT.targetPlatform;
                        serializer.PACK_PLAT_RegionFormat = PACK.PLAT.regionFormat;
                        serializer.PACK_PLAT_Language = PACK.PLAT.language;
                        serializer.PACK_PLAT_TargetGame = PACK.PLAT.targetGame;

                        if (currentGame == Game.BFBB)
                            serializer.PACK_PLAT_TargetPlatformName = PACK.PLAT.targetPlatformName;
                    }
                    else if (currentGame != Game.Scooby)
                        throw new Exception("Unknown game");
                }
                else if (i is Section_DICT DICT)
                {
                    ExtractAssetsToFolders(unpackFolder, ref DICT, multiFolder);

                    serializer.DICT_ATOC_AINF_value = DICT.ATOC.AINF.value;
                    serializer.DICT_LTOC_LINF_value = DICT.LTOC.LINF.value;

                    Dictionary<uint, AssetSerializer> assetDictionary = new Dictionary<uint, AssetSerializer>();

                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                        assetDictionary.Add(AHDR.assetID, new AssetSerializer()
                        {
                            assetType = AHDR.assetType,
                            flags = AHDR.flags,
                            ADBG_alignment = AHDR.ADBG.alignment,
                            ADBG_assetName = AHDR.ADBG.assetName,
                            ADBG_assetFileName = AHDR.ADBG.assetFileName,
                            checksum = AHDR.ADBG.checksum
                        });

                    serializer.layers = new List<LayerSerializer>();
                    foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    {
                        Dictionary<uint, AssetSerializer> assets = new Dictionary<uint, AssetSerializer>();

                        foreach (uint j in LHDR.assetIDlist)
                            assets.Add(j, assetDictionary[j]);

                        serializer.layers.Add(new LayerSerializer()
                        {
                            layerType = LHDR.layerType,
                            LHDR_LDBG_value = LHDR.LDBG.value,
                            assets = assets
                        });
                    }
                }
                else if (i is Section_STRM STRM)
                {
                    serializer.SRTM_DHDR_value = STRM.DHDR.value;
                }
            }

            File.WriteAllText(Path.Combine(unpackFolder, "Settings.json"), JsonConvert.SerializeObject(serializer, Formatting.Indented));
        }

        private static void ExtractAssetsToFolders(string unpackFolder, ref Section_DICT DICT, bool multiFolder)
        {
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

                File.WriteAllBytes(Path.Combine(directoryToUnpack, assetFileName), AHDR.data);
            }
        }

        private static readonly char v0s = ',';
        private static readonly char v1s = ';';

        public static HipSection[] IniToHipArray(string INIFile)
        {
            // Let's load the data from the INI first

            string[] INI = File.ReadAllLines(INIFile);
            char sep = v0s;

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

            List<uint> assetIDlist = new List<uint>();
            Section_LHDR CurrentLHDR = new Section_LHDR();

            foreach (string s in INI)
            {
                if (s.StartsWith("Game="))
                {
                    SetGame(s.Split('=')[1]);
                }
                else if (s.StartsWith("IniVersion"))
                {
                    if (Convert.ToInt32(s.Split('=')[1]) == 1)
                        sep = v1s;
                }
                else if (s.StartsWith("PACK.PVER"))
                {
                    string[] j = s.Split('=')[1].Split(sep);
                    PACK.PVER = new Section_PVER(Convert.ToInt32(j[0]), Convert.ToInt32(j[1]), Convert.ToInt32(j[2]));
                }
                else if (s.StartsWith("PACK.PFLG"))
                {
                    PACK.PFLG = new Section_PFLG(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("PACK.PCRT"))
                {
                    string[] j = s.Split('=')[1].Split(sep);
                    PACK.PMOD = new Section_PMOD(Convert.ToInt32(j[0]));
                    PACK.PCRT = new Section_PCRT(Convert.ToInt32(j[0]), j[1]);
                }
                else if (s.StartsWith("PACK.PLAT.Target="))
                {
                    PACK.PLAT.targetPlatform = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.TargetPlatformName"))
                {
                    PACK.PLAT.targetPlatformName = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.RegionFormat"))
                {
                    PACK.PLAT.regionFormat = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.Language"))
                {
                    PACK.PLAT.language = s.Split('=')[1];
                }
                else if (s.StartsWith("PACK.PLAT.TargetGame"))
                {
                    PACK.PLAT.targetGame = s.Split('=')[1];
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
                else if (s.StartsWith("LayerType"))
                {
                    CurrentLHDR.layerType = (LayerType)Convert.ToInt32(s.Split('=')[1].Split()[0]);
                }
                else if (s.StartsWith("Asset="))
                {
                    AddAsset(s.Split('=')[1].Split(sep), ref assetIDlist, ref DICT);
                }
                else if (s.StartsWith("LHDR.LDBG"))
                {
                    CurrentLHDR.LDBG = new Section_LDBG(Convert.ToInt32(s.Split('=')[1]));
                }
                else if (s.StartsWith("EndLayer"))
                {
                    CurrentLHDR.assetIDlist = assetIDlist;
                    DICT.LTOC.LHDRList.Add(CurrentLHDR);

                    assetIDlist = new List<uint>();
                    CurrentLHDR = new Section_LHDR();
                }
            }

            return GetFilesData(INIFile, ref HIPA, ref PACK, ref DICT, ref STRM);
        }

        private static void SetGame(string v)
        {
            switch (v)
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

        private static void AddAsset(string[] j, ref List<uint> assetIDlist, ref Section_DICT DICT)
        {
            uint assetID = Convert.ToUInt32(j[0], 16);
            string assetType = j[1];
            AHDRFlags flags = (AHDRFlags)Convert.ToInt32(j[2]);
            int align = Convert.ToInt32(j[3]);
            string assetName = j[4];
            string assetFileName = j[5];
            int checksum = Convert.ToInt32(j[6], 16);

            assetIDlist.Add(assetID);

            Section_ADBG newADBG = new Section_ADBG(align, assetName, assetFileName, checksum);
            Section_AHDR newAHDR = new Section_AHDR(assetID, assetType, flags, newADBG);

            DICT.ATOC.AHDRList.Add(newAHDR);
        }

        private static HipSection[] GetFilesData(string INIFile, ref Section_HIPA HIPA, ref Section_PACK PACK, ref Section_DICT DICT, ref Section_STRM STRM)
        {
            // Let's get the data from the files now and put them in a dictionary
            Dictionary<uint, byte[]> assetDataDictionary = new Dictionary<uint, byte[]>();

            foreach (string f in Directory.GetDirectories(Path.GetDirectoryName(INIFile)))
                foreach (string i in Directory.GetFiles(f))
                {
                    byte[] file = File.ReadAllBytes(i);
                    try
                    {
                        assetDataDictionary.Add(Convert.ToUInt32(Path.GetFileName(i).Substring(1, 8), 16), file);
                    }
                    catch (Exception e)
                    {
                        SendMessage("Error importing asset " + Path.GetFileName(i) + ": " + e.Message);
                    }
                }

            // Now send all the data to the function that'll put the data in the AHDR then fill the STRM section with that
            return SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM, false, assetDataDictionary);
        }

        public static HipSection[] SetupStream(ref Section_HIPA HIPA, ref Section_PACK PACK, ref Section_DICT DICT, ref Section_STRM STRM, bool alreadyHasData = true, Dictionary<uint, byte[]> assetDataDictionary = null)
        {
            // Let's generate a temporary HIP file that will be discarded.
            List<byte> temporaryFile = new List<byte>();

            HIPA.SetBytes(ref temporaryFile);

            PACK.PCNT = new Section_PCNT(0, 0, 0, 0, 0);
            PACK.SetBytes(ref temporaryFile);

            DICT.SetBytes(ref temporaryFile);

            STRM.DPAK = new Section_DPAK() { data = new byte[0] };
            STRM.SetBytes(ref temporaryFile);
            
            // Create the new STRM stream.
            List<byte> newStream = new List<byte>();

            // We'll create these variables but won't really use them. Meh.
            int LargestSourceFileAsset = 0;
            int LargestLayer = 0;
            int LargestSourceVirtualAsset = 0;
            
            // Sort the ATOC data (AHDR sections) by their asset ID. Unsure if this is necessary, but just in case.
            DICT.ATOC.AHDRList = DICT.ATOC.AHDRList.OrderBy(AHDR => AHDR.assetID).ToList();

            // Let's build a temporary dictionary with the assets, so we can write them in layer order.
            Dictionary<uint, Section_AHDR> assetDictionary = new Dictionary<uint, Section_AHDR>();
            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                assetDictionary.Add(AHDR.assetID, AHDR);

            // Let's go through each layer.
            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                // Sort the LDBG asset IDs. The AHDR data will then be written in this order.
                LHDR.assetIDlist = LHDR.assetIDlist.OrderBy(i => i).ToList();

                foreach (uint assetID in LHDR.assetIDlist)
                {
                    Section_AHDR AHDR = assetDictionary[assetID];

                    // Does the AHDR section already have the asset data, or should we get it from the dictionary?
                    // AHDRs from IP will already have data, but the ones from the INI builder won't!
                    if (!alreadyHasData) 
                    {
                        if (!assetDataDictionary.Keys.Contains(AHDR.assetID))
                        {
                            SendMessage($"Error: asset with ID [{AHDR.assetID.ToString("X8")}] was not found. The archive will not be saved correctly and will be unusable.");
                            continue;
                        }
                        AHDR.data = assetDataDictionary[AHDR.assetID];
                    }

                    // Set stream dependant AHDR data...
                    AHDR.fileOffset = newStream.Count + STRM.DPAK.globalRelativeStartOffset;
                    AHDR.fileSize = AHDR.data.Length;

                    // And add the data to the stream.
                    newStream.AddRange(AHDR.data);

                    // Calculate alignment data which I don't understand, but hey it works.
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

                while ((newStream.Count + STRM.DPAK.globalRelativeStartOffset) % 0x20 != 0)
                    newStream.Add(0x33);
            }
            
            // Assign list as stream! We're done with the worst part.
            STRM.DPAK.data = newStream.ToArray();

            // I'll create a new PCNT, because I'm sure you'll forget to do so.
            PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count, LargestSourceFileAsset, LargestLayer, LargestSourceVirtualAsset);

            // We're done!
            return new HipSection[] { HIPA, PACK, DICT, STRM };
        }
    }
}