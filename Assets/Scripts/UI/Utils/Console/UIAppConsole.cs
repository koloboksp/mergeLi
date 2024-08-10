using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Utils.Console
{
    public class UIAppConsole : MonoBehaviour
    {
        public event Action<UIAppConsole> OnUnreadMessagesCountChanged;

        [SerializeField] private RectTransform _root;
        [SerializeField] private UIAppConsole_TextLine _textLinePrefab;
        [SerializeField] private RectTransform _contentRoot;

        [SerializeField] private GameObject _detailedInfoPanel;
        [SerializeField] private Text _detailedInfoTime;
        [SerializeField] private Text _detailedInfoMessageType;
        [SerializeField] private ScrollRect _detailedInfoTextScrollRect;
        [SerializeField] private Text _detailedInfoText;
        [SerializeField] private UISimpleClickArea _closeDetailedInfoPanelBtn;
    
        [SerializeField] private int _maxMessagesCount = 100;
        
        private readonly ReuseList<TextLineInfo> _textLinesInstances = new(() => new TextLineInfo());
        private readonly List<TextLineInfo> _messagesOrder = new();
        private readonly List<AppConsole.MessageInfo> _newMessagesBetweenFrames = new();
        private readonly List<AppConsole.MessageInfo> _newMessagesBetweenFramesCopy = new();
       
        private bool _activeState;

        private int _unreadInfoMessagesCount;
        private int _unreadWarningMessagesCount;
        private int _unreadErrorMessagesCount;
        
        public bool ActiveState => _activeState;
        public int UnreadInfoMessagesCount => _unreadInfoMessagesCount;
        public int UnreadWarningMessagesCount => _unreadWarningMessagesCount;
        public int UnreadErrorMessagesCount => _unreadErrorMessagesCount;

        public void Awake()
        {
            AppConsole.OnLogMessage += AppConsole_OnLogMessageInNonActiveState;
      
            _closeDetailedInfoPanelBtn.OnClick += CloseDetailedInfoPanelBtn_OnClick;
            var messages = new List<AppConsole.MessageInfo>();
            AppConsole.ReadLastMessages(messages);
            foreach (var message in messages)
                AppConsole_OnLogMessageInNonActiveState(message);
        
            _detailedInfoPanel.SetActive(false);
        }
    
        void Update()
        {
            lock (_newMessagesBetweenFrames)
            {
                _newMessagesBetweenFramesCopy.Clear();
                _newMessagesBetweenFramesCopy.AddRange(_newMessagesBetweenFrames);
                _newMessagesBetweenFrames.Clear();
            }

            if (_newMessagesBetweenFramesCopy.Count > 0)
            {
                int startIndex = Mathf.Max(_newMessagesBetweenFramesCopy.Count - _maxMessagesCount, 0); 
                for (var mIndex = startIndex; mIndex < _newMessagesBetweenFramesCopy.Count; mIndex++)
                    ProcessMessage(_newMessagesBetweenFramesCopy[mIndex]);

                UpdateMessagesPositions();

                _newMessagesBetweenFramesCopy.Clear();
            }
        }

        void ProcessMessage(AppConsole.MessageInfo message)
        {
            var textLineInfo = _textLinesInstances.Get(out var allocIndex);
            textLineInfo.AllocIndex = allocIndex;
            
            _messagesOrder.Add(textLineInfo);
            if (_messagesOrder.Count > _maxMessagesCount)
            {
                var textLinesInstance = _textLinesInstances[0];
                textLinesInstance.Instance.gameObject.SetActive(false);
            
                _textLinesInstances.Return(textLinesInstance.AllocIndex);
                _messagesOrder.RemoveAt(0);
            }

            if (textLineInfo.Instance == null)
            {
                textLineInfo.Instance = GameObject.Instantiate(_textLinePrefab, _contentRoot.transform);
                textLineInfo.Instance.OnClick += TextLineInstance_OnClick;         
            }

            textLineInfo.Instance.ActualizeWidth(_contentRoot.rect.width);

            var t = TimeSpan.FromSeconds(message.Time);

            var hours = t.Hours + t.Days * 24;
            var minutes = t.Minutes;
            var seconds = t.Seconds;
            var milliseconds = t.Milliseconds;

            var resultMessage = $"{message.Message} {message.StackTrace}";
            textLineInfo.Instance.Time = $"{hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}";

            //textLineInfo.Instance.Time = $"[{message.H.ToString("00")}:{message.M.ToString("00")}:{message.S.ToString("00.00").Replace(",", ".")}]";
            textLineInfo.Instance.SetText(resultMessage, message.LogType);
            textLineInfo.Instance.gameObject.SetActive(true);
        }

        
        void UpdateMessagesPositions()
        {
            float contentHeight = 0;
            for (int invIndex = _messagesOrder.Count - 1, index = 0; invIndex >= 0; invIndex--, index++)
            {
                var textLineInfo = _messagesOrder[invIndex];

                float textLineHeight = textLineInfo.Instance.Root.rect.height;
                textLineInfo.Instance.Root.anchoredPosition = new Vector2(0, contentHeight);

                contentHeight += textLineHeight;
            }

            _contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);      
        }

        private void ClearMessages()
        {
            while (_textLinesInstances.Count > 0)
            {
                var textLinesInstance = _textLinesInstances[0];
                textLinesInstance.Instance.gameObject.SetActive(false);
                
                _textLinesInstances.Return(textLinesInstance.AllocIndex);
            }

            _messagesOrder.Clear();
        }

        public void ChangeActiveState(bool activeState)
        {
            _activeState = activeState;

            if (activeState)
            {
                lock (_newMessagesBetweenFrames)
                {
                    _newMessagesBetweenFrames.Clear();
                    AppConsole.ReadLastMessages(_newMessagesBetweenFrames, _maxMessagesCount);         
                }

                _unreadInfoMessagesCount = 0;
                _unreadWarningMessagesCount = 0;
                _unreadErrorMessagesCount = 0;
                OnUnreadMessagesCountChanged?.Invoke(this);

                AppConsole.OnLogMessage += AppConsole_OnLogMessageInActiveState;
                AppConsole.OnLogMessage -= AppConsole_OnLogMessageInNonActiveState;

                _root.gameObject.SetActive(true);
                _root.anchoredPosition = new Vector2(0, 0);
                _detailedInfoPanel.SetActive(false);
            }
            else
            {
                AppConsole.OnLogMessage -= AppConsole_OnLogMessageInActiveState;
                AppConsole.OnLogMessage += AppConsole_OnLogMessageInNonActiveState;

                _root.gameObject.SetActive(false);
                _root.anchoredPosition = new Vector2(0, _root.rect.height);

                ClearMessages();
            }        
        }

        void AppConsole_OnLogMessageInActiveState(AppConsole.MessageInfo message)
        {
            lock (_newMessagesBetweenFrames)
            {
                _newMessagesBetweenFrames.Add(message);
            }
        }

        void AppConsole_OnLogMessageInNonActiveState(AppConsole.MessageInfo message)
        {          
            if (message.LogType == LogType.Log)
                _unreadInfoMessagesCount++;
            if (message.LogType == LogType.Warning)
                _unreadWarningMessagesCount++;
            if (message.LogType == LogType.Assert || message.LogType == LogType.Error || message.LogType == LogType.Exception)
                _unreadErrorMessagesCount++;

            OnUnreadMessagesCountChanged?.Invoke(this);         
        }

        void TextLineInstance_OnClick(UIAppConsole_TextLine sender)
        {
            _detailedInfoPanel.SetActive(true);
            _detailedInfoTime.text = sender.Time;
            _detailedInfoMessageType.text = sender.MessageTypeText;
            _detailedInfoMessageType.color = sender.TextColor;

            _detailedInfoText.text = sender.FullText;
            _detailedInfoText.color = sender.TextColor;

            _detailedInfoTextScrollRect.verticalNormalizedPosition = 1;
            _detailedInfoTextScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalculatePreferredHeight(_detailedInfoText, sender.FullText));
        }

        void CloseDetailedInfoPanelBtn_OnClick(UISimpleClickArea sender)
        {
            _detailedInfoPanel.SetActive(false);
        }

        public static float CalculatePreferredHeight(Text target, string text)
        {
            target.font.RequestCharactersInTexture(text, target.fontSize, target.fontStyle);

            var textGenerationSettings = target.GetGenerationSettings(target.rectTransform.rect.size);
            textGenerationSettings.scaleFactor = 1;
            var preferredHeight = target.cachedTextGenerator.GetPreferredHeight(text, textGenerationSettings);

            return preferredHeight;
        }

        public class TextLineInfo
        {
            public UIAppConsole_TextLine Instance;
            public int AllocIndex;
        }
    }

    public interface IDragHandlerPass : IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        
    }
}