using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FalconNet.Common
{
    class LzWindowDictionary
    {
        int WindowSize = 0x1000;
        int WindowStart = 0;
        int WindowLength = 0;
        int MinMatchAmount = 3;
        int MaxMatchAmount = 18;
        int BlockSize = 0;
        List<int>[] OffsetList;

        public LzWindowDictionary()
        {
            // Build the offset list, so Lz compression will become significantly faster
            OffsetList = new List<int>[0x100];
            for (int i = 0; i < OffsetList.Length; i++)
                OffsetList[i] = new List<int>();
        }

        public int[] Search(byte[] DecompressedData, uint offset, uint length)
        {
            RemoveOldEntries(DecompressedData[offset]); // Remove old entries for this index

            if (offset < MinMatchAmount || length - offset < MinMatchAmount) // Can't find matches if there isn't enough data
                return new int[] { 0, 0 };

            // Start finding matches
            int[] Match = new int[] { 0, 0 };
            int MatchStart;
            int MatchSize;

            for (int i = OffsetList[DecompressedData[offset]].Count - 1; i >= 0; i--)
            {
                MatchStart = OffsetList[DecompressedData[offset]][i];
                MatchSize = 1;

                while (MatchSize < MaxMatchAmount && MatchSize < WindowLength && MatchStart + MatchSize < offset && offset + MatchSize < length && DecompressedData[offset + MatchSize] == DecompressedData[MatchStart + MatchSize])
                    MatchSize++;

                if (MatchSize >= MinMatchAmount && MatchSize > Match[1]) // This is a good match
                {
                    Match = new int[] { (int)(offset - MatchStart), MatchSize };

                    if (MatchSize == MaxMatchAmount) // Don't look for more matches
                        break;
                }
            }

            // Return the match.
            // If no match was made, the distance & length pair will be zero
            return Match;
        }

        // Slide the window
        public void SlideWindow(int Amount)
        {
            if (WindowLength == WindowSize)
                WindowStart += Amount;
            else
            {
                if (WindowLength + Amount <= WindowSize)
                    WindowLength += Amount;
                else
                {
                    Amount -= (WindowSize - WindowLength);
                    WindowLength = WindowSize;
                    WindowStart += Amount;
                }
            }
        }

        // Slide the window to the next block
        public void SlideBlock()
        {
            WindowStart += BlockSize;
        }

        // Remove old entries
        private void RemoveOldEntries(byte index)
        {
            for (int i = 0; i < OffsetList[index].Count; ) // Don't increment i
            {
                if (OffsetList[index][i] >= WindowStart)
                    break;
                else
                    OffsetList[index].RemoveAt(0);
            }
        }

        // Set variables
        public void SetWindowSize(int size)
        {
            WindowSize = size;
        }
        public void SetMinMatchAmount(int amount)
        {
            MinMatchAmount = amount;
        }
        public void SetMaxMatchAmount(int amount)
        {
            MaxMatchAmount = amount;
        }
        public void SetBlockSize(int size)
        {
            BlockSize = size;
            WindowLength = size; // The window will work in blocks now
        }

        // Add entries
        public void AddEntry(byte[] DecompressedData, int offset)
        {
            OffsetList[DecompressedData[offset]].Add(offset);
        }
        public void AddEntryRange(byte[] DecompressedData, int offset, int length)
        {
            for (int i = 0; i < length; i++)
                AddEntry(DecompressedData, offset + i);
        }
    }

    public class LZSS
    {

        /* Decompress */
        public static MemoryStream Decompress(byte[] data)
        {
			return new MemoryStream(DecompressData(data));
		}
		public static byte[] DecompressData(byte[] data)
        {
            //try
            //{
                // Compressed & Decompressed Data Information
                uint CompressedSize = (uint)data.Length;
                uint DecompressedSize = (uint)(data[0] + data[1]*256);

                uint SourcePointer = 0x4;
                uint DestPointer = 0x0;

                byte[] CompressedData = data;
                byte[] decompressedData = new byte[DecompressedSize];

                // Start Decompression
                while (SourcePointer < CompressedSize && DestPointer < DecompressedSize)
                {
                    byte Flag = CompressedData[SourcePointer]; // Compression Flag
                    SourcePointer++;

                    for (int i = 7; i >= 0; i--)
                    {
                        if ((Flag & (1 << i)) == 0) // Data is not compressed
                        {
                            decompressedData[DestPointer] = CompressedData[SourcePointer];
                            SourcePointer++;
                            DestPointer++;
                        }
                        else // Data is compressed
                        {	
							int length = CompressedData[SourcePointer];
							int pos = CompressedData[SourcePointer + 1];
							int pos2 = ((length & 0xF) << 8) | pos;
							int length2 = (length >> 4) + 1;
                            int Distance = (((CompressedData[SourcePointer] & 0xF) << 8) | CompressedData[SourcePointer + 1]);
							int Amount = (CompressedData[SourcePointer] >> 4) + 1;
                            SourcePointer += 2;

                            // Copy the data
                            for (int j = 0; j < Amount; j++)
                                decompressedData[DestPointer + j] = decompressedData[DestPointer - Distance + j];
                            DestPointer += (uint)Amount;
                        }

                        // Check for out of range
                        if (SourcePointer >= CompressedSize || DestPointer >= DecompressedSize)
                            break;
                    }
                }

                return decompressedData;
            //}
            //catch (Exception e)
            //{
           //     return null; // An error occured while decompressing
            //}
        }

        /* Compress */
        public static MemoryStream Compress(ref Stream data, string filename)
        {
            try
            {
                uint DecompressedSize = (uint)data.Length;

                MemoryStream CompressedData = new MemoryStream();
                byte[] DecompressedData = data.ToByteArray();

                uint SourcePointer = 0x0;
                uint DestPointer = 0x4;

                // Test if the file is too large to be compressed
                if (data.Length > 0xFFFFFF)
                    throw new Exception("Input file is too large to compress.");

                // Set up the Lz Compression Dictionary
                LzWindowDictionary LzDictionary = new LzWindowDictionary();
                LzDictionary.SetWindowSize(0x1000);
                LzDictionary.SetMaxMatchAmount(0xF + 3);

                // Start compression
                CompressedData.Write((uint)('\x10' | (DecompressedSize << 8)));
                while (SourcePointer < DecompressedSize)
                {
                    byte Flag = 0x0;
                    uint FlagPosition = DestPointer;
                    CompressedData.WriteByte(Flag); // It will be filled in later
                    DestPointer++;

                    for (int i = 7; i >= 0; i--)
                    {
                        int[] LzSearchMatch = LzDictionary.Search(DecompressedData, SourcePointer, DecompressedSize);
                        if (LzSearchMatch[1] > 0) // There is a compression match
                        {
                            Flag |= (byte)(1 << i);

                            CompressedData.WriteByte((byte)((((LzSearchMatch[1] - 3) & 0xF) << 4) | (((LzSearchMatch[0] - 1) & 0xFFF) >> 8)));
                            CompressedData.WriteByte((byte)((LzSearchMatch[0] - 1) & 0xFF));

                            LzDictionary.AddEntryRange(DecompressedData, (int)SourcePointer, LzSearchMatch[1]);
                            LzDictionary.SlideWindow(LzSearchMatch[1]);

                            SourcePointer += (uint)LzSearchMatch[1];
                            DestPointer += 2;
                        }
                        else // There wasn't a match
                        {
                            Flag |= (byte)(0 << i);

                            CompressedData.WriteByte(DecompressedData[SourcePointer]);

                            LzDictionary.AddEntry(DecompressedData, (int)SourcePointer);
                            LzDictionary.SlideWindow(1);

                            SourcePointer++;
                            DestPointer++;
                        }

                        // Check for out of bounds
                        if (SourcePointer >= DecompressedSize)
                            break;
                    }

                    // Write the flag.
                    // Note that the original position gets reset after writing.
                    CompressedData.Seek(FlagPosition, SeekOrigin.Begin);
                    CompressedData.WriteByte(Flag);
                    CompressedData.Seek(DestPointer, SeekOrigin.Begin);
                }

                return CompressedData;
            }
            catch
            {
                return null; // An error occured while compressing
            }
        }

        // Check
        public static bool Check(ref Stream data, string filename)
        {
            try
            {
                // Because this can conflict with other compression formats we are going to add a check them too
                return (data.ReadString(0x0, 1) == "\x10"); //&&
                    //!Compression.Dictionary[CompressionFormat.PRS].Check(ref data, filename) &&
                    //!Images.Dictionary[GraphicFormat.PVR].Check(ref data, filename));
            }
            catch
            {
                return false;
            }
        }

		/// <summary>
		///   Decompresses an array of bytes.
		/// </summary>
		/// <param name = "toDecompress">an array of bytes containing the data to decompress</param>
		/// <returns>an array of bytes containing the decompressed data</returns>
		public static byte[] Decompress (byte[] toDecompress, int uncompressedDataSize)
		{
			var toReturn = new byte[uncompressedDataSize];
			ExpandLZSS(toDecompress, toReturn);
			return toReturn;
		}

		private const int WINLEN = 0x1000;
		static byte[] SlideWin = new byte[WINLEN + 19];

		static ulong ExpandLZSS (byte[] inpBuf, byte[] OutBuf )
		{
			ulong inpLen = (ulong)inpBuf.Length;
			ulong outLen = (ulong)OutBuf.Length;
			int s = 0; // index to SlideWin;
			int q = WINLEN; // index to SlideWin + ;

			ulong Fl = 0;
			ulong pi = 0;

			//**************************************************
			// Decompression loop
			//**************************************************
			while ((outLen > 0) && (pi < inpLen)) {
				byte Flag = inpBuf [pi++];

				//**************************************************
				// up to 8 bytes of data or 8 pointers or
				//**************************************************
				for (byte bits=0; bits < 8; bits++, Flag >>= 1) {
					if (pi >= inpLen) 
						return Fl;
					if ((Flag & 0x01) != 0) {               // raw data
						SlideWin [s++] = OutBuf [Fl++] = inpBuf [pi++];
						if (s >= q) 
							s = 0; //SlideWin;
						if (--outLen == 0) 
							return Fl;
						if (pi >= inpLen) 
							return Fl;
						continue;
					}

					Debug.Assert((pi + 2) <= inpLen); 

					byte a = inpBuf [pi++];
					byte b = inpBuf [pi++];
					ulong u =  (ulong)((a << 8) | b);

					byte rlen = (byte)((byte)(u >> 12) + 2);
					ulong Rpos = (u & 0x0FFF) - 1;
					Debug.Assert (Rpos < WINLEN);

					int src = (int)Rpos; //&SlideWin [Rpos];
					int dst = (int)Fl; //&OutBuf [Fl];
					Fl += rlen;
					while (rlen-- != 0) {
						//ASSERT(src <= q); 
						if (src == q) 
							src = 0; //SlideWin;
						SlideWin [s++] = OutBuf[dst++] = SlideWin[src++];
						if (s >= q) 
							s = 0; //SlideWin;
						if (--outLen == 0) 
							return Fl;
					}
				}
			}
			return Fl;
		}

		/**
         * Decompress the given byte array using the LZSS algorithm and
         * produce the output into the given out array.
         * @param data the compressed input data
         * @param src the index to start the decompression
         * @param out the output array to store the bytes
         * @param dst the output index into this array
         * @return
         */
		private const int windowSize = 0x1000;

		protected static void AnotherVersioDecompress (byte[] data, int src, byte[] @out, int dst, int decompressedDataLength)
		{
			int marker = 0;
			int nextChar = 0xFEE;
			try {
				byte[] slidingWindow = new byte[windowSize + 19];
				while (src < data.Length) {
					marker = data [src++] & 0xFF;
					for (int i = 0; i < 8 && src < data.Length; i++) {
						bool type = (marker & (1 << i)) != 0;
						if (type) {
							byte d = data [src++];
							 @out [dst++] = d;
							slidingWindow [nextChar] = d;
							nextChar = (nextChar + 1) % windowSize;
						} else {
							int offset = data [src++] & 0xFF;
							int len = data [src++] & 0xFF;
							offset = offset | (len & 0xF0) << 4;
							len = (len & 0x0F) + 3;
							for (int j = 0; j < len; j++) {
								byte d = slidingWindow [(offset + j) % windowSize];
								 @out [dst++] = d;
								slidingWindow [nextChar] = d;
								nextChar = (nextChar + 1) % windowSize;
							}
						}
					}
				}
			} catch (Exception e) {
				Console.WriteLine ("Exception :" + e);
			}
		}
	}
}
