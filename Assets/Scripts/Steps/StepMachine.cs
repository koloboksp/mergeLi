using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps
{
    public class StepMachine : MonoBehaviour
    {
        public event Action<Step, StepExecutionType> OnStepCompleted;
        public event Action<Step, StepExecutionType> OnBeforeStepStarted;
        public event Action OnUndoStepsClear;

        private readonly List<(Step step, StepExecutionType executionType)> _steps = new ();
        private readonly List<Step> _undoSteps = new ();
        private CancellationTokenSource _cancellationTokenSource;

        public void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async void Start()
        {
            await ProcessSteps(_cancellationTokenSource.Token);
        }

        private async Task ProcessSteps(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_steps.Count > 0)
                    {
                        var stepInfo = _steps[0];
                        _steps.RemoveAt(0);

                        OnBeforeStepStarted?.Invoke(stepInfo.step, stepInfo.executionType);
                        await stepInfo.step.ExecuteAsync(cancellationToken);
                        OnStepCompleted?.Invoke(stepInfo.step, stepInfo.executionType);
                    }
                    else
                    {
                        await Task.Yield();
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void AddStep(Step step)
        {
            _steps.Add((step, StepExecutionType.Redo));
        }
        
        public void AddUndoStep(Step step)
        {
            _undoSteps.Add(step);
        }

        public void Undo()
        {
            if (_undoSteps.Count > 0)
            {
                var undoStep = _undoSteps[_undoSteps.Count - 1];
                _undoSteps.Remove(undoStep);
                _steps.Add((undoStep, StepExecutionType.Undo));
            }
        }

        public bool HasUndoSteps()
        {
            return _undoSteps.Count > 0;
        }

        public void ClearUndoSteps()
        {
            _undoSteps.Clear();
            OnUndoStepsClear?.Invoke();
        }
    }
}