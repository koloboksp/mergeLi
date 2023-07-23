using System;
using System.Collections;
using System.Collections.Generic;
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

    public class PushPanelAndWaitForScreenReady<T> : CustomYieldInstruction where T : UIPanel
    {
        private bool _ready;
        private T _panel;
        
        public T Panel => _panel;

        protected void OnScreenReady(UIPanel sender)
        {
            _panel = sender as T;
            _ready = true;
        }

        public override bool keepWaiting => !_ready;
    }
    public class PushPopupAndWaitForScreenReady<T> : PushPanelAndWaitForScreenReady<T> where T : UIPanel
    {
        public PushPopupAndWaitForScreenReady(UIScreenData data)
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(T), data, OnScreenReady);
        }

    }
    public class PushAndWaitForScreenReady<T> : PushPanelAndWaitForScreenReady<T> where T : UIPanel
    {
        public PushAndWaitForScreenReady(UIScreenData data)
        {
            ApplicationController.Instance.UIPanelController.PushScreen(typeof(T), data, OnScreenReady);
        }
    }
    
    public class UIPanelController
    {
        private readonly ScreenStack _stack = new ScreenStack();
        private Transform _screensRoot;

        public void SetScreensRoot(RectTransform screensRoot) => _screensRoot = screensRoot;
        
        public void PushScreen(Type screenType, UIScreenData data, Action<UIPanel> onScreenReady = null)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Assets/UI/Screens/{screenType.Name}.prefab");
            handle.Completed += senderHandle => 
            {
                var screenObject = Object.Instantiate(senderHandle.Result);
                var screen = screenObject.GetComponent<UIPanel>();
                screen.Root.SetParent(_screensRoot);
                screen.Root.anchorMin = Vector2.zero;
                screen.Root.anchorMax = Vector2.one;
                screen.Root.offsetMin = Vector2.zero;
                screen.Root.offsetMax = Vector2.zero;
                screen.Root.localScale = Vector3.one;
                _stack.Push(senderHandle, screen, data);
                
                onScreenReady?.Invoke(screen);
            };
        }

        public void PushPopupScreen(Type screenType, UIScreenData data, Action<UIPanel> onScreenReady = null)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Assets/UI/Screens/{screenType.Name}.prefab");
            handle.Completed += senderHandle => 
            {
                var screenObject = Object.Instantiate(senderHandle.Result);
                var screen = screenObject.GetComponent<UIPanel>();
                screen.Root.SetParent(_screensRoot);
                screen.Root.anchorMin = Vector2.zero;
                screen.Root.anchorMax = Vector2.one;
                screen.Root.offsetMin = Vector2.zero;
                screen.Root.offsetMax = Vector2.zero;
                screen.Root.localScale = Vector3.one;
                _stack.PushPopup(senderHandle, screen, data);
                
                onScreenReady?.Invoke(screen);
            };
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
        
        class ScreenStack
        {
            private List<StackItem> _items = new List<StackItem>();
            public int Count => _items.Count;

            public void PushPopup(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
            {
                var stackItem = new StackItem(handle, screen, data);
                _items.Add(stackItem);
                stackItem.Screen.SetData(data);
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