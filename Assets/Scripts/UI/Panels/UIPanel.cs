using System;
using System.Linq;
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
        
        public virtual void SetData(UIScreenData undefinedData)
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
            InnerDeactivate();
        }
        
        protected virtual void InnerDeactivate()
        {
            
        }
        
        public void LockInput(bool state)
        {
            _canvasGroup.interactable = !state;
        }

        public async Task ShowAsync(CancellationToken cancellationToken)
        {
            while (_active)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Yield();
            }
        }
    }

    public class UIScreenData
    {
        public string Layer;
    }
}