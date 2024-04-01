using System;
using Core;
using UnityEngine;

namespace UI.Common
{
    public class UIButtonSoundFX : MonoBehaviour
    {
        [SerializeField] private UIExtendedButton _button;
        [SerializeField] private UICommonSounds _sound;

        private void Awake()
        {
            _button.onPressed.AddListener(Button_OnPressed);
            _button.onClickFail.AddListener(Button_OnClickFail);
        }
        
        private void Button_OnPressed()
        {
            ApplicationController.Instance.UIPanelController.SoundsPlayer.Play(_sound);
        }
        
        private void Button_OnClickFail()
        {
            ApplicationController.Instance.UIPanelController.SoundsPlayer.Play(UICommonSounds.Unavailable);
        }
    }
}