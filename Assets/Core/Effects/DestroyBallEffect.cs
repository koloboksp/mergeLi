using UnityEngine;

public class DestroyBallEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public float Duration => _particleSystem.main.duration;
    
    public void Run()
    {
        Destroy(this.gameObject, Duration);
    }
}