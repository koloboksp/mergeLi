using UnityEngine;

namespace Core
{
    public class ApplicationController
    {
        private static ApplicationController _instance;
        
        private UIPanelController _uiPanelController;

        public static ApplicationController Instance => _instance;
        
        public UIPanelController UIPanelController => _uiPanelController;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Start()
        {
            _instance = new ApplicationController();
            
            _instance._uiPanelController = new UIPanelController();
        }
    }
}