using Assets.Scripts.Core;
using Atom;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UICastlesLibraryPanel_CastleLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private UIStaticTextLocalizator _nameLoc;
        [SerializeField] private Text _points;
        
        public RectTransform Root => _root;
       
        public void SetData(GuidEx nameKey, int points, int cost)
        {
            _nameLoc.Id = nameKey;
            
            if (points == cost)
            {
                _points.text = $"{cost}";
            }
            else
            {
                _points.text = $"{points}/{cost}";
            }
        }
    }
}