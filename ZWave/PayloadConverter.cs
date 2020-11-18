using System;
using System.Linq;

namespace ZWave
{
    public static class PayloadConverter
    {
        public static byte ToUInt8(byte[] value, int startIndex = 0)
        {
            return value.Skip(startIndex).First();
        }

        public static sbyte ToInt8(byte[] value, int startIndex = 0)
        {
            return unchecked((sbyte) value.Skip(startIndex).First());
        }

        public static ushort ToUInt16(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(value.Skip(startIndex).Take(sizeof(ushort)).Reverse().ToArray(), 0);
            return BitConverter.ToUInt16(value, startIndex);
        }

        public static short ToInt16(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(value.Skip(startIndex).Take(sizeof(short)).Reverse().ToArray(), 0);
            return BitConverter.ToInt16(value, startIndex);
        }

        public static uint ToUInt32(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt32(value.Skip(startIndex).Take(sizeof(uint)).Reverse().ToArray(), 0);
            return BitConverter.ToUInt32(value, startIndex);
        }

        public static int ToInt32(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(value.Skip(startIndex).Take(sizeof(int)).Reverse().ToArray(), 0);
            return BitConverter.ToInt32(value, startIndex);
        }

        public static ulong ToUInt64(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt64(value.Skip(startIndex).Take(sizeof(ulong)).Reverse().ToArray(), 0);
            return BitConverter.ToUInt64(value, startIndex);
        }

        public static long ToInt64(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt64(value.Skip(startIndex).Take(sizeof(long)).Reverse().ToArray(), 0);
            return BitConverter.ToInt64(value, startIndex);
        }


        public static byte[] GetBytes(sbyte value)
        {
            return new[] {unchecked((byte) value)};
        }

        public static byte[] GetBytes(byte value)
        {
            return new[] {value};
        }

        public static byte[] GetBytes(short value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(ushort value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(uint value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(long value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(ulong value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        public static float ToFloat(byte[] payload, out byte scale)
        {
            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            var precision = (byte) ((payload[0] & 0xE0) >> 5);
            scale = (byte) ((payload[0] & 0x18) >> 3);
            var size = (byte) (payload[0] & 0x07);

            switch (size)
            {
                case sizeof(sbyte):
                {
                    var value = (sbyte) payload[1];
                    return (float) (value / Math.Pow(10, precision));
                }
                case sizeof(short):
                {
                    var value = ToInt16(payload, 1);
                    return (float) (value / Math.Pow(10, precision));
                }
                case sizeof(int):
                {
                    var value = ToInt32(payload, 1);
                    return (float) (value / Math.Pow(10, precision));
                }
            }

            return 0;
        }

        public static bool IsBitSet(byte record, byte bitIndex)
        {
            if (bitIndex > 7)
                throw new ArgumentOutOfRangeException($"Invalid bit index {bitIndex} for byte mask");

            return (record & (1 << bitIndex)) > 0;
        }
    }
}
