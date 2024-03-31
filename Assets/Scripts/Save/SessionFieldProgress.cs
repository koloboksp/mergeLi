using System;
using System.Collections.Generic;

namespace Save
{
    [Serializable]
    public class SessionFieldProgress
    {
        public List<SessionBallProgress> Balls = new List<SessionBallProgress>();
    }
}