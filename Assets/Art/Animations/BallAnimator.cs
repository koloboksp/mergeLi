
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Skins.Custom;

public class BallAnimator : MonoBehaviour
{
    [System.Serializable]
    private class Item
    {
        public DefaultBallSkin.BallState state;
        public AnimationClip clip;
    }
    
    private const float CROSS_FADE = .1f;

    [SerializeField] private DefaultBallSkin ballSkin;
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private List<Item> items;
    private AnimationClip lastLoopClip;

    private void Awake()
    {
        if (ballSkin == null || anim == null || idleClip == null)
            return;

        ballSkin.ChangeStateEvent += SetAnimationState;

        anim[idleClip.name].normalizedTime = Random.Range(0, 1f);
    }

    private void OnDestroy()
    {
        if (ballSkin != null)
            ballSkin.ChangeStateEvent -= SetAnimationState;
    }

    private void SetAnimationState(DefaultBallSkin.BallState state, bool instant)
    {
        foreach (var item in items)
        {
            if ((item.state & state) == state)
            {
                if (instant)
                {
                    if (state == DefaultBallSkin.BallState.Born)
                    {
                        anim.Play(item.clip.name);
                        anim.PlayQueued(idleClip.name);
                    }
                    else
                    {
                        anim.Play(item.clip.name);
                    }
                }
                else
                {
                    anim.CrossFade(item.clip.name, CROSS_FADE);

                    if (item.clip.wrapMode == WrapMode.Loop)
                    {
                        lastLoopClip = item.clip;
                    }
                    else if (lastLoopClip != null)
                    {
                        anim.CrossFadeQueued(lastLoopClip.name, CROSS_FADE);
                    }
                }
                break;
            }
        }
    }
}
