
using Core.Gameplay;
using UnityEngine;

public class Follower : MonoBehaviour
{
    private Transform target;

    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        Ball.OnMovingStateChangedGlobal += Ball_OnMovingStateChangedGlobal;
    }

    private void Ball_OnMovingStateChangedGlobal(Ball ball, bool move)
    {
        if (!move)
            return;

        target = ball.transform;
        BlobTrail.SetColor(ball.View.MainColor);
    }

    private void Update()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;
    }
}
