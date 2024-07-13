using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public interface IBall : IFieldMovable, IFieldSelectable, IFieldMergeable
    {
        public Vector3Int IntGridPosition { get; }
        public int Points { get; }
        public string HatName { get; }
    }

    public interface IField
    {
        IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class;
        List<List<Ball>> CheckCollapse(Vector3Int checkingPosition);
        void DestroyBalls(List<Ball> ballsToRemove, bool force);
        List<BallDesc> GenerateBalls(int count, int[] availableValues, string[] availableHats);
        Vector3Int CreateBall(Vector3Int position, int points, string hat);
        public Vector3 GetPositionFromGrid(Vector3 gridPosition);
        Vector3 GetPositionFromGrid(Vector3Int gridPosition);
        public IFieldView View { get; }
        void GenerateNextBallPositions(int count, int[] availableValues, string[] availableHats);
        IEnumerable<T> GetAll<T>();
        Vector2Int Size { get; }
        Vector3 CellSize();
        Vector3 GetWorldPosition(Vector3Int gridPosition);
        Vector3 GetWorldPosition(Vector3 gridPosition);
        Vector3 GetWorldCellSize();
        int CalculateEmptySpacesCount();
        bool HasMergeSteps(int[] availableValues);
        Vector3Int TransformToIntPosition(Vector3 gridPosition);
        List<Vector2Int> GetPath(Vector3Int from, Vector3Int to);
        void DestroyBall(Ball ball);
        IScene Scene { get; }
        IReadOnlyList<BallDesc> NextBallsData { get; }
        bool IsFullFilled { get; }
        bool IsEmpty { get; }
        Vector3 ScreenPointToWorld(Vector3 position);
        Vector3Int GetPointGridIntPosition(Vector3 position);
        void Clear();
        Ball PureCreateBall(Vector3Int gridPosition, int points, string hat);
        public List<BallDesc> AddBalls(IEnumerable<BallDesc> newBallsData);
        void UpdateSiblingIndex(Vector3 gridPosition, Transform target);
    }

    public interface IFieldView
    {
        Transform Root { get; }
        Vector2 RootSize { get; }
        void LockInput(bool state);
    }
}