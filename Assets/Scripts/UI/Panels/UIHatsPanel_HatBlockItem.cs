using Atom;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel_HatBlockItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _contentRoot;
        [SerializeField] private Text _title;

        public RectTransform ContentRoot => _contentRoot;
        
        public void SetData(GuidEx titleKey)
        {
            _title.text = ApplicationController.Instance.LocalizationController.GetText(titleKey);
        }
    }
}