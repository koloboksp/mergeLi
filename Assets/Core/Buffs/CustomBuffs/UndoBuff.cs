using Core.Steps;
using UnityEngine;

public class UndoBuff : Buff
{
    private bool _hasUndoSteps = false;
    
    public override bool Available
    {
        get => base.Available && _hasUndoSteps;
        set => base.Available = value;
    }

    protected override void Inner_OnStepCompleted(Step step)
    {
        //if ()
        {
            var hasUndoStepsNewState = _gameProcessor.HasUndoSteps();
            if (_hasUndoSteps != hasUndoStepsNewState)
            {
                _hasUndoSteps = hasUndoStepsNewState;
                _availableStateChanged?.Invoke();
            }
        }
    }

    protected override bool UndoAvailable => false;
    protected override StepTag UndoStepTag => StepTag.None;

    protected override bool InnerProcessUsing()
    {
        return _gameProcessor.UseUndoBuff(Cost);
    }
}