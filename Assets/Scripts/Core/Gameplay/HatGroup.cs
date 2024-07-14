using Atom;
using UnityEngine;

namespace Core.Gameplay
{
    public class HatGroup : MonoBehaviour
    {
        [SerializeField] private GuidEx _nameKey;

        public GuidEx NameKey => _nameKey;
    }
}