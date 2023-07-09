using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        [SerializeField] private List<Color> _colors;
        [SerializeField] private BallSelectionEffect _selectionEffect;
        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        public override bool Selected
        {
            set => _selectionEffect.SetActiveState(value);
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