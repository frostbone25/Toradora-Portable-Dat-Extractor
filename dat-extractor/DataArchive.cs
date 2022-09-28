using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace dat_extractor
{
    public class DataArchive
    {
        /// <summary>
        /// [4 Bytes] 4 chars that make up the magic keyword to identify the file.
        /// </summary>
        public string Magic { get; set; }

        /// <summary>
        /// [4 Bytes] Total size of the data archive.
        /// </summary>
        public uint ArchiveSize { get; set; }

        /// <summary>
        /// [4 Bytes] No idea.
        /// </summary>
        public uint Unknown { get; set; }

        /// <summary>
        /// [4 Bytes] Amount of tile entires.
        /// </summary>
        public uint FileEntriesLength { get; set; }

        /// <summary>
        /// [x Bytes] Each file entry in the data archive.
        /// </summary>
        public DataArchiveFileEntry[] FileEntries { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public DataArchive() { }

        /// <summary>
        /// Parsing a .dat file on the disk into a data archive object.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="showConsole"></param>
        public DataArchive(string inputPath, bool showConsole)
        {
            //open the .dat file, and we will read the binary file.
            using (BinaryReader reader = new BinaryReader(File.OpenRead(inputPath)))
            {
                //parse the header
                Magic = Utilities.CombineChars(reader.ReadChars(4));
                ArchiveSize = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();
                FileEntriesLength = reader.ReadUInt32();

                if (showConsole)
                {
                    Console.WriteLine("Magic: {0}", Magic);
                    Console.WriteLine("ArchiveSize: {0}", ArchiveSize);
                    Console.WriteLine("Unknown: {0}", Unknown);
                    Console.WriteLine("FileEntriesLength: {0}", FileEntriesLength);
                }

                //create an array with the amount of files in the dat archive.
                FileEntries = new DataArchiveFileEntry[FileEntriesLength];

                //iterate through the dat file entry table.
                for (int i = 0; i < FileEntries.Length; i++)
                {
                    if (showConsole)
                        Console.WriteLine("-------- Item {0} --------", i);

                    FileEntries[i] = new DataArchiveFileEntry(reader, showConsole);

                    if (showConsole)
                        Console.WriteLine("-------- Item End --------");
                }

                //table ends
                //now we will iterate through the entires again, but with the offsets that we parsed.
                //we will go through the file and set the pointer at the offsets to parse the needed data for the file entry so we have it. 

                //iterate once again through each of the file entires
                for (int i = 0; i < FileEntries.Length; i++)
                {
                    FileEntries[i].ParseFileNameAndDataFromArchive(reader);
                }
            }
        }

        /// <summary>
        /// Estimate the byte size of the header.
        /// </summary>
        /// <returns></returns>
        public uint GetHeaderByteSize()
        {
            uint result = 0;

            result += 4; //[4 bytes] Magic
            result += 4; //[4 bytes] ArchiveSize
            result += 4; //[4 bytes] Unknown
            result += 4; //[4 bytes] FileEntriesLength

            //File Entry Table
            for (int i = 0; i < FileEntries.Length; i++)
            {
                result += FileEntries[i].GetTableEntryByteSize(); //[16 bytes] File Table Entry
            }

            return result;
        }

        /// <summary>
        /// Recalculate offsets for the file entry table.
        /// </summary>
        public void RecalculateOffsets()
        {
            uint estimatedPointerPosition = 0;

            estimatedPointerPosition += GetHeaderByteSize();

            //file name entry offsets
            for(int i = 0; i < FileEntries.Length; i++)
            {
                FileEntries[i].FileNameOffsetInArchive = estimatedPointerPosition;

                uint fileNameEntryByteSize = 4 + (uint)FileEntries[i].FileName.Length;
                estimatedPointerPosition += fileNameEntryByteSize;
            }

            //----------------------------------------------------------
            //FIX?: some byte padding apparently is needed?
            //most of the first file offsets start at 2048.
            //however when the header size seems to get beyond a cetain point, the file data offset then starts at 4096 instead of 2048.
            if (estimatedPointerPosition < 2048)
            {
                while (estimatedPointerPosition < 2048)
                {
                    estimatedPointerPosition++;
                }
            }
            //else, if the pointer position is beyond 2048 then pad until we reach 4096, this is another funky trend i noticed with certain header sizes?
            else
            {
                while (estimatedPointerPosition < 4096)
                {
                    estimatedPointerPosition++;
                }
            }
            //----------------------------------------------------------

            //file data entry offsets
            for (int i = 0; i < FileEntries.Length; i++)
            {
                FileEntries[i].FileDataOffsetInArchive = estimatedPointerPosition;

                estimatedPointerPosition += FileEntries[i].FileDataSize;
            }

            //we'd have reached the end of the file, so this is the total size
            ArchiveSize = estimatedPointerPosition;
        }

        /// <summary>
        /// Writes the data archive in binary format to the disk.
        /// </summary>
        /// <param name="outputPath"></param>
        public void WriteDataArchive(string outputPath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(outputPath)))
            {
                Utilities.WriteString(writer, Magic);
                writer.Write(ArchiveSize);
                writer.Write(Unknown);
                writer.Write(FileEntriesLength);

                //write the table
                for (int i = 0; i < FileEntries.Length; i++)
                {
                    DataArchiveFileEntry entry = FileEntries[i];
                    entry.WriteTableEntry(writer);
                }

                //write the file name chunks
                for (int i = 0; i < FileEntries.Length; i++)
                {
                    DataArchiveFileEntry entry = FileEntries[i];
                    Utilities.WriteLengthPrefixedString(writer, entry.FileName);
                }

                //----------------------------------------------------------
                //FIX?: some byte padding apparently is needed?
                //most of the first file offsets start at 2048.
                //however when the header size seems to get beyond a cetain point, the file data offset then starts at 4096 instead of 2048.
                if (writer.BaseStream.Position < 2048)
                {
                    while(writer.BaseStream.Position < 2048)
                    {
                        writer.Write(0);
                    }
                }
                //else, if the pointer position is beyond 2048 then pad until we reach 4096, this is another funky trend i noticed with certain header sizes?
                else
                {
                    while (writer.BaseStream.Position < 4096)
                    {
                        writer.Write(0);
                    }
                }
                //----------------------------------------------------------

                //write the file data chunks
                for (int i = 0; i < FileEntries.Length; i++)
                {
                    DataArchiveFileEntry entry = FileEntries[i];
                    writer.Write(entry.FileData);
                }
            }
        }
    }
}
