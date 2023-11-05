using System;

namespace Atom
{
    [Serializable]
    public struct GuidEx : IEquatable<GuidEx>
    {

        public static GuidEx Empty = new();

#if UNITY_2019_1_OR_NEWER
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint m03;
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint m47;
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint m8B;
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint mCF;
#else
        private readonly uint m03;
        private readonly uint m47;
        private readonly uint m8B;
        private readonly uint mCF;
#endif
        public GuidEx(string guid)
        {
            if (!Conversion.IsGuidEx(guid))
                throw new Exception("Guid must be 32 characters long and only contain a-f, 0-9 character.");

            var byte0 = (Conversion.HexToValue(guid[00]) << 4) | Conversion.HexToValue(guid[01]);
            var byte1 = (Conversion.HexToValue(guid[02]) << 4) | Conversion.HexToValue(guid[03]);
            var byte2 = (Conversion.HexToValue(guid[04]) << 4) | Conversion.HexToValue(guid[05]);
            var byte3 = (Conversion.HexToValue(guid[06]) << 4) | Conversion.HexToValue(guid[07]);
            var byte4 = (Conversion.HexToValue(guid[08]) << 4) | Conversion.HexToValue(guid[09]);
            var byte5 = (Conversion.HexToValue(guid[10]) << 4) | Conversion.HexToValue(guid[11]);
            var byte6 = (Conversion.HexToValue(guid[12]) << 4) | Conversion.HexToValue(guid[13]);
            var byte7 = (Conversion.HexToValue(guid[14]) << 4) | Conversion.HexToValue(guid[15]);
            var byte8 = (Conversion.HexToValue(guid[16]) << 4) | Conversion.HexToValue(guid[17]);
            var byte9 = (Conversion.HexToValue(guid[18]) << 4) | Conversion.HexToValue(guid[19]);
            var byteA = (Conversion.HexToValue(guid[20]) << 4) | Conversion.HexToValue(guid[21]);
            var byteB = (Conversion.HexToValue(guid[22]) << 4) | Conversion.HexToValue(guid[23]);
            var byteC = (Conversion.HexToValue(guid[24]) << 4) | Conversion.HexToValue(guid[25]);
            var byteD = (Conversion.HexToValue(guid[26]) << 4) | Conversion.HexToValue(guid[27]);
            var byteE = (Conversion.HexToValue(guid[28]) << 4) | Conversion.HexToValue(guid[29]);
            var byteF = (Conversion.HexToValue(guid[30]) << 4) | Conversion.HexToValue(guid[31]);

            m03 = (byte0 << 24) | (byte1 << 16) | (byte2 << 8) | (byte3 << 0);
            m47 = (byte4 << 24) | (byte5 << 16) | (byte6 << 8) | (byte7 << 0);
            m8B = (byte8 << 24) | (byte9 << 16) | (byteA << 8) | (byteB << 0);
            mCF = (byteC << 24) | (byteD << 16) | (byteE << 8) | (byteF << 0);
        }

        internal GuidEx(byte[] guid, int offset = 0)
        {
            m03 = Memory.UnpackingUInt(guid, ref offset);
            m47 = Memory.UnpackingUInt(guid, ref offset);
            m8B = Memory.UnpackingUInt(guid, ref offset);
            mCF = Memory.UnpackingUInt(guid, ref offset);
        }

        public static bool operator ==(GuidEx l, GuidEx r)
        {
            return l.m03 == r.m03 && l.m47 == r.m47 && l.m8B == r.m8B && l.mCF == r.mCF;
        }

        public static bool operator !=(GuidEx l, GuidEx r)
        {
            return !(l == r);
        }

        public bool Equals(GuidEx other)
        {
            return this == other;
        }

        public override string ToString()
        {
            Span<char> cache = stackalloc char[32];

            cache[0] = Conversion.ValueToHex(((m03 >> 24) & 0xff) >> 4);
            cache[1] = Conversion.ValueToHex(((m03 >> 24) & 0xff) & 0x0f);
            cache[2] = Conversion.ValueToHex(((m03 >> 16) & 0xff) >> 4);
            cache[3] = Conversion.ValueToHex(((m03 >> 16) & 0xff) & 0x0f);
            cache[4] = Conversion.ValueToHex(((m03 >> 8) & 0xff) >> 4);
            cache[5] = Conversion.ValueToHex(((m03 >> 8) & 0xff) & 0x0f);
            cache[6] = Conversion.ValueToHex(((m03 >> 0) & 0xff) >> 4);
            cache[7] = Conversion.ValueToHex(((m03 >> 0) & 0xff) & 0x0f);

            cache[8] = Conversion.ValueToHex(((m47 >> 24) & 0xff) >> 4);
            cache[9] = Conversion.ValueToHex(((m47 >> 24) & 0xff) & 0x0f);
            cache[10] = Conversion.ValueToHex(((m47 >> 16) & 0xff) >> 4);
            cache[11] = Conversion.ValueToHex(((m47 >> 16) & 0xff) & 0x0f);
            cache[12] = Conversion.ValueToHex(((m47 >> 8) & 0xff) >> 4);
            cache[13] = Conversion.ValueToHex(((m47 >> 8) & 0xff) & 0x0f);
            cache[14] = Conversion.ValueToHex(((m47 >> 0) & 0xff) >> 4);
            cache[15] = Conversion.ValueToHex(((m47 >> 0) & 0xff) & 0x0f);

            cache[16] = Conversion.ValueToHex(((m8B >> 24) & 0xff) >> 4);
            cache[17] = Conversion.ValueToHex(((m8B >> 24) & 0xff) & 0x0f);
            cache[18] = Conversion.ValueToHex(((m8B >> 16) & 0xff) >> 4);
            cache[19] = Conversion.ValueToHex(((m8B >> 16) & 0xff) & 0x0f);
            cache[20] = Conversion.ValueToHex(((m8B >> 8) & 0xff) >> 4);
            cache[21] = Conversion.ValueToHex(((m8B >> 8) & 0xff) & 0x0f);
            cache[22] = Conversion.ValueToHex(((m8B >> 0) & 0xff) >> 4);
            cache[23] = Conversion.ValueToHex(((m8B >> 0) & 0xff) & 0x0f);
 
            cache[24] = Conversion.ValueToHex(((mCF >> 24) & 0xff) >> 4);
            cache[25] = Conversion.ValueToHex(((mCF >> 24) & 0xff) & 0x0f);
            cache[26] = Conversion.ValueToHex(((mCF >> 16) & 0xff) >> 4);
            cache[27] = Conversion.ValueToHex(((mCF >> 16) & 0xff) & 0x0f);
            cache[28] = Conversion.ValueToHex(((mCF >> 8) & 0xff) >> 4);
            cache[29] = Conversion.ValueToHex(((mCF >> 8) & 0xff) & 0x0f);
            cache[30] = Conversion.ValueToHex(((mCF >> 0) & 0xff) >> 4);
            cache[31] = Conversion.ValueToHex(((mCF >> 0) & 0xff) & 0x0f);

            return new string(cache);
        }

        public override int GetHashCode()
        {
            return m03.GetHashCode() + m47.GetHashCode() + m8B.GetHashCode() + mCF.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not GuidEx guidEx)
                return false;

            return this == guidEx;
        }

        public static GuidEx NewGuid()
        {
            return Conversion.ToGuidEx(Guid.NewGuid());
        }

        internal byte[] ToByteArray()
        {
            var result = new byte[Memory.GetSizeOfPackedGuidEx()];
            var offset = 0;
            Memory.Packing(result, ref offset, m03);
            Memory.Packing(result, ref offset, m47);
            Memory.Packing(result, ref offset, m8B);
            Memory.Packing(result, ref offset, mCF);
            return result;
        }
    }
}

namespace Atom
{
    public static partial class Conversion
    {
        public static Guid ToGuid(GuidEx guidEx)
        {
            return new Guid(guidEx.ToString());
        }

        public static GuidEx ToGuidEx(Guid guid)
        {
            return new GuidEx(guid.ToString("N"));
        }

        public static GuidEx ToGuidEx(byte[] guid)
        {
            return new GuidEx(guid);
        }

        public static string ToString(Guid guid)
        {
            return ToGuidEx(guid).ToString();
        }

        public static byte[] ToByteArray(GuidEx guidEx)
        {
            return guidEx.ToByteArray();
        }

        public static GuidEx ToGuidEx(Variant.Var root)
        {
            return new GuidEx((byte[])root);
        }

        public static Variant.Var ToVar(GuidEx guidEx)
        {
            return ToByteArray(guidEx);
        }

        public static bool IsGuidEx(string sources)
        {
            if (sources.Length != 32)
                return false;

            for (var i = 0; i != sources.Length; i++)
            {
                var ch = sources[i];
                if (!('0' <= ch && ch <= '9') && !('a' <= ch && ch <= 'f'))
                    return false;
            }
            return true;
        }

        private static readonly char[] mValueToHex =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        internal static char ValueToHex(uint value)
        {
            return mValueToHex[value];
        }

        private static readonly uint[] mHexToValue =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 10, 11, 12, 13, 14, 15, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue,
            uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue, 10, 11, 12, 13, 14, 15
        };

        internal static uint HexToValue(char value)
        {
            return mHexToValue[value - '0'];
        }
    }
}