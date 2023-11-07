using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        public enum BallState { Idle, Select, Move, PathNotFound, Upgrade, Downgrade }
        
        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        public UnityAction<BallState> ChangeStateEvent;

        public override bool Selected
        {
            set => ChangeStateEvent?.Invoke(value ? BallState.Select : BallState.Idle);
        }

        public override bool Moving
        {
            set => ChangeStateEvent?.Invoke(value ? BallState.Move : BallState.Idle);
        }

        public override void SetPoints(int points, int oldPoints, bool force)
        {
            _valueLabel.text = points.ToString();

            if (force)
                return;
            
            var ballState = BallState.Downgrade;
            if (points > oldPoints)
                ballState = BallState.Upgrade;
            
            ChangeStateEvent?.Invoke(ballState);
        }

        public override Color MainColor
        {
            set => _ballIcon.color = value;
        }

        public override float Transparency
        {
            set => _canvasGroup.alpha = 1.0f - value;
        }

        public override void PathNotFount()
        {
            ChangeStateEvent?.Invoke(BallState.PathNotFound);
        }
    }
}