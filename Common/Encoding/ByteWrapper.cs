using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Encoding
{
    /// <summary>
    /// Utility class for managing data in byte arrays.
    /// </summary>
    public class ByteWrapper
    {

        private static readonly byte[] ZERO_LENGTH_BUFFER = new byte[0];

        /// <summary>
        /// Offset for the start position in the buffer.
        /// </summary>
        private int _offset;


        /// <summary>
        /// The current postion (or index) in the buffer.
        /// </summary>
        private int _pos;


        /// <summary>
        /// The length of the buffer. 
        /// </summary>
        private int _limit;


        /// <summary>
        /// The backing byte array.
        /// </summary>
        private byte[] _buffer;
        public static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

        /// <summary>
        /// Construct a ByteWrapper backed by a zero-length byte array.
        /// </summary>
        public ByteWrapper()
            : this(ZERO_LENGTH_BUFFER)
        {
        }

        /// <summary>
        /// Construct a ByteWrapper backed by a byte array with the specified <code>length</code>.
        /// </summary>
        /// <param name='length'>
        /// length of the backing byte array
        /// </param>
        public ByteWrapper(int length)
            : this(new byte[length])
        {

        }

        /// <summary>
        /// Constructs a <code>ByteWrapper</code> backed by the specified byte array. (Changes to
        /// the ByteWrapper will write through to the specified byte array.)
        /// </summary>
        /// <param name='buffer'>
        /// backing byte array
        /// </param>
        public ByteWrapper(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {

        }

        /// <summary>
        /// Constructs a <code>ByteWrapper</code> backed by the specified byte array. (Changes to
        /// the ByteWrapper will write through to the specified byte array.)
        /// The <code>offset</code> will be at the start position. Limit will be at <code>buffer.length</code>.
        /// </summary>
        /// <param name='buffer'>
        /// backing byte array
        /// </param>
        /// <param name='offset'>
        /// start position offset
        /// </param>
        public ByteWrapper(byte[] buffer, int offset)
            : this(buffer, offset, buffer.Length - offset)
        {

        }

        /// <summary>
        /// Constructs a <code>ByteWrapper</code> backed by the specified byte array. (Changes to
        /// the ByteWrapper will write through to the specified byte array.)
        /// </summary>
        /// <param name='buffer'>
        /// backing byte array.
        /// </param>
        /// <param name='offset'>
        /// start position offset.
        /// </param>
        /// <param name='length'>
        /// length of the segment to use.
        /// </param>
        public ByteWrapper(byte[] buffer, int offset, int length)
        {
            setBuffer(buffer, offset, length);
        }

        /// <summary>
        /// Changes the backing store used by this ByteWrapper. Changes to the
        /// ByteWrapper will write through to the specified byte array.
        /// </summary>
        /// <param name='buffer'>
        /// backing byte array.
        /// </param>
        /// <param name='offset'>
        /// start position offset.
        /// </param>
        /// <param name='length'>
        /// length of the segment to use.
        /// </param>
        public void reassign(byte[] buffer, int offset, int length)
        {
            setBuffer(buffer, offset, length);
        }

        private void setBuffer(byte[] buffer, int offset, int length)
        {
            checkBounds(buffer, offset, length);
            _buffer = buffer;
            _offset = offset;
            _limit = _offset + length;
            _pos = _offset;
        }

        private void checkBounds(byte[] buffer, int offset, int length)
        {
            if (offset < 0)
            {
                throw new IndexOutOfRangeException("Negative offset: " + offset);
            }
            if (length < 0 || offset + length > buffer.Length)
            {
                throw new IndexOutOfRangeException("Offset + length (" + offset + " + " + length + ") past end of buffer: " + buffer.Length);
            }
        }

        /// <summary>
        /// Resets current position to the start of the ByteWrapper.
        /// </summary>
        public virtual void reset()
        {
            _pos = _offset;
        }


        /// <summary>
        /// Verify that <code>length</code> bytes can be read.
        /// </summary>
        /// <param name='length'>
        /// number of byte to verify.
        /// </param>
        /// <exception cref='IndexOutOfRangeException'>
        /// if <code>length</code> bytes can not be read.
        /// </exception>
        public virtual void verify(int length)
        {
            if (length < 0)
            {
                throw new IndexOutOfRangeException("length = " + length);
            }
            if (_pos + length > _limit)
            {
                throw new IndexOutOfRangeException("_pos + length = " + (_pos + length));
            }
        }

        /// <summary>
        /// Reads the next four byte of the ByteWrapper as a hi-endian 32-bit integer.
        /// The current position is increased by 4.
        /// </summary>
        /// <returns>
        /// decoded value
        /// </returns>
        /// <exception cref='IndexOutOfRangeException'>
        /// if <code>length</code> bytes can not be read.
        /// </exception
        public int getInt()
        {
            int rst;
            verify(4);
            int pos = _pos;
            byte[] buffer = _buffer;
            if (!BitConverter.IsLittleEndian)
            {
                rst =
         (((int)buffer[pos] & 0xFF) << 24) +
         (((int)buffer[pos + 1] & 0xFF) << 16) +
         (((int)buffer[pos + 2] & 0xFF) << 8) +
         ((int)buffer[pos + 3] & 0xFF);
            }
            else
            {
                rst =
         (((int)buffer[pos + 3] & 0xFF) << 24) +
         (((int)buffer[pos + 2] & 0xFF) << 16) +
         (((int)buffer[pos + 1] & 0xFF) << 8) +
         ((int)buffer[pos] & 0xFF);
            }
            _pos += 4;

            return rst;
        }

        /// <summary>
        /// Reads the next byte of the ByteWrapper. The current position is increased by 1.
        /// </summary>
        /// <returns>
        /// decoded value
        /// </returns>
        /// <exception cref='IndexOutOfRangeException'>
        /// if <code>length</code> bytes can not be read.
        /// </exception
        public int get()
        {
            verify(1);
            return (int)_buffer[_pos++] & 0xFF;
        }

        /// <summary>
        /// Reads <code>dest.length</code> bytes from the ByteWrapper into <code>dest</code>. The
        /// current position is increased by <code>dest.length</code>.
        /// </summary>
        /// <param name='dest'>
        /// destination for the read bytes
        /// </param>
        /// <exception cref='IndexOutOfRangeException'>
        /// if <code>length</code> bytes can not be read.
        /// </exception
        public void get(byte[] dest)
        {
            verify(dest.Length);
            Array.Copy(_buffer, _pos, dest, 0, dest.Length);
            _pos += dest.Length;
        }

        /**
    * Writes <code>value</code> to the ByteWrapper as a hi-endian 32-bit integer. The
    * current position is increased by 4.
    *
    * @param value value to write
    *
    * @throws IndexOutOfRangeException if the bytes can not be written
    *
    * @noinspection PointlessBitwiseExpression
    */
        public virtual void putInt(int value_)
        {
            verify(4);
            uint val = (uint)value_;
            if (!BitConverter.IsLittleEndian)
            {
                put((val >> 24) & 0xFF);
                put((val >> 16) & 0xFF);
                put((val >> 8) & 0xFF);
                put((val >> 0) & 0xFF);
            }
            else
            {
                put((val >> 0) & 0xFF);
                put((val >> 8) & 0xFF);
                put((val >> 16) & 0xFF);
                put((val >> 24) & 0xFF);
            }
        }

        /**
    * Writes <code>byte</code> to the ByteWrapper and advances the current position
    * by 1.
    *
    * @param b byte to write
    *
    * @throws IndexOutOfRangeException if the bytes can not be written
    */
        public virtual void put(uint b)
        {
            verify(1);
            _buffer[_pos++] = (byte)b;
        }

        /**
    * Writes a byte array to the ByteWrapper and advances the current
    * posisiton by the size of the byte array.
    *
    * @param src byte array to write
    *
    * @throws IndexOutOfRangeException if the bytes can not be written
    */
        public virtual void put(byte[] src)
        {
            verify(src.Length);
            Array.Copy(src, 0, _buffer, _pos, src.Length);
            _pos += src.Length;
        }


        /// <summary>
        /// Writes a subset of a byte array to the ByteWrapper and advances the current
        /// posisiton by the size of the subset.
        /// </summary>
        /// <param name='src'>
        /// byte array to write.
        /// </param>
        /// <param name='offset'>
        /// offset of subset to write.
        /// </param>
        /// <param name='count'>
        /// size of offset to write.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// if the bytes can not be written
        /// </exception>
        public virtual void put(byte[] src, int offset, int count)
        {
            verify(count);
            Array.Copy(src, offset, _buffer, _pos, count);
            _pos += count;
        }

        /// <summary>
        /// Returns the backing array.
        /// </summary>
        /// <returns>
        /// the backing byte array
        /// </returns>
        public byte[] array()
        {
            return _buffer;
        }

        /// <summary>
        /// Returns the current position.
        /// </summary>
        /// <returns>
        /// the current potition within the byte array
        /// </returns>
        public int getPos()
        {
            return _pos;
        }

        /// <summary>
        /// Returns the number of remaining bytes in the byte array.
        /// </summary>
        /// <returns>
        /// the number of remaining bytes in the byte array
        /// </returns>
        public int remaining()
        {
            return _limit - _pos;
        }

        /// <summary>
        /// Advances the current position by <code>n</code>.
        /// </summary>
        /// <param name='n'>
        /// number of positions to advance
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// if the position can not be advanced
        /// </exception>
        public void advance(int n)
        {
            verify(n);
            _pos += n;
        }


        /// <summary>
        /// Advances the current position until the specified <code>alignment</code> is
        /// achieved.
        /// </summary>
        /// <param name='alignment'>
        /// alignment that the current position must support
        /// </param>
        public void align(int alignment)
        {
            while (((_pos - _offset) % alignment) != 0)
            {
                advance(1);
            }
        }

        /// <summary>
        /// Slice this instance.
        /// </summary>
        /// <returns>
        /// a new <code>ByteWrapper</code> backed by the same byte array starting at the current position
        /// </returns>
        public ByteWrapper slice()
        {
            return new ByteWrapper(_buffer, _pos);
        }

        /// <summary>
        /// reates a <code>ByteWrapper</code> backed by the same byte array using the current
        /// position as its offset, and the specified <code>length</code> to mark the limit.
        /// </summary>
        /// <param name='length'>
        /// length of the new <code>ByteWrapper</code>
        /// </param>
        /// <returns>
        /// a new <code>ByteWrapper</code> backed by the same byte array starting at the current position
        /// with the defined <code>length</code>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// if the <code>length</code> is to long
        /// </exception>
        public ByteWrapper slice(int length)
        {
            verify(length);
            return new ByteWrapper(_buffer, _pos, length);
        }


        /// <summary>
        /// Returns a string representation of the ByteWrapper.
        /// </summary>
        /// <returns>
        /// a string representation of the ByteWrapper
        /// </returns>
        public override string ToString()
        {
            return "ByteWrapper{_offset=" + _offset + ", _pos=" + _pos + ", _limit=" + _limit + ", _buffer=" + _buffer + "}";
        }

        public static byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }
    }

}
