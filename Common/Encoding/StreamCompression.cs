﻿using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    public static class StreamCompression
    {
        public static MemoryStream ExpandLE(this Stream compressed)
        {
            var compressedSize = Int32EncodingLE.Decode(compressed);
            var uncompressedSize = Int32EncodingLE.Decode(compressed);
            if (uncompressedSize == 0) 
                return null;
            int remaining = (int)Math.Min(compressedSize, compressed.Length - compressed.Position);
            var actualCompressed = compressed.ReadBytes(remaining);
            byte[] uncompressed = LZSS.Decompress(actualCompressed, uncompressedSize);
            return new MemoryStream(uncompressed);
        }
        public static MemoryStream ExpandBE(this MemoryStream compressed)
        {
#if TODO
            var compressedSize = Int32EncodingBE.Decode(compressed);
            var uncompressedSize = Int32EncodingBE.Decode(compressed);
            if (uncompressedSize == 0)
                return null;
            var actualCompressed = new byte[compressed.Remaining];
            compressed.GetBytes(actualCompressed);
            byte[] uncompressed = null;
            uncompressed = LZSS.Decompress(actualCompressed, uncompressedSize);
            return new ByteWrapper(uncompressed);
#endif
            throw new NotImplementedException();
        }
    }
}
