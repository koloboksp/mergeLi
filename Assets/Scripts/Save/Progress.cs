using Atom.Protected;
using System;
using System.Collections.Generic;

namespace Save
{
    [Serializable]
    public class Progress
    {
        public List<string> CompletedCastles = new();
        public int BestSessionScore;
        public ProtectedInt Coins = new(0);
        public List<TutorialProgress> Tutorials = new();
        public List<GiftProgress> Gifts = new();
        public List<HatProgress> Hats = new();
        public int BestSessionCollapsedLines;
        public int BestSessionMergedBalls;
    }
}