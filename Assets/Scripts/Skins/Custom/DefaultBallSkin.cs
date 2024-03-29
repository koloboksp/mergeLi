using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        [Flags]
        public enum BallState
        {
            None = 0,
            Idle = 1,
            Select = 2,
            Move = 4,
            PathNotFound = 8,
            Upgrade = 16,
            Downgrade = 32
        }

        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private Transform _hatAnchor;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Hat _hat;

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

            ChangeStateEvent?.Invoke(points >= oldPoints ? BallState.Upgrade : BallState.Downgrade);
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

        public override void SetHat(Hat hat)
        {
            if (_hat != null)
            {
                Destroy(_hat.gameObject);
                _hat = null;
            }

            if (hat != null)
                _hat = Instantiate(hat, _hatAnchor);
        }
    }
}