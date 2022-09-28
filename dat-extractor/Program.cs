using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dat_extractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //if there are not exactly 2 arguments, don't continue.
            if(args.Length != 2)
            {
                Console.WriteLine("Incorrect amount of arguments!");
                Utilities.PrintProgramUsage();
                return;
            }    

            //get both arguments
            string inputPath = args[0];
            string outputPath = args[1];

            //validate the input path
            if(!Directory.Exists(inputPath))
            {
                Console.WriteLine("Input directory does not exist!");
                Utilities.PrintProgramUsage();
                return;
            }

            //validate the output path
            if (!Directory.Exists(outputPath))
            {
                Console.WriteLine("Output directory does not exist!");
                Utilities.PrintProgramUsage();
                return;
            }

            //declare and initalize a dynamic array of dat files in the input directory.
            List<string> datFiles = new List<string>();

            //get all the files in the input directory
            string[] inputDirectoryFiles = Directory.GetFiles(inputPath);

            //iterate through each file
            foreach (string inputDirectoryFile in inputDirectoryFiles)
            {
                //if we find a file with a .dat extension
                if (Path.GetExtension(inputDirectoryFile) == ".dat")
                {
                    //add it to the list
                    Console.WriteLine("Found {0}", inputDirectoryFile);
                    datFiles.Add(inputDirectoryFile);
                }
            }

            //let the user know what we found.
            Console.WriteLine("Found {0} files, commencing extraction...", datFiles.Count);

            //iterate through each dat file in the directory.
            foreach(string datFile in datFiles)
            {
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("Extracting {0}", Path.GetFileName(datFile));
                ExtractDatFile(datFile);
                Console.WriteLine("Finished extracting {0}", Path.GetFileName(datFile));
                Console.WriteLine("------------------------------------------------------");
            }

            Console.WriteLine("Finished, press a key to end.", datFiles.Count);
            Console.ReadKey();
        }

        /// <summary>
        /// Extracts a single .dat file, and creates a directory with the dat file name, and extracts the files into the directory.
        /// </summary>
        /// <param name="inputPath"></param>
        private static void ExtractDatFile(string inputPath)
        {
            //construct the directory to which the extracted files in the .dat archive will be written to.
            string inputFileName = Path.GetFileNameWithoutExtension(inputPath);
            string inputDirectory = Path.GetDirectoryName(inputPath);
            string extractedDirectory = inputDirectory + "/" + inputFileName;

            //make sure it exists, otherwise create it.
            if(Directory.Exists(extractedDirectory) == false)
                Directory.CreateDirectory(extractedDirectory);

            //open the .dat file, and we will read the binary file.
            using(BinaryReader reader = new BinaryReader(File.OpenRead(inputPath)))
            {
                //parse the magic
                string magic = Utilities.CombineChars(reader.ReadChars(4));
                Console.WriteLine("Magic: {0}", magic);

                //parse the total file size
                uint fileSize = reader.ReadUInt32(); //total file size
                Console.WriteLine("File Size: {0}", fileSize);

                //parse the unknown value
                uint unknown1 = reader.ReadUInt32(); //unknown1
                Console.WriteLine("unknown1: {0}", unknown1);

                //parse the amount of files listed in the dat archive
                uint filecount = reader.ReadUInt32(); //filecount
                Console.WriteLine("filecount: {0}", filecount);

                //create an array with the amount of files in the dat archive.
                DatFileEntry[] datFileEntries = new DatFileEntry[filecount];

                //iterate through the dat file entry table.
                for (int i = 0; i < filecount; i++)
                {
                    //create our struct
                    datFileEntries[i] = new DatFileEntry();

                    //parse the file data offset in the dat file
                    Console.WriteLine("-------- Item {0} --------");
                    datFileEntries[i].FileOffset = reader.ReadUInt32();
                    Console.WriteLine("FileOffset: {0}", datFileEntries[i].FileOffset);

                    //parse the unknown value
                    datFileEntries[i].Unknown = reader.ReadUInt32();
                    Console.WriteLine("Unknown: {0}", datFileEntries[i].Unknown); //always 0

                    //parse the size of the file data
                    datFileEntries[i].FileDataSize = reader.ReadUInt32();
                    Console.WriteLine("FileDataSize: {0}", datFileEntries[i].FileDataSize);

                    //parse the file name offset in the dat file
                    datFileEntries[i].FileNameOffset = reader.ReadUInt32();
                    Console.WriteLine("FileNameOffset: {0}", datFileEntries[i].FileNameOffset); 

                    Console.WriteLine("-------- Item End --------");
                }

                //table ends
                //now we will iterate through the entires again, but with the offsets that we parsed.
                //we will go through the file and set the pointer at the offsets to parse the needed data for the file. 

                //iterate once again through each of the file entires
                for(int i = 0; i < datFileEntries.Length; i++)
                {
                    //use the file name offset and seek to the position in the dat file where the file name string is located and parse it.
                    reader.BaseStream.Seek(datFileEntries[i].FileNameOffset, SeekOrigin.Begin);
                    datFileEntries[i].FileName = Utilities.ReadLengthPrefixedString(reader);
                    Console.WriteLine("[ENTRY {0}] File Name: {1}", i, datFileEntries[i].FileName);

                    //use the file data offset and seek to the position in the dat file where the file data is located and parse all of the bytes.
                    reader.BaseStream.Seek(datFileEntries[i].FileOffset, SeekOrigin.Begin);
                    datFileEntries[i].Data = reader.ReadBytes((int)datFileEntries[i].FileDataSize);
                    Console.WriteLine("[ENTRY {0}] Data Size: {1}", i, datFileEntries[i].Data.Length);

                    //construct the new file path and write the binary data for the file into the disk.
                    string newFilePath = string.Format("{0}/{1}", extractedDirectory, datFileEntries[i].FileName);
                    File.WriteAllBytes(newFilePath, datFileEntries[i].Data);
                }

                Console.WriteLine("left off at: {0}", reader.BaseStream.Position);
            }
        }
    }
}
