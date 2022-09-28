using System;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace dat_extractor
{
    public class DataArchiveFileEntry
    {
        //|||||||||||||||||||||||||||||| TABLE ENTRY START ||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||| TABLE ENTRY START ||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||| TABLE ENTRY START ||||||||||||||||||||||||||||||
        //in the main header of the data archive, there is a table of file entries.
        //for each entry here these 4 uint32s are stored in that table for each file.

        /// <summary>
        /// [4 bytes] Pointer offset in the .dat file where the data for the file starts.
        /// </summary>
        public uint FileDataOffsetInArchive { get; set; }

        /// <summary>
        /// [4 bytes] Not sure, this value is always zero.
        /// </summary>
        public uint Unknown { get; set; }

        /// <summary>
        /// [4 bytes] How large the file data chunk is in the dat file.
        /// </summary>
        public uint FileDataSize { get; set; }

        /// <summary>
        /// [4 bytes] Pointer offset in the .dat file where the string for the file name starts.
        /// </summary>
        public uint FileNameOffsetInArchive { get; set; }

        //|||||||||||||||||||||||||||||| TABLE ENTRY END ||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||| TABLE ENTRY END ||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||| TABLE ENTRY END ||||||||||||||||||||||||||||||
        //at the end of the tables and the end of the data archive header, the file name strings are stored in bulk right after eachother.
        //same thing with the main file data for each entry.

        /// <summary>
        /// [x bytes] The parsed string in the .dat file which is the file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// [x bytes] The parsed byte array for the actual file data itself.
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>.
        /// Empty Constructor
        /// </summary>
        public DataArchiveFileEntry() { }

        /// <summary>
        /// Builds a file entry object from an input file on the disk.
        /// <para>Note: offsets are not calculated.</para>
        /// </summary>
        /// <param name="inputFile"></param>
        public DataArchiveFileEntry(string inputFile)
        {
            FileName = Path.GetFileName(inputFile);
            FileData = File.ReadAllBytes(inputFile);
            FileDataSize = (uint)FileData.Length;
            Unknown = 0;
        }

        /// <summary>
        /// Parsing a file entry from the data archive table.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="showConsole"></param>
        public DataArchiveFileEntry(BinaryReader reader, bool showConsole = false)
        {
            FileDataOffsetInArchive = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            FileDataSize = reader.ReadUInt32();
            FileNameOffsetInArchive = reader.ReadUInt32();

            if (!showConsole)
                return;

            Console.WriteLine("FileDataOffsetInArchive: {0}", FileDataOffsetInArchive);
            Console.WriteLine("Unknown: {0}", Unknown); //always 0
            Console.WriteLine("FileDataSize: {0}", FileDataSize);
            Console.WriteLine("FileNameOffset: {0}", FileNameOffsetInArchive);
        }

        /// <summary>
        /// Goes through the data archive and parses the file name string, and the file data itself (since those are located in different places in the archive).
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="showConsole"></param>
        public void ParseFileNameAndDataFromArchive(BinaryReader reader, bool showConsole = false)
        {
            //use the file name offset and seek to the position in the dat file where the file name string is located and parse it.
            reader.BaseStream.Seek(FileNameOffsetInArchive, SeekOrigin.Begin);
            FileName = Utilities.ReadLengthPrefixedString(reader);

            //use the file data offset and seek to the position in the dat file where the file data is located and parse all of the bytes.
            reader.BaseStream.Seek(FileDataOffsetInArchive, SeekOrigin.Begin);
            FileData = reader.ReadBytes((int)FileDataSize);

            if (!showConsole)
                return;

            Console.WriteLine("File Name: {1}", FileName);
            Console.WriteLine("Data Size: {1}", FileData.Length);
        }

        public void WriteTableEntry(BinaryWriter writer)
        {
            writer.Write(FileDataOffsetInArchive); //[4 bytes] FileDataOffsetInArchive
            writer.Write(Unknown); //[4 bytes] Unknown
            writer.Write(FileDataSize); //[4 bytes] FileDataSize
            writer.Write(FileNameOffsetInArchive); //[4 bytes] FileNameOffsetInArchive
        }

        public uint GetTableEntryByteSize()
        {
            uint result = 0;

            result += 4; //[4 bytes] FileDataOffsetInArchive
            result += 4; //[4 bytes] Unknown
            result += 4; //[4 bytes] FileDataSize
            result += 4; //[4 bytes] FileNameOffsetInArchive

            return result;
        }
    }
}
