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
    public class UIScreenController
    {
        private readonly ScreenStack _stack = new ScreenStack();
        private Transform _screensRoot;

        public void SetScreensRoot(RectTransform screensRoot) => _screensRoot = screensRoot;
        
        public void PushScreen(Type screenType, UIScreenData data)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Assets/UI/Screens/{screenType.Name}.prefab");
            handle.Completed += senderHandle => 
            {
                var screenObject = senderHandle.Result;
                var screen = screenObject.GetComponent<UIScreen>();
            
                _stack.Push(senderHandle, screen, data);
            };
        }

        public void PopScreen(UIScreen screen)
        {
            var stackItem = _stack.PopScreen(screen);

            stackItem.Screen.Hide();
            Object.Destroy(stackItem.Screen.gameObject);
            Addressables.Release(stackItem.Handle);

            if (_stack.Count > 0)
            {
                var lastItem = _items[_items.Count - 1];
                lastItem.Screen.Activate();
            }
        }
        
        class ScreenStack
        {
            private List<StackItem> _items = new List<StackItem>();
            public int Count => _items.Count;

            public void Push(AsyncOperationHandle<GameObject> handle, UIScreen screen, UIScreenData data)
            {
                _items.Add(new StackItem(handle, screen, data));
            }

            public StackItem PopScreen(UIScreen screen)
            {
                var stackI = _items.FindIndex(i => i.Screen == screen);
                if (stackI >= 0)
                {
                    StackItem stackItem = _items[stackI];
                    _items.RemoveAt(stackI);
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
                private UIScreen _screen;
                private UIScreenData _data;

                public AsyncOperationHandle<GameObject> Handle => _handle;
                public UIScreen Screen => _screen;

                public StackItem(AsyncOperationHandle<GameObject> handle, UIScreen screen, UIScreenData data)
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