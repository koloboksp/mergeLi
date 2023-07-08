using UnityEngine;
using UnityEngine.UI;

public class UIShowNextBallsBuff : UIBuff
{
    [SerializeField] private Button _clickArea;

    private void Awake()
    {
        _clickArea.onClick.AddListener(ClickArea_OnClick);
    }

    protected override void AvailableStateChanged()
    {
        _clickArea.interactable = _model.Available;
    }

    private void ClickArea_OnClick()
    {
        if(!ShowShopScreenRequired())
            _onClick?.Invoke();
    }
}