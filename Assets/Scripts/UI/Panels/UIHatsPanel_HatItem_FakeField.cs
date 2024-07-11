using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class UIHatsPanel_HatItem_FakeField : MonoBehaviour, IField
    {
        [SerializeField] private UIHatsPanel_HatItem_FakeScene _scene;
        [SerializeField] private Ball _ballPrefab;

        private Vector3 _ballPosition = Vector3.zero;
        private Ball _ball;
        
        public IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class
        {
            return new List<T>() { _ball as T };
        }

        public List<List<Ball>> CheckCollapse(Vector3Int checkingPosition)
        {
            throw new System.NotImplementedException();
        }

        public void DestroyBalls(List<Ball> ballsToRemove, bool force)
        {
            throw new System.NotImplementedException();
        }

        public List<BallDesc> GenerateBalls(int count, int[] availableValues, string[] availableHats)
        {
            throw new System.NotImplementedException();
        }

        public Vector3Int CreateBall(Vector3Int position, int points, string hat)
        {
            _ball = Instantiate(_ballPrefab, transform);
            _ball.transform.localScale = Vector3.one;
            _ball.transform.position = _ballPosition;
            _ball.transform.rotation = Quaternion.identity;

            _ball.SetData(this, _ballPosition, points, hat);
            var subComponents = _ball.GetComponents<ISubComponent>();
            foreach (var subComponent in subComponents)
                subComponent.SetData();

            _ball.View.ShowHat(true);
            
            return position;
        }

        public Vector3 GetPositionFromGrid(Vector3 gridPosition)
        {
            return _ballPosition;
        }

        public Vector3 GetPositionFromGrid(Vector3Int gridPosition)
        {
            throw new System.NotImplementedException();
        }

        public IFieldView View { get; }
        public void GenerateNextBallPositions(int count, int[] availableValues, string[] availableHats)
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

        public bool HasMergeSteps(int[] availableValues)
        {
            throw new System.NotImplementedException();
        }

        public Vector3Int TransformToIntPosition(Vector3 gridPosition)
        {
            return new Vector3Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y), 0);
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
        public IReadOnlyList<BallDesc> NextBallsData { get; }
        public bool IsFullFilled { get; }
        public bool IsEmpty { get; }

        public Vector3 ScreenPointToWorld(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public Vector3Int GetPointGridIntPosition(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public Ball PureCreateBall(Vector3Int gridPosition, int points, string hat)
        {
            throw new System.NotImplementedException();
        }

        public List<BallDesc> AddBalls(IEnumerable<BallDesc> newBallsData)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateSiblingIndex(Vector3 gridPosition, Transform target)
        {
            throw new System.NotImplementedException();
        }
    }
}