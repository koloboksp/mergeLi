using System;
using System.Runtime.InteropServices;

namespace Atom
{
    //*********************************************************************************************
    [Serializable]
    public enum ParamType
    {   
        Bool = 0,
        Int = 1,
        Float = 2
    }

    [Serializable]
    public struct Param
    {
#if UNITY_2019_1_OR_NEWER
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint m03;

        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private uint m47;
#else
        private uint m03;
        private uint m47;
#endif
        [StructLayout(LayoutKind.Explicit)]
        private struct IntUnion
        {
            [FieldOffset(0)]
            public int Value;
            [FieldOffset(0)]
            public uint Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatUnion
        {
            [FieldOffset(0)]
            public float Value;
            [FieldOffset(0)]
            public uint Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct BoolUnion
        {
            [FieldOffset(0)]
            public bool Value;
            [FieldOffset(0)]
            public uint Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ParamTypeUnion
        {
            [FieldOffset(0)]
            public ParamType Value;
            [FieldOffset(0)]
            public uint Data;
        }

        //-----------------------------------------------------------------------------------------
        public Param(string value)
        {
            var param = Conversion.ToParam(value);
            m03 = param.m03;
            m47 = param.m47;
        }
        //-----------------------------------------------------------------------------------------
        public Param(bool b)
        {
            m03 = new BoolUnion { Value = b }.Data;
            m47 = new ParamTypeUnion { Value = ParamType.Bool }.Data;
        }
        //-----------------------------------------------------------------------------------------
        public Param(int i)
        {
            m03 = new IntUnion { Value = i }.Data;
            m47 = new ParamTypeUnion { Value = ParamType.Int }.Data;
        }
        //-----------------------------------------------------------------------------------------
        public Param(float f)
        {
            m03 = new FloatUnion { Value = f }.Data;
            m47 = new ParamTypeUnion { Value = ParamType.Float }.Data;
        }
        //-----------------------------------------------------------------------------------------
        internal static void Packing(byte[] buffer, int offset, Param value)
        {
            Packing(buffer, ref offset, value);
        }

        public static void Packing(byte[] buffer, ref int offset, Param value)
        {
            Memory.Packing(buffer, ref offset, value.m03);
            Memory.Packing(buffer, ref offset, value.m47);
        }
        //-----------------------------------------------------------------------------------------
        public static Param UnpackingParam(byte[] buffer, int offset)
        {
            return UnpackingParam(buffer, ref offset);
        }

        public static Param UnpackingParam(byte[] buffer, ref int offset)
        {
            var value03 = Memory.UnpackingUInt(buffer, ref offset);
            var type47 = Memory.UnpackingUInt(buffer, ref offset);
            return new Param(value03, type47);
        }
        //-----------------------------------------------------------------------------------------
        private Param(uint value03, uint type47)
        {
            m03 = value03;
            m47 = type47;
        }
        //-----------------------------------------------------------------------------------------
        private static bool GetBool(Param p) { return new BoolUnion { Data = p.m03 }.Value; }
        private static int GetInt(Param p) { return new IntUnion { Data = p.m03 }.Value; }
        private static float GetFloat(Param p) { return new FloatUnion { Data = p.m03 }.Value; }
        private static ParamType GetType(Param p) { return new ParamTypeUnion { Data = p.m47 }.Value; }
        //-----------------------------------------------------------------------------------------
        public static implicit operator bool(Param p)
        {
            if (p.Type != ParamType.Bool)
                throw new Exception($"impossible cast: {p.Type} to {ParamType.Bool}");

            return GetBool(p);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator int(Param p)
        {
            if (p.Type != ParamType.Int)
                throw new Exception($"impossible cast: {p.Type} to {ParamType.Int}");

            return GetInt(p); ;
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator float(Param p)
        {
            if (p.Type != ParamType.Float)
                throw new Exception($"impossible cast: {p.Type} to {ParamType.Float}");

            return GetFloat(p);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator Param(int p) => new Param(p);
        //-----------------------------------------------------------------------------------------
        public static implicit operator Param(bool p) => new Param(p);
        //-----------------------------------------------------------------------------------------
        public static implicit operator Param(float p) => new Param(p);
        //-----------------------------------------------------------------------------------------
        public override string ToString()
        {
            return Type switch
            {
                ParamType.Bool => Conversion.ToString(GetBool(this)),
                ParamType.Int => Conversion.ToString(GetInt(this)),
                ParamType.Float => Conversion.ToString(GetFloat(this)),
                _ => throw new Exception()
            };
        }
        //-----------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Param param))
                return false;

            return this == param;
        }
        //-----------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return (int)(m03 * m47);
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator !(Param param)
        {
            if (param.Type == ParamType.Bool)
                return !(bool)param;

            throw new Exception($"cannot invert operation (!): {param}");
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator ==(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Bool => GetBool(left) == GetBool(right),
                ParamType.Int => GetInt(left) == GetInt(right),
                ParamType.Float => Math.CompareFloat(GetFloat(left), right),
                _ => throw new Exception()
            };
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator !=(Param left, Param right)
        {
            return !(left == right);
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator >=(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) >= GetInt(right),
                ParamType.Float => GetFloat(left) >= GetFloat(right),
                _ => false
            };
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator <=(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) <= GetInt(right),
                ParamType.Float => GetFloat(left) <= GetFloat(right),
                _ => false
            };
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator >(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) > GetInt(right),
                ParamType.Float => GetFloat(left) > GetFloat(right),
                _ => false
            };
        }
        //-----------------------------------------------------------------------------------------
        public static bool operator <(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) < GetInt(right),
                ParamType.Float => GetFloat(left) < GetFloat(right),
                _ => false
            };
        }
        //-----------------------------------------------------------------------------------------
        public static Param operator -(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) - GetInt(right),
                ParamType.Float => GetFloat(left) - GetFloat(right),
                _ => throw new Exception($"impossible to perform a mathematical operation (-), left: {left} right: {right}")
            };
        }
        //-----------------------------------------------------------------------------------------
        public static Param operator +(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) + GetInt(right),
                ParamType.Float => GetFloat(left) + GetFloat(right),
                _ => throw new Exception($"impossible to perform a mathematical operation (+): {left} right: {right}")
            };
        }
        //-----------------------------------------------------------------------------------------
        public static Param operator *(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) * GetInt(right),
                ParamType.Float => GetFloat(left) * GetFloat(right),
                _ => throw new Exception($"impossible to perform a mathematical operation (*): {left} right: {right}")
            };
        }
        //-----------------------------------------------------------------------------------------
        public static Param operator /(Param left, Param right)
        {
            if (left.Type != right.Type)
                throw new Exception($"variable types must be the same, left: {left} right: {right}");

            return left.Type switch
            {
                ParamType.Int => GetInt(left) / GetInt(right),
                ParamType.Float => GetFloat(left) / GetFloat(right),
                _ => throw new Exception($"impossible to perform a mathematical operation (/): {left} right: {right}")
            };
        }
        //-----------------------------------------------------------------------------------------
        public ParamType Type => GetType(this);  
    }
    //*********************************************************************************************
}

namespace Atom
{
    public static partial class Conversion
    {
        public static Param ToParam(string value)
        {
            var param = IsParam(value);

            if (param == null)
                throw new Exception("cannot convert string: " + value + " to parameter");

            return param.Value;
        }

        public static Param? IsParam(string value)
        {
            return IsParam(value, 0, value.Length);
        }

        public static Param? IsParam(ParamType type, string value)
        {
            if (type == ParamType.Int)
            {
                if (FastCheckInt(value, 0, value.Length))
                    return ToInt(value.Substring(0, value.Length));
            }
            else if (type == ParamType.Float)
            {
                if (FastCheckFloat(value, 0, value.Length))
                    return ToFloat(value.Substring(0, value.Length));
            }
            else if (type == ParamType.Bool)
            {
                if (FastCheckBool(value, 0, value.Length))
                    return ToBool(value.Substring(0, value.Length));
            }

            return null;
        }

        public static Param? IsParam(string value, int begin, int end)
        {
            if (string.IsNullOrWhiteSpace(value) || end - begin == 0)
                return null;
            if (FastCheckInt(value, begin, end))
                return ToInt(value.Substring(begin, end - begin));
            if (FastCheckFloat(value, begin, end))
                return ToFloat(value.Substring(begin, end - begin));
            if (FastCheckBool(value, begin, end))
                return ToBool(value.Substring(begin, end - begin));

            return null;
        }
    }
}