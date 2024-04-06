using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class UIHatsPanel_HatItem_FakeField : MonoBehaviour, IField
    {
        [SerializeField] private UIHatsPanel_HatItem_FakeScene _scene;
        [SerializeField] private Ball _ballPrefab;

        public IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class
        {
            throw new System.NotImplementedException();
        }

        public List<List<Ball>> CheckCollapse(Vector3Int checkingPosition)
        {
            throw new System.NotImplementedException();
        }

        public void DestroyBalls(List<Ball> ballsToRemove, bool force)
        {
            throw new System.NotImplementedException();
        }

        public List<(Vector3Int intPosition, int points)> GenerateBalls(int count, List<int> availableValues)
        {
            throw new System.NotImplementedException();
        }

        public Vector3Int CreateBall(Vector3Int position, int points)
        {
            var ball = Instantiate(_ballPrefab, transform);
            ball.transform.localScale = Vector3.one;
            ball.transform.position = Vector3.zero;
            ball.transform.rotation = Quaternion.identity;

            ball.SetData(this, Vector3.zero, points);
            var subComponents = ball.GetComponents<ISubComponent>();
            foreach (var subComponent in subComponents)
                subComponent.SetData();
            
            return position;
        }

        public Vector3 GetPositionFromGrid(Vector3 gridPosition)
        {
            return Vector3.zero;
        }

        public Vector3 GetPositionFromGrid(Vector3Int gridPosition)
        {
            throw new System.NotImplementedException();
        }

        public IFieldView View { get; }
        public void GenerateNextBallPositions(int count, List<int> availableValues)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new System.NotImplementedException();
        }

        public Vector2Int Size { get; }
        public Vector3 CellSize()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetWorldPosition(Vector3 gridPosition)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetWorldCellSize()
        {
            throw new System.NotImplementedException();
        }

        public int CalculateEmptySpacesCount()
        {
            throw new System.NotImplementedException();
        }

        public Vector3Int TransformToIntPosition(Vector3 gridPosition)
        {
            throw new System.NotImplementedException();
        }

        public List<Vector2Int> GetPath(Vector3Int from, Vector3Int to)
        {
            throw new System.NotImplementedException();
        }

        public void DestroyBall(Ball ball)
        {
            throw new System.NotImplementedException();
        }

        public IScene Scene => _scene;
    }
}