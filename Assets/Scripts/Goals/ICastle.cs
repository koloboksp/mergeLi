namespace Core.Goals
{
    public interface ICastle
    {
        int GetPoints();
        string Id { get; }
        bool Completed { get; }
    }
}