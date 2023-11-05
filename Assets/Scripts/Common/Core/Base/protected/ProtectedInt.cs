using Atom.Variant;

namespace Atom.Protected
{
    //fixed memory issue
    //user can find value in memory and change it
    public struct ProtectedInt
    {
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

        private readonly int _seed;
        private readonly int _value;
    }
}
