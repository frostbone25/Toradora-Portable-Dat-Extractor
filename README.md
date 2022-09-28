# Toradora Portable (PSP) Dat File Extractor
This is a simple command line application that extracts files from .dat archives for the [Toradora Portable (PSP)](https://cdromance.com/psp/torador-portable-english/) Game.

Usage: in the windows command prompt...
`app.exe [input directory] [output directory]`

- ***[input directory]*** is the directory path on the disk where the .dat files for the game are stored. The application looks for any .dat files in this directory for extraction.
- ***[output directory]*** is the directory path on the disk where the extracted files for the .dat archive will be stored. When a dat file is extracted, there is a folder created in the output directory with the name of the dat file, and inside that folder all of the files inside the archive will be extracted here.

### TODO
- Add functionality to rebuild a Toradora Portable .dat file from a directory for modding (should be fairly simple).
