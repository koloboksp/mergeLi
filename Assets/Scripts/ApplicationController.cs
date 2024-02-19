using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core.Localization;
using Core.Steps.CustomOperations;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private SoundController _soundController;
        private SaveController _saveController;

        private TaskCompletionSource<bool> _initialization;
        public static ApplicationController Instance => _instance;

        public SaveController SaveController => _saveController;
        public LocalizationController LocalizationController => _localizationController;
        public UIPanelController UIPanelController => _uiPanelController;
        public PurchaseController PurchaseController => _purchaseController;
        public IAdsController AdsController => _adsController;
        public ISocialService ISocialService => _socialService;
        public SoundController SoundController => _soundController;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void Start()
        {
            
            
            _instance = new ApplicationController();
            _instance._initialization = new TaskCompletionSource<bool>();

            _instance._saveController = new SaveController();
            await _instance._saveController.InitializeAsync(CancellationToken.None);
            
            _instance._localizationController = new LocalizationController();
            await _instance._localizationController.InitializeAsync(CancellationToken.None);
           
            _instance._soundController = new SoundController(_instance._saveController.SaveSettings);
            await _instance._soundController.InitializeAsync(CancellationToken.None);

            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/RequiredPrefabs/purchaseLibrary.prefab");
            var purchaseLibraryObject = await handle.Task;
            var purchasesLibrary = purchaseLibraryObject.GetComponent<PurchasesLibrary>();
            
            _instance._purchaseController = new PurchaseController();
            await _instance._purchaseController.InitializeAsync(purchasesLibrary.Items.Select(i=>i.ProductId));
            
            _instance._adsController = new CASWrapper();
            await _instance._adsController.InitializeAsync();

//#if UNITY_ANDROID
            _instance._socialService = new GooglePlayGames();
//#endif
            if(_instance._socialService.IsAutoAuthenticationAvailable())
                _ = _instance._socialService.Authenticate();
            
            _instance._uiPanelController = new UIPanelController();

       
            _instance._initialization.SetResult(true);
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

        public async Task<bool> WaitForInitializationAsync(CancellationToken cancellationToken)
        {
            var cancellationTokenCompletion = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => cancellationTokenCompletion.SetResult(true));
            
            var initializationTask = _initialization.Task;
            await Task.WhenAny(initializationTask, cancellationTokenCompletion.Task);

            if (initializationTask.IsCompleted)
                return initializationTask.Result;

            throw new OperationCanceledException();
        }
    }
}