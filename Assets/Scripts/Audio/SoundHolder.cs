using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SoundHolder : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private float _volume = 0.5f;

        private List<(string source, float volume)> _volumeModificators = new List<(string source, float volume)>() ;

        public float Time => _source.time;

        public AudioClip Clip
        {
            set => _source.clip = value;
        }

        public void SetVolumeModificator(string source, float volume)
        {
            var foundI = _volumeModificators.FindIndex(i => i.source == source);
            if (foundI >= 0)
            {
                if (volume >= 1)
                {
                    _volumeModificators.RemoveAt(foundI);
                }
                else
                {
                    var volumeModificator = _volumeModificators[foundI];
                    volumeModificator.volume = volume;
                    _volumeModificators[foundI] = volumeModificator;
                }
            }
            else
            {
                _volumeModificators.Add((source, _volume));
            }
            
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            if (_source != null)
            {
                var derivedVolume = 1.0f;
                for (var i = 0; i < _volumeModificators.Count; i++)
                    derivedVolume *= _volumeModificators[i].volume;
                
                _source.volume = _volume * derivedVolume;
            }
        }

        public void Play()
        {
            _source.Play();
        }
        
        public void Pause()
        {
            _source.Pause();
        }
        
        private void OnValidate()
        {
            UpdateVolume();
        }

        public void Stop()
        {
            _source.Stop();
        }
    }
}