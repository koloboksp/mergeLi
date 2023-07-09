using Core.Steps;
using UnityEngine;

public class UndoBuff : Buff
{
    private bool _hasUndoSteps = false;
    protected override void InnerOnClick()
    {
        _gameProcessor.UseUndoBuff(Cost);
    }

    public override bool Available
    {
        get => base.Available && _hasUndoSteps;
        set => base.Available = value;
    }

    protected override void Inner_OnStepCompleted(Step step)
    {
        if (step.Tag == GameProcessor.MoveStepTag
            || step.Tag == GameProcessor.MergeStepTag
            || step.Tag == GameProcessor.UndoMoveStepTag
            || step.Tag == GameProcessor.UndoMergeStepTag)
        {
            var hasUndoStepsNewState = _gameProcessor.HasUndoSteps();
            if (_hasUndoSteps != hasUndoStepsNewState)
            {
                _hasUndoSteps = hasUndoStepsNewState;
                _availableStateChanged?.Invoke();
            }
        }
    }
}