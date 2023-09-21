using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace Core
{
    public class WaitForScreenClosed : CustomYieldInstruction
    {
        private UIPanel _panel;
        private bool _ready;
        
        public WaitForScreenClosed(UIPanel panel)
        {
            _panel = panel;
            _panel.OnHided += Panel_OnHided;
        }

        private void Panel_OnHided(UIPanel sender)
        {
            _panel.OnHided -= Panel_OnHided;
            _ready = true;
        }

        public override bool keepWaiting => !_ready;
    }
    
    public class UIPanelController
    {
        private readonly ScreenStack _stack = new ScreenStack();
        private Transform _screensRoot;

        public void SetScreensRoot(RectTransform screensRoot) => _screensRoot = screensRoot;
        
        public async Task<UIPanel> PushScreenAsync(Type screenType, UIScreenData data, CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/UI/Screens/{screenType.Name}.prefab");
            var result = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var screenObject = Object.Instantiate(result);
                var screen = screenObject.GetComponent<UIPanel>();
                screen.Root.SetParent(_screensRoot);
                screen.Root.anchorMin = Vector2.zero;
                screen.Root.anchorMax = Vector2.one;
                screen.Root.offsetMin = Vector2.zero;
                screen.Root.offsetMax = Vector2.zero;
                screen.Root.localScale = Vector3.one;
                _stack.Push(handle, screen, data);
                
                return screen;
            }

            return null;
        }
        
        public async Task<UIPanel> PushPopupScreenAsync(Type screenType, UIScreenData data, CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/UI/Screens/{screenType.Name}.prefab");
            var result = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var screenObject = Object.Instantiate(result);
                var screen = screenObject.GetComponent<UIPanel>();
                screen.Root.SetParent(_screensRoot);
                screen.Root.anchorMin = Vector2.zero;
                screen.Root.anchorMax = Vector2.one;
                screen.Root.offsetMin = Vector2.zero;
                screen.Root.offsetMax = Vector2.zero;
                screen.Root.localScale = Vector3.one;
                _stack.PushPopup(handle, screen, data);
                
                return screen;
            }

            return null;
        }
        public void PopScreen(UIPanel screen)
        {
            var stackItem = _stack.PopScreen(screen);

            stackItem.Screen.Hide();
            Object.Destroy(stackItem.Screen.gameObject);
            Addressables.Release(stackItem.Handle);

            if (_stack.Count > 0)
            {
                var lastItem = _stack.GetLast();
                lastItem.Screen.Activate();
            }
        }

        public T GetPanel<T>() where T : UIPanel
        {
            return _stack.GetByPanelType(typeof(T)).Screen as T;
        } 
        
        class ScreenStack
        {
            private List<StackItem> _items = new List<StackItem>();
            public int Count => _items.Count;

            public void PushPopup(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
            {
                var stackItem = new StackItem(handle, screen, data);
                _items.Add(stackItem);
                stackItem.Screen.SetData(data);
                stackItem.Screen.Activate();
            }
            
            public void Push(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
            {
                if (_items.Count > 0)
                {
                    StackItem lastItem = _items[_items.Count - 1];
                    _items.RemoveAt(_items.Count - 1);
                    lastItem.Screen.Deactivate();
                }
                var stackItem = new StackItem(handle, screen, data);
                _items.Add(stackItem);
                stackItem.Screen.SetData(data);
                stackItem.Screen.Activate();
            }

            public StackItem PopScreen(UIPanel screen)
            {
                var stackI = _items.FindIndex(i => i.Screen == screen);
                if (stackI >= 0)
                {
                    StackItem stackItem = _items[stackI];
                    _items.RemoveAt(stackI);
                    stackItem.Screen.Deactivate();
                    return stackItem;
                }
                
                return null;
            }

            public StackItem GetLast()
            {
                if (_items.Count > 0)
                {
                    return _items[_items.Count - 1];
                }

                return null;
            }
            
            public StackItem GetByPanelType(Type type)
            {
                var stackItem = _items.FindLast(i => i.Screen.GetType() == type);
               
                return stackItem;
            }
            
            public class StackItem
            {
                private AsyncOperationHandle<GameObject> _handle;
                private UIPanel _screen;
                private UIScreenData _data;

                public AsyncOperationHandle<GameObject> Handle => _handle;
                public UIPanel Screen => _screen;

                public StackItem(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
                {
                    _handle = handle;
                    _screen = screen;
                    _data = data;
                }
            }
        }

        public void HideAll()
        {
            var stackItem = _stack.GetLast();
            while (stackItem != null)
            {
                PopScreen(stackItem.Screen);
                stackItem = _stack.GetLast();
            }
        }
    }
}