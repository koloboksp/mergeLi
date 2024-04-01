using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public enum SoundGroup
    {
        Sound,
        Music,
    }

    public interface ISoundControllerListener
    {
        SoundGroup Group { get; }
        void ChangeVolume(float volume);
    }

    public class SoundControllerListener : MonoBehaviour, ISoundControllerListener
    {
        public const string MODIFICATOR_NAME = "Controller";
        
        [SerializeField] private SoundGroup _group = SoundGroup.Sound;
        [SerializeField] private SoundHolder _soundHolder;

        private void OnEnable()
        {
            SoundController.AddListener(this);
            ChangeVolume(SoundController.GetVolume(Group));
        }

        private void OnDisable()
        {
            SoundController.RemoveListener(this);
        }

        public SoundGroup Group => _group;
        public void ChangeVolume(float volume)
        {
            if (_soundHolder != null)
            {
                _soundHolder.SetVolumeModificator(MODIFICATOR_NAME, volume);
            }
        }
    }
}