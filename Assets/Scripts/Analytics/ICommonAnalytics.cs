using System.Collections;
using System.Collections.Generic;
using Save;

namespace Analytics
{
    public interface ICommonAnalytics
    {
        int GetStep();
        IEnumerable<StepTakeIntoInfo> GetStepsTakenIntoInfos();
        void SetProgress(SessionAnalyticsProgress analytics);
    }
}