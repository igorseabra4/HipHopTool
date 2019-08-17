using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class HipFile
    {
        public Game game;
        public Platform platform;

        public Section_HIPA HIPA;
        public Section_PACK PACK;
        public Section_DICT DICT;
        public Section_STRM STRM;

        public HipFile(Game game, Platform platform, Section_HIPA HIPA, Section_PACK PACK, Section_DICT DICT, Section_STRM STRM)
        {
            this.game = game;
            this.platform = platform;
            this.HIPA = HIPA;
            this.PACK = PACK;
            this.DICT = DICT;
            this.STRM = STRM;
        }

        public HipFile(string fileName)
        {
            BinaryReader binaryReader = new BinaryReader(new FileStream(fileName, FileMode.Open));

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                string currentSection = new string(binaryReader.ReadChars(4));
                if (currentSection == Section.HIPA.ToString()) HIPA = new Section_HIPA(binaryReader);
                else if (currentSection == Section.PACK.ToString()) PACK = new Section_PACK(binaryReader, out game, out platform);
                else if (currentSection == Section.DICT.ToString()) DICT = new Section_DICT(binaryReader);
                else if (currentSection == Section.STRM.ToString()) STRM = new Section_STRM(binaryReader);
                else throw new Exception(currentSection);
            }

            binaryReader.Close();
        }

        public static HipFile FromINI(string fileName)
        {
            HipFile hipFile = new HipFile();
            hipFile.SetupFromINI(fileName);
            return hipFile;
        }

        private HipFile()
        {
            game = Game.Unknown;
            platform = Platform.Unknown;

            HIPA = new Section_HIPA();
            PACK = new Section_PACK();
            DICT = new Section_DICT
            {
                ATOC = new Section_ATOC(),
                LTOC = new Section_LTOC()
            };
            STRM = new Section_STRM();
        }

        private void SetGame(string v)
        {
            switch (v)
            {
                case "Scooby":
                    game = Game.Scooby;
                    break;
                case "BFBB":
                    game = Game.BFBB;
                    break;
                case "Incredibles":
                    game = Game.Incredibles;
                    break;
                default:
                    throw new Exception("Unknown game");
            }
        }

        private void SetupFromINI(string iniFileName)
        {
            string[] INI = File.ReadAllLines(iniFileName);
            char sep = v0s;

            List<uint> assetIDlist = new List<uint>();
            Section_LHDR CurrentLHDR = new Section_LHDR();

            foreach (string s in INI)
            {
                if (s.StartsWith("Game="))
                {
                    SetGame(s.Split('=')[1]);

                    if (game == Game.BFBB || game == Game.Incredibles)
                        PACK.PLAT = new Section_PLAT();
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
                    CurrentLHDR.layerType = Convert.ToInt32(s.Split('=')[1].Split()[0]);
                }
                else if (s.StartsWith("Asset="))
                {
                    AddAsset(s.Split('=')[1].Split(sep), ref assetIDlist);
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

            // Let's get the data from the files now and put them in a dictionary
            Dictionary<uint, byte[]> assetDataDictionary = new Dictionary<uint, byte[]>();

            foreach (string f in Directory.GetDirectories(Path.GetDirectoryName(iniFileName)))
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

            // Now that we have all files data, we can give them to the AHDRs.
            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                if (!assetDataDictionary.Keys.Contains(AHDR.assetID))
                {
                    SendMessage($"Error: asset with ID [{AHDR.assetID.ToString("X8")}] was not found. The asset will be removed from the archive.");
                    DICT.ATOC.AHDRList.Remove(AHDR);
                }
                else
                    AHDR.data = assetDataDictionary[AHDR.assetID];
            }
        }

        private void AddAsset(string[] j, ref List<uint> assetIDlist)
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
        
        private void SetupSTRM()
        {
            // Let's generate a temporary HIP file that will be discarded. This sets a correct STRM.DPAK.globalRelativeStartOffset
            List<byte> temporaryFile = new List<byte>();

            HIPA.SetBytes(game, platform, ref temporaryFile);

            PACK.PCNT = new Section_PCNT(0, 0, 0, 0, 0);
            PACK.SetBytes(game, platform, ref temporaryFile);

            DICT.SetBytes(game, platform, ref temporaryFile);

            STRM.DPAK = new Section_DPAK() { data = new byte[0] };
            STRM.SetBytes(game, platform, ref temporaryFile);

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

                int finalAlignment = platform == Platform.GameCube ? 0x20 : 0x800;

                for (int i = 0; i < LHDR.assetIDlist.Count; i++)
                {
                    if (!assetDictionary.ContainsKey(LHDR.assetIDlist[i]))
                    {
                        LHDR.assetIDlist.RemoveAt(i);
                        i--;
                        continue;
                    }

                    Section_AHDR AHDR = assetDictionary[LHDR.assetIDlist[i]];
                    
                    // Set stream dependant AHDR data...
                    AHDR.fileOffset = newStream.Count + STRM.DPAK.globalRelativeStartOffset;
                    AHDR.fileSize = AHDR.data.Length;

                    // And add the data to the stream.
                    newStream.AddRange(AHDR.data);

                    // Calculate alignment data which I don't understand, but hey it works.
                    AHDR.plusValue = 0;

                    int alignment = 16;

                    if (game == Game.BFBB && AHDR.assetType == AssetType.CSN || AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                    {
                        alignment = finalAlignment;
                        AHDR.ADBG.alignment = alignment;
                    }
                    else
                        AHDR.ADBG.alignment = 0;

                    int value = AHDR.fileSize % alignment;
                    if (value != 0)
                        AHDR.plusValue = alignment - value;
                    for (int j = 0; j < AHDR.plusValue; j++)
                        newStream.Add(0x33);
                }

                while ((newStream.Count + STRM.DPAK.globalRelativeStartOffset) % finalAlignment != 0)
                    newStream.Add(0x33);
            }

            // Assign list as stream! We're done with the worst part.
            STRM.DPAK.data = newStream.ToArray();

            // I'll create a new PCNT, because I'm sure you'll forget to do so.
            PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count, LargestSourceFileAsset, LargestLayer, LargestSourceVirtualAsset);
        }

        public byte[] ToBytes()
        {
            SetupSTRM();

            List<byte> list = new List<byte>();

            HIPA.SetBytes(game, platform, ref list);
            PACK.SetBytes(game, platform, ref list);
            DICT.SetBytes(game, platform, ref list);
            STRM.SetBytes(game, platform, ref list);

            return list.ToArray();
        }

        public void ToIni(string unpackFolder, bool multiFolder, bool alphabetical)
        {
            Directory.CreateDirectory(unpackFolder);

            switch (game)
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
            INIWriter.WriteLine("Game=" + game.ToString());
            INIWriter.WriteLine("IniVersion=1");

            INIWriter.WriteLine("PACK.PVER=" + PACK.PVER.subVersion.ToString() + v1s + PACK.PVER.clientVersion.ToString() + v1s + PACK.PVER.compatible.ToString());
            INIWriter.WriteLine("PACK.PFLG=" + PACK.PFLG.flags.ToString());
            INIWriter.WriteLine("PACK.PCRT=" + PACK.PCRT.fileDate.ToString() + v1s + PACK.PCRT.dateString);
            if (game == Game.BFBB | game == Game.Incredibles)
            {
                INIWriter.WriteLine("PACK.PLAT.Target=" + PACK.PLAT.targetPlatform);
                INIWriter.WriteLine("PACK.PLAT.RegionFormat=" + PACK.PLAT.regionFormat);
                INIWriter.WriteLine("PACK.PLAT.Language=" + PACK.PLAT.language);
                INIWriter.WriteLine("PACK.PLAT.TargetGame=" + PACK.PLAT.targetGame);
                if (game == Game.BFBB)
                    INIWriter.WriteLine("PACK.PLAT.TargetPlatformName=" + PACK.PLAT.targetPlatformName);
            }
            else if (game != Game.Scooby) throw new Exception("Unknown game");

            INIWriter.WriteLine();

            ExtractAssetsToFolders(unpackFolder, multiFolder);

            INIWriter.WriteLine("DICT.ATOC.AINF=" + DICT.ATOC.AINF.value.ToString());
            INIWriter.WriteLine("DICT.LTOC.LINF=" + DICT.LTOC.LINF.value.ToString());
            INIWriter.WriteLine();

            Dictionary<uint, Section_AHDR> ahdrDictionary = new Dictionary<uint, Section_AHDR>();

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                ahdrDictionary.Add(AHDR.assetID, AHDR);

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                if (game == Game.Incredibles)
                    INIWriter.WriteLine("LayerType=" + LHDR.layerType + " " + ((LayerType_TSSM)LHDR.layerType).ToString());
                else
                    INIWriter.WriteLine("LayerType=" + LHDR.layerType + " " + ((LayerType_BFBB)LHDR.layerType).ToString());

                if (LHDR.assetIDlist.Count == 0)
                    INIWriter.WriteLine("AssetAmount=0");
                INIWriter.WriteLine("LHDR.LDBG=" + LHDR.LDBG.value.ToString());

                List<Section_AHDR> ahdrList = new List<Section_AHDR>(LHDR.assetIDlist.Count);
                foreach (uint j in LHDR.assetIDlist)
                    ahdrList.Add(ahdrDictionary[j]);

                if (alphabetical)
                    ahdrList = ahdrList.OrderBy(ahdr => ahdr.ADBG.assetName).ToList();

                foreach (Section_AHDR AHDR in ahdrList)
                {
                    string assetToString = AHDR.assetID.ToString("X8") + v1s + AHDR.assetType + v1s + ((int)AHDR.flags).ToString() + v1s + AHDR.ADBG.alignment.ToString() + v1s + AHDR.ADBG.assetName.Replace(v1s, '_') + v1s + AHDR.ADBG.assetFileName + v1s + AHDR.ADBG.checksum.ToString("X8");
                    INIWriter.WriteLine("Asset=" + assetToString);
                }

                INIWriter.WriteLine("EndLayer");

                INIWriter.WriteLine();
            }

            INIWriter.WriteLine("STRM.DHDR=" + STRM.DHDR.value.ToString());
            INIWriter.WriteLine();

            INIWriter.Close();
        }

        public void ToJson(string unpackFolder, bool multiFolder)
        {
            Directory.CreateDirectory(unpackFolder);

            switch (game)
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
                currentGame = game
            };

            serializer.PACK_PVER_subVersion = PACK.PVER.subVersion;
            serializer.PACK_PVER_clientVersion = PACK.PVER.clientVersion;
            serializer.PACK_PVER_compatible = PACK.PVER.compatible;
            serializer.PACK_PFLG_flags = PACK.PFLG.flags;
            serializer.PACK_PCRT_fileDate = PACK.PCRT.fileDate;
            serializer.PACK_PCRT_dateString = PACK.PCRT.dateString;

            if (game == Game.BFBB || game == Game.Incredibles)
            {
                serializer.PACK_PLAT_TargetPlatform = PACK.PLAT.targetPlatform;
                serializer.PACK_PLAT_RegionFormat = PACK.PLAT.regionFormat;
                serializer.PACK_PLAT_Language = PACK.PLAT.language;
                serializer.PACK_PLAT_TargetGame = PACK.PLAT.targetGame;

                if (game == Game.BFBB)
                    serializer.PACK_PLAT_TargetPlatformName = PACK.PLAT.targetPlatformName;
            }
            else if (game != Game.Scooby)
                throw new Exception("Unknown game");

            ExtractAssetsToFolders(unpackFolder, multiFolder);

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

            serializer.SRTM_DHDR_value = STRM.DHDR.value;

            throw new NotImplementedException("Unable to create Settings.json");
            //File.WriteAllText(Path.Combine(unpackFolder, "Settings.json"), JsonConvert.SerializeObject(serializer, Formatting.Indented));
        }

        private void ExtractAssetsToFolders(string unpackFolder, bool multiFolder)
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
    }
}