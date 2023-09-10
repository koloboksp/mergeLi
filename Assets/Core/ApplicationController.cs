using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class ApplicationController
    {
        private static ApplicationController _instance;
        
        private UIPanelController _uiPanelController;

        public static ApplicationController Instance => _instance;
        
        public UIPanelController UIPanelController => _uiPanelController;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static async void Start()
        {
            _instance = new ApplicationController();

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