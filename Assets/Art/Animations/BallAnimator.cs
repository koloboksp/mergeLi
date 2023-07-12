
using System.Collections.Generic;
using UnityEngine;

public class BallAnimator : MonoBehaviour
{
    private const float FADE = .2f;
    
    [SerializeField] private Core.DefaultBallSkin ballSkin;
    [SerializeField] private Animation anim;

    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip select;
    [SerializeField] private AnimationClip move;

    [SerializeField] private AnimationClip[] idles;

    private Dictionary<Core.DefaultBallSkin.BallState, AnimationClip> clips;

    private void Awake()
    {
        if (ballSkin == null || anim == null || idle == null)
            return;

        ballSkin.ChangeStateEvent += SetAnimationState;

        clips = new Dictionary<Core.DefaultBallSkin.BallState, AnimationClip>();
        clips.Add(Core.DefaultBallSkin.BallState.Idle, idle);
        clips.TryAdd(Core.DefaultBallSkin.BallState.Select, select);
        clips.TryAdd(Core.DefaultBallSkin.BallState.Move, move);

        anim[idle.name].normalizedTime = Random.Range(0, 1f);
    }

    private void OnDestroy()
    {
        if (ballSkin != null)
            ballSkin.ChangeStateEvent -= SetAnimationState;
    }

    private void SetAnimationState(Core.DefaultBallSkin.BallState ballState)
    {
        string clipName = clips[ballState] != null ? clips[ballState].name : idle.name;

        anim.CrossFadeQueued(clipName, FADE);
    }
}
