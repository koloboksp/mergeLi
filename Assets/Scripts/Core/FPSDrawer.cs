using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class FPSDrawer : MonoBehaviour
    {
        private const int DIGITS_NUM = 4;
        private const int FRAMETIME_LABEL_SIZE = 3;
        private const int FPS_LABEL_SIZE = 3;
        private static readonly char[] _numAsChar = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        [SerializeField] private Text _text;
        [SerializeField] private bool _showFrameTime;
    
        private float _updateTime = 1.0f;
        private float _averageTimer;
        private int _frameCount;
        private float _averageTime;
        private float _averageFPS;
    
        private StringBuilder _labelBuilder;
       
        private void Start()
        {
            if (_text == null)
            {
                var foundCanvas = FindObjectOfType<Canvas>();
                if (foundCanvas != null)
                {
                    GameObject fpsDrawer = new GameObject("FPS", typeof(RectTransform));
                    fpsDrawer.transform.SetParent(foundCanvas.transform);
                    var rectTransform = fpsDrawer.GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 1);
                    rectTransform.anchoredPosition = Vector2.zero;

                    _text = fpsDrawer.AddComponent<Text>();
                    _text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    _text.color = Color.yellow;
                    _text.text = "";
                    _text.fontSize = 45;
                    _text.raycastTarget = false;
                }
                else
                {
                    Debug.LogWarning("Can't display fps. Object with component 'Canvas' not found.");
                }
            }

            if(_showFrameTime)
                _labelBuilder = new StringBuilder("fps0000#ft0000", FPS_LABEL_SIZE + DIGITS_NUM + FRAMETIME_LABEL_SIZE + DIGITS_NUM);
            else
                _labelBuilder = new StringBuilder("fps0000", FPS_LABEL_SIZE + DIGITS_NUM);
        }

        private void IntToString(int value, int maxDigits, int offset, StringBuilder result)
        {
            var number = value;
            var digitsCount = 0;
            while (number > 0)
            {
                number = number / 10;
                digitsCount++;
            }

            number = value;

            for (var dI = 0; dI < digitsCount; dI++)
            {
                var rest = number % 10;
                number = number / 10;
                if (dI < maxDigits)
                    result[maxDigits - 1 - dI + offset] = _numAsChar[rest];
            }

            if (digitsCount < maxDigits)
            {
                for (var i = digitsCount; i < maxDigits; i++)
                    result[maxDigits - (i + 1) + offset] = _numAsChar[0];
            }
        }
    
        private void Update()
        {
            _averageTimer += GetClampedUnscaledDeltaTime();
            _frameCount += 1;

            if (_averageTimer >= _updateTime)
            {
                _averageTime = _averageTimer / _frameCount;
                _averageFPS = _frameCount / _updateTime;
            
                _averageTimer = _averageTimer - _updateTime;
                _frameCount = 0;

                if (_text != null)
                {
                    var ms = Mathf.RoundToInt(_averageTime * 1000.0f);
                    var fps = Mathf.RoundToInt(_averageFPS);
                    var offset = FPS_LABEL_SIZE;
                    
                    IntToString(fps, DIGITS_NUM, offset, _labelBuilder);
                    offset += DIGITS_NUM ;
                    if (_showFrameTime)
                    {
                        offset += FRAMETIME_LABEL_SIZE;
                        IntToString(ms, DIGITS_NUM, offset, _labelBuilder);
                    } 
                    _text.text = _labelBuilder.ToString();
                }
            }
        }

        static float GetClampedUnscaledDeltaTime()
        {
            var clampedUnscaledDeltaTime = Time.unscaledDeltaTime;
            if (clampedUnscaledDeltaTime > Time.maximumDeltaTime)
                clampedUnscaledDeltaTime = Time.maximumDeltaTime;

            return clampedUnscaledDeltaTime;
        }
    }
}