using UnityEngine;

namespace Core.Tutorials
{
    public abstract class ModuleTutorialStep : MonoBehaviour
    {
        public abstract void OnExecute(TutorialStep step);
        public abstract void OnComplete(TutorialStep step);

    }
}