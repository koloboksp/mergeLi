using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UITimer : MonoBehaviour
    {
        public event Action<UITimer> OnComplete;

        [SerializeField] private Text _timeLabel;
        [SerializeField] private Text _readyLabel;

        private double _timeStamp;
        private long _restTicks;

        private void OnEnable()
        {
            if (_restTicks <= 0)
            {
                enabled = false;
            }
        }

        public void Set(long restTicks)
        {
            _restTicks = restTicks;
          
            if (_restTicks > 0)
            {
                _timeStamp = Time.realtimeSinceStartupAsDouble;
                
                _timeLabel.gameObject.SetActive(true);
                _readyLabel.gameObject.SetActive(false);

                UpdateTimeLabel();
                
                enabled = true;
            }
            else
            {
                _timeLabel.gameObject.SetActive(false);
                _readyLabel.gameObject.SetActive(true);
                
                enabled = false;
            }
        }

        private void Update()
        {
            UpdateTimeLabel();
            var timeLeft = GetTimeLeft();
            
            if (timeLeft < 0.0f)
            {
                _timeLabel.gameObject.SetActive(false);
                _readyLabel.gameObject.SetActive(true);
                
                enabled = false;
                
                OnComplete?.Invoke(this);
            }
        }

        private double GetTimeLeft()
        {
            var elapsedTime = Time.realtimeSinceStartupAsDouble - _timeStamp;
            var restTime = TimeSpan.FromTicks(_restTicks);
            var timeLeft = restTime.TotalSeconds - elapsedTime;
            return timeLeft;
        }
        
        private void UpdateTimeLabel()
        {
            var tsTimeLeft = TimeSpan.FromSeconds(GetTimeLeft());
            _timeLabel.text = $"{tsTimeLeft.Hours:D2}:{tsTimeLeft.Minutes:D2}:{tsTimeLeft.Seconds:D2}";
        }
    }
}