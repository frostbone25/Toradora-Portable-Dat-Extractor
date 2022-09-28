# Toradora Portable (PSP) Dat File Extractor
This is a simple command line application that extracts files from .dat archives for the [Toradora Portable (PSP)](https://cdromance.com/psp/torador-portable-english/) Game.

Usage: in the windows command prompt...

`app.exe -extract input.dat [output directory]`

`app.exe -extract [input directory] [output directory]`

`app.exe -rebuild [input directory] [output directory]`

- ***-extract*** extraction mode for the application, extracting data from a dat file.
- ***-rebuild*** build mode for the application, packs a folder and the files inside of it into a dat file.
- ***[input directory]*** is the directory path on the disk where the .dat files for the game are stored. The application looks for any .dat files in this directory for extraction.
- ***[output directory]*** is the directory path on the disk where the extracted files for the .dat archive will be stored. When a dat file is extracted, there is a folder created in the output directory with the name of the dat file, and inside that folder all of the files inside the archive will be extracted here.

### TODO
- More ressarch on the building mode. There is a quick with my testing where some data archives that I extracted and rebuilt. Some offsets and overall data archive size aren't 1:1. There seems to be some additional byte padding that is going in the original data archive. If I can get some help correcting this you can make a pull request or get in contact, it would be much appreciated.
