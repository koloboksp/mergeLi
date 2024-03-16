using System.Threading;
using System.Threading.Tasks;
using Atom;
using UnityEngine;

namespace Core
{
    public class UIBubbleDialog : MonoBehaviour
    {
        [SerializeField] private UIText _dialogText;
        [SerializeField] private GameObject _iconRoot;
        public GameObject IconRoot => _iconRoot;

        public async Task ShowTextAsync(GuidEx textKey, CancellationToken cancellationToken)
        {
            _dialogText.gameObject.SetActive(true);
            
            var text = ApplicationController.Instance.LocalizationController.GetText(textKey);
            await _dialogText.ShowAsync(text, cancellationToken);
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (!state)
            {
                _dialogText.gameObject.SetActive(false);
            }
        }
    }
}