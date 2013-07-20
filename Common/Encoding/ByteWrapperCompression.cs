using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    public static class ByteWrapperCompression
    {
        public static ByteWrapper ExpandLE(this ByteWrapper compressed)
        {
            var compressedSize = Int32EncodingLE.Decode(compressed);
            var uncompressedSize = Int32EncodingLE.Decode(compressed);
            if (uncompressedSize == 0) 
                return null;
            var actualCompressed = new byte[compressed.Remaining];
            compressed.GetBytes(actualCompressed);
            byte[] uncompressed = null;
            uncompressed = LZSS.Decompress(actualCompressed, uncompressedSize);
            return new ByteWrapper(uncompressed);
        }
        public static ByteWrapper ExpandBE(this ByteWrapper compressed)
        {
            var compressedSize = Int32EncodingBE.Decode(compressed);
            var uncompressedSize = Int32EncodingBE.Decode(compressed);
            if (uncompressedSize == 0)
                return null;
            var actualCompressed = new byte[compressed.Remaining];
            compressed.GetBytes(actualCompressed);
            byte[] uncompressed = null;
            uncompressed = LZSS.Decompress(actualCompressed, uncompressedSize);
            return new ByteWrapper(uncompressed);
        }
    }
}
