using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Utils.Console
{
    public class UIAppConsole_TextLine : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandlerPass
    {
        static readonly Dictionary<LogType, KeyValuePair<string, Color>> mLogTypeTextAndColorAssociations = new Dictionary<LogType, KeyValuePair<string, Color>>()
        {
            { LogType.Log, new KeyValuePair<string, Color>("log", Color.white)},
            { LogType.Warning, new KeyValuePair<string, Color>("war", Color.yellow)},
            { LogType.Assert,new KeyValuePair<string, Color>("a", Color.red)},
            { LogType.Error, new KeyValuePair<string, Color>("er", Color.red)},
            { LogType.Exception, new KeyValuePair<string, Color>("ex", Color.red)},
        };
        static readonly List<IDragHandler> _noAllocPassDragHandlers = new();
        static readonly List<IBeginDragHandler> _noAllocPassBeginDragHandlers = new();
        static readonly List<IEndDragHandler> _noAllocPassEndDragHandlers = new();

        public event Action<UIAppConsole_TextLine> OnClick;

        [SerializeField] private RectTransform _root;
        [SerializeField] private Text _timeTxt;
        [SerializeField] private Text _messageTypeTxt;
        [SerializeField] private Image _canExpandIcon;
        [SerializeField] private Text _textTxt;
        
        private string _fullText;
        private string _caption;
        private bool _dragged;
        private bool _canExpand;
        private Color _textColor;
        private string _messageTypeText;

        public RectTransform Root => _root;
        public string FullText => _fullText;
        public string Caption => _caption;
        public Color TextColor => _textColor;
        public string MessageTypeText => _messageTypeText;

        public string Time
        {
            set => _timeTxt.text = value;
            get => _timeTxt.text;
        }
      
        public void SetText(string text, LogType logType)
        {  
            _textColor = mLogTypeTextAndColorAssociations[logType].Value;
            _messageTypeText = mLogTypeTextAndColorAssociations[logType].Key;
            _fullText = text;
            _caption = _fullText;//GetCaption(_textTxt, _fullText);

            _textTxt.text = _caption;
            _textTxt.color = _textColor;

            _messageTypeTxt.text = _messageTypeText;
            _messageTypeTxt.color = _textColor;

            var preferredHeight = UIAppConsole.CalculatePreferredHeight(_textTxt, _fullText);
            _canExpand = preferredHeight > _root.rect.size.y;
            _canExpandIcon.gameObject.SetActive(_canExpand);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_dragged)
            {   
                OnClick?.Invoke(this);
            }     
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragged = false;
        }

        public void ActualizeWidth(float rectWidth)
        {
            _root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectWidth);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _dragged = true;

            _noAllocPassDragHandlers.Clear();
            gameObject.GetComponentsInParent<IDragHandler>(false, _noAllocPassDragHandlers);
            if (_noAllocPassDragHandlers.Count >= 2)
                _noAllocPassDragHandlers[1].OnDrag(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _noAllocPassBeginDragHandlers.Clear();
            gameObject.GetComponentsInParent<IBeginDragHandler>(false, _noAllocPassBeginDragHandlers);
            if (_noAllocPassBeginDragHandlers.Count >= 2)
                _noAllocPassBeginDragHandlers[1].OnBeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _noAllocPassEndDragHandlers.Clear();
            gameObject.GetComponentsInParent<IEndDragHandler>(false, _noAllocPassEndDragHandlers);
            if (_noAllocPassEndDragHandlers.Count >= 2)
                _noAllocPassEndDragHandlers[1].OnEndDrag(eventData);
        }
    }
}