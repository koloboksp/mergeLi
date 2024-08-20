using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutCorrector : MonoBehaviour
{
    [SerializeField] private LayoutPattern pattern;

    [SerializeField] private Image panel;
    [SerializeField] private Text title;
    [SerializeField] private List<Image> buttons;

    public void Correct()
    {
        if (Application.isPlaying)
            return;
        
        if (pattern == null)
            return;

        // Set Panel
        float panelWidth = pattern.buttonSize.x + pattern.panelPadding * 2;
        float panelHeight = pattern.titleHeight + buttons.Count * pattern.buttonSize.y + pattern.panelPadding * 2;
        SetRT((RectTransform)panel.transform, 
            new Vector2(panelWidth, panelHeight), Vector2.zero, false);

        panel.color = Color.white;
        panel.sprite = pattern.panelImg;
        panel.type = Image.Type.Sliced;


        // Set Title
        CorrectText(title, pattern.titleTextSize);
        SetRT((RectTransform)title.transform, new Vector2(panelWidth, pattern.titleHeight), Vector2.down * pattern.panelPadding);

        // Set Buttons
        for(int i = 0; i < buttons.Count; i++)
        {
            var b = buttons[i];

            float buttonPosY = pattern.titleHeight + pattern.panelPadding + i * pattern.buttonSize.y;
            SetRT((RectTransform)b.transform, pattern.buttonSize, new Vector2(0, -buttonPosY));

            b.color = Color.white;
            b.sprite = pattern.buttonImg;
            b.type = Image.Type.Sliced;

            CorrectText(b.GetComponentInChildren<Text>(), pattern.buttonTextSize);
        }
    }

    private void CorrectText(Text text, int size)
    {
        if (text == null)
            return;

        text.font = pattern.font;
        text.fontSize = size;
        text.color = pattern.textColor;
        
        var outline = text.GetComponent<Outline>();
        if (pattern.textOutlineSize > 0)
        {
            if (outline == null)
                outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = pattern.textOutlineColor;
            outline.effectDistance = Vector2.one * pattern.textOutlineSize;
        }
        else
        {
            if (outline != null)
                DestroyImmediate(outline);
        }
    }

    private void SetRT(RectTransform rt, Vector2 size, Vector2 pos, bool alignTop = true)
    {
        // align top center
        var anc = new Vector2(.5f, alignTop ? 1f : .5f);

        rt.anchorMin = anc;
        rt.anchorMax = anc;
        rt.pivot = anc;

        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
    }
}