using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core
{
    public class SoundController
    {
        private static readonly List<ISoundControllerListener> _listeners = new ();
        private static readonly Dictionary<SoundGroup, float> _volumes = new Dictionary<SoundGroup, float>();
        
        private readonly SaveSettings _saveSettings;
        
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
                    _volumes[SoundGroup.Sound] = GetDerivedVolume(_saveSettings.SoundEnable, _saveSettings.SoundVolume);

                    ProcessListeners(SoundGroup.Sound);
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
                    _volumes[SoundGroup.Sound] = GetDerivedVolume(_saveSettings.SoundEnable, _saveSettings.SoundVolume);

                    ProcessListeners(SoundGroup.Sound);
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
                    _volumes[SoundGroup.Music] = GetDerivedVolume(_saveSettings.MusicEnable, _saveSettings.MusicVolume);

                    ProcessListeners(SoundGroup.Music);
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
                    _volumes[SoundGroup.Music] = GetDerivedVolume(_saveSettings.MusicEnable, _saveSettings.MusicVolume);
                    
                    ProcessListeners(SoundGroup.Music);
                }
            }
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _volumes[SoundGroup.Sound] = GetDerivedVolume(_saveSettings.SoundEnable, _saveSettings.SoundVolume);
            _volumes[SoundGroup.Music] = GetDerivedVolume(_saveSettings.MusicEnable, _saveSettings.MusicVolume);
            
            ProcessListeners(SoundGroup.Sound);
            ProcessListeners(SoundGroup.Music);
        }

        public static void AddListener(SoundControllerListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public static bool RemoveListener(SoundControllerListener listener)
        {
            return _listeners.Remove(listener);
        }

        private static float GetDerivedVolume(bool enabled, float volume)
        {
            return enabled ? volume : 0.0f;
        }
        
        private void ProcessListeners(SoundGroup group)
        {
            _volumes.TryGetValue(group, out var volume);
                    
            for (var lI = 0; lI < _listeners.Count; lI++)
            {
                var listener = _listeners[lI];
                if (listener.Group == group)
                    listener.ChangeVolume(volume);
            }
        }
    }
}