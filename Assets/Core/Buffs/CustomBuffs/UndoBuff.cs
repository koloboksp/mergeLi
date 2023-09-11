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
        UpdateAvailability();
    }
    
    protected override void Inner_OnUndoStepsClear()
    {
        UpdateAvailability();
    }

    public override string Id => "Undo";

    protected virtual bool UndoAvailable => false;
    protected virtual StepTag UndoStepTag => StepTag.None;

    protected override bool InnerOnClick()
    {
        return _gameProcessor.HasUndoSteps();
    }

    protected override void InnerProcessUsing()
    {
        _gameProcessor.UseUndoBuff(Cost, this);
    }
    
    private void UpdateAvailability()
    {
        var hasUndoStepsNewState = _gameProcessor.HasUndoSteps();
        if (_hasUndoSteps != hasUndoStepsNewState)
        {
            _hasUndoSteps = hasUndoStepsNewState;
            _availableStateChanged?.Invoke();
        }
    }
}