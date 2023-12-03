using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core.Localization;
using Core.Steps.CustomOperations;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Core
{
    public class ApplicationController : ILanguage, ILanguageChanger
    {
        private static ApplicationController _instance;

        private PurchaseController _purchaseController;
        private IAdsController _adsController;
        private UIPanelController _uiPanelController;
        private LocalizationController _localizationController;
        private ISocialService _socialService;
        public static ApplicationController Instance => _instance;
        
        public LocalizationController LocalizationController => _localizationController;
        public UIPanelController UIPanelController => _uiPanelController;
        public PurchaseController PurchaseController => _purchaseController;
        public IAdsController AdsController => _adsController;

        public ISocialService ISocialService => _socialService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void Start()
        {
            var purchasesLibrarySceneReference = Object.FindObjectOfType<PurchasesLibrarySceneReference>();
            if (purchasesLibrarySceneReference == null)
            {
                Debug.LogError($"");
                return;
            }
            
            _instance = new ApplicationController();
           
            _instance._localizationController = new LocalizationController();
            await _instance._localizationController.InitializeAsync(CancellationToken.None);
           
            _instance._purchaseController = new PurchaseController();
            await _instance._purchaseController.InitializeAsync(purchasesLibrarySceneReference.Reference.Items.Select(i=>i.ProductId));
            
            _instance._adsController = new CASWrapper();
            await _instance._adsController.InitializeAsync();

            _instance._socialService = new GooglePlayGames();
            _ = _instance._socialService.Authentication();
            
            await StartAsync();
        }
        
       
        
        public static async Task StartAsync()
        {
            _instance._uiPanelController = new UIPanelController();
        }

        public static void LoadGameScene()
        {
            SceneManager.LoadSceneAsync("GameScene");
        }

        public SystemLanguage Language
        {
            get => Application.systemLanguage;
            set
            {
                
            }
        }

        public SystemLanguage ActiveLanguage
        {
            get
            {
                return _localizationController.ActiveActiveLanguage;
            }
        }

        public void SetLanguage(SystemLanguage language)
        {
            _localizationController.SetLanguage(language);
        }
    }
}