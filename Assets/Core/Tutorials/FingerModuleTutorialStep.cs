using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{

    public interface IFocusedOnSomething
    {
        Rect GetFocusedRect();
    }
    
    public class FingerModuleTutorialStep : ModuleTutorialStep
    {
        public override async Task OnExecute(TutorialStep step)
        {
            var focusedOnSomething = step as IFocusedOnSomething;
            step.Tutorial.Controller.Finger.Show(focusedOnSomething.GetFocusedRect());
        }

        public override void OnComplete(TutorialStep step)
        {
            step.Tutorial.Controller.Finger.Hide();
        }

        public override void OnUpdate(TutorialStep step)
        {
            
        }
    }
}