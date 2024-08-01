using Atom.Variant;
using System;

namespace Atom.Protected
{
    //fixed memory issue
    //user can find value in memory and change it
    [Serializable]
    public struct ProtectedInt
    {
        public static GuidEx Empty = new();

#if UNITY_2019_1_OR_NEWER
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private int _seed;
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private int _value;
#else
        private readonly int _seed;
        private readonly int _value;
#endif
        /*
        public ProtectedInt()
        {
            _seed = Random.GetRandom() % 1000 + 999;
            _value = _seed;
        }
        */

        public ProtectedInt(int value)
        {
            _seed = Random.GetRandom() % 1000 + 999;
            _value = _seed + value;
        }

        public static implicit operator int(ProtectedInt value)
        {
            return value._value - value._seed;
        }

        public static implicit operator ProtectedInt(int value)
        {
            return new ProtectedInt(value);
        }

        public static implicit operator Var(ProtectedInt value)
        {
            return new Var((int)value);
        }

        public override string ToString()
        {
            return Conversion.ToString(this);
        }
    }
}
