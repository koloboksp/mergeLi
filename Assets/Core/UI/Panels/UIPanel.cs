using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIPanel : MonoBehaviour
    {
        public event Action<UIPanel> OnHided;
        
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _active;
        
        public RectTransform Root => _root;
        
        public virtual void SetData(UIScreenData data)
        {
            
        }

        public void Activate()
        {
            _active = true;
            InnerActivate();
        }
        protected virtual void InnerActivate()
        {
            
        }
        
        public void Hide()
        {
            InnerHide();
            
            OnHided?.Invoke(this);

            _active = false;
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

        public async Task Showing(CancellationToken cancellationToken)
        {
            while (_active)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                
                await Task.Yield();
            }
        }
    }

    public class UIScreenData
    {
        
    }
}