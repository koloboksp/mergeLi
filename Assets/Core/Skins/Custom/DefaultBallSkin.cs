using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DefaultBallSkin : BallSkin
    {
        public static Dictionary<int, Color> _colors = new Dictionary<int,Color>()
        {
            {1, Color.yellow},
            {2, Color.green},
            {4, Color.magenta},
            {8, Color.red},
            {16, Color.white},
            {32, Color.cyan},
            {64, Color.gray},
            {128, Color.blue},
            {256, Color.black},
        };

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
                if(_colors.ContainsKey(value))
                    _ballIcon.color = _colors[value];
                
                _valueLabel.text = value.ToString();
            }
        }

        public override float Transparency
        {
            set => _canvasGroup.alpha = 1.0f - value;
        }
    }
}