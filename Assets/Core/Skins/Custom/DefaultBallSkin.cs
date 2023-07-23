using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        public enum BallState { Idle, Select, Move }
        
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
                var colorIndex = Mathf.RoundToInt(Mathf.Log(value, 2));
                colorIndex %= _colors.Count;
                _ballIcon.color = _colors[colorIndex];

                _valueLabel.text = value.ToString();
            }
        }

        public override float Transparency
        {
            set => _canvasGroup.alpha = 1.0f - value;
        }
    }
}