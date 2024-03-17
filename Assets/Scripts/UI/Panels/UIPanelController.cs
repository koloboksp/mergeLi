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

        public async Task<TPanel> PushScreenAsync<TPanel>(UIScreenData data, CancellationToken cancellationToken) where TPanel : UIPanel
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/Prefabs/UI/Screens/{typeof(TPanel).Name}.prefab");
                var result = await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var screenObject = Object.Instantiate(result);
                    var screen = screenObject.GetComponent<TPanel>();
                    screen.Root.SetParent(_screensRoot);
                    screen.Root.anchorMin = Vector2.zero;
                    screen.Root.anchorMax = Vector2.one;
                    screen.Root.offsetMin = Vector2.zero;
                    screen.Root.offsetMax = Vector2.zero;
                    screen.Root.localScale = Vector3.one;
                    _stack.Push(handle, screen, data);

                    return screen;
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        public async Task<TPanel> PushPopupScreenAsync<TPanel>(UIScreenData data, CancellationToken cancellationToken) where TPanel : UIPanel
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/Prefabs/UI/Screens/{typeof(TPanel).Name}.prefab");
                var result = await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var screenObject = Object.Instantiate(result);
                    var screen = screenObject.GetComponent<TPanel>();
                    screen.Root.SetParent(_screensRoot);
                    screen.Root.anchorMin = Vector2.zero;
                    screen.Root.anchorMax = Vector2.one;
                    screen.Root.offsetMin = Vector2.zero;
                    screen.Root.offsetMax = Vector2.zero;
                    screen.Root.localScale = Vector3.one;
                    _stack.PushPopup(handle, screen, data);

                    return screen;
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        public void PopScreen(UIPanel screen)
        {
            _stack.PopScreen(screen);
        }

        public T GetPanel<T>() where T : UIPanel
        {
            return _stack.GetByPanelType(typeof(T)).Screen as T;
        }

        private class ScreenStack
        {
            private readonly List<StackItem> _items = new();
            
            public void PushPopup(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
            {
                var stackItem = new StackItem(handle, screen, data);
                _items.Add(stackItem);
                stackItem.SetDataAndActivate();
            }

            public void Push(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
            {
                for (var itemI = _items.Count - 1; itemI >= 0; itemI--)
                {
                    var stackItem = _items[itemI];
                    stackItem.Hide();
                    _items.RemoveAt(itemI);
                }

                PushPopup(handle, screen, data);
            }

            public void PopScreen(UIPanel screen)
            {
                var foundItemI = _items.FindIndex(i => i.Screen == screen);
                if (foundItemI >= 0)
                {
                    var stackItem = _items[foundItemI];
                    stackItem.Hide();
                    _items.RemoveAt(foundItemI);
                }

                if (_items.Count > 0)
                {
                    var lastItem = _items[_items.Count - 1];
                    lastItem.Activate();
                }
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
                private readonly AsyncOperationHandle<GameObject> _handle;
                private readonly UIPanel _screen;
                private readonly UIScreenData _data;
                
                public UIPanel Screen => _screen;

                public StackItem(AsyncOperationHandle<GameObject> handle, UIPanel screen, UIScreenData data)
                {
                    _handle = handle;
                    _screen = screen;
                    _data = data;
                }

                public void SetDataAndActivate()
                {
                    _screen.SetData(_data);
                    _screen.Activate();
                }
                
                public void Activate()
                {
                    _screen.Activate();
                }
                
                public void Hide()
                {
                    _screen.Deactivate();
                    _screen.Hide();
                    Object.Destroy(_screen.gameObject);
                    Addressables.Release(_handle);
                }

                
            }
        }
    }
}