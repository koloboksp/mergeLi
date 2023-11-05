using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        public enum BallState { Idle, Select, Move, PathNotFound }
        
        [SerializeField] private List<Color> _colors;
        [SerializeField] private BallSelectionEffect _selectionEffect;
        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        public UnityAction<BallState> ChangeStateEvent;

        public override bool Selected
        {
            set => ChangeStateEvent?.Invoke(value ? BallState.Select : BallState.Idle); // _selectionEffect.SetActiveState(value);
        }

        public override bool Moving
        {
            set => ChangeStateEvent?.Invoke(value ? BallState.Move : BallState.Idle);
        }

        public override int Points
        {
            set
            {
                _valueLabel.text = value.ToString();
            }
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