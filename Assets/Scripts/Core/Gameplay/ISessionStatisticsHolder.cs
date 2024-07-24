namespace Core.Gameplay
{
    public interface ISessionStatisticsHolder
    {
        void ChangeMergeCount(int delta);
        void ChangeCollapseLinesCount(int delta);
    }
}