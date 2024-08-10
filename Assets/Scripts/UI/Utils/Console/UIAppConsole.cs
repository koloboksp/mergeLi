using System;
using System.Collections.Generic;
using Core;
using Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAppConsole : MonoBehaviour
{
    public event Action<UIAppConsole> OnUnreadMessagesCountChanged;

    readonly List<TextLineInfo> mTextLinesInstances = new();
    readonly List<TextLineInfo> mMessagesOrder = new();
    readonly List<AppConsole.MessageInfo> mNewMessagesBetweenFrames = new();
    readonly List<AppConsole.MessageInfo> mNewMessagesBetweenFramesCopy = new();
       
    bool mActiveState;

    int mUnreadInfoMessagesCount;
    int mUnreadWarningMessagesCount;
    int mUnreadErrorMessagesCount;

    public RectTransform Root;
    public UIAppConsole_TextLine TextLinePrefab;
    public RectTransform ContentRoot;

    public GameObject DetailedInfoPanel;
    public Text DetailedInfoTime;
    public Text DetailedInfoMessageType;
    public ScrollRect DetailedInfoTextScrollRect;
    public Text DetailedInfoText;
    public UISimpleClickArea CloseDetailedInfoPanelBtn;
    
    public int MaxMessagesCount = 100;
  
    public bool ActiveState => mActiveState;
    public int UnreadInfoMessagesCount => mUnreadInfoMessagesCount;
    public int UnreadWarningMessagesCount => mUnreadWarningMessagesCount;
    public int UnreadErrorMessagesCount => mUnreadErrorMessagesCount;

    public void Awake()
    {
        AppConsole.OnLogMessage += AppConsole_OnLogMessageInNonActiveState;
      
        CloseDetailedInfoPanelBtn.OnClick += CloseDetailedInfoPanelBtn_OnClick;
        var messages = new List<AppConsole.MessageInfo>();
        AppConsole.ReadLastMessages(messages);
        foreach (var message in messages)
            AppConsole_OnLogMessageInNonActiveState(message);
        
        DetailedInfoPanel.SetActive(false);
    }
    
    void Update()
    {
        lock (mNewMessagesBetweenFrames)
        {
            mNewMessagesBetweenFramesCopy.Clear();
            mNewMessagesBetweenFramesCopy.AddRange(mNewMessagesBetweenFrames);
            mNewMessagesBetweenFrames.Clear();
        }

        if (mNewMessagesBetweenFramesCopy.Count > 0)
        {
            int startIndex = Mathf.Max(mNewMessagesBetweenFramesCopy.Count - MaxMessagesCount, 0); 
            for (var mIndex = startIndex; mIndex < mNewMessagesBetweenFramesCopy.Count; mIndex++)
                ProcessMessage(mNewMessagesBetweenFramesCopy[mIndex]);

            UpdateMessagesPositions();

            mNewMessagesBetweenFramesCopy.Clear();
        }
    }

    void ProcessMessage(AppConsole.MessageInfo message)
    {
        var textLineInfo = new TextLineInfo();
        
        mMessagesOrder.Add(textLineInfo);
        if (mMessagesOrder.Count > MaxMessagesCount)
        {
            var textLinesInstance = mTextLinesInstances[0];
            textLinesInstance.Instance.gameObject.SetActive(false);
            
            mMessagesOrder.RemoveAt(0);
        }

        if (textLineInfo.Instance == null)
        {
            textLineInfo.Instance = GameObject.Instantiate(TextLinePrefab, ContentRoot.transform);
            textLineInfo.Instance.OnClick += TextLineInstance_OnClick;         
        }

        textLineInfo.Instance.ActualizeWidth(ContentRoot.rect.width);

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
        for (int invIndex = mMessagesOrder.Count - 1, index = 0; invIndex >= 0; invIndex--, index++)
        {
            var textLineInfo = mMessagesOrder[invIndex];

            float textLineHeight = textLineInfo.Instance.Root.rect.height;
            textLineInfo.Instance.Root.anchoredPosition = new Vector2(0, contentHeight);

            contentHeight += textLineHeight;
        }

        ContentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);      
    }

    private void ClearMessages()
    {
        while (mTextLinesInstances.Count > 0)
        {
            var textLinesInstance = mTextLinesInstances[0];
            textLinesInstance.Instance.gameObject.SetActive(false);
        }

        mMessagesOrder.Clear();
    }

    public void ChangeActiveState(bool activeState)
    {
        mActiveState = activeState;

        if (activeState)
        {
            lock (mNewMessagesBetweenFrames)
            {
                mNewMessagesBetweenFrames.Clear();
                AppConsole.ReadLastMessages(mNewMessagesBetweenFrames, MaxMessagesCount);         
            }

            mUnreadInfoMessagesCount = 0;
            mUnreadWarningMessagesCount = 0;
            mUnreadErrorMessagesCount = 0;
            OnUnreadMessagesCountChanged?.Invoke(this);

            AppConsole.OnLogMessage += AppConsole_OnLogMessageInActiveState;
            AppConsole.OnLogMessage -= AppConsole_OnLogMessageInNonActiveState;

            gameObject.SetActive(true);
            Root.anchoredPosition = new Vector2(0, 0);
            DetailedInfoPanel.SetActive(false);
        }
        else
        {
            AppConsole.OnLogMessage -= AppConsole_OnLogMessageInActiveState;
            AppConsole.OnLogMessage += AppConsole_OnLogMessageInNonActiveState;

            gameObject.SetActive(false);
            Root.anchoredPosition = new Vector2(0, Root.rect.height);

            ClearMessages();
        }        
    }

    void AppConsole_OnLogMessageInActiveState(AppConsole.MessageInfo message)
    {
        lock (mNewMessagesBetweenFrames)
        {
            mNewMessagesBetweenFrames.Add(message);
        }
    }

    void AppConsole_OnLogMessageInNonActiveState(AppConsole.MessageInfo message)
    {          
        if (message.LogType == LogType.Log)
            mUnreadInfoMessagesCount++;
        if (message.LogType == LogType.Warning)
            mUnreadWarningMessagesCount++;
        if (message.LogType == LogType.Assert || message.LogType == LogType.Error || message.LogType == LogType.Exception)
            mUnreadErrorMessagesCount++;

        OnUnreadMessagesCountChanged?.Invoke(this);         
    }

    void TextLineInstance_OnClick(UIAppConsole_TextLine sender)
    {
        DetailedInfoPanel.SetActive(true);
        DetailedInfoTime.text = sender.TimeTxt.text;
        DetailedInfoMessageType.text = sender.MessageTypeTxt.text;
        DetailedInfoMessageType.color = sender.MessageTypeTxt.color;

        DetailedInfoText.text = sender.FullText;
        DetailedInfoText.color = sender.TextTxt.color;

        DetailedInfoTextScrollRect.verticalNormalizedPosition = 1;
        DetailedInfoTextScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CalculatePreferredHeight(DetailedInfoText, sender.FullText));
    }

    void CloseDetailedInfoPanelBtn_OnClick(UISimpleClickArea sender)
    {
        DetailedInfoPanel.SetActive(false);
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
    }
}

public interface IDragHandlerPass : IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        
    }