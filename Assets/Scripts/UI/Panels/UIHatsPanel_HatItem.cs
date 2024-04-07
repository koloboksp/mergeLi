using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel_HatItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selectionFrame;
        [SerializeField] private UIHatsPanel_HatItem_FakeField _fakeField;
        [SerializeField] private UIHatsPanel_HatItem_FakeScene _fakeScene;

        private Model _model;
        private GameProcessor _gameProcessor;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(Model model, GameProcessor gameProcessor)
        {
            _model = model;
            _model
                .OnSelectionChanged(OnSelectionChanged);

            _name.text = _model.Id;

            _gameProcessor = gameProcessor;

            _fakeScene.GameProcessor = gameProcessor;
            _fakeScene.ActiveSkin = gameProcessor.Scene.ActiveSkin;
            _fakeScene.ActiveHat = model.Hat;

            _fakeField.CreateBall(Vector3Int.zero, 2);
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

            private readonly Hat _hat;
            private readonly UIHatsPanel.Model _owner;
            private bool _selected;
            
            public Model(Hat hat, UIHatsPanel.Model owner)
            {
                _hat = hat;
                _owner = owner;
            }

            public Hat Hat => _hat;
            public bool Selected => _selected;
            public string Id => _hat.Id;
            public bool Available => _hat.Available;
            public int Cost => _hat.Cost;

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
        }
    }
}