using System.Collections.Generic;
using Analytics;
using Core;
using Core.Gameplay;
using Core.Goals;

namespace Save
{
    public interface ISessionProgressHolder
    {
        IReadOnlyList<ICastle> GetCompletedCastles();
        ICastle GetActiveCastle();
        IField GetField();
        IEnumerable<IBuff> GetBuffs();
        
        string GetFirstUncompletedCastleName();

        ICommonAnalytics GetCommonAnalyticsAnalytics();
    }
}