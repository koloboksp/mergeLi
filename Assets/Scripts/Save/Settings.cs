using System;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class Settings
    {
        public bool ActiveLanguageDetected;
        public int ActiveLanguage;

        public float SoundVolume = 0.5f;
        public bool SoundEnable = true;
        public float MusicVolume;
        public bool MusicEnable;

        public string ActiveSkin;
        public string[] UserInactiveHatsFilter;
    }
}