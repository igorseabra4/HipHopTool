using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipHopTool
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
        
        public static byte[] ReadContainedFile(int position, int lenght)
        {
            long savePosition = Program.masterFileReader.BaseStream.Position;
            Program.masterFileReader.BaseStream.Position = position;
            byte[] data = Program.masterFileReader.ReadBytes(lenght);
            Program.masterFileReader.BaseStream.Position = savePosition;
            return data;
        }
    }
}
