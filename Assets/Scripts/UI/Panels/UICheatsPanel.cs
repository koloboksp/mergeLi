#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICheatsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private UICheatsPanel_BoolItem _boolItemPrefab;
        [SerializeField] private UICheatsPanel_IntItem _intItemPrefab;
        [SerializeField] private UICheatsPanel_DropdownItem _dropdownItemPrefab;
        [SerializeField] private UICheatsPanel_Group _groupPrefab;

        private Model _model;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }

        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);
            
            var data = undefinedData as UICheatsPanelData;

            var valueHolders = new List<(string, List<ValueHolder>)>();

            var common = new List<ValueHolder>();
            valueHolders.Add(("common", common));
            common.Add(new RuleSettingsDropdownHolder("rules", data.GameProcessor));
            common.Add(new CoinsIntHolder("coins"));
            common.Add(new BestScoreIntHolder("bestSessionScore"));
            foreach (var tutorial in data.GameProcessor.TutorialController.AvailableTutorials)
                common.Add(new TutorialCompleteBoolHolder(tutorial.Id));
            
            var castles = new List<ValueHolder>();
            valueHolders.Add(("castles", castles));
            foreach (var castle in data.GameProcessor.CastleSelector.Library.Castles)
                castles.Add(new CastleCompleteBoolHolder(castle.Id));
            
            var hats = new List<ValueHolder>();
            valueHolders.Add(("hats", hats));
            foreach (var hat in data.GameProcessor.Hats)
                hats.Add(new HatBoolHolder(hat.Id));

            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(valueHolders);
        }

        private void OnItemsUpdated(IEnumerable<(string title, List<ValueHolder> items)> groups)
        {
            var oldViews = _container.content.GetComponents<UICheatsPanel_Group>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _groupPrefab.gameObject.SetActive(false);
            _boolItemPrefab.gameObject.SetActive(false);
            _intItemPrefab.gameObject.SetActive(false);
            _dropdownItemPrefab.gameObject.SetActive(false);

            foreach (var group in groups)
            {
                var groupView = Instantiate(_groupPrefab, _container.content);
                groupView.gameObject.SetActive(true);
                groupView.Title = group.title;

                foreach (var item in group.items)
                {
                    if (item is BoolHolder)
                    {
                        var itemView = Instantiate(_boolItemPrefab, groupView.Content);
                        itemView.gameObject.SetActive(true);
                        itemView.SetModel(item as BoolHolder);
                    }
                    if (item is IntHolder)
                    {
                        var itemView = Instantiate(_intItemPrefab, groupView.Content);
                        itemView.gameObject.SetActive(true);
                        itemView.SetModel(item as IntHolder);
                    }

                    if (item is DropdownHolder)
                    {
                        var itemView = Instantiate(_dropdownItemPrefab, groupView.Content);
                        itemView.gameObject.SetActive(true);
                        itemView.SetModel(item as DropdownHolder);
                    }
                }
                
            }
        }
        
        public class Model
        {
            private Action<IEnumerable<(string, List<ValueHolder>)>> _onItemsUpdated;
            
            private readonly List<(string, List<ValueHolder>)> _items = new();
            private IHatsChanger _changer;
            
            public void SetData(IEnumerable<(string, List<ValueHolder>)> valueHolders)
            {
                _items.AddRange(valueHolders);
                _onItemsUpdated?.Invoke(_items);
            }

            public Model OnItemsUpdated(Action<IEnumerable<(string, List<ValueHolder>)>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }
        }
    }
    
    public class UICheatsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
    }
    
    public abstract class ValueHolder
    {
        public string Id { get; }

        protected ValueHolder(string id)
        {
            Id = id;
        }
    }
        
    public class BoolHolder : ValueHolder
    {
        private bool _value;

        public bool Value
        {
            get => _value;
            protected set => _value = value;
        }
        
        public BoolHolder(string id) : base(id)
        {
        }
        
        public void ChangeValue(bool value)
        {
            _value = value;
            OnValueChanged(_value);
        }
        
        protected virtual void OnValueChanged(bool value)
        {
        }
    }
    
    public class IntHolder : ValueHolder
    {
        private int _value;

        public int Value
        {
            get => _value;
            protected set => _value = value;
        }
        
        public IntHolder(string id) : base(id)
        {
        }
        
        public void ChangeValue(int value)
        {
            _value = value;
            OnValueChanged(_value);
        }
        
        protected virtual void OnValueChanged(int value)
        {
        }
    }

    public class CoinsIntHolder : IntHolder
    {
        public CoinsIntHolder(string id) : base(id)
        {
            Value = ApplicationController.Instance.SaveController.SaveProgress.GetAvailableCoins();
        }

        protected override void OnValueChanged(int value)
        {
            ApplicationController.Instance.SaveController.SaveProgress.DebugSetCoins(value);
        }
    }
    
    public class BestScoreIntHolder : IntHolder
    {
        public BestScoreIntHolder(string id) : base(id)
        {
            Value = ApplicationController.Instance.SaveController.SaveProgress.BestSessionScore;
        }

        protected override void OnValueChanged(int value)
        {
            ApplicationController.Instance.SaveController.SaveProgress.DebugSetBestSessionScore(value);
        }
    }
    
    public class CastleCompleteBoolHolder : BoolHolder
    {
        public CastleCompleteBoolHolder(string id) : base(id)
        {
            Value = ApplicationController.Instance.SaveController.SaveProgress.IsCastleCompleted(id);
        }

        protected override void OnValueChanged(bool value)
        {
            ApplicationController.Instance.SaveController.SaveProgress.DebugChangeCastleComplete(Id, value);
        }
    }
    
    public class TutorialCompleteBoolHolder : BoolHolder
    {
        public TutorialCompleteBoolHolder(string id) : base(id)
        {
            Value = ApplicationController.Instance.SaveController.SaveProgress.IsTutorialComplete(id);
        }

        protected override void OnValueChanged(bool value)
        {
            ApplicationController.Instance.SaveController.SaveProgress.DebugChangeTutorialCompleteState(Id, value);
        }
    }
    
    public class HatBoolHolder : BoolHolder
    {
        public HatBoolHolder(string id) : base(id)
        {
            Value = ApplicationController.Instance.SaveController.SaveProgress.IsHatBought(id);
        }

        protected override void OnValueChanged(bool value)
        {
            ApplicationController.Instance.SaveController.SaveProgress.DebugChangeHatBought(Id, value);
        }
    }

    public abstract class DropdownHolder : ValueHolder
    {
        public DropdownHolder(string id) : base(id)
        {
            
        }

        public abstract IReadOnlyList<string> Items { get; }
        public abstract int SelectedIndex { get; set; } 
    }

    public class RuleSettingsDropdownHolder : DropdownHolder
    {
        private GameProcessor _gameProcessor;
        
        public RuleSettingsDropdownHolder(string id, GameProcessor gameProcessor) : base(id)
        {
            _gameProcessor = gameProcessor;
        }
        
        public override IReadOnlyList<string> Items
        {
            get
            {
                return _gameProcessor.GameRulesSettings
                    .Select(i => i.gameObject.name)
                    .ToList();
            }
        }

        public override int SelectedIndex
        {
            get
            {
                return _gameProcessor.ActiveGameRulesSettingsIndex;
            }
            set
            {
                _gameProcessor.ActiveGameRulesSettingsIndex = value;
            }
        }
    }
    
    
}
#endif