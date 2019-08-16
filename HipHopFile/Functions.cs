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

            //if (length > 31)
            //    length = 31;

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

        public static readonly char v0s = ',';
        public static readonly char v1s = ';';
    }
}