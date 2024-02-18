using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core
{
    public class SoundController
    {
        public Action OnSoundVolumeChanged;
        public Action OnSoundEnableChanged;

        public Action OnMusicVolumeChanged;
        public Action OnnMusicEnableChanged;

        private readonly SaveSettings _saveSettings;
        private SoundMixers _soundMixers;
        public SoundController(SaveSettings saveSettings)
        {
            _saveSettings = saveSettings;
        }

        public bool SoundEnable
        {
            get => _saveSettings.SoundEnable;
            set
            {
                if (value != _saveSettings.SoundEnable)
                {
                    _saveSettings.SoundEnable = value;
                    OnSoundEnableChanged?.Invoke();
                }
            }
        }

        public float SoundVolume
        {
            get => _saveSettings.SoundVolume;
            set
            {
                if (value != _saveSettings.SoundVolume)
                {
                    _saveSettings.SoundVolume = value;
                    OnSoundVolumeChanged?.Invoke();
                }
            }
        }

        public bool MusicEnable
        {
            get => _saveSettings.MusicEnable;
            set
            {
                if (value != _saveSettings.MusicEnable)
                {
                    _saveSettings.MusicEnable = value;
                    OnnMusicEnableChanged?.Invoke();
                }
            }
        }

        public float MusicVolume
        {
            get => _saveSettings.MusicVolume;
            set
            {
                if (value != _saveSettings.MusicVolume)
                {
                    _saveSettings.MusicVolume = value;
                    OnMusicVolumeChanged?.Invoke();
                }
            }
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>($"Assets/RequiredPrefabs/soundMixers.prefab");
            var soundMixersObject = await handle.Task;

            _soundMixers = soundMixersObject.GetComponent<SoundMixers>();
            _soundMixers.Subscribe();
        }
    }
}