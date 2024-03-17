using Core;
using Core.Steps.CustomOperations;
using UnityEngine;

public class AllOnZero : BallsCombinationAchievement
{
    protected override void InnerCheck()
    {
    }

    protected override void OnCollapse(CollapseOperationData data)
    {
        var passedCondition = true;

        var field = GameProcessor.GetField();

        foreach (var collapseLine in data.CollapseLines)
        {
            var lineDirection = GetLineDirection(collapseLine);
            switch (lineDirection)
            {
                case LineDirection.Horizontal:
                {
                    if (collapseLine.Count != field.Size.x)
                        passedCondition = false;
                    break;
                }
                case LineDirection.Vertical:
                {
                    if (collapseLine.Count != field.Size.y)
                        passedCondition = false;
                    break;
                }
                case LineDirection.Diagonal:
                {
                    if (collapseLine.Count != Mathf.Min(field.Size.x, field.Size.y))
                        passedCondition = false;
                    break;
                }
            }

            foreach (var ball in collapseLine)
            {
                if (ball.points != 0)
                {
                    passedCondition = false;
                    break;
                }
            }

            if (passedCondition)
            {
                Unlock();
            }
        }
    }
}