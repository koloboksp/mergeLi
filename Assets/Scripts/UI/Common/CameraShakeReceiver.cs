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
        [SerializeField] private float _forceScale = 10.0f;

        private readonly List<SourceInfo> _sources = new();

        private void Awake()
        {
            _receivers.Add(this);
        }

        private void OnDestroy()
        {
            _receivers.Remove(this);
        }

        public void AddSource(SourceInfo source)
        {
            _sources.Add(source);
        }

        public void Update()
        {
            var amount = 0.0f;

            for (var sourceI = _sources.Count - 1; sourceI >= 0; sourceI--)
            {
                var sourceInfo = _sources[sourceI];
                
                sourceInfo.Update();
                if (sourceInfo.IsComplete)
                    _sources.Remove(sourceInfo);
            }

            for (var sourceI = 0; sourceI < _sources.Count; sourceI++)
            {
                var sourceInfo = _sources[sourceI];
                var sourceAmount = sourceInfo.Amount;

                amount = Mathf.Max(amount, sourceAmount);
            }

            if (amount >= 0.0f)
            {
                var iAmount = 1.0f - Mathf.Clamp01(amount);
                var offset = new Vector2(
                    _xAxis.Evaluate(iAmount),
                    _yAxis.Evaluate(iAmount));
               
                for (var targetI = 0; targetI < _targets.Count; targetI++)
                {
                    var target = _targets[targetI];
                    target.anchoredPosition = offset * _forceScale;
                }
            }
        }

        public class SourceInfo
        {
            private float _amount;
            private float _attenuation;
            
            public float Amount => _amount;
            public bool IsComplete => _amount <= 0.0f;

            public SourceInfo(float amount, float attenuation)
            {
                _amount = amount;
                _attenuation = attenuation;
            }
            
            public void Update()
            {
                _amount -= _attenuation * Time.deltaTime;
            }
        }
    }
}