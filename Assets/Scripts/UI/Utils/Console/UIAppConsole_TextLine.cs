using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public event Action<UIAppConsole_TextLine> OnClick;

    string mFullText;
    string mCaption;
    bool mDragged;
    bool mCanExpand;
       
    public RectTransform Root;

    public Text TimeTxt;
    public Text MessageTypeTxt;
    public Image CanExpandIcon;
    public Text TextTxt;

    public string FullText => mFullText;
    public string Time { set => TimeTxt.text = value; }
      
    public void SetText(string text, LogType logType)
    {  
        var messageColor = mLogTypeTextAndColorAssociations[logType].Value;

        mFullText = text;
        mCaption = GetCaption(TextTxt, mFullText);

        TextTxt.text = mCaption;
        TextTxt.color = messageColor;

        MessageTypeTxt.text = mLogTypeTextAndColorAssociations[logType].Key;
        MessageTypeTxt.color = messageColor;

        var preferredHeight = UIAppConsole.CalculatePreferredHeight(TextTxt, mFullText);
        mCanExpand = preferredHeight > Root.rect.size.y;
        CanExpandIcon.gameObject.SetActive(mCanExpand);
    }

    string GetCaption(Text target, string text)
    {
        target.font.RequestCharactersInTexture(text, target.fontSize, target.fontStyle);
        float maxTextWidth = target.rectTransform.rect.size.x;

        float textWidth = 0;
        for (var cIndex = 0; cIndex < text.Length; cIndex++)
        {
            var c = text[cIndex];
            CharacterInfo charInfo;
            target.font.GetCharacterInfo(c, out charInfo, target.fontSize, target.fontStyle);
			
            if (textWidth + charInfo.advance > maxTextWidth || c == '\n')
            {
                string caption = text.Substring(0, cIndex - 1);
                return caption;
            }
            else
            {
                textWidth += charInfo.advance;
            }
        }
			
        return text;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!mDragged)
        {   
            OnClick?.Invoke(this);
        }     
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mDragged = false;
    }

    public void ActualizeWidth(float rectWidth)
    {
        Root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectWidth);
    }

    static readonly List<IDragHandler> mNoAllocPassDragHandlers = new List<IDragHandler>();
    static readonly List<IBeginDragHandler> mNoAllocPassBeginDragHandlers = new List<IBeginDragHandler>();
    static readonly List<IEndDragHandler> mNoAllocPassEndDragHandlers = new List<IEndDragHandler>();

    public void OnDrag(PointerEventData eventData)
    {
        mDragged = true;

        mNoAllocPassDragHandlers.Clear();
        gameObject.GetComponentsInParent<IDragHandler>(false, mNoAllocPassDragHandlers);
        if (mNoAllocPassDragHandlers.Count >= 2)
            mNoAllocPassDragHandlers[1].OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mNoAllocPassBeginDragHandlers.Clear();
        gameObject.GetComponentsInParent<IBeginDragHandler>(false, mNoAllocPassBeginDragHandlers);
        if (mNoAllocPassBeginDragHandlers.Count >= 2)
            mNoAllocPassBeginDragHandlers[1].OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mNoAllocPassEndDragHandlers.Clear();
        gameObject.GetComponentsInParent<IEndDragHandler>(false, mNoAllocPassEndDragHandlers);
        if (mNoAllocPassEndDragHandlers.Count >= 2)
            mNoAllocPassEndDragHandlers[1].OnEndDrag(eventData);
    }
}