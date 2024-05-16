using Assets.Scripts.Core;
using Atom;
using UI.Panels;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UICastlesLibraryPanel_CastleLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private UIStaticTextLocalizator _nameLoc;
        [SerializeField] private Text _points;
        [SerializeField] private string _hiddenCastleName = "????? ????";
        [SerializeField] private string _hiddenCastleCost = "????";

        public RectTransform Root => _root;
       
        public void SetData(GuidEx nameKey, UICastlesLibraryPanel.CastleViewType castleViewType, int points, int cost)
        {
            if (castleViewType == UICastlesLibraryPanel.CastleViewType.Completed)
            {
                _nameLoc.Id = nameKey;
                _points.text = $"{cost}";
            }
            else if (castleViewType == UICastlesLibraryPanel.CastleViewType.PartiallyReady)
            {
                _nameLoc.Id = nameKey;
                _points.text = $"{points}/{cost}";
            }
            else
            {
                _nameLoc.Target.text = _hiddenCastleName;
                _points.text = _hiddenCastleCost;
            }
        }
    }
}