﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UITutorialDialog : MonoBehaviour
    {
        [SerializeField] private RectTransform _areaRoot;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Text _dialogText;
        [SerializeField] private int _showingSpeed = 30;
        [SerializeField] private RectTransform _bottomAnchor;
        [SerializeField] private RectTransform _centerAnchor;
        [SerializeField] private RectTransform _topAnchor;

        public async Task Show(string textKey)
        {
            gameObject.SetActive(true);
            await ShowEffect(textKey);
        }
        
        public void Move(DialogPosition dialogPosition)
        {
            if (dialogPosition == DialogPosition.Bottom)
                _root.transform.position = _bottomAnchor.transform.position;
            if (dialogPosition == DialogPosition.Center)
                _root.transform.position = _centerAnchor.transform.position;
            if (dialogPosition == DialogPosition.Top)
                _root.transform.position = _topAnchor.transform.position;

           // _root.anchoredPosition = _areaRoot.InverseTransformPoint(new Vector2(0, rect.position.y) + new Vector2(0, 0));
        }
        
        async Task ShowEffect(string text)
        {
            int textLenght = text.Length;
            float showTime = (float)text.Length / _showingSpeed;
            float showTimer = 0;
            while (showTimer < showTime)
            {
                showTimer += Time.deltaTime;
                if (showTimer > showTime)
                    showTimer = showTime;
                
                var nTimer = showTimer / showTime;
                var charNum = (int)Mathf.Lerp(0, textLenght, nTimer);
                var visiblePart = text.Substring(0, charNum);
                var unvisible = $"<color=#00000000>{text.Substring(charNum, textLenght - charNum)}</color>";
                _dialogText.text = $"{visiblePart}{unvisible}";
                await Task.Yield();
            }
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public enum DialogPosition
    {
        Bottom,
        Center,
        Top
    }
}