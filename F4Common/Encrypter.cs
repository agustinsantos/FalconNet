using System.Text;

namespace FalconNet.F4Common
{
    public static class Encrypter
    {
        private static byte[] MasterXOR = Encoding.ASCII.GetBytes("Falcon is your Master");
        private static readonly int xrlen = MasterXOR.Length;

        public static void EncryptBuffer(byte startkey, byte[] buffer, long length)
        {
            long idx;
            int ptr = 0;
            byte nextkey;

            if (buffer  == null|| length <= 0)
                return;

            idx = 0;

            for (int i = 0; i < length; i++)
            {
                buffer[ptr] ^= MasterXOR[(idx++) % xrlen];
                buffer[ptr] ^= startkey;
                nextkey = buffer[ptr++];
                startkey = nextkey;
            }
        }

        public static void DecryptBuffer(byte startkey, byte[] buffer, long length)
        {
            long idx;
            int ptr = 0;
            byte nextkey;

            if (buffer == null || length <= 0)
                return;

            idx = 0;
            
            for (int i = 0; i < length; i++)
            {
                nextkey = buffer[ptr];
                buffer[ptr] ^= startkey;
                buffer[ptr++] ^= MasterXOR[(idx++) % xrlen];
                startkey = nextkey;
            }
        }


    }
}
