using System.Collections.Generic;
using Analytics;
using Core.Goals;

namespace Save
{
    public interface ISessionProgressHolder
    {
        ICastle GetCastle();
        IField GetField();
        IEnumerable<IBuff> GetBuffs();
        int GetScore();

        string GetFirstUncompletedCastle();

        ICommonAnalytics GetCommonAnalyticsAnalytics();
    }
}