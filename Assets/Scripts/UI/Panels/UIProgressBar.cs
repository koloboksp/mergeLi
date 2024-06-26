﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private RectTransform _desiredRect;
        [SerializeField] private RectTransform _barRect;
        
        [SerializeField] private Text _valueLabel;
        [SerializeField] private Text _maxValueLabel;
        [SerializeField] private UIUpScaleEffect _scoreLabelUpScaleEffect;

        private int _maxValue;
        private int _desiredValue;
        private int _value;
        private float _fValue;

        private int _startValue;
        private float _time;
        private float _timer;
        
        public void InstantSet(int value, int maxValue)
        {   
            _maxValue = maxValue;
            _value = Mathf.Clamp(value, 0, _maxValue);
            _fValue = _value;
            _desiredValue = _value;
            
            UpdateDesiredValueBar();
            UpdateValueBar();

            UpdateValueLabels();
        }
        
        public void Set(float duration, int oldValue, int newValue, int maxValue)
        {
            _timer = 0.0f;
            _time = duration;

            _maxValue = maxValue;
            _value = Mathf.Clamp(oldValue, 0, _maxValue);
            _fValue = _value;
            _startValue = _value;
            _desiredValue = Mathf.Clamp(newValue, 0, _maxValue);
            
            UpdateDesiredValueBar();
            
            _scoreLabelUpScaleEffect.Add();
            
            enabled = true;
        }

        private void Update()
        {
            if (_timer < _time)
            {
                _timer += Time.deltaTime;
                
                var nTimer = Mathf.Clamp01(_timer / _time);
                _fValue = _startValue + (float)((_desiredValue - _startValue) * nTimer);
                _value = Mathf.FloorToInt(_fValue);
                
                UpdateValueBar();
                UpdateValueLabels();
            }
            else
            {
                enabled = false;
            }
        }
        
        private void UpdateDesiredValueBar()
        {
            var nDesiredValue = (float)_desiredValue / _maxValue;
            _desiredRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rect.rect.width * nDesiredValue);
        }
        
        private void UpdateValueBar()
        {
            var nValue = _fValue / _maxValue;
            _barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rect.rect.width * nValue);
        }
        
        private void UpdateValueLabels()
        {
            _valueLabel.text = _value.ToString();
            _maxValueLabel.text = _maxValue.ToString();
        }
    }
}