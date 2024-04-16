using Core;
using UI.Common;
using UnityEngine;

public class UIClickBuff : UIBuff
{
    [SerializeField] private UIExtendedButton _clickArea;
    [SerializeField] private AudioClip _onClickClip;

    private DependencyHolder<SoundsPlayer> _soundPlayer;

    protected void Awake()
    {
        _clickArea.onClick.AddListener(ClickArea_OnClick);
        _clickArea.onClickFail.AddListener(ClickArea_OnFail);
    }

    private void ClickArea_OnClick()
    {
        if (!ParentGroupAllowsInteraction())
            return;
        
        if (!ShowShopScreenRequired())
        {
            _soundPlayer.Value.Play(_onClickClip);
            _onClick?.Invoke();
        }
    }

    private void ClickArea_OnFail()
    {
        _soundPlayer.Value.Play(UICommonSounds.Unavailable);
    }
}