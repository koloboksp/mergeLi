using System;
using System.Linq;
using System.Threading.Tasks;
using Atom;
using Core.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core
{
    public class UIHatsPanel_HatItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        
        [SerializeField] private Button _button;
        [SerializeField] private Text _extraPoints;
        [SerializeField] private GameObject _selectionFrame;
        [SerializeField] private GameObject _pricePanel;
        [SerializeField] private Text _priceText;

        [SerializeField] private Image _background;
        [SerializeField] private Sprite _userActiveFilterIconSelected;
        [SerializeField] private Sprite _userActiveFilterIconNotSelected;

        [SerializeField] private UIHatsPanel_HatItem_FakeField _fakeField;
        [SerializeField] private UIHatsPanel_HatItem_FakeScene _fakeScene;

        private Model _model;
        private GameProcessor _gameProcessor;
        public RectTransform Root => _root;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(Model model, GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            _model = model;
            _model.OnSelectedStateChanged += OnSelectionChanged;
            _model.OnAvailableStateChanged += OnAvailableChanged;
            _model.OnUserActiveFilterStateChanged += OnUserActiveStateChanged;

            _fakeScene.GameProcessor = gameProcessor;
            _fakeScene.HatsLibrary = gameProcessor.Scene.HatsLibrary;
            _fakeScene.ActiveSkin = gameProcessor.Scene.ActiveSkin;
            _fakeScene.UserInactiveHatsFilter = null;
            _fakeField.CreateBall(Vector3Int.zero, (int)Mathf.Pow(2, Random.Range(0, 9)), model.Hat.Id);
            var balls = _fakeField.GetSomething<Ball>(Vector3Int.zero).ToList();
            balls[0].View.ShowPoints(false);
            
            OnSelectionChanged();
            SetUserActiveIcon();
            SetLockIcon();
            SetExtraPoints();
        }

        private void SetExtraPoints()
        {
            _extraPoints.text = $"+{_model.ExtraPoints}";
        }

        private void OnAvailableChanged()
        {
            SetLockIcon();
            SetUserActiveIcon();
        }
        
        private void OnUserActiveStateChanged(Model model)
        {
            SetUserActiveIcon();
        }   
        
        private void SetLockIcon()
        {
            _pricePanel.gameObject.SetActive(!_model.Available);
            _priceText.text = _model.Cost.ToString();
        }

        private void SetUserActiveIcon()
        {
            _background.sprite = _model.Available 
                ? _model.UserActive 
                    ? _userActiveFilterIconSelected  
                    : _userActiveFilterIconNotSelected
                : _userActiveFilterIconNotSelected;
            
            var balls = _fakeField.GetSomething<Ball>(Vector3Int.zero).ToList();
            balls[0].View.ShowEyes(_model.Available && _model.UserActive);
        }
        
        private void OnClick()
        {
            _model.SelectMe();
        }

        private void UserInactiveFilterButton_OnClick()
        {
            _model.SetUserActiveFilter(!_model.UserActive);
        }
        
        private void OnSelectionChanged()
        {
            _selectionFrame.SetActive(_model.Selected);
        }
        
        public class Model
        {
            public event Action OnSelectedStateChanged;
            public event Action OnAvailableStateChanged;
            public event Action<Model> OnUserActiveFilterStateChanged;

            private readonly Hat _hat;
            private readonly UIHatsPanel.Model _owner;
            private bool _selected;
            private bool _userActive;

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
            public int ExtraPoints => _hat.ExtraPoints;
            public GuidEx NameKey => Hat.NameKey;
            public bool UserActive => _userActive;

            public void SelectMe()
            {
                _owner.TrySelect(this);
            }

            public void SetData(bool userActive)
            {
                _userActive = userActive;
            }
            
            public void SetUserActiveFilter(bool state)
            {
                if (state)
                {
                    if (_owner.BalanceActivateHats())
                    {
                        _userActive = true;
                        OnUserActiveFilterStateChanged?.Invoke(this);
                    }
                }
                else
                {
                    _userActive = false;
                    OnUserActiveFilterStateChanged?.Invoke(this);
                }
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