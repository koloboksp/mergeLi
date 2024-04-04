using System;
using System.Collections.Generic;
using Core;
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
            var data = undefinedData as UICheatsPanelData;

            var valueHolders = new List<ValueHolder>();
            foreach (var castle in data.GameProcessor.CastleSelector.Library.Castles)
                valueHolders.Add(new CastleCompleteBoolHolder(castle.Id));

            foreach (var tutorial in data.GameProcessor.TutorialController.AvailableTutorials)
                valueHolders.Add(new TutorialCompleteBoolHolder(tutorial.Id));
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(valueHolders);
        }

        private void OnItemsUpdated(IEnumerable<ValueHolder> items)
        {
            var oldViews = _container.content.GetComponents<UICheatsPanel_BoolItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _boolItemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                if (item is BoolHolder)
                {
                    var itemView = Instantiate(_boolItemPrefab, _container.content);
                    itemView.gameObject.SetActive(true);
                    itemView.SetModel(item as BoolHolder);
                }
            }
        }
        
        public class Model
        {
            private Action<IEnumerable<ValueHolder>> _onItemsUpdated;
            
            private readonly List<ValueHolder> _items = new List<ValueHolder>();
            private IHatsChanger _changer;
            
            public void SetData(IEnumerable<ValueHolder> valueHolders)
            {
                _items.AddRange(valueHolders);
                _onItemsUpdated?.Invoke(_items);
            }

            public Model OnItemsUpdated(Action<IEnumerable<ValueHolder>> onItemsUpdated)
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
}