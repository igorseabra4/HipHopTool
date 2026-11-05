using static HipHopFile.Functions;

namespace HipHopFile;

public class HipFile
{
    public static HipFile FromPath(string fileName) => new HipFile(fileName, false);

    public static HipFile FromINI(string fileName) => new HipFile(fileName, true);

    private static readonly byte[] paddingBuffer = CreatePaddingBuffer();

    private static byte[] CreatePaddingBuffer()
    {
        byte[] buffer = new byte[0x800];
        Array.Fill(buffer, (byte)0x33);
        return buffer;
    }

    public static void ThrowIfInvalidSection(BigEndianBinaryReader reader, Section expected)
    {
        var found = new string(reader.ReadChars(4));
        if (found != expected.ToString())
            throw new InvalidDataException($"HIP section expected: {expected}, found : {found} at {reader.BaseStream.Position:X}");
    }

    public Section_PACK PACK;
    public Section_DICT DICT;
    public Section_HIPB HIPB;

    public Game game;
    public Platform platform;

    public HipFile(Game game, Platform platform, Section_PACK PACK, Section_DICT DICT, Section_HIPB HIPB)
    {
        this.game = game;
        this.platform = platform;
        this.PACK = PACK;
        this.DICT = DICT;
        this.HIPB = HIPB;
    }

    private HipFile(string fileName, bool isFromINI = false)
    {
        game = Game.Unknown;
        platform = Platform.Unknown;

        if (!isFromINI)
            SetupFromPath(fileName);
        else
            SetupFromINI(fileName);

        if (platform == Platform.Unknown && HIPB != null && HIPB.ScoobyPlatform != Platform.Unknown)
            platform = HIPB.ScoobyPlatform;
    }

    private void SetupFromPath(string fileName)
    {
        using (var reader = new BigEndianBinaryReader(new FileStream(fileName, FileMode.Open)))
        {
            _ = new Section_HIPA(reader);
            PACK = new Section_PACK(reader, out game, out platform);
            DICT = new Section_DICT(reader, platform);
            _ = new Section_STRM(reader);
            if (reader.BaseStream.Position != reader.BaseStream.Length)
                HIPB = new Section_HIPB(reader);
        }
        HIPB ??= new Section_HIPB();

        DateTimeOffset hipCreated = DateTimeOffset.FromUnixTimeSeconds(PACK.PCRT.fileDate);

        if (game == Game.Incredibles)
        {
            if (HIPB != null && HIPB.IncrediblesGame != Game.Unknown)
            {
                game = HIPB.IncrediblesGame;
            }
            else if (!PACK.PCRT.dateString.Contains("/"))
            {
                if (hipCreated.Year == 2004)
                    game = Game.Incredibles;
                else if (hipCreated.Year == 2005)
                    game = Game.ROTU;
                else if (hipCreated.Year == 2006)
                    game = Game.RatProto;
            }
        }
        else if (game == Game.Scooby)
        {
            if (hipCreated.Year == 2002 && hipCreated.Month == 4)
                platform = Platform.PS2;
            else if (hipCreated.Year == 2002 && hipCreated.Month == 8)
                platform = Platform.GameCube;
            else if (hipCreated.Year == 2003)
                platform = Platform.Xbox;
            else
                platform = Platform.Unknown;
        }
    }

    private static Game GetGame(string v) => v switch
    {
        "Scooby" => Game.Scooby,
        "BFBB" => Game.BFBB,
        "Incredibles" => Game.Incredibles,
        "ROTU" => Game.ROTU,
        "RatProto" => Game.RatProto,
        _ => throw new Exception("Unknown game"),
    };

    private static string GetINIValue(string line)
    {
        int separator = line.IndexOf('=');
        if (separator < 0)
            throw new InvalidDataException($"Invalid INI line: {line}");

        return line[(separator + 1)..];
    }

    private void SetupFromINI(string iniFileName)
    {
        PACK = new Section_PACK();
        DICT = new Section_DICT();

        string[] INI = File.ReadAllLines(iniFileName);
        char sep = ';';

        List<uint> assetIDlist = new List<uint>();
        Section_LHDR CurrentLHDR = new Section_LHDR();

        game = Game.Unknown;

        foreach (string s in INI)
        {
            if (s.StartsWith("Game="))
            {
                game = GetGame(GetINIValue(s));

                if (game == Game.BFBB || game >= Game.Incredibles)
                    PACK.PLAT = new Section_PLAT();
            }
            else if (s.StartsWith("IniVersion"))
            {
                if (Convert.ToInt32(GetINIValue(s)) != 2)
                    throw new Exception("Unsupported INI file. Please use HipHopFile v0.5.0 to import this INI.");
            }
            else if (s.StartsWith("PACK.PVER"))
            {
                string[] j = GetINIValue(s).Split(sep);
                PACK.PVER = new Section_PVER(Convert.ToInt32(j[0]), Convert.ToInt32(j[1]), Convert.ToInt32(j[2]));
            }
            else if (s.StartsWith("PACK.PFLG"))
            {
                PACK.PFLG = new Section_PFLG(Convert.ToInt32(GetINIValue(s)));
            }
            else if (s.StartsWith("PACK.PCRT"))
            {
                string[] j = GetINIValue(s).Split(sep);
                PACK.PMOD = new Section_PMOD(Convert.ToInt32(j[0]));
                PACK.PCRT = new Section_PCRT(Convert.ToInt32(j[0]), j[1]);
            }
            else if (s.StartsWith("PACK.PLAT.Target="))
            {
                PACK.PLAT.targetPlatform = GetINIValue(s);
            }
            else if (s.StartsWith("PACK.PLAT.TargetPlatformName"))
            {
                PACK.PLAT.targetPlatformName = GetINIValue(s);
            }
            else if (s.StartsWith("PACK.PLAT.RegionFormat"))
            {
                PACK.PLAT.regionFormat = GetINIValue(s);
            }
            else if (s.StartsWith("PACK.PLAT.Language"))
            {
                PACK.PLAT.language = GetINIValue(s);
            }
            else if (s.StartsWith("PACK.PLAT.TargetGame"))
            {
                PACK.PLAT.targetGame = GetINIValue(s);
            }
            else if (s.StartsWith("DICT.ATOC.AINF"))
            {
                DICT.ATOC.AINF = new Section_AINF(Convert.ToInt32(GetINIValue(s)));
            }
            else if (s.StartsWith("DICT.LTOC.LINF"))
            {
                DICT.LTOC.LINF = new Section_LINF(Convert.ToInt32(GetINIValue(s)));
            }
            else if (s.StartsWith("LayerType"))
            {
                CurrentLHDR.layerType = Convert.ToInt32(GetINIValue(s).Split()[0]);
            }
            else if (s.StartsWith("Asset="))
            {
                AddAsset(GetINIValue(s).Split(sep), ref assetIDlist);
            }
            else if (s.StartsWith("LHDR.LDBG"))
            {
                CurrentLHDR.LDBG = new Section_LDBG(Convert.ToInt32(GetINIValue(s)));
            }
            else if (s.StartsWith("EndLayer"))
            {
                CurrentLHDR.assetIDlist = assetIDlist;
                DICT.LTOC.LHDRList.Add(CurrentLHDR);

                assetIDlist = new List<uint>();
                CurrentLHDR = new Section_LHDR();
            }
        }

        platform = PACK.PLAT?.GetPlatform() ?? Platform.Unknown;

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
        foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList.ToList())
        {
            if (!assetDataDictionary.TryGetValue(AHDR.assetID, out byte[] value))
            {
                SendMessage($"Error: asset with ID [{AHDR.assetID:X8}] was not found. The asset will be removed from the archive.");
                DICT.ATOC.AHDRList.Remove(AHDR);
                foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    LHDR.assetIDlist.Remove(AHDR.assetID);
            }
            else
                AHDR.data = value;
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

    public void ToFile(string fileName)
    {
        using var stream = new FileStream(fileName, FileMode.Create);
        ToStream(stream);
    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        ToStream(stream);
        return stream.ToArray();
    }

    private void ToStream(Stream stream)
    {
        DICT.ATOC.SortAHDRList();

        using BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);

        var HIPA = new Section_HIPA();
        var STRM = new Section_STRM();

        HIPA.Write(game, platform, writer);

        PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count);
        PACK.Write(game, platform, writer);

        var dictStartPos = writer.BaseStream.Position;

        DICT.Write(game, platform, writer);

        // STRM header (8) + DHDR header/value (12) + DPAK header (8).
        // This is the start of the DPAK payload, including its firstPadding field.
        long strmStartPosition = writer.BaseStream.Position;
        const int sectionHeaderSize = 8;
        const int dhdrSize = sectionHeaderSize + 4;
        long strmDpakPayloadStartPosition = strmStartPosition + sectionHeaderSize + dhdrSize + sectionHeaderSize;

        STRM.Write(game, platform, writer);

        var assetDictionary = new Dictionary<uint, Section_AHDR>(DICT.ATOC.AHDRList.Count);
        foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            assetDictionary[AHDR.assetID] = AHDR;

        foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
        {
            int startLayer = (int)writer.BaseStream.Position;
            bool isSRAM = game >= Game.Incredibles ? (LayerType_TSSM)LHDR.layerType == LayerType_TSSM.SRAM : (LayerType_BFBB)LHDR.layerType == LayerType_BFBB.SRAM;

            // Sort the LDBG asset IDs. The AHDR data will then be written in this order.
            LHDR.assetIDlist = LHDR.assetIDlist.OrderBy(u => assetDictionary[u].GetCompareValue(game)).ToList();

            // Ensuring an equal order of entries in SNDI and assets in sram layer is apparently mandatory on ps2, we sort them by their type and assetid 
            if (platform == Platform.PS2 && isSRAM)
                LHDR.assetIDlist = LHDR.assetIDlist.GroupBy(u => assetDictionary[u].assetType).SelectMany(g => g.OrderBy(i => i)).ToList();

            int finalAlignment = platform == Platform.GameCube ? 0x20 : 0x800;

            int lastDataIndex = LHDR.assetIDlist.FindLastIndex(aid => assetDictionary[aid].data.Length != 0);
            uint lastSoundID = LHDR.assetIDlist.LastOrDefault(aid => assetDictionary[aid].assetType == AssetType.Sound);

            for (int i = 0; i < LHDR.assetIDlist.Count; i++)
            {
                Section_AHDR AHDR = assetDictionary[LHDR.assetIDlist[i]];

                // Set stream dependant AHDR data...
                AHDR.fileOffset = (int)writer.BaseStream.Position;

                // And add the data to the stream.
                writer.Write(AHDR.data);

                AHDR.fileSize = (int)writer.BaseStream.Position - AHDR.fileOffset;

                // Calculate alignment data
                AHDR.plusValue = 0;
                int alignment = 16;

                switch (AHDR.assetType)
                {
                    case AssetType.SoundStream when game >= Game.Incredibles && platform == Platform.GameCube:
                        AHDR.ADBG.alignment = 1;
                        break;
                    case AssetType.SoundStream:
                        alignment = finalAlignment;
                        AHDR.ADBG.alignment = alignment;
                        break;
                    case AssetType.Sound:
                        if (platform == Platform.PS2)
                        {
                            if (game == Game.Scooby)
                                alignment = 2048;
                            AHDR.ADBG.alignment = alignment;
                        }
                        else if (game >= Game.Incredibles && platform == Platform.GameCube)
                            AHDR.ADBG.alignment = 1;
                        else
                        {
                            alignment = finalAlignment;
                            AHDR.ADBG.alignment = alignment;
                        }
                        break;
                    case AssetType.Cutscene:
                        AHDR.ADBG.alignment = finalAlignment;
                        break;
                    case AssetType.TextureStream:
                    case AssetType.BinkVideo:
                    case AssetType.WireframeModel:
                        alignment = finalAlignment;
                        break;
                    case AssetType.JSP:
                    case AssetType.JSPInfo:
                    case AssetType.CutsceneStreamingSound:
                    case AssetType.LightKit:
                        AHDR.ADBG.alignment = alignment;
                        break;
                    case AssetType.BSP:
                        AHDR.ADBG.alignment = -1;
                        break;
                    default:
                        if (Functions.IsDyna(AHDR.assetType))
                            AHDR.ADBG.alignment = -1;
                        else
                            AHDR.ADBG.alignment = 0;
                        break;
                }

                // Last asset with data in layer is not aligned
                if (i < lastDataIndex)
                {
                    AHDR.plusValue = (alignment - (AHDR.fileSize % alignment)) % alignment;
                    WritePadding(writer, AHDR.plusValue);
                }

                // SND section alignment, only on PS2 games after Scooby-Doo
                if (isSRAM && platform == Platform.PS2 && game != Game.Scooby)
                {
                    if (AHDR.assetID == lastSoundID && AHDR.assetID != LHDR.assetIDlist.Last())
                    {
                        AHDR.plusValue = (int)(2048 - ((writer.BaseStream.Length - startLayer) % 2048)) % 2048;
                        WritePadding(writer, AHDR.plusValue);
                    }
                }
            }

            int padding = (int)((finalAlignment - writer.BaseStream.Position % finalAlignment) % finalAlignment);
            WritePadding(writer, padding);
        }

        var strmDpakEndPosition = writer.BaseStream.Position;
        STRM.DPAK.SectionSize = (int)(strmDpakEndPosition - strmDpakPayloadStartPosition);
        STRM.SectionSize = STRM.DPAK.SectionSize + 20;

        writer.BaseStream.Position = dictStartPos;

        DICT.Write(game, platform, writer);

        if (writer.BaseStream.Position != strmStartPosition)
            throw new InvalidOperationException("An unknown error has occured while generating the file.");

        STRM.Write(game, platform, writer);

        writer.BaseStream.Position = strmDpakEndPosition;

        HIPB?.Write(game, platform, writer);
    }

    public static void WritePadding(BigEndianBinaryWriter writer, int count)
    {
        while (count > 0)
        {
            int length = Math.Min(count, paddingBuffer.Length);
            writer.Write(paddingBuffer, 0, length);
            count -= length;
        }
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
                SendMessage("Game: The Incredibles, The Spongebob Squarepants Movie");
                break;
            case Game.ROTU:
                SendMessage("Game: The Incredibles: Rise of the Underminer");
                break;
            case Game.RatProto:
                SendMessage("Game: Ratatouille Prototype");
                break;
            case Game.BFBB:
                SendMessage("Game: Spongebob Squarepants: Battle For Bikini Bottom");
                break;
            default:
                SendMessage("Error: Unknown game.");
                break;
        }

        string fileName = Path.Combine(unpackFolder, "Settings.ini");

        using StreamWriter INIWriter = new StreamWriter(new FileStream(fileName, FileMode.Create));
        INIWriter.WriteLine("Game=" + game.ToString());
        INIWriter.WriteLine("IniVersion=2");

        INIWriter.WriteLine("PACK.PVER=" + PACK.PVER.subVersion.ToString() + ";" + PACK.PVER.clientVersion.ToString() + ";" + PACK.PVER.compatible.ToString());
        INIWriter.WriteLine("PACK.PFLG=" + PACK.PFLG.flags.ToString());
        INIWriter.WriteLine("PACK.PCRT=" + PACK.PCRT.fileDate.ToString() + ";" + PACK.PCRT.dateString);
        if (game == Game.BFBB || game >= Game.Incredibles)
        {
            INIWriter.WriteLine("PACK.PLAT.Target=" + PACK.PLAT.targetPlatform);
            INIWriter.WriteLine("PACK.PLAT.RegionFormat=" + PACK.PLAT.regionFormat);
            INIWriter.WriteLine("PACK.PLAT.Language=" + PACK.PLAT.language);
            INIWriter.WriteLine("PACK.PLAT.TargetGame=" + PACK.PLAT.targetGame);
            if (game == Game.BFBB)
                INIWriter.WriteLine("PACK.PLAT.TargetPlatformName=" + PACK.PLAT.targetPlatformName);
        }
        else if (game != Game.Scooby)
            throw new Exception("Unknown game");

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
            if (game >= Game.Incredibles)
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
                string assetToString = AHDR.assetID.ToString("X8") + ";" + AHDR.assetType.ToString() + ";" + ((int)AHDR.flags).ToString() + ";" + AHDR.ADBG.alignment.ToString() + ";" + AHDR.ADBG.assetName.Replace(';', '_') + ";" + AHDR.ADBG.assetFileName + ";" + AHDR.ADBG.checksum.ToString("X8");
                INIWriter.WriteLine("Asset=" + assetToString);
            }

            INIWriter.WriteLine("EndLayer");

            INIWriter.WriteLine();
        }

        INIWriter.Close();
    }

    public void ToJson(Game game, string unpackFolder, bool multiFolder)
    {
        throw new NotImplementedException("Unable to create Settings.json");
        Directory.CreateDirectory(unpackFolder);

        switch (game)
        {
            case Game.Scooby:
                SendMessage("Game: Scooby-Doo: Night of 100 Frights");
                break;
            case Game.Incredibles:
                SendMessage("Game: The Incredibles, The Spongebob Squarepants Movie");
                break;
            case Game.ROTU:
                SendMessage("Game: The Incredibles: Rise of the Underminer");
                break;
            case Game.RatProto:
                SendMessage("Game: Ratatouille Prototype");
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

        if (game == Game.BFBB || game >= Game.Incredibles)
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