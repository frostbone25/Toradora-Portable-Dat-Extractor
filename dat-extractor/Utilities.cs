using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dat_extractor
{
    public static class Utilities
    {
        /// <summary>
        /// Combines a char array into a single string.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string CombineChars(char[] chars)
        {
            string result = "";

            for (int i = 0; i < chars.Length; i++)
            {
                result += chars[i];
            }

            return result;
        }

        /// <summary>
        /// Reads a string, which has in binary a uint32 serialized before the actual string itself to indicate length.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadLengthPrefixedString(BinaryReader reader)
        {
            //read the uint32 that is serialized right before the actual string, this is the length of the string.
            uint length = reader.ReadUInt32();

            //read each char and combine them into a single string.
            return CombineChars(reader.ReadChars((int)length));
        }

        public static void PrintProgramUsage()
        {
            Console.WriteLine("Program Usage:");
            Console.WriteLine("app.exe [input directory] [output directory]");
        }
    }
}
