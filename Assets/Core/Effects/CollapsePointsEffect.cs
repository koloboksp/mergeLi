using UnityEngine;
using UnityEngine.UI;

public class CollapsePointsEffect : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private AnimationClip _clip;
    [SerializeField] private Text _points;

    public float Duration => _clip.length;
    
    public void Run(int points)
    {
        _points.text = points.ToString();
        
        Destroy(this.gameObject, Duration);
    }
}