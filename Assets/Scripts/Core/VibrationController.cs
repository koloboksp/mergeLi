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
                Vibration.Init();
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
                    Vibration.VibratePop();
                    break;
                case VibrationType.Peek:
                    Vibration.VibratePeek();
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