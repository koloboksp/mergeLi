using System.Collections.Generic;
using UnityEngine;

namespace UI.Common
{
    public class CameraShakeReceiver : MonoBehaviour
    {
        static readonly List<CameraShakeReceiver> _receivers = new List<CameraShakeReceiver>();
        public static List<CameraShakeReceiver> Receivers => _receivers;

        
        [SerializeField] private List<RectTransform> _targets = new List<RectTransform>();
        [SerializeField] private AnimationCurve _xAxis;
        [SerializeField] private AnimationCurve _yAxis;
        [SerializeField] private float _speedScale = 4.0f;
        [SerializeField] private float _forceScale = 10.0f;

        readonly List<SourceInfo> _sources = new List<SourceInfo>();

        private void Awake()
        {
            _receivers.Add(this);
        }

        private void OnDestroy()
        {
            _receivers.Remove(this);
        }

        public void AddSource(CameraShakeSource value)
        {
            var fIndex = -1;
            for (var i = 0; i < _sources.Count; i++)
                if (_sources[i].Source == value)
                    fIndex = i;

            if (fIndex >= 0)
                _sources[fIndex].Reset(value);
            else
            {
                var sourceInfo = new SourceInfo();
                sourceInfo.Initialize(value);
                _sources.Add(sourceInfo);
            }
        }

        public void RemoveSource(CameraShakeSource value)
        {
            var fIndex = -1;
            for (var i = 0; i < _sources.Count; i++)
                if (_sources[i].Source == value)
                    fIndex = i;

            _sources[fIndex].SourceRemoved();
        }

        public void Update()
        {
            var maxAmount = 0.0f;

            for (var sIndex = _sources.Count - 1; sIndex >= 0; sIndex--)
            {
                var sourceInfo = _sources[sIndex];
                sourceInfo.Update();
                if (sourceInfo.IsComplete)
                    _sources.Remove(sourceInfo);
            }

            for (var sIndex = 0; sIndex < _sources.Count; sIndex++)
            {
                var sourceInfo = _sources[sIndex];
                var sourceAmount = sourceInfo.CurrentAmount;

                maxAmount = Mathf.Max(maxAmount, sourceAmount);
            }

            if (maxAmount > 0.0f)
            {
                var x = _xAxis.Evaluate(1 - Mathf.Clamp01(maxAmount));
                var y = _yAxis.Evaluate(1 - Mathf.Clamp01(maxAmount));

               // var rotationAmount = Random.insideUnitSphere * maxAmount;
               // rotationAmount.z = 0.0f;
               // rotationAmount *= 10.0f;
                for (var targetI = 0; targetI < _targets.Count; targetI++)
                {
                    var target = _targets[targetI];
                    
                    target.anchoredPosition = new Vector2(x, y) * _forceScale;
                }
            }
        }

        private class SourceInfo
        {
            public CameraShakeSource Source;

            private float _timer;
            private float _currentAmount;
            private float _currentDuration;
            private float _attenuation;
            private float _distance;
            private Vector3 _position;
            private bool _isComplete;
            
            public float CurrentAmount => _currentAmount;
            public bool IsComplete => _isComplete;

            public void Initialize(CameraShakeSource value)
            {
                Reset(value);
            }

            public void Reset(CameraShakeSource source)
            {
                Source = source;

                _timer = 0;
                _isComplete = false;

                _currentAmount = Source.Amount;
                _currentDuration = Source.Duration;
                _attenuation = source.Attenuation;
                _currentDuration = source.Duration;
            }

            public void SourceRemoved()
            {
                Source = null;
            }

            public void Update()
            {
                if (_timer <= _currentDuration)
                {
                    _timer += Time.deltaTime;
                    _currentAmount -= _attenuation * Time.deltaTime;
                }
                else
                {
                    _isComplete = true;
                }
            }
        }
    }
}