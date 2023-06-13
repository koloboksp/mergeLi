using UnityEngine;

namespace Core
{
    public class UIScreen : MonoBehaviour
    {
        public virtual void SetData(UIScreenData data)
        {
            
        }

        public void Activate()
        {
            InnerActivate();
        }
        protected virtual void InnerActivate()
        {
            
        }
        
        public void Hide()
        {
            InnerHide();
        }

        protected virtual void InnerHide()
        {
            
        }
    }

    public class UIScreenData
    {
        
    }
}