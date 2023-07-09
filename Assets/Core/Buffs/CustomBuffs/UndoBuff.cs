using UnityEngine;

public class UndoBuff : Buff
{
    private bool _hasUndoSteps = false;
    protected override void InnerOnClick()
    {
        _gameProcessor.Undo();
    }

    public override bool Available
    {
        get => base.Available && _hasUndoSteps;
        set => base.Available = value;
    }

    protected override void Inner_OnStepCompleted()
    {
        var hasUndoStepsNewState = _gameProcessor.HasUndoSteps();
        if (_hasUndoSteps != hasUndoStepsNewState)
        {
            _hasUndoSteps = hasUndoStepsNewState;
            _availableStateChanged?.Invoke();
        }
        base.Inner_OnStepCompleted();
    }
}