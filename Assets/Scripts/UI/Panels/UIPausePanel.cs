using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIPausePanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _mainMenuBtn;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _shopBtn;
      
        private Model _model;
        private UIPausePanelData _data;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _mainMenuBtn.onClick.AddListener(MainMenuBtn_OnClick);
            _restartBtn.onClick.AddListener(RestartBtn_OnClick);
            _shopBtn.onClick.AddListener(ShopBtn_OnClick);
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        private async void MainMenuBtn_OnClick()
        {
            var startPanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIStartPanel), 
                new UIStartPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Instant = true
                }, 
                _cancellationTokenSource.Token) as UIStartPanel;
            await startPanel.ShowAsync(_cancellationTokenSource.Token);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void RestartBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            _data.GameProcessor.RestartSession();
        }
        
        private void ShopBtn_OnClick()
        {
            _ = ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIShopPanel),
                new UIShopScreenData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    PurchaseItems = _data.GameProcessor.PurchasesLibrary.Items
                },
                _cancellationTokenSource.Token);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIPausePanelData;
            _model = new Model();
        }
     
        public class Model
        {
         
        }
    }
    
    public class UIPausePanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}