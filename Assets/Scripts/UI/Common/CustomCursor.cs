using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private Sprite _cursorNormal;
        [SerializeField] private Sprite _cursorClick;

        [SerializeField] private Image _cursorIcon;
        [SerializeField] private RectTransform _cursorTransform;

        private bool _leftMouseDown = false;
        
        private void OnEnable()
        {
#if UNITY_ANDROID
            gameObject.SetActive(false);
#else
            Cursor.visible = false;
#endif
        }

        private void OnDisable()
        {
#if UNITY_ANDROID
#else
            Cursor.visible = true;
#endif
        }

        void Update()
        {
            var screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _cursorTransform.anchoredPosition = _cursorTransform.parent.InverseTransformPoint(screenToWorldPoint);
            if (Input.GetMouseButton(0))
            {
                _cursorIcon.sprite = _cursorClick;
            }
            else
            {
                
                _cursorIcon.sprite = _cursorNormal;
            }
        }
    }
}