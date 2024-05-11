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
                _volumeModificators.Add((source, volume));
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

        public void Play(AudioClip clip)
        {
            _source.clip = clip;
            _source.loop = false;
            _source.Play();
        }
        
        public void StartPlay(AudioClip clip)
        {
            _source.clip = clip;
            _source.loop = true;
            _source.Play();
        }
        
        public void StopPlay()
        {
            _source.loop = false;
            _source.Stop();
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