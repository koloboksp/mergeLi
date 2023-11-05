using UnityEngine;

public class NoPathEffect : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private Animation _animation;
    [SerializeField] private AnimationClip _clip;

    public float Duration => _clip.length;
    
    public void Run()
    {
        Destroy(this.gameObject, Duration);
    }

    public void AdjustSize(Vector3 cellSize)
    {
        _root.localScale = new Vector3(cellSize.x / _root.rect.size.x, cellSize.y / _root.rect.size.y, 1);
    }
}