#if UNITY_WEBGL
using Core.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Core
{
    public class UIYGLeaderboardPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private LeaderboardYG _leaderboard;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            var data = undefinedData as UIYGLeaderboardPanelData;
            _leaderboard.nameLB = data.Id;
            _leaderboard.UpdateLB();
        }
    }

    public class UIYGLeaderboardPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public string Id { get; set; }
    }

}
#endif