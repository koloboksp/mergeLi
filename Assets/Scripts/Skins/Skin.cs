using UnityEngine;

namespace Skins
{
    public class Skin : MonoBehaviour
    {
        [SerializeField] private string _name;
    
        public string Name => _name;
    }
}