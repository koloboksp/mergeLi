using UnityEngine;

namespace Core.Effects
{
    public class PointsEffect : MonoBehaviour
    {
        [SerializeField] private Transform _destination;
        [SerializeField] private ParticleSystem _mainSystem;

        public void Run(int startEmissionCount, Vector3 destinationPosition)
        {
            var emission = _mainSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, startEmissionCount));
            _destination.position = destinationPosition;
        }
    }
}