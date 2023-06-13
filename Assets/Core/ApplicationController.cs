using UnityEngine;

namespace Core
{
    public class ApplicationController
    {
        private static ApplicationController _instance;
        
        private UIScreenController _uiScreenController;

        public static ApplicationController Instance => _instance;
        
        public UIScreenController UIScreenController => _uiScreenController;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Start()
        {
            _instance = new ApplicationController();
            
            _instance._uiScreenController = new UIScreenController();
        }
    }
}