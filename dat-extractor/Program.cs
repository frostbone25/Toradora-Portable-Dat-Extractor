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
            //if there are not exactly 3 arguments, don't continue.
            if(args.Length != 3)
            {
                Console.WriteLine("Incorrect amount of arguments!");
                Utilities.PrintProgramUsage();
                return;
            }

            //get both arguments
            string mode = args[0];
            string inputPath = args[1];
            string outputPath = args[2];

            //some validations and path checks
            bool isModeValid = mode.Equals("-extract") || mode.Equals("-rebuild");
            bool isSingleFile = File.Exists(inputPath);

            //if the mode was mis-typed or something unknown...
            if (!isModeValid)
            {
                Console.WriteLine("Application mode is not recongized!");
                Utilities.PrintProgramUsage();
                return;
            }

            if (!isSingleFile && !Directory.Exists(inputPath))
            {
                Console.WriteLine("Input directory/file does not exist!");
                Utilities.PrintProgramUsage();
                return;
            }

            //check if the output directory is valid/exists...
            if (!Directory.Exists(outputPath))
            {
                Console.WriteLine("Output directory does not exist!");
                Utilities.PrintProgramUsage();
                return;
            }

            if(mode.Equals("-extract"))
            {
                //|||||||||||||||||||||| EXTRACTION ||||||||||||||||||||||
                //|||||||||||||||||||||| EXTRACTION ||||||||||||||||||||||
                //|||||||||||||||||||||| EXTRACTION ||||||||||||||||||||||

                //if we are only converting a single file
                if (isSingleFile)
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Extracting {0}", Path.GetFileName(inputPath));
                    ExtractDatFile(inputPath, outputPath);
                    Console.WriteLine("Finished extracting {0}", Path.GetFileName(inputPath));
                    Console.WriteLine("------------------------------------------------------");
                }
                else //we are converting multiple files
                {
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
                    foreach (string datFile in datFiles)
                    {
                        Console.WriteLine("------------------------------------------------------");
                        Console.WriteLine("Extracting {0}", Path.GetFileName(datFile));
                        ExtractDatFile(datFile, outputPath);
                        Console.WriteLine("Finished extracting {0}", Path.GetFileName(datFile));
                        Console.WriteLine("------------------------------------------------------");
                    }
                }
            }
            else
            {
                //|||||||||||||||||||||| BUILDING ||||||||||||||||||||||
                //|||||||||||||||||||||| BUILDING ||||||||||||||||||||||
                //|||||||||||||||||||||| BUILDING ||||||||||||||||||||||
                Console.WriteLine("Building...");
                Console.WriteLine("NOTE: This is an experimental mode, there is a chance it doesn't work as offsets/sizes might be off in the archive.");
                Console.WriteLine("Which could lead to crashing when loading it in game");

                BuildDatFile(inputPath, outputPath);
            }
            
            Console.WriteLine("Finished!");
        }

        /// <summary>
        /// Extracts a single .dat file, and creates a directory with the dat file name, and extracts the files into the directory.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        private static void ExtractDatFile(string inputPath, string outputPath)
        {
            //construct the directory to which the extracted files in the .dat archive will be written to.
            string inputFileName = Path.GetFileNameWithoutExtension(inputPath);
            string extractedDirectory = outputPath + "/" + inputFileName;

            //make sure it exists, otherwise create it.
            if(Directory.Exists(extractedDirectory) == false)
                Directory.CreateDirectory(extractedDirectory);

            //parse the input data file
            DataArchive dataArchive = new DataArchive(inputPath, true);

            //iterate through each of the file entires and write the file data to the disk
            for (int i = 0; i < dataArchive.FileEntries.Length; i++)
            {
                //construct the new file path and write the binary data for the file into the disk.
                string newFilePath = string.Format("{0}/{1}", extractedDirectory, dataArchive.FileEntries[i].FileName);
                File.WriteAllBytes(newFilePath, dataArchive.FileEntries[i].FileData);
            }
        }

        /// <summary>
        /// Builds a single .dat file, using the input directory, it gets all of the files in the directory and packs them into a data archive file.
        /// </summary>
        /// <param name="inputPath"></param>
        private static void BuildDatFile(string inputDirectory, string outputDirectory)
        {
            //construct the output file path
            string inputDirectoryName = inputDirectory.Remove(0, Path.GetDirectoryName(inputDirectory).Length);
            string outputFilePath = outputDirectory + "/" + inputDirectoryName + ".dat";

            //get the files in the input directory
            string[] inputDirectoryFiles = Directory.GetFiles(inputDirectory);

            //build the data archive
            DataArchive dataArchive = new DataArchive();
            dataArchive.Magic = "GPDA";
            dataArchive.Unknown = 0; //NOTE TO SELF: If there are issues with rebuilding the archive this might be the culprit (on all of the dat files i've looked at, this value always seems to be zero).
            dataArchive.FileEntriesLength = (uint)inputDirectoryFiles.Length;
            dataArchive.FileEntries = new DataArchiveFileEntry[dataArchive.FileEntriesLength];

            //first step
            //create a file entry object for each file, and assign the file name and data first.
            for(int i = 0; i < dataArchive.FileEntries.Length; i++)
            {
                dataArchive.FileEntries[i] = new DataArchiveFileEntry(inputDirectoryFiles[i]);
            }

            //second step
            //recalculate offsets in the archive file where the data is stored for the file name and data.
            dataArchive.RecalculateOffsets();

            //third step
            //write the final binary data to the disk.
            dataArchive.WriteDataArchive(outputFilePath);
        }
    }
}
