using UnityEngine;

public class DestroyBallEffect : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private AnimationClip _clip;

    public float Duration => _clip.length;
    
    public void Run()
    {
        Destroy(this.gameObject, Duration);
    }
}