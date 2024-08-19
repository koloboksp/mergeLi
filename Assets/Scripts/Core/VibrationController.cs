using System;
using System.Threading.Tasks;
using Save;
using UnityEngine;

namespace Core
{
    public enum VibrationType
    {
        Pop,
        Peek,
    }
    
    public interface IVibrationController
    {
        Task InitializeAsync();

        bool Enable { get; set; }
        void Vibrate(VibrationType vibrationType);
    }
    
    public class VibrationController : IVibrationController
    {
        private SaveSettings _saveSettings;
        
        public VibrationController(SaveSettings saveSettings)
        {
            _saveSettings = saveSettings;
        }

        public Task InitializeAsync()
        {
            try
            {
#if UNITY_ANDROID || UNITY_IOS
                Vibration.Init();
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return Task.CompletedTask;
        }

        public void Vibrate(VibrationType vibrationType)
        {
            if (!_saveSettings.VibrationEnable)
            {
                return;
            }
            
            switch (vibrationType)
            {
                case VibrationType.Pop:
#if UNITY_ANDROID || UNITY_IOS
                    Vibration.VibratePop();
#endif
                    break;
                case VibrationType.Peek:
#if UNITY_ANDROID || UNITY_IOS
                    Vibration.VibratePeek();
#endif
                    break;
            }
        }
        
        public bool Enable
        {
            get => _saveSettings.VibrationEnable;
            set
            {
                if (value != _saveSettings.VibrationEnable)
                {
                    _saveSettings.VibrationEnable = value;
                }
            }
        }
    }
}