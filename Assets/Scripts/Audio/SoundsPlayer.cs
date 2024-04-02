using UnityEngine;

namespace Core
{
    public enum UICommonSounds
    {
        Click,
        Back,
        Unavailable,
    }
    
    public class SoundsPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _click;
        [SerializeField] private AudioClip _back;
        [SerializeField] private AudioClip _unavailable;

        [SerializeField] private SoundHolder _source;

        public void Play(UICommonSounds sound)
        {
            AudioClip clip = null;
            if (sound == UICommonSounds.Click)
                clip = _click;
            if (sound == UICommonSounds.Back)
                clip = _back;
            if (sound == UICommonSounds.Unavailable)
                clip = _unavailable;

            _source.Clip = clip;
            _source.Play();
        }

        public void Play(AudioClip clip)
        {
            _source.Clip = clip;
            _source.Play();
        }
    }
}