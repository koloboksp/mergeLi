using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core;
using Atom;
using UI.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class UIBubbleDialog : MonoBehaviour
    {
        [SerializeField] private UIText _dialogText;
        [SerializeField] private GameObject _iconRoot;
        [SerializeField] private UIBubbleSpeakEffect _speakEffect;
        [FormerlySerializedAs("_nextBtn")] [SerializeField] private UIExtendedButton _tapBtn;
        [SerializeField] private GameObject _clickToContinuePanel;

        private TaskCompletionSource<bool> _tapCompletionSource;
        public GameObject IconRoot => _iconRoot;

        private void Awake()
        {
            _tapBtn.onClick.AddListener(TapBtn_OnClick);
        }

        private void TapBtn_OnClick()
        {
            if (_tapCompletionSource != null)
            {
                _tapCompletionSource.TrySetResult(true);
            }
        }
        
        public async Task ShowTextAsync(GuidEx textKey, bool tapRequired, CancellationToken cancellationToken)
        {
            _clickToContinuePanel.SetActive(false);
            _dialogText.gameObject.SetActive(true);
            
            var text = ApplicationController.Instance.LocalizationController.GetText(textKey);
            _speakEffect.Play(text);
            await _dialogText.ShowAsync(text, Application.exitCancellationToken, cancellationToken);
            await AsyncExtensions.WaitForSecondsAsync(1, cancellationToken);
            
            if (tapRequired)
            {
                _tapBtn.gameObject.SetActive(true);
                
                _clickToContinuePanel.gameObject.SetActive(true);
                
                _tapCompletionSource = new TaskCompletionSource<bool>();
                var cancellationTokenRegistration = cancellationToken.Register(() => _tapCompletionSource.TrySetResult(false));
                try
                {
                    await _tapCompletionSource.Task;
                }
                finally
                {
                    cancellationTokenRegistration.Dispose();
                }

                _clickToContinuePanel.gameObject.SetActive(false);
            }
            else
            {
                _tapBtn.gameObject.SetActive(false);
            }
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
            if (!state)
            {
                _dialogText.gameObject.SetActive(false);
            }
        }
        
        public void SetDialogActive(bool state)
        {
            _dialogText.gameObject.SetActive(state);
        }
    }
}