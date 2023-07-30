
using System.Collections.Generic;
using UnityEngine;
using Core;

public class BallAnimator : MonoBehaviour
{
    private const float CROSS_FADE = .1f;

    private Ball ball;
    [SerializeField] private DefaultBallSkin ballSkin;
    [SerializeField] private Animation anim;

    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip select;
    [SerializeField] private AnimationClip move;
    [SerializeField] private AnimationClip noPath;
    [SerializeField] private AnimationClip[] idles;

    private Dictionary<DefaultBallSkin.BallState, AnimationClip> clips;

    private void Awake()
    {
        if (ballSkin == null || anim == null || idle == null)
            return;

        ballSkin.ChangeStateEvent += SetAnimationState;

        ball = GetComponentInParent<Ball>();
        if (ball != null)
        {
            ball.OnPathNotFound += Ball_OnPathNotFound;
            ball.OnMovingStateChanged += Ball_OnMovingStateChanged;
        }

        clips = new Dictionary<DefaultBallSkin.BallState, AnimationClip>
        {
            { DefaultBallSkin.BallState.Idle, idle },
            { DefaultBallSkin.BallState.Select, select },
            { DefaultBallSkin.BallState.Move, move },
            { DefaultBallSkin.BallState.PathNotFound, move }
        };

        anim[idle.name].normalizedTime = Random.Range(0, 1f);
    }

    private void Ball_OnMovingStateChanged()
    {
        if (ball.Moving)
            BlobTrail.ResetTail();

        Follower.Follow(transform, ball.Moving);
    }

    private void Ball_OnPathNotFound()
    {
        anim.CrossFade(noPath.name, CROSS_FADE);
        anim.CrossFadeQueued(select.name, CROSS_FADE);
    }

    private void OnDestroy()
    {
        if (ballSkin != null)
            ballSkin.ChangeStateEvent -= SetAnimationState;

        if (ball != null)
        {
            ball.OnPathNotFound -= Ball_OnPathNotFound;
            ball.OnMovingStateChanged -= Ball_OnMovingStateChanged;
        }
    }

    private void SetAnimationState(DefaultBallSkin.BallState ballState)
    {
        string clipName = clips[ballState] != null ? clips[ballState].name : idle.name;

        anim.CrossFade(clipName, CROSS_FADE);
    }
}
