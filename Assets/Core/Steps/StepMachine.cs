using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps
{
    public class StepMachine : MonoBehaviour
    {
        public Action<Step> OnStepCompleted;
        public Action<Step> OnStepExecute;

        private List<Step> _steps = new List<Step>();
        private List<Step> _undoSteps = new List<Step>();

        public StepMachine()
        {
       
        }

        public void AddStep(Step step)
        {
            _steps.Add(step);
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
                OnStepExecute?.Invoke(step);
               
                if (!step.Launched)
                {
                    step.OnComplete += Step_OnCompleted;
                    step.Execute();
                }
            }
        }
    
        void Step_OnCompleted(Step sender)
        {
            var foundI = _steps.IndexOf(sender);
            if (foundI >= 0)
            {
                var step = _steps[foundI];
                step.OnComplete -= Step_OnCompleted;
                _steps.RemoveAt(foundI);
            
                OnStepCompleted?.Invoke(step);
            }   
        }

        public void Undo()
        {
            if (_undoSteps.Count > 0)
            {
                var undoStep = _undoSteps[_undoSteps.Count - 1];
                _undoSteps.Remove(undoStep);
                _steps.Add(undoStep);
            }
        }

        public bool HasUndoSteps()
        {
            return _undoSteps.Count > 0;
        }
    }
}