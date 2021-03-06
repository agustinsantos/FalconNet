﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace F4Utils.Resources
{
    public enum F4ResourceType : uint
    {
        Unknown = 0,
        ImageResource = 100,
        SoundResource = 101,
        FlatResource = 102,
    }

    public class F4ResourceBundleReader
    {
        private const string RESOURCE_FILE_EXTENSION = ".rsc";
        private F4ResourceBundleIndex _resourceIndex;


        public int NumResources
        {
            get
            {
                if (_resourceIndex == null)
                {
                    return -1;
                }
                return (int) _resourceIndex.NumResources;
            }
        }

        public virtual void Load(string resourceBundleIndexPath)
        {
            var resourceIndexFileInfo = new FileInfo(resourceBundleIndexPath);
            if (resourceIndexFileInfo.Exists)
            {
                var bytes = new byte[resourceIndexFileInfo.Length];
                using (var fs = new FileStream(resourceBundleIndexPath, FileMode.Open))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(bytes, 0, (int) resourceIndexFileInfo.Length);
                }
                _resourceIndex = new F4ResourceBundleIndex();
                var curByte = 0;
                _resourceIndex.Size = BitConverter.ToUInt32(bytes, curByte);
                curByte += 4;
                _resourceIndex.ResourceIndexVersion = BitConverter.ToUInt32(bytes, curByte);
                curByte += 4;
                var size = _resourceIndex.Size;
                var headers = new List<F4ResourceHeader>();

                while (size > 0)
                {
                    _resourceIndex.NumResources++;
                    var resourceType = BitConverter.ToUInt32(bytes, curByte);
                    curByte += 4;
                    var resourceId = new byte[32];
                    for (var j = 0; j < 32; j++)
                    {
                        resourceId[j] = bytes[curByte];
                        curByte++;
                    }
                    var resourceName = Encoding.ASCII.GetString(resourceId);
                    var nullLoc = resourceName.IndexOf('\0');
                    resourceName = nullLoc > 0 ? resourceName.Substring(0, nullLoc) : null;
                    if (resourceType == (uint) (F4ResourceType.ImageResource))
                    {
                        var thisResourceHeader = new F4ImageResourceHeader
                                                     {
                                                         Type = resourceType,
                                                         ID = resourceName,
                                                         Flags = BitConverter.ToUInt32(bytes, curByte)
                                                     };
                        curByte += 4;
                        thisResourceHeader.CenterX = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.CenterY = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.Width = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.Height = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.ImageOffset = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        thisResourceHeader.PaletteSize = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        thisResourceHeader.PaletteOffset = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        headers.Add(thisResourceHeader);
                        size -= 60;
                    }
                    else if (resourceType == (uint) (F4ResourceType.SoundResource))
                    {
                        var thisResourceHeader = new F4SoundResourceHeader
                                                     {
                                                         Type = resourceType,
                                                         ID = resourceName,
                                                         Flags = BitConverter.ToUInt32(bytes, curByte)
                                                     };
                        curByte += 4;
                        thisResourceHeader.Channels = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.SoundType = BitConverter.ToUInt16(bytes, curByte);
                        curByte += 2;
                        thisResourceHeader.Offset = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        thisResourceHeader.HeaderSize = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        headers.Add(thisResourceHeader);
                        size -= 52;
                    }
                    else if (resourceType == (uint) (F4ResourceType.FlatResource))
                    {
                        var thisResourceHeader = new F4FlatResourceHeader
                                                     {
                                                         Type = resourceType,
                                                         ID = resourceName,
                                                         Offset = BitConverter.ToUInt32(bytes, curByte)
                                                     };
                        curByte += 4;
                        thisResourceHeader.Size = BitConverter.ToUInt32(bytes, curByte);
                        curByte += 4;
                        headers.Add(thisResourceHeader);
                        size -= 44;
                    }
                }
                _resourceIndex.ResourceHeaders = headers.ToArray();

                var resourceDataFileInfo = new FileInfo(
                    Path.GetDirectoryName(resourceIndexFileInfo.FullName) + Path.DirectorySeparatorChar +
                    Path.GetFileNameWithoutExtension(resourceIndexFileInfo.FullName) + RESOURCE_FILE_EXTENSION);
                if (resourceDataFileInfo.Exists)
                {
                    bytes = new byte[resourceDataFileInfo.Length];

                    using (var fs = new FileStream(resourceDataFileInfo.FullName, FileMode.Open))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Read(bytes, 0, (int) resourceDataFileInfo.Length);
                    }
                    var rawDataPackage = new F4ResourceRawDataPackage();
                    curByte = 0;
                    rawDataPackage.Size = BitConverter.ToUInt32(bytes, curByte);
                    curByte += 4;
                    rawDataPackage.Version = BitConverter.ToUInt32(bytes, curByte);
                    curByte += 4;
                    rawDataPackage.Data = new byte[rawDataPackage.Size];
                    var numBytesToCopy = Math.Min(rawDataPackage.Data.Length, bytes.Length - curByte);
                    Array.Copy(bytes, curByte, rawDataPackage.Data, 0, numBytesToCopy);
                    curByte += numBytesToCopy;
                    _resourceIndex.ResourceData = rawDataPackage;
                }
                else
                {
                    throw new FileNotFoundException(resourceDataFileInfo.FullName);
                }
            }
            else
            {
                throw new FileNotFoundException(resourceBundleIndexPath);
            }
        }

        public virtual F4ResourceType GetResourceType(int resourceNum)
        {
            if (_resourceIndex == null)
            {
                return F4ResourceType.Unknown;
            }
            return (F4ResourceType) _resourceIndex.ResourceHeaders[resourceNum].Type;
        }

        public virtual string GetResourceID(int resourceNum)
        {
            if (_resourceIndex == null)
            {
                return null;
            }
            return _resourceIndex.ResourceHeaders[resourceNum].ID;
        }

        public virtual byte[] GetSoundResource(string resourceId)
        {
            var resourceHeader = GetResourceHeader(resourceId) as F4SoundResourceHeader;
            return GetSoundResource(resourceHeader);
        }

        public virtual byte[] GetSoundResource(int resourceNum)
        {
            if (_resourceIndex == null || _resourceIndex.ResourceHeaders == null ||
                resourceNum >= _resourceIndex.ResourceHeaders.Length)
            {
                return null;
            }
            var resourceHeader = _resourceIndex.ResourceHeaders[resourceNum] as F4SoundResourceHeader;
            return GetSoundResource(resourceHeader);
        }

        protected virtual byte[] GetSoundResource(F4SoundResourceHeader resourceHeader)
        {
            if (resourceHeader == null) return null;
            var curByte = (int) resourceHeader.Offset;
            curByte += 4;
            var dataSize = BitConverter.ToUInt32(_resourceIndex.ResourceData.Data, curByte);
            curByte += 4;
            var toReturn = new byte[dataSize + 8];
            Array.Copy(_resourceIndex.ResourceData.Data, curByte - 8, toReturn, 0, dataSize + 8);
            return toReturn;
        }

        public virtual byte[] GetFlatResource(int resourceNum)
        {
            if (_resourceIndex == null || _resourceIndex.ResourceHeaders == null ||
                resourceNum >= _resourceIndex.ResourceHeaders.Length)
            {
                return null;
            }
            var resourceHeader = _resourceIndex.ResourceHeaders[resourceNum] as F4FlatResourceHeader;
            return GetFlatResource(resourceHeader);
        }

        public virtual byte[] GetFlatResource(string resourceId)
        {
            var resourceHeader = GetResourceHeader(resourceId) as F4FlatResourceHeader;
            return GetFlatResource(resourceHeader);
        }

        protected virtual byte[] GetFlatResource(F4FlatResourceHeader resourceHeader)
        {
            if (resourceHeader == null) return null;
            var bytes = new byte[resourceHeader.Size];
            for (var i = 0; i < resourceHeader.Size; i++)
            {
                bytes[i] = _resourceIndex.ResourceData.Data[resourceHeader.Offset + i];
            }
            return bytes;
        }

        public virtual Bitmap GetImageResource(string resourceId)
        {
            var imageHeader = GetResourceHeader(resourceId) as F4ImageResourceHeader;
            return GetImageResource(imageHeader);
        }

        public virtual Bitmap GetImageResource(int resourceNum)
        {
            if (_resourceIndex == null || _resourceIndex.ResourceHeaders == null ||
                resourceNum >= _resourceIndex.ResourceHeaders.Length)
            {
                return null;
            }
            var imageHeader = _resourceIndex.ResourceHeaders[resourceNum] as F4ImageResourceHeader;
            return GetImageResource(imageHeader);
        }
		
		public string GetResourceName(int resourceNum)
		{
			if (_resourceIndex == null || _resourceIndex.ResourceHeaders == null || resourceNum >= _resourceIndex.ResourceHeaders.Length)
            {
                return null;
            }
            F4ResourceHeader header = _resourceIndex.ResourceHeaders[resourceNum];
			return header.ID;
		}
		
        protected virtual Bitmap GetImageResource(F4ImageResourceHeader imageHeader)
        {
            if (imageHeader == null) return null;
            var toReturn = new Bitmap(imageHeader.Width, imageHeader.Height);
            var palette = new ushort[imageHeader.PaletteSize];
            if ((imageHeader.Flags & (uint) F4ResourceFlags.EightBit) == (uint) F4ResourceFlags.EightBit)
            {
                for (var i = 0; i < palette.Length; i++)
                {
                    palette[i] = BitConverter.ToUInt16(_resourceIndex.ResourceData.Data,
                                                       (int) imageHeader.PaletteOffset + (i*2));
                }
            }
            var curByte = 0;
            for (var y = 0; y < imageHeader.Height; y++)
            {
                for (var x = 0; x < imageHeader.Width; x++)
                {
                    var A = 0;
                    var R = 0;
                    var G = 0;
                    var B = 0;
                    if ((imageHeader.Flags & (uint) F4ResourceFlags.EightBit) == (uint) F4ResourceFlags.EightBit)
                    {
                        var thisPixelPaletteIndex = _resourceIndex.ResourceData.Data[imageHeader.ImageOffset + curByte];
                        var thisPixelPaletteEntry = palette[thisPixelPaletteIndex];
                        if (thisPixelPaletteIndex != 0)
                        {
                            A = 255;
                            R = ((thisPixelPaletteEntry & 0x7C00) >> 10) << 3;
                            G = ((thisPixelPaletteEntry & 0x3E0) >> 5) << 3;
                            B = (thisPixelPaletteEntry & 0x1F) << 3;
                        }
                        curByte++;
                    }
                    else if ((imageHeader.Flags & (uint) F4ResourceFlags.SixteenBit) == (uint) F4ResourceFlags.SixteenBit)
                    {
                        var thisPixelPaletteEntry = BitConverter.ToUInt16(_resourceIndex.ResourceData.Data,
                                                                          (int)
                                                                          (imageHeader.ImageOffset + curByte));
                        A = 255;
                        R = ((thisPixelPaletteEntry & 0x7C00) >> 10) << 3;
                        G = ((thisPixelPaletteEntry & 0x3E0) >> 5) << 3;
                        B = (thisPixelPaletteEntry & 0x1F) << 3;
                        curByte += 2;
                    }
                    toReturn.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }
            return toReturn;
        }

        protected virtual F4ResourceHeader GetResourceHeader(string resourceId)
        {
            if (_resourceIndex == null || _resourceIndex.ResourceHeaders == null || resourceId == null)
            {
                return null;
            }
            return (from thisResourceHeader in _resourceIndex.ResourceHeaders
                    let thisResourceId = thisResourceHeader.ID
                    where thisResourceId.ToLowerInvariant() == resourceId.ToLowerInvariant()
                    select thisResourceHeader).FirstOrDefault();
        }

        #region Nested type: F4FlatResourceHeader

        protected internal class F4FlatResourceHeader : F4ResourceHeader
        {
            public uint Offset;
            public uint Size;
        }

        #endregion

        #region Nested type: F4ImageResourceHeader

        protected internal class F4ImageResourceHeader : F4ResourceHeader
        {
            public ushort CenterX;
            public ushort CenterY;
            public uint Flags;
            public ushort Height;
            public uint ImageOffset;
            public uint PaletteOffset;
            public uint PaletteSize;
            public ushort Width;
        }

        #endregion

        #region Nested type: F4ResourceBundleIndex

        protected internal class F4ResourceBundleIndex
        {
            public uint NumResources;
            public F4ResourceRawDataPackage ResourceData;
            public F4ResourceHeader[] ResourceHeaders;
            public uint ResourceIndexVersion;
            public uint Size;
        }

        #endregion

        #region Nested type: F4ResourceFlags

        [Flags]
        protected internal enum F4ResourceFlags : uint
        {
            EightBit = 0x00000001,
            SixteenBit = 0x00000002,
            UseColorKey = 0x40000000,
        }

        #endregion

        #region Nested type: F4ResourceHeader

        protected internal class F4ResourceHeader
        {
            public string ID;
            public uint Type;
        }

        #endregion

        #region Nested type: F4ResourceRawDataPackage

        protected internal class F4ResourceRawDataPackage
        {
            public byte[] Data;
            public uint Size;
            public uint Version;
        }

        #endregion

        #region Nested type: F4SoundResourceHeader

        protected internal class F4SoundResourceHeader : F4ResourceHeader
        {
            public ushort Channels;
            public uint Flags;
            public uint HeaderSize;
            public uint Offset;
            public ushort SoundType;
        }

        #endregion
    }
}