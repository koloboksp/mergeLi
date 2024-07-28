using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Common
{
    public class CameraShakeSource : MonoBehaviour
    {
        [SerializeField] private float _amount = 0.4f;
        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private float _attenuation = 0.4f;

        Transform _source;
        float _currentDuration;
        float _timer;
        
        public Transform Source => _source;
        public float Amount => _amount;
        public float Duration => _duration;
        public float Attenuation => _attenuation;
        

        public void ManagedEnable(Transform source)
        {
            _source = source;
           
            enabled = false;
        }

        public void Activate()
        {
            enabled = true;

            _timer = 0;
            _currentDuration = _duration;

            for (int rIndex = 0; rIndex < CameraShakeReceiver.Receivers.Count; rIndex++)
            {
                var cameraShakeReceiver = CameraShakeReceiver.Receivers[rIndex];
                cameraShakeReceiver.AddSource(this);
            }
        }

        void Update()
        {
            if (_timer > _currentDuration)
            {
                enabled = false;

                for (int rIndex = 0; rIndex < CameraShakeReceiver.Receivers.Count; rIndex++)
                {
                    var cameraShakeReceiver = CameraShakeReceiver.Receivers[rIndex];
                    cameraShakeReceiver.RemoveSource(this);
                }
            }
        }   
    }
}