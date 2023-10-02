using UnityEngine;

namespace Core.Tutorials
{
    public class TutorialEntry : MonoBehaviour
    {
        [SerializeField] private Tutorial _owner;

        public Tutorial Owner => _owner;
        
        public virtual bool CanStart(bool forceStart)
        {
            return true || forceStart;
        }
    }
}