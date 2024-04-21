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
        
        [SerializeField] private GameObject _userInactiveFilterPanel;
        [SerializeField] private Button _userInactiveFilterButton;
        [SerializeField] private Image _userInactiveFilterIcon;
        [SerializeField] private Sprite _userInactiveFilterIconSelected;
        [SerializeField] private Sprite _userInactiveFilterIconNotSelected;

        [SerializeField] private UIHatsPanel_HatItem_FakeField _fakeField;
        [SerializeField] private UIHatsPanel_HatItem_FakeScene _fakeScene;

        private Model _model;
        private GameProcessor _gameProcessor;
        
        private void Awake()
        {
            
            _button.onClick.AddListener(OnClick);
            _userInactiveFilterButton.onClick.AddListener(UserInactiveFilterButton_OnClick);
        }
        
        public void SetModel(Model model, GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            _model = model;
            _model.OnSelectedStateChanged += OnSelectionChanged;
            _model.OnAvailableStateChanged += OnAvailableChanged;
            _model.OnUserInactiveFilterStateChanged += OnUserActiveStateChanged;

            _name.text = ApplicationController.Instance.LocalizationController.GetText(_model.NameKey);
            OnSelectionChanged();
            SetUserActiveIcon();
            SetLockIcon();
            
            _fakeScene.GameProcessor = gameProcessor;
            _fakeScene.HatsLibrary = gameProcessor.Scene.HatsLibrary;
            _fakeScene.ActiveSkin = gameProcessor.Scene.ActiveSkin;
            _fakeScene.UserInactiveHatsFilter = null;

            var indexOf = gameProcessor.Scene.HatsLibrary.Hats.ToList().IndexOf(model.Hat);

            _fakeField.CreateBall(Vector3Int.zero, 2, indexOf);
        }

        private void OnAvailableChanged()
        {
            SetLockIcon();
            SetUserActiveIcon();
        }
        
        private void OnUserActiveStateChanged()
        {
            SetUserActiveIcon();
        }   
        
        private void SetLockIcon()
        {
            _lockIcon.gameObject.SetActive(!_model.Available);
        }

        private void SetUserActiveIcon()
        {
            _userInactiveFilterPanel.SetActive(_model.Available);
            _userInactiveFilterIcon.sprite = _model.UserInactiveFilter ? _userInactiveFilterIconNotSelected : _userInactiveFilterIconSelected;
        }
        
        private void OnClick()
        {
            _model.SelectMe();
        }

        private void UserInactiveFilterButton_OnClick()
        {
            _model.SetUserInactiveFilter(!_model.UserInactiveFilter);
        }
        
        private void OnSelectionChanged()
        {
            _selectionFrame.SetActive(_model.Selected);
        }
        
        public class Model
        {
            public event Action OnSelectedStateChanged;
            public event Action OnAvailableStateChanged;
            public event Action OnUserInactiveFilterStateChanged;

            private readonly Hat _hat;
            private readonly UIHatsPanel.Model _owner;
            private bool _selected;
            private bool _userInactiveFilter;

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
            public bool UserInactiveFilter => _userInactiveFilter;

            public void SelectMe()
            {
                _owner.TrySelect(this);
            }
            
            public Model SetUserInactiveFilter(bool state)
            {
                _userInactiveFilter = state;
                OnUserInactiveFilterStateChanged?.Invoke();
                return this;
            }
            
            internal void SetSelectedState(bool newState)
            {
                if (_selected != newState)
                {
                    _selected = newState;
                    OnSelectedStateChanged?.Invoke();
                }
            }
            
            public async Task Buy()
            {
                await Hat.Buy();
                OnAvailableStateChanged?.Invoke();
            }

            
        }
    }
}