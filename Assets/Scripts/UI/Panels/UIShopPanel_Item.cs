using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel_Item : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _button;
        [SerializeField] private AssetImage _offerIcon;
        
        private UIShopPanel_ItemModel _model;

        public UIShopPanel_ItemModel Model => _model;
        public RectTransform Root => _root;

        protected virtual void Awake()
        {
            _button.onClick.AddListener(() => OnClick());
        }

        protected virtual void OnDestroy()
        {
            
        }

        public virtual void SetModel(UIShopPanel_ItemModel model)
        {
            _model = model;
        }
        
        protected virtual async void OnClick()
        {
            
        }

        public bool Interactable
        {
            get => _group.interactable;
            set => _group.interactable = value;
        }
    }
    
    public class UIShopPanel_ItemModel 
    {
        private UIShopPanel.Model _owner;
        private IShopPanelItem _item;
        
        public UIShopPanel.Model Owner => _owner;
        public IShopPanelItem Item => _item;
        
        public UIShopPanel_ItemModel(UIShopPanel.Model owner)
        {
            _owner = owner;
        }
        
        public UIShopPanel_ItemModel Init(IShopPanelItem item)
        {
            _item = item;
            return this;
        }
    }
}