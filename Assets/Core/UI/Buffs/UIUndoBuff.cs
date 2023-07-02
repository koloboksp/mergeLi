using UnityEngine;
using UnityEngine.UI;

public class UIUndoBuff : UIBuff
{
    [SerializeField] private Button _clickArea;

    private void Awake()
    {
        _clickArea.onClick.AddListener(ClickArea_OnClick);
    }

    private void ClickArea_OnClick()
    {
        _onClick?.Invoke();
    }
}