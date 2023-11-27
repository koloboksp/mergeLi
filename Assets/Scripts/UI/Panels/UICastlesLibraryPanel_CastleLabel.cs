using Assets.Scripts.Core;
using Atom;
using UnityEngine;

namespace Core
{
    public class UICastlesLibraryPanel_CastleLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private UIStaticTextLocalizator _nameLoc;

        public RectTransform Root => _root;
        public GuidEx NameKey 
        {
            set
            {
                _nameLoc.Id = value;
            }
        }
    }
}