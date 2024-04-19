using System;
using System.Linq;
using System.Threading.Tasks;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel_HatItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _name;
        [SerializeField] private Image _lockIcon;
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
            _gameProcessor = gameProcessor;

            _model = model;
            _model
                .OnSelectionChanged(OnSelectionChanged)
                .OnAvailableChanged(OnAvailableChanged);
            
            _name.text = ApplicationController.Instance.LocalizationController.GetText(_model.NameKey);
            SetLockIcon();
            
            _fakeScene.GameProcessor = gameProcessor;
            _fakeScene.HatsLibrary = gameProcessor.Scene.HatsLibrary;
            _fakeScene.ActiveSkin = gameProcessor.Scene.ActiveSkin;
            _fakeScene.ActiveHat = model.Hat;

            var indexOf = gameProcessor.Scene.HatsLibrary.Hats.ToList().IndexOf(model.Hat);

            _fakeField.CreateBall(Vector3Int.zero, 2, indexOf);
        }

        private void OnAvailableChanged()
        {
            SetLockIcon();
        }

        private void SetLockIcon()
        {
            _lockIcon.gameObject.SetActive(!_model.Available);
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
            private Action _onAvailableStateChanged;

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
            public GuidEx NameKey => Hat.NameKey;

            public void SelectMe()
            {
                _owner.TrySelect(this);
            }
            
            public Model OnSelectionChanged(Action onChanged)
            {
                _onSelectedStateChanged = onChanged;
                _onSelectedStateChanged?.Invoke();
                return this;
            }
            
            public Model OnAvailableChanged(Action onChanged)
            {
                _onAvailableStateChanged = onChanged;
                return this;
            }

            internal void SetSelectedState(bool newState)
            {
                if (_selected != newState)
                {
                    _selected = newState;
                    _onSelectedStateChanged?.Invoke();
                }
            }
            
            public async Task Buy()
            {
                await Hat.Buy();
                _onAvailableStateChanged?.Invoke();
            }
        }
    }
}