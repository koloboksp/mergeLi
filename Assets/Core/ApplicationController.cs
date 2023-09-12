using System.Linq;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Core
{
    public class ApplicationController
    {
        private static ApplicationController _instance;

        private PurchaseController _purchaseController;
        private UIPanelController _uiPanelController;

        public static ApplicationController Instance => _instance;
        
        public UIPanelController UIPanelController => _uiPanelController;
        public PurchaseController PurchaseController => _purchaseController;
        
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
            
            await StartAsync();
        }
        
        public static async Task WaitForSecondsAsync(float time)
        {
            var timer = 0.0f;

            while (timer < time)
            {
                timer += Time.deltaTime;
                await Task.Yield();
            }
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