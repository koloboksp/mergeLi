namespace Atom
{
    public class Random
    {
        private static int mId = 1;

        private readonly System.Random mRand = new(++mId);
        public int Next() { return mRand.Next(); }

        private static readonly System.Random mRandom = new(0);
        public static int GetRandom()
        {
            return mRandom.Next();
        }
    }
}
