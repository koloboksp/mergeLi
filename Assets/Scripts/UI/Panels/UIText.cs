using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIText : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private int _showingSpeed = 130;

        private StringBuilder _textBuilder;

        public async Task ShowAsync(
            string text,
            CancellationToken exitToken,
            CancellationToken cancellationToken)
        {
            _textBuilder ??= new StringBuilder();

            gameObject.SetActive(true);

            var textLength = text.Length;
            var showTime = (float)text.Length / _showingSpeed;
            var showTimer = 0.0f;
            var previousVisibleCharIndex = -1;

            while (showTimer < showTime)
            {
                exitToken.ThrowIfCancellationRequested();

                if (cancellationToken.IsCancellationRequested)
                    return;

                showTimer += Time.deltaTime;
                if (showTimer > showTime)
                    showTimer = showTime;

                var nTimer = showTimer / showTime;
                var visibleCharIndex = (int)Mathf.Lerp(0, textLength, nTimer);

                if (previousVisibleCharIndex != visibleCharIndex)
                {
                    previousVisibleCharIndex = visibleCharIndex;
                    _textBuilder.Clear();
                    _textBuilder.Append(text, 0, visibleCharIndex);
                    _textBuilder.Append("<color=#00000000>");
                    _textBuilder.Append(text, visibleCharIndex, textLength - visibleCharIndex);
                    _textBuilder.Append("</color>");
                    _text.text = _textBuilder.ToString();
                }

                await Task.Yield();
            }
        }
    }
}