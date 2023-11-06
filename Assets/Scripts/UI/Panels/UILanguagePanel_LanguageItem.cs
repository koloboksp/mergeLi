using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UILanguagePanel_LanguageItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selectionFrame;

        private Model _model;
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(Model model)
        {
            _model = model;
            _model
                .OnSelectionChanged(OnSelectionChanged);

            _name.text = _model.Language.ToString();
            _icon.sprite = _model.Icon;
        }
        
        private void OnClick()
        {
            _model.SelectMe();
        }
        
        private void OnSelectionChanged()
        {
            _selectionFrame.SetActive(_model.Selected);
        }
        
        public class Model
        {
            private Action _onSelectedStateChanged;
            
            private UILanguagePanel.Model _owner;
            private SystemLanguage _language;
            private Sprite _icon;
            private bool _selected;
            
            public Model(UILanguagePanel.Model owner)
            {
                _owner = owner;
            }

            public bool Selected => _selected;
            public SystemLanguage Language => _language;

            public Sprite Icon => _icon;
            
            public void SelectMe()
            {
                _owner.TrySelect(this);
            }
            
            public Model OnSelectionChanged(Action onSelectionChanged)
            {
                _onSelectedStateChanged = onSelectionChanged;
                _onSelectedStateChanged?.Invoke();
                return this;
            }

            public void SetSelectedState(bool newState)
            {
                if (_selected != newState)
                {
                    _selected = newState;
                    _onSelectedStateChanged?.Invoke();
                }
            }

            public Model SetLanguage(SystemLanguage language)
            {
                _language = language;
                return this;
            }
            
            public Model SetIcon(Sprite icon)
            {
                _icon = icon;
                return this;
            }
        }
    }
}