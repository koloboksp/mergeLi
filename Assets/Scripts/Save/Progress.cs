using System;
using System.Collections.Generic;

namespace Save
{
    [Serializable]
    public class Progress
    {
        public List<string> CompletedCastles = new List<string>();
        public int BestSessionScore;
        public int Coins;
        public List<TutorialProgress> Tutorials = new();
        public List<GiftProgress> Gifts = new();
    }
}