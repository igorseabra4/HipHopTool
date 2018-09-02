using System.Collections.Generic;

namespace HipHopFile
{
    public class AssetSerializer
    {
        public AssetType assetType;
        public AHDRFlags flags;
        public int ADBG_alignment;
        public string ADBG_assetName;
        public string ADBG_assetFileName;
        public int checksum;
    }

    public class LayerSerializer
    {
        public LayerType layerType;
        public int LHDR_LDBG_value;
        public Dictionary<uint, AssetSerializer> assets;
    }

    public class HipSerializer
    {
        public Game currentGame;

        public int PACK_PVER_subVersion;
        public int PACK_PVER_clientVersion;
        public int PACK_PVER_compatible;

        public int PACK_PFLG_flags;

        public int PACK_PCRT_fileDate;
        public string PACK_PCRT_dateString;

        public string PACK_PLAT_TargetPlatform;
        public string PACK_PLAT_RegionFormat;
        public string PACK_PLAT_Language;
        public string PACK_PLAT_TargetGame;
        public string PACK_PLAT_TargetPlatformName;

        public int DICT_ATOC_AINF_value;
        public int DICT_LTOC_LINF_value;
        public List<LayerSerializer> layers;

        public int SRTM_DHDR_value;
    }
}
