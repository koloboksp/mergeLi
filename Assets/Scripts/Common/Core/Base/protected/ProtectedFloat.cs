using Atom.Variant;

namespace Atom.Protected
{
    //fixed memory issue
    //user can find value in memory and change it
    public struct ProtectedFloat
    {
        /*
        public ProtectedFloat()
        {
            _seed = Random.GetRandom() % 1000 + 999;
            _value = _seed;
        }
        */

        public ProtectedFloat(float value)
        {
            _seed = Random.GetRandom() % 1000 + 999;
            _value = _seed + value;
        }

        public static implicit operator float(ProtectedFloat value)
        {
            return value._value - value._seed;
        }

        public static implicit operator ProtectedFloat(float value)
        {
            return new ProtectedFloat(value);
        }

        public static implicit operator Var(ProtectedFloat value)
        {
            return new Var((int)value);
        }

        public override string ToString()
        {
            return Conversion.ToString(this);
        }

        private readonly float _seed;
        private readonly float _value;
    }
}
