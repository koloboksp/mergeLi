using UnityEngine;
using UnityEngine.Audio;

namespace Core
{
    public class SoundMixers : MonoBehaviour
    {
        [SerializeField] private AudioMixer _musicMixer;
        [SerializeField] private AudioMixer _soundMixer;

        public void Subscribe()
        {
            ApplicationController.Instance.SoundController.OnMusicVolumeChanged += MusicVolumeChanged;
        }

        private void MusicVolumeChanged()
        {
            var findMatchingGroups = _musicMixer.FindMatchingGroups("Master");
            
            findMatchingGroups[0].audioMixer.SetFloat("MasterVolume", ApplicationController.Instance.SoundController.MusicVolume);
        }
    }
}