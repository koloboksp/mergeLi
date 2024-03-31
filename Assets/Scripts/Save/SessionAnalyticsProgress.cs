using System;
using System.Collections.Generic;

namespace Save
{
    [Serializable]
    public class SessionAnalyticsProgress
    {
        public int Step;
        public List<StepTakenInto> StepsTakenInto;
    }
}