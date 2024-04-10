#if DEBUG
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICheatsPanel_Group : MonoBehaviour
    {
        [SerializeField] private Text _title;
        [SerializeField] private RectTransform _content;

        public RectTransform Content => _content;
        
        public string Title
        {
            set
            {
                _title.text = value;
            }
        }
    }
}
#endif