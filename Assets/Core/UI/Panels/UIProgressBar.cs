using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private RectTransform _barRect;
        [SerializeField] private UIProgressBarMark _markPrefab;

        private List<UIProgressBarMark> _markInstances = new();

        private void Awake()
        {
            _markPrefab.gameObject.SetActive(false);
        }

        public void SetProgress(float nValue)
        {
            _barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rect.rect.width * nValue);
        }

        public void SetMarks(IEnumerable<int> marks)
        {
            foreach (var markInstance in _markInstances)
                Destroy(markInstance.gameObject);
            _markInstances.Clear();
            
            var pointSum = marks.Sum(i => i);
            var absoluteMark = 0;
            foreach (var mark in marks)
            {
                absoluteMark += mark;
                var instantiate = Instantiate(_markPrefab, _rect);
                instantiate.gameObject.SetActive(true);
                instantiate.Root.anchoredPosition = new Vector2(((float)absoluteMark / pointSum) * _rect.rect.width, 0);
                _markInstances.Add(instantiate);
            }
        }
    }
}