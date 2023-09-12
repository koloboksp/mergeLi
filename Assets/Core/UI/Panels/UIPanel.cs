using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIPanel : MonoBehaviour
    {
        public event Action<UIPanel> OnHided;
        
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        public RectTransform Root => _root;
        
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
            
            OnHided?.Invoke(this);
        }

        protected virtual void InnerHide()
        {
            
        }

        public void Deactivate()
        {
            
        }

        public void LockInput(bool state)
        {
            _canvasGroup.blocksRaycasts = !state;
        }
    }

    public class UIScreenData
    {
        
    }
}