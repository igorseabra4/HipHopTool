using System;
using System.Collections.Generic;
using System.Linq;

namespace HipHopTool
{
    public enum Section
    {
        HIPA,
        PACK,
        PVER,
        PFLG,
        PCNT,
        PCRT,
        PMOD,
        PLAT,
        DICT,
        ATOC,
        AINF,
        AHDR,
        ADBG,
        LTOC,
        LINF,
        LHDR,
        LDBG,
        STRM,
        DHDR,
        DPAK
    }

    public abstract class HipSection
    {
        public Section sectionName;
        public int sectionSize;

        public byte[] GetBytes()
        {
            List<byte> listBytes = new List<byte>()
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
            };

            SetListBytes(ref listBytes);

            sectionSize = listBytes.Count() - 0x8;

            listBytes[0] = (byte)sectionName.ToString()[0];
            listBytes[1] = (byte)sectionName.ToString()[1];
            listBytes[2] = (byte)sectionName.ToString()[2];
            listBytes[3] = (byte)sectionName.ToString()[3];
            listBytes[4] = BitConverter.GetBytes(sectionSize)[3];
            listBytes[5] = BitConverter.GetBytes(sectionSize)[2];
            listBytes[6] = BitConverter.GetBytes(sectionSize)[1];
            listBytes[7] = BitConverter.GetBytes(sectionSize)[0];

            return listBytes.ToArray();
        }
        
        public abstract void SetListBytes(ref List<byte> listBytes);
    }
}
