using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class SessionProgress
    {
        public SessionFieldProgress Field;
        public SessionCastleProgress Castle;
        public List<SessionBuffProgress> Buffs = new();
        public int Score;
        public SessionAnalyticsProgress Analytics;
        
        public bool IsValid()
        {
            if (Castle == null || string.IsNullOrEmpty(Castle.Id))
                return false;

            return true;
        }
    }
}