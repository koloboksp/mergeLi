using System;
using Core;
using Core.Utils;
using UnityEngine;

namespace UI.Common
{
    public class UIButtonSoundFX : MonoBehaviour
    {
        [SerializeField] private UIExtendedButton _button;
        [SerializeField] private UICommonSounds _sound;

        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        
        private void Awake()
        {
            _button.onPressed.AddListener(Button_OnPressed);
            _button.onClickFail.AddListener(Button_OnClickFail);
        }
        
        private void Button_OnPressed()
        {
            _soundsPlayer.Value.Play(_sound);
        }
        
        private void Button_OnClickFail()
        {
            _soundsPlayer.Value.Play(UICommonSounds.Unavailable);
        }
    }
}