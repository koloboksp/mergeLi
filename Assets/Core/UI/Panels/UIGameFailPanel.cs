using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    
    
    public class UIGameFailPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _nextBtn;

        private Model _model;
        private UIGameFailPanelData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _nextBtn.onClick.AddListener(NextBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        private void NextBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIGameFailPanelData;
            _model = new Model();
        }
        
        public class Model
        {
         
        }
    }
    
    public class UIGameFailPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}