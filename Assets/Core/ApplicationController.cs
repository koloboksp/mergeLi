using System.Linq;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Core
{
    public class ApplicationController
    {
        private static ApplicationController _instance;

        private PurchaseController _purchaseController;
        private IAdsController _adsController;
        private UIPanelController _uiPanelController;

        public static ApplicationController Instance => _instance;
        
        public UIPanelController UIPanelController => _uiPanelController;
        public PurchaseController PurchaseController => _purchaseController;
        public IAdsController AdsController => _adsController;

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
            
            var timer = new SmallTimer();
            _instance._purchaseController = new PurchaseController();
            await _instance._purchaseController.InitializeAsync(purchasesLibrarySceneReference.Reference.Items.Select(i=>i.ProductId));
            Debug.Log($"<color=#99ff99>Time initialize {nameof(PurchaseController)}: {timer.Update()}.</color>");
            
            _instance._adsController = new CASWrapper();
            await _instance._adsController.InitializeAsync();
            
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
    }
}