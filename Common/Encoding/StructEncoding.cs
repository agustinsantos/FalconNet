using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FalconNet.Common.Encoding
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// [StructLayout(LayoutKind.Sequential)]
    /// public struct Result
    /// {
    ///     public int Number;
    ///     [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    ///     public string Name;
    ///     public int Size;
    /// }
    /// 
    /// [StructLayout(LayoutKind.Sequential)]
    /// public struct CoverObject
    /// {
    ///     public int NumOfResults;
    ///     [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
    ///     public Result[] Results;
    /// }
    /// </example>
    /// <typeparam name="T"></typeparam>
    public static class StructEncoding<T> where T : struct
    {
        public static readonly int Size;

        static StructEncoding()
        {
            Size = Marshal.SizeOf(typeof(T));
        }

        public static byte[] Encode(T str, int offset = 0)
        {
            byte[] arr = new byte[Size];

            IntPtr ptr = Marshal.AllocHGlobal(Size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, offset, Size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static byte[] Encode(T[] str, int numElements)
        {
            int size = Size * str.Length;
            byte[] arr = new byte[size];
            int pos = 0;

            for (int i = 0; i < numElements; i++)
            {
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(str[i], ptr, true);
                Marshal.Copy(ptr, arr, pos, Size);
                Marshal.FreeHGlobal(ptr);
                pos += Size;
            }
            return arr;
        }

        public static T Decode(byte[] arr, int startIndex = 0)
        {
            T rst = default(T);

            IntPtr ptr = Marshal.AllocHGlobal(Size);

            Marshal.Copy(arr, startIndex, ptr, Size);

            rst = (T)Marshal.PtrToStructure(ptr, rst.GetType());
            Marshal.FreeHGlobal(ptr);

            return rst;
        }

        public static bool Decode(Stream stream, out T rst)
        {
            rst = default(T);
            byte[] buffer = new byte[Size];
            int read = stream.Read(buffer, 0, Size);
            if (read != Size)
                return false;

            IntPtr ptr = Marshal.AllocHGlobal(Size);

            Marshal.Copy(buffer, 0, ptr, Size);

            rst = (T)Marshal.PtrToStructure(ptr, rst.GetType());
            Marshal.FreeHGlobal(ptr);
            return true;
        }

        public static T[] DecodeArray(byte[] arr, int numElements, int startIndex = 0)
        {
            T[] str = new T[numElements];
            int pos = startIndex;

            for (int i = 0; i < numElements; i++)
            {
                int size = Marshal.SizeOf(str);
                IntPtr ptr = Marshal.AllocHGlobal(Size);

                Marshal.Copy(arr, pos, ptr, Size);

                str[i] = (T)Marshal.PtrToStructure(ptr, str.GetType());
                Marshal.FreeHGlobal(ptr);
                pos += Size;
            }
            return str;
        }

        public static void Encode(Stream stream, T val)
        {
            Debug.Assert(stream.CanWrite);
            byte[] buf = Encode(val);
            stream.Write(buf, 0, buf.Length);
        }

        public static T Decode(Stream stream)
        {
            byte[] buffer = new byte[Size];
            stream.Read(buffer, 0, Size);

            return Decode(buffer);
        }

        public static void Encode(Stream stream, T[] val)
        {
            byte[] buf = Encode(val, val.Length);
            stream.Write(buf, 0, buf.Length);
        }

        public static T[] DecodeArray(Stream stream, int numElements)
        {
            byte[] buffer = new byte[Size * numElements];
            stream.Read(buffer, 0, Size * numElements);

            return DecodeArray(buffer, numElements);
        }

    }


}