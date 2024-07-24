using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class SessionProgress
    {
        public SessionFieldProgress Field;
        public SessionCastleProgress ActiveCastle;
        public List<SessionCastleProgress> CompletedCastles = new();
        public List<SessionBuffProgress> Buffs = new();
        public SessionAnalyticsProgress Analytics;
        public int CollapseLinesCount;
        public int MergedBallsCount;

        public bool IsValid()
        {
            if (ActiveCastle == null || !ActiveCastle.IsValid)
                return false;

            return true;
        }
    }
}