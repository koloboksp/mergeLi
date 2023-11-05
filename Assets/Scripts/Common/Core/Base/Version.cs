using System;

namespace Atom
{
    [Serializable]
    public struct Version : IEquatable<Version>
    {
        public static Version Empty = new();

#if UNITY_2019_1_OR_NEWER
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint mRelease;
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint mRevision;
#else
        private readonly uint _Release;
        private readonly uint _Revision;
#endif
        public Version(string version)
        {
            mRelease = 0;
            mRevision = 0;

            if (string.IsNullOrEmpty(version))
                return;

            var list = version.Split('.');

            if (!(list.Length == 2 || list.Length == 4))
                throw new Exception("The version must contain 2 or 4 blocks separated by a dot.");

            for (var i = 0; i != list.Length; i++)
                if (!Conversion.IsUInt32(list[i]))
                    throw new Exception($"In a expression: {version}, it is not possible to convert the sequence: {list[i]} to a number.");

            mRelease = Conversion.ToUInt32(list[list.Length == 2 ? 0 : 2]);
            mRevision = Conversion.ToUInt32(list[list.Length == 2 ? 1 : 3]);

            if (mRelease > 999)
                throw new Exception($"The maximum release value: 999.");
            if (mRevision > 999)
                throw new Exception($"The maximum revision: 999.");
        }

        internal Version(byte[] version, int offset = 0)
        {
            mRelease = Memory.UnpackingUInt(version, ref offset);
            mRevision = Memory.UnpackingUInt(version, ref offset);
        }

        public static bool operator ==(Version l, Version r)
        {
            if (l.mRelease != r.mRelease)
                return false;
            if (l.mRevision != r.mRevision)
                return false;

            return true;
        }

        public static bool operator !=(Version l, Version r)
        {
            return !(l == r);
        }

        public bool Equals(Version other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return $"{mRelease}.{mRevision}";
        }

        public override int GetHashCode()
        {
            return Conversion.ToInt(mRelease * 1000 + mRevision);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Version version))
                return false;

            return this == version;
        }

        public static bool operator >(Version version1, Version version2)
        {
            return version1.GetHashCode() > version2.GetHashCode();
        }

        public static bool operator <(Version version1, Version version2)
        {
            return version1.GetHashCode() < version2.GetHashCode();
        }

        public static bool operator >=(Version version1, Version version2)
        {
            return version1.GetHashCode() >= version2.GetHashCode();
        }

        public static bool operator <=(Version version1, Version version2)
        {
            return version1.GetHashCode() <= version2.GetHashCode();
        }

        internal byte[] ToByteArray()
        {
            var result = new byte[Memory.GetSizeOfPackedGuidEx()];
            var offset = 0;
            Memory.Packing(result, ref offset, mRelease);
            Memory.Packing(result, ref offset, mRevision);
            return result;
        }
    }
}

namespace Atom
{
    public static partial class Conversion
    {
        public static string ToString(Version version)
        {
            return version.ToString();
        }

        public static byte[] ToByteArray(Version version)
        {
            return version.ToByteArray();
        }

        public static Variant.Var ToVar(Version version)
        {
            return ToByteArray(version);
        }

        public static bool IsVersion(string sources)
        {
            var list = sources.Split('.');

            if (!(list.Length == 2 || list.Length == 4))
                return false;

            for (var b = 0; b != list.Length; b++)
            {
                var block = list[b];

                for (var i = 0; i != block.Length; i++)
                {
                    var ch = block[i];
                    if (!('0' <= ch && ch <= '9'))
                        return false;
                }
            }

            if (list.Length == 2)
            {
                if (list[1].Length > 3 || list[0].Length > 3)
                    return false;
            }
            else
            {
                if (list[3].Length > 3 || list[2].Length > 2 || list[1].Length > 2 || list[0].Length > 2)
                    return false;
            }

            return true;
        }
    }
}