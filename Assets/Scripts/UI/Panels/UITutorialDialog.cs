using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UITutorialDialog : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private UIBubbleDialog _speaker;
        [SerializeField] private RectTransform _bottomAnchor;
        [SerializeField] private RectTransform _centerAnchor;
        [SerializeField] private RectTransform _topAnchor;

        public UIBubbleDialog Speaker => _speaker;
        
        public async Task ShowAsync(GuidEx textKey, bool tapRequired, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            await _speaker.ShowTextAsync(textKey, tapRequired, cancellationToken);
        }
        
        public void Move(DialogPosition dialogPosition)
        {
            if (dialogPosition == DialogPosition.Bottom)
                _root.transform.position = _bottomAnchor.transform.position;
            if (dialogPosition == DialogPosition.Center)
                _root.transform.position = _centerAnchor.transform.position;
            if (dialogPosition == DialogPosition.Top)
                _root.transform.position = _topAnchor.transform.position;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public enum DialogPosition
    {
        Bottom,
        Center,
        Top
    }
}