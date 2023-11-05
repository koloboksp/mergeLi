using System;
using System.Collections.Generic;

namespace Atom.Variant
{
    public enum VarType
    {
        Null = 0,
        Bool = 1,
        Int = 3,
        Float = 4,
        String = 5,
        ByteArray = 6,
        Type = 7, //VarType
        List = 8,
        Tree = 9,
        Object = 10
    }
}

namespace Atom
{
    using Variant;

    public static partial class Conversion
    {
        private static readonly Dictionary<string, VarType> mVarTypeCash = new Dictionary<string, VarType>
        {
            {nameof(VarType.Null), VarType.Null},
            {nameof(VarType.Bool), VarType.Bool},
            {nameof(VarType.Int), VarType.Int},
            {nameof(VarType.Float), VarType.Float},
            {nameof(VarType.String), VarType.String},
            {nameof(VarType.ByteArray), VarType.ByteArray},
            {nameof(VarType.Type), VarType.Type},
            {nameof(VarType.List), VarType.List},
            {nameof(VarType.Tree), VarType.Tree},
            {nameof(VarType.Object), VarType.Object}
        };

        public static VarType ToVarType(string value)
        {
            if (!mVarTypeCash.TryGetValue(value, out var result))
                throw new Exception("cannot convert: " + value + "to type: VarType");

            return result;
        }

        public static bool IsVarType(string value)
        {
            return mVarTypeCash.ContainsKey(value);
        }

        public static string ToString(VarType value)
        {
            return value.ToString();
        }
    }
}
