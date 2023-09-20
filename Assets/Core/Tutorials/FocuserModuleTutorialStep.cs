using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FocuserModuleTutorialStep : ModuleTutorialStep
    {
        [SerializeField] private bool _smooth;
        
        public override async Task OnExecute(TutorialStep step)
        {
            UpdateFocus(step as IFocusedOnSomething, step.Tutorial.Controller);
        }

        public override void OnComplete(TutorialStep step)
        {
            
        }

        public override void OnUpdate(TutorialStep step)
        {
            UpdateFocus(step as IFocusedOnSomething, step.Tutorial.Controller);
        }

        void UpdateFocus(IFocusedOnSomething focusedOnSomething, TutorialController controller)
        {
            controller.Focuser.gameObject.SetActive(true);
            
            if (focusedOnSomething != null)
                controller.Focuser.FocusOn(focusedOnSomething.GetFocusedRect(), _smooth);
            else
                controller.Focuser.FocusOn(_smooth);
        }
    }
}