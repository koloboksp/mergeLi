﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Core
{
    [Serializable]
    public class ColorToPointsAssociation
    {
        [SerializeField] private int _points;
        [SerializeField] private Color _color;

        public int Points => _points;
        public Color Color => _color;
    }
    
    public class BallView : MonoBehaviour, ISkinChangeable, IHatChangeable, ISubComponent
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private RectTransform _root;
        [SerializeField] private List<ColorToPointsAssociation> _colorsAssociations;
      
        private BallSkin _ballSkin;
        private Color _mainColor = Color.magenta;
        
        public RectTransform Root => _root;
        
        public void SetData()
        {
            _ball.OnPointsChanged += Ball_OnPointsChanged;
            _ball.OnSelectedChanged += Ball_OnSelectedChanged;
            _ball.OnMovingStateChanged += Ball_OnMovingStateChanged;
            _ball.OnTransparencyChanged += Ball_TransparencyChanged;
            _ball.OnPathNotFound += Ball_OnPathNotFound;

            ChangeSkin(_ball.Field.Scene.ActiveSkin);
        }

        private void Ball_OnMovingStateChanged()
        {
            _ballSkin.Moving = _ball.Moving;
        }

        public void ChangeSkin(SkinContainer container)
        {
            if (_ballSkin != null)
                Destroy(_ballSkin.gameObject);

            var skin = container.GetSkin($"ball") as BallSkin;
            _ballSkin = Object.Instantiate(skin, _root);
            _ballSkin.transform.localPosition = Vector3.zero;
            _ballSkin.transform.localRotation = Quaternion.identity;
           
            Ball_OnPointsChanged(_ball.Points, true);
            Ball_OnSelectedChanged();
            Ball_TransparencyChanged();
        }
        
        public void ChangeHat(Hat hat)
        {
            if (_ballSkin != null)
            {
                _ballSkin.SetHat(hat);
            }
        }
        
        private void Ball_OnSelectedChanged()
        {
            _ballSkin.Selected = _ball.Selected;
            if (_ball.Selected)
                _root.transform.SetAsLastSibling();
        }

        private void Ball_OnPointsChanged(int oldPoints, bool force)
        {
            _ballSkin.SetPoints(_ball.Points, oldPoints, force);
            var foundAssociation = _colorsAssociations.Find(i => i.Points == _ball.Points);
            if (foundAssociation != null)
                _mainColor = foundAssociation.Color;
            _ballSkin.MainColor = _mainColor;
        }
        
        private void Ball_TransparencyChanged()
        {
            _ballSkin.Transparency = _ball.Transparency;
        }
        
        private void Ball_OnPathNotFound()
        {
            _ballSkin.PathNotFount();
        }

        public Color MainColor => _mainColor;
    }
}