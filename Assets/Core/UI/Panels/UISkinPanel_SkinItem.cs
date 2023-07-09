using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UISkinPanel_SkinItem : MonoBehaviour
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

            _name.text = _model.Name;
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
            
            private UISkinPanel.Model _owner;
            private string _name;
            private bool _selected;
            
            public Model(UISkinPanel.Model owner)
            {
                _owner = owner;
            }

            public bool Selected => _selected;
            public string Name => _name;

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

            public Model SetName(string name)
            {
                _name = name;
                return this;
            }
        }
    }
}