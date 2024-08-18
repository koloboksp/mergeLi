using UnityEngine;

namespace Core.Ads
{
    public class AdsBanner : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        
        private IAdsViewer _adsViewer;

        public RectTransform Root => _root;

        public void SetData(IAdsViewer adsViewer)
        {
            _adsViewer = adsViewer;
        }

        public void Activate()
        {
            _adsViewer.AddBanner(this);
        }
        
        public void Deactivate()
        {
            _adsViewer.RemoveBanner(this);
        }
    }
}