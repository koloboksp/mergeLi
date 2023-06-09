﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public interface ISubComponent
    {
        void SetData();
    }
    public class BallView : MonoBehaviour, ISkinChangeable, ISubComponent
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private RectTransform _root;
 
        BallSkin _ballSkin;

        public void SetData()
        {
            _ball.OnPointsChanged += Ball_OnPointsChanged;
            _ball.OnSelectedChanged += Ball_OnSelectedChanged;
            _ball.OnTransparencyChanged += Ball_TransparencyChanged;

            ChangeSkin(_ball.Field.Scene.ActiveSkin);
        }

        public void ChangeSkin(SkinContainer container)
        {
            if (_ballSkin != null)
                Destroy(_ballSkin.gameObject);

            var skin = container.GetSkin($"ball") as BallSkin;
            _ballSkin = Object.Instantiate(skin, _root);
            _ballSkin.transform.localPosition = Vector3.zero;
            _ballSkin.transform.localRotation = Quaternion.identity;
           
            Ball_OnPointsChanged(_ball.Points);
            Ball_OnSelectedChanged();
            Ball_TransparencyChanged();
        }
        
        private void Ball_OnSelectedChanged()
        {
            _ballSkin.Selected = _ball.Selected;
        }

        private void Ball_OnPointsChanged(int oldPoints)
        {
            _ballSkin.Points = _ball.Points;
        }
        
        private void Ball_TransparencyChanged()
        {
            _ballSkin.Transparency = _ball.Transparency;
        }
    }
}