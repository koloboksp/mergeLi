using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UILanguagePanel_LanguageItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _normalColor = Color.gray;
        [SerializeField] private GameObject _selectionFrame;

        private LanguageItemModel _model;
        
        public LanguageItemModel Model => _model;
        public RectTransform Root => _root;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(LanguageItemModel languageItemModel)
        {
            _model = languageItemModel;
            _model.OnSelectedStateChanged += OnSelectionChanged;
            _name.text = _model.Label;
            _icon.sprite = _model.Icon;
            
            OnSelectionChanged();
        }
        
        private void OnClick()
        {
            _model.SelectMe();
        }
        
        private void OnSelectionChanged()
        {
            _button.targetGraphic.color = _model.Selected ? _selectedColor : _normalColor;
        }
    }
    
    public class LanguageItemModel
    {
        public Action OnSelectedStateChanged;
        
        private UILanguagePanel.Model _owner;
        private SystemLanguage _language;
        private Sprite _icon;
        private bool _selected;
        private string _label;

        public LanguageItemModel(UILanguagePanel.Model owner)
        {
            _owner = owner;
        }

        public bool Selected => _selected;
        public SystemLanguage Language => _language;
        public Sprite Icon => _icon;
        public string Label => _label;
       
        public void SelectMe()
        {
            _owner.TrySelect(this);
        }
        
        public void SetSelectedState(bool newState)
        {
            if (_selected != newState)
            {
                _selected = newState;
                OnSelectedStateChanged?.Invoke();
            }
        }

        public LanguageItemModel SetLanguage(SystemLanguage language)
        {
            _language = language;
            return this;
        }
        
        public LanguageItemModel SetIcon(Sprite icon)
        {
            _icon = icon;
            return this;
        }

        public LanguageItemModel SetLabel(string label)
        {
            _label = label;
            return this;
        }
    }
}