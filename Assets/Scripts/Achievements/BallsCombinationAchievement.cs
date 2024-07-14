using System.Collections.Generic;
using Core;
using Core.Gameplay;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Achievements
{
    public class BallsCombinationAchievement : Achievement
    {
        [SerializeField] private BallsMask[] _masks;
    
        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (stepExecutionType != StepExecutionType.Redo) 
                return;
        
            var collapseOperationData = step.GetData<CollapseOperationData>();
            if (collapseOperationData == null || collapseOperationData.CollapseLines.Count <= 0) 
                return;
        
            var source = ConvertToArray(collapseOperationData.CollapseLines);
            if (source == null)
                return;
        
            var matchFound = false;
            for (int maskI = 0; maskI < _masks.Length; maskI++)
            {
                var mask = ConvertToArray(_masks[maskI].Balls);
                if(mask == null)
                    continue;
            
                var maskMatchFound = FindMaskMatch(source, mask);
                if (maskMatchFound)
                {
                    matchFound = true;
                    break;
                }
            }
        
            if(matchFound)
            {
                Unlock();
            }
        }

        private static bool FindMaskMatch(int[,] source, int[,] mask)
        {
            var searchArea = new Vector2Int(
                source.GetLength(0) - mask.GetLength(0),
                source.GetLength(1) - mask.GetLength(1));
        
            if (searchArea.x < 0 || searchArea.y < 0)
                return false;

            for (var xOffset = 0; xOffset <= searchArea.x; xOffset++)
            for (var yOffset = 0; yOffset <= searchArea.y; yOffset++)
            {
                var checkFail = false;
                for (var maskX = 0; maskX < mask.GetLength(0) && !checkFail; maskX++)
                for (var maskY = 0; maskY < mask.GetLength(1) && !checkFail; maskY++)
                {
                    var maskValue = mask[maskX, maskY];
                    var sourceValue = source[xOffset + maskX, yOffset + maskY];

                    if (maskValue != sourceValue)
                    {
                        if (maskValue == -1 && sourceValue != int.MinValue)
                        {
                        }
                        else
                        {
                            checkFail = true;
                        }
                    }
                }
            
                if (!checkFail)
                {
                    return true;
                }
            }
        
            return false;
        }

        private int[,] ConvertToArray(IReadOnlyList<BallDesc> balls)
        {
            if (balls.Count == 0)
                return null;
        
            var minBallGridPosition = new Vector3Int(int.MaxValue, int.MaxValue, 0);
            var maxBallGridPosition = new Vector3Int(int.MinValue, int.MinValue, 0);
            for (var ballI = 0; ballI < balls.Count; ballI++)
            {
                var ballDesc = balls[ballI];
                minBallGridPosition = Vector3Int.Min(minBallGridPosition, ballDesc.GridPosition);
                maxBallGridPosition = Vector3Int.Max(maxBallGridPosition, ballDesc.GridPosition);
            }

            var ballGridSize = (Vector2Int)maxBallGridPosition - (Vector2Int)minBallGridPosition;
        
            var mask = new int[ballGridSize.x + 1, ballGridSize.y + 1];
            for (var x = 0; x < mask.GetLength(0); x++)
            for (var y = 0; y < mask.GetLength(1); y++)
                mask[x, y] = int.MinValue;

            for (var ballI = 0; ballI < balls.Count; ballI++)
            {
                var ballDesc = balls[ballI];
                var ballGridPosition = ballDesc.GridPosition - minBallGridPosition;
                mask[ballGridPosition.x, ballGridPosition.y] = ballDesc.Points;
            }
        
            return mask;
        }
    
        private int[,] ConvertToArray(IReadOnlyList<IReadOnlyList<BallDesc>> ballsGroups)
        {
            if (ballsGroups.Count == 0)
                return null;

            var allBallsGroupsEmpty = true;
            for (var ballsGroupI = 0; ballsGroupI < ballsGroups.Count; ballsGroupI++)
            {
                if (ballsGroups[ballsGroupI].Count != 0)
                {
                    allBallsGroupsEmpty = false;
                    break;
                }
            }

            if (allBallsGroupsEmpty)
                return null;
        
            var minBallGridPosition = new Vector3Int(int.MaxValue, int.MaxValue, 0);
            var maxBallGridPosition = new Vector3Int(int.MinValue, int.MinValue, 0);
            for (var ballsGroupI = 0; ballsGroupI < ballsGroups.Count; ballsGroupI++)
            {
                var ballsGroup = ballsGroups[ballsGroupI];
                for (var ballI = 0; ballI < ballsGroup.Count; ballI++)
                {
                    var ballDesc = ballsGroup[ballI];
                    minBallGridPosition = Vector3Int.Min(minBallGridPosition, ballDesc.GridPosition);
                    maxBallGridPosition = Vector3Int.Max(maxBallGridPosition, ballDesc.GridPosition);
                }
            }

            var ballGridSize = (Vector2Int)maxBallGridPosition - (Vector2Int)minBallGridPosition;
        
            var mask = new int[ballGridSize.x + 1, ballGridSize.y + 1];
            for (var x = 0; x < mask.GetLength(0); x++)
            for (var y = 0; y < mask.GetLength(1); y++)
                mask[x, y] = int.MinValue;

            for (var ballsGroupI = 0; ballsGroupI < ballsGroups.Count; ballsGroupI++)
            {
                var ballsGroup = ballsGroups[ballsGroupI];
                for (var ballI = 0; ballI < ballsGroup.Count; ballI++)
                {
                    var ballDesc = ballsGroup[ballI];
                    var ballGridPosition = ballDesc.GridPosition - minBallGridPosition;
                    mask[ballGridPosition.x, ballGridPosition.y] = ballDesc.Points;
                }
            }
        
            return mask;
        }
    
        public enum LineDirection
        {
            Vertical,
            Horizontal,
            Diagonal,
            Undefined
        }

        public static LineDirection GetLineDirection(List<(Vector3Int gridPosition, int points)> line)
        {
            var h = new List<int>();
            var v = new List<int>();

            foreach (var ball in line)
            {
                if (!h.Contains(ball.gridPosition.x))
                    h.Add(ball.gridPosition.x);
                if (!v.Contains(ball.gridPosition.y))
                    v.Add(ball.gridPosition.y);
            }

            h.Sort();
            v.Sort();

            if (h.Count == 1 && v.Count == line.Count)
            {
                var lineInterrupted = false;
                for (var i = 0; i < v.Count - 1; i++)
                    if (v[i] + 1 != v[i + 1])
                    {
                        lineInterrupted = true;
                        break;
                    }

                if (!lineInterrupted)
                    return LineDirection.Vertical;
            }

            if (v.Count == 1 && h.Count == line.Count)
            {
                var lineInterrupted = false;
                for (var i = 0; i < h.Count - 1; i++)
                    if (h[i] + 1 != h[i + 1])
                    {
                        lineInterrupted = true;
                        break;
                    }

                if (!lineInterrupted)
                    return LineDirection.Horizontal;
            }

            if (v.Count == line.Count && h.Count == line.Count)
            {
                var lineInterrupted = false;
                for (var i = 0; i < v.Count - 1; i++)
                    if (v[i] + 1 != v[i + 1])
                    {
                        lineInterrupted = true;
                        break;
                    }

                for (var i = 0; i < h.Count - 1; i++)
                    if (h[i] + 1 != h[i + 1])
                    {
                        lineInterrupted = true;
                        break;
                    }

                if (!lineInterrupted)
                    return LineDirection.Diagonal;
            }

            return LineDirection.Undefined;
        }
    }
}