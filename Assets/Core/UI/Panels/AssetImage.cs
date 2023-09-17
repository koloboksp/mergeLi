using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Core
{
    public class AssetImage : MonoBehaviour
    {
        [SerializeField] private Image _target;
        [SerializeField] private string _spriteName;

        private AsyncOperationHandle<Sprite> _operationHandle; 
        public string SpriteName
        {
            set
            {
                if (_spriteName != value)
                {
                    _spriteName = value;
                    if (!string.IsNullOrEmpty(_spriteName))
                    {
                        _operationHandle = Addressables.LoadAssetAsync<Sprite>(_spriteName);
                        _operationHandle.Completed += OperationHandle_OnCompleted;
                    }
                }
            }
        }

        private void OperationHandle_OnCompleted(AsyncOperationHandle<Sprite> sender)
        {
            if(sender.Status == AsyncOperationStatus.Succeeded)
                _target.sprite = sender.Result;
            else
                _target.sprite = null;
        }
    }
}