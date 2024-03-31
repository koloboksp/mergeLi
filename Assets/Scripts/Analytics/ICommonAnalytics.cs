using System.Collections;
using System.Collections.Generic;

namespace Analytics
{
    public interface ICommonAnalytics
    {
        int GetStep();
        IEnumerable<StepTakeIntoInfo> GetStepsTakenIntoInfos();
    }
}