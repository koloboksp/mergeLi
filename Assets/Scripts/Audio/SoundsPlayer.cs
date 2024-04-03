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

            _source.Play(clip);
        }

        public void Play(AudioClip clip)
        {
            _source.Play(clip);
        }

        public void StartPlay(AudioClip clip)
        {
            _source.StartPlay(clip);
        }

        public void StopPlay()
        {
            _source.StopPlay();
        }
    }
}