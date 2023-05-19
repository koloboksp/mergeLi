using UnityEngine;

public class BallSelectionEffect : MonoBehaviour
{
    [SerializeField] private RectTransform _upMotionRoot;
    [SerializeField] private AnimationCurve _upMotion;
    
    public void SetActiveState(bool state)
    {
        enabled = state;
    }

    void Update()
    {
        var anchoredPosition = _upMotionRoot.anchoredPosition;
        anchoredPosition.y = _upMotion.Evaluate(Time.time % 1) * 10;
        _upMotionRoot.anchoredPosition = anchoredPosition;
    }
}