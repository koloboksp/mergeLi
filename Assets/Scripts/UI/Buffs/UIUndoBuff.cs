using Core;
using UnityEngine;
using UnityEngine.UI;

public class UIUndoBuff : UIBuff
{
    [SerializeField] private Button _clickArea;

    protected void Awake()
    { 
        _clickArea.onClick.AddListener(ClickArea_OnClick);
    }

    private void ClickArea_OnClick()
    {
        if(!ShowShopScreenRequired())
            _onClick?.Invoke();
    }
}