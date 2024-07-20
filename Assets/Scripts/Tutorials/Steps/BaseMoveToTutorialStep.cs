using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public abstract class BaseMoveToTutorialStep : TutorialStep, IFocusedOnSomething
    {
        private Rect _focusedRect;

        protected abstract (Vector2 position, Vector2 size) GetToPose();
       
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var focusedRect = Tutorial.Controller.FocusedRect;
            var fromMin = focusedRect.min;
            var fromSize = focusedRect.size;
            var toPose = GetToPose();
            
            var moveVector = fromMin - toPose.position;

            var moveTime = moveVector.magnitude / Tutorial.Controller.FocusMoveSpeed;
            var moveTimer = 0.0f;
            
            var modules = gameObject.GetComponents<ModuleTutorialStep>();
            
            foreach (var module in modules)
                module.OnBeginUpdate(this);
            while (moveTimer < moveTime)
            {
                moveTimer += Time.deltaTime;
                var moveNormParam = moveTimer / moveTime;
                var moveNorm = Tutorial.Controller.FocusMoveSpeedCurve.Evaluate(moveNormParam);
                var worldPosition = Vector2.Lerp(fromMin, toPose.position, moveNorm);
                var worldSize = Vector2.Lerp(fromSize, toPose.size, moveNorm);

                _focusedRect = new Rect(worldPosition, worldSize);
                Tutorial.Controller.SetFocusedRect(_focusedRect);
                foreach (var module in modules)
                    module.OnUpdate(this);
                
                await Task.Yield();
            }
            foreach (var module in modules)
                module.OnEndUpdate(this);

            return true;
        }

        public Rect GetFocusedRect()
        {
            return _focusedRect;
        }
    }
}