using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps
{
    public class StepMachine : MonoBehaviour
    {
        public Action<Step, StepExecutionType> OnStepCompleted;
        public Action<Step, StepExecutionType> OnStepExecute;

        private readonly List<(Step step, StepExecutionType executionType)> _steps = new ();
        private readonly List<Step> _undoSteps = new ();

        public StepMachine()
        {
       
        }

        public void AddStep(Step step)
        {
            _steps.Add((step, StepExecutionType.Redo));
        }
        
        public void AddUndoStep(Step step)
        {
            _undoSteps.Add(step);
        }

        public void Update()
        {
            if (_steps.Count > 0)
            {
                var step = _steps[0];
                OnStepExecute?.Invoke(step.step, step.executionType);
               
                if (!step.step.Launched)
                {
                    step.step.OnComplete += Step_OnCompleted;
                    step.step.Execute();
                }
            }
        }
    
        void Step_OnCompleted(Step sender)
        {
            var foundI = _steps.FindIndex(i => i.step == sender);
            if (foundI >= 0)
            {
                var step = _steps[foundI];
                step.step.OnComplete -= Step_OnCompleted;
                _steps.RemoveAt(foundI);
            
                OnStepCompleted?.Invoke(step.step, step.executionType);
            }   
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
    }
}