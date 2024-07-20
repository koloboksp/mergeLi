using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;
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
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _mainMenuBtn.onClick.AddListener(MainMenuBtn_OnClick);
            _restartBtn.onClick.AddListener(RestartBtn_OnClick);
            _shopBtn.onClick.AddListener(ShopBtn_OnClick);
        }
        
        private async void MainMenuBtn_OnClick()
        {
            var startPanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIStartPanel>(
                new UIStartPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Instant = true
                }, 
                Application.exitCancellationToken);
            await startPanel.ShowAsync(Application.exitCancellationToken);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private async void RestartBtn_OnClick()
        {
            var confirmRestartPanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIConfirmRestartPanel>(
                null, 
                Application.exitCancellationToken);
            await confirmRestartPanel.ShowAsync(Application.exitCancellationToken);
            
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            
            if(confirmRestartPanel.IsConfirmed)
                _data.GameProcessor.SessionProcessor.RestartSession();
        }
        
        private void ShopBtn_OnClick()
        {
            _ = ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    Items = UIShopPanel.FillShopItems(_data.GameProcessor),
                },
                Application.exitCancellationToken);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

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