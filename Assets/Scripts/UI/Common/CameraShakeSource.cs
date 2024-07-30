using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Common
{
    public class CameraShakeSource : MonoBehaviour
    {
        [SerializeField] private float _amount = 0.4f;
        [SerializeField] private float _attenuation = 0.4f;
        
        public void Run()
        {
            for (var receiverI = 0; receiverI < CameraShakeReceiver.Receivers.Count; receiverI++)
            {
                var cameraShakeReceiver = CameraShakeReceiver.Receivers[receiverI];
                cameraShakeReceiver.AddSource(new CameraShakeReceiver.SourceInfo(_amount, _attenuation));
            }
        }
    }
}