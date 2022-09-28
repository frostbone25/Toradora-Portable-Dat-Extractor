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

        /// <summary>
        /// Writes a string, which has in binary a uint32 serialized before the actual string itself to indicate length.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static void WriteLengthPrefixedString(BinaryWriter writer, string value)
        {
            writer.Write(value.Length);
            
            for(int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }

        /// <summary>
        /// Writes a string, with no uint32 length serialized before the actual string.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static void WriteString(BinaryWriter writer, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }

        public static void PrintProgramUsage()
        {
            Console.WriteLine("Program Usage:");
            Console.WriteLine("----------- EXTRACTION ----------");
            Console.WriteLine("For a single file.");
            Console.WriteLine("app.exe -extract input.dat [output directory]");
            Console.WriteLine("For multiple files.");
            Console.WriteLine("app.exe -extract [input directory] [output directory]");
            Console.WriteLine("----------- REBUILDING ----------");
            Console.WriteLine("app.exe -rebuild [input directory] [output directory]");
        }
    }
}
