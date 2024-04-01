using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core;
using Atom;
using UnityEngine;

namespace Core
{
    public class UIBubbleDialog : MonoBehaviour
    {
        [SerializeField] private UIText _dialogText;
        [SerializeField] private GameObject _iconRoot;
        [SerializeField] private UIBubbleSpeakEffect _speakEffect;
        
        public GameObject IconRoot => _iconRoot;

        public async Task ShowTextAsync(GuidEx textKey, CancellationToken cancellationToken)
        {
            _dialogText.gameObject.SetActive(true);
            
            var text = ApplicationController.Instance.LocalizationController.GetText(textKey);
            _speakEffect.Play(text);
            await _dialogText.ShowAsync(text, Application.exitCancellationToken, cancellationToken);
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