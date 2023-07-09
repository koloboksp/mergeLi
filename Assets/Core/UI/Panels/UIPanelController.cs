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
    public class UIPanelController
    {
        private readonly ScreenStack _stack = new ScreenStack();
        private Transform _screensRoot;

        public void SetScreensRoot(RectTransform screensRoot) => _screensRoot = screensRoot;
        
        public void PushScreen(Type screenType, UIScreenData data)
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
            };
        }

        public void PushPopupScreen(Type screenType, UIScreenData data)
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
        
        IEnumerator LoadGameObjectAndMaterial()
        {
            //Load a GameObject
            AsyncOperationHandle<GameObject> goHandle = Addressables.LoadAssetAsync<GameObject>("gameObjectKey");
            yield return goHandle;
            if(goHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = goHandle.Result;
                //etc...
            }

            //Load a Material
            AsyncOperationHandle<IList<IResourceLocation>> locationHandle = Addressables.LoadResourceLocationsAsync("Assets/UIScreens/GameScreen.prefab");
            yield return locationHandle;
            AsyncOperationHandle<Material> matHandle = Addressables.LoadAssetAsync<Material>(locationHandle.Result[0]);
            yield return matHandle;
            if (matHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Material mat = matHandle.Result;
                //etc...
            }

            //Use this only when the objects are no longer needed
            Addressables.Release(goHandle);
            Addressables.Release(matHandle);
        }
    }
}