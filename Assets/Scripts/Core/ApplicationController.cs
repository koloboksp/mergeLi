using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analytics;
using Assets.Scripts.Core.Localization;
using Assets.Scripts.Core.Storage;
using Core.Ads;
using Core.Market;
using Core.Social;
using Core.Utils;
using Save;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Core
{
    public class ApplicationController : ILanguageChanger
    {
        private const string LOGOSCENE_NAME = "LogoScene";
        private const string GAMESCENE_NAME = "GameScene";
        
        private static ApplicationController _instance;

        private PurchaseController _purchaseController;
        private IAdsController _adsController;
        private UIPanelController _uiPanelController;
        private LocalizationController _localizationController;
        private ISocialService _socialService;
        private SoundController _soundController;
        private SaveController _saveController;
        private IAnalyticsController _analyticsController;
        private IVibrationController _vibrationController;
        private TaskCompletionSource<bool> _initialization;
        private bool _initialized = false;
        private Atom.Version _version;

        public static ApplicationController Instance => _instance;

        public SaveController SaveController => _saveController;
        public LocalizationController LocalizationController => _localizationController;
        public UIPanelController UIPanelController => _uiPanelController;
        public PurchaseController PurchaseController => _purchaseController;
        public IAdsController AdsController => _adsController;
        public ISocialService ISocialService => _socialService;
        public SoundController SoundController => _soundController;
        public IAnalyticsController AnalyticsController => _analyticsController;
        public IVibrationController VibrationController => _vibrationController;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void Start()
        {
            _instance = new ApplicationController();
            _instance._initialization = new TaskCompletionSource<bool>();

            Application.logMessageReceived += _instance.Application_logMessageReceived;
            
            IStorage storage = null;
#if UNITY_WEBGL && !UNITY_EDITOR
            storage = new UserDataManager();
#else
            storage = new BaseStorage();
#endif
            await storage.InitializeAsync();
            
            _instance._version = new(Application.version);
            _instance._saveController = new SaveController();
            await _instance._saveController.InitializeAsync(storage, Application.exitCancellationToken);
            DependenciesController.Instance.Set(_instance._saveController);
            _instance._localizationController = new LocalizationController();
            await _instance._localizationController.InitializeAsync(_instance._saveController.SaveSettings, Application.exitCancellationToken);
            if (!_instance._localizationController.ActiveLanguageDetected)
            {
                _instance._localizationController.ActiveLanguage = Application.systemLanguage;
            }
            _instance._soundController = new SoundController(_instance._saveController.SaveSettings);
            await _instance._soundController.InitializeAsync(Application.exitCancellationToken);
            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/RequiredPrefabs/purchaseLibrary.prefab");
            var purchaseLibraryObject = await handle.Task;
            var purchasesLibrary = purchaseLibraryObject.GetComponent<PurchasesLibrary>();
            
            _instance._purchaseController = new PurchaseController();
            await _instance._purchaseController.InitializeAsync(purchasesLibrary.Items.Select(i => i.ProductId));
            
#if UNITY_ANDROID || UNITY_IOS
            _instance._analyticsController = new FirebaseAnalyticsController();
#else
            _instance._analyticsController = new DefaultAnalytics();
#endif
            await _instance._analyticsController.InitializeAsync(_instance._version);
#if UNITY_ANDROID || UNITY_IOS
            _instance._adsController = new CASWrapper();
#else
            _instance._adsController = new DefaultAdsController();
#endif
 
            await _instance._adsController.InitializeAsync();

            _instance._vibrationController = new VibrationController(_instance._saveController.SaveSettings);
            await _instance._vibrationController.InitializeAsync();
#if UNITY_ANDROID
            _instance._socialService = new Social.GooglePlayGames();
#else
            _instance._socialService = new Default();
#endif
            if(_instance._socialService != null && _instance._socialService.IsAutoAuthenticationAvailable())
                _ = _instance._socialService.AuthenticateAsync(Application.exitCancellationToken);
            _instance._uiPanelController = new UIPanelController();
            DependenciesController.Instance.Set(_instance._uiPanelController);

            _instance._initialized = true;
            _instance._initialization.SetResult(true);
        }

        private void Application_logMessageReceived(string condition, string stacktrace, LogType logType)
        {
            var time = Time.realtimeSinceStartup;

            AppConsole.LogMessage(time,condition, stacktrace, logType);
        }

        public static void LoadGameScene()
        {
            SceneManager.LoadSceneAsync(GAMESCENE_NAME, LoadSceneMode.Additive);
        }
        
        public static void UnloadLogoScene()
        {
            var scene = SceneManager.GetSceneByName(LOGOSCENE_NAME);
            if (scene.IsValid()) 
                SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
        
        public void SetLanguage(SystemLanguage language)
        {
            _localizationController.ActiveLanguage = language;
        }

        public async Task<bool> WaitForInitializationAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
                return true;
            
            var cancellationTokenCompletion = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => cancellationTokenCompletion.SetResult(true));
            
            var initializationTask = _initialization.Task;
            await Task.WhenAny(initializationTask, cancellationTokenCompletion.Task);

            if (initializationTask.IsCompleted)
                return initializationTask.Result;

            throw new OperationCanceledException();
        }
        
        public static void RateUsInStore()
        {
#if UNITY_ANDROID
            Application.OpenURL($"market://details?id={Application.identifier}");
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/id6443549299");
#endif
        }
    }

    
}