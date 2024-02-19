using UnityEngine;

namespace Core
{
    public class SoundHolder : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private float _volume = 0.5f;

        private float _volumeModificator = 1.0f;
        
        public void SetVolumeModificator(float modificator)
        {
            _volumeModificator = modificator;
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            if (_source != null)
            {
                _source.volume = _volume * _volumeModificator;
            }
        }
        private void OnValidate()
        {
            UpdateVolume();
        }
    }
}