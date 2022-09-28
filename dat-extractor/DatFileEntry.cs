namespace dat_extractor
{
    public struct DatFileEntry
    {
        /// <summary>
        /// Pointer offset in the .dat file where the data for the file starts.
        /// </summary>
        public uint FileOffset;

        /// <summary>
        /// Not sure, this value is always zero.
        /// </summary>
        public uint Unknown;

        /// <summary>
        /// How large the file data chunk is in the dat file.
        /// </summary>
        public uint FileDataSize;

        /// <summary>
        /// Pointer offset in the .dat file where the string for the file name starts.
        /// </summary>
        public uint FileNameOffset;

        /// <summary>
        /// The parsed string in the .dat file which is the file name
        /// </summary>
        public string FileName;

        /// <summary>
        /// The parsed byte array for the actual file data itself.
        /// </summary>
        public byte[] Data;
    }
}
