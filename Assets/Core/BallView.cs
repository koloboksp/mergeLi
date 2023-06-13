using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core
{
    public class BallView : MonoBehaviour, ISkinChangeable
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private RectTransform _root;
 
        BallSkin _ballSkin;

        public void Awake()
        {
            _ball.OnPointsChanged += Ball_OnPointsChanged;
            Ball_OnPointsChanged(_ball.Points);
            _ball.OnSelectedChanged += Ball_OnSelectedChanged;
            Ball_OnSelectedChanged();
        }
        
        public void ChangeSkin(string skinName, SkinContainer skinContainer)
        {
            if (_ballSkin != null)
                Destroy(_ballSkin.gameObject);

            var skin = skinContainer.GetSkin($"{skinName}_ball") as BallSkin;
            _ballSkin = Object.Instantiate(skin, _root);
            _ballSkin.transform.localPosition = Vector3.zero;
            _ballSkin.transform.localRotation = Quaternion.identity;
           
            Ball_OnPointsChanged(_ball.Points);
            Ball_OnSelectedChanged();
        }
        
        private void Ball_OnSelectedChanged()
        {
            _ballSkin.Selected = _ball.Selected;
        }

        private void Ball_OnPointsChanged(int oldPoints)
        {
            _ballSkin.Points = _ball.Points;
        }
    }


   
    
   

    
    

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

        public override bool Selected
        {
            set
            {
                _selectionEffect.SetActiveState(value);
            }
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
    }
}