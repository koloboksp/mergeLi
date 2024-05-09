using Skins;
using UnityEngine;

namespace Core
{
    public class UIHatsPanel_HatItem_FakeScene : MonoBehaviour, IScene
    {
        public GameProcessor GameProcessor { get; set; }
        public SkinContainer ActiveSkin { get; set; }
        public HatsLibrary HatsLibrary { get; set; }
        public string[] ActiveHats { get; }
        
        public string[] UserInactiveHatsFilter { get; set; }
        public bool IsHatActive(string hatName)
        {
            return true;
        }
    }
}