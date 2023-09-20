using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public abstract class ModuleTutorialStep : MonoBehaviour
    {
        public abstract Task OnExecute(TutorialStep step);
        public abstract void OnComplete(TutorialStep step);

        public abstract void OnUpdate(TutorialStep step);
    }
}