using UnityEngine;

namespace Core.Effects
{
    public class VibrationEffect : MonoBehaviour
    {
        [SerializeField] private VibrationType _vibrationType;
        
        private void OnEnable()
        {
            ApplicationController.Instance.VibrationController.Vibrate(_vibrationType);
        }
    }
}