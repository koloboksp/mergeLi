using System;
using UnityEngine;

namespace Core.Ads
{
    public class AdsBanner : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        
        private IAdsViewer _adsViewer;

        public RectTransform Root => _root;

        private void OnEnable()
        {
            if (_adsViewer != null)
            {
                _adsViewer.AddBanner(this);
            }
        }

        private void OnDisable()
        {
            if (_adsViewer != null)
            {
                _adsViewer.RemoveBanner(this);
            }
        }

        public void SetData(IAdsViewer adsViewer)
        {
            _adsViewer = adsViewer;
        }

        public void Activate()
        {
            if (isActiveAndEnabled)
            {
                _adsViewer.AddBanner(this);
            }
        }
        
        public void Deactivate()
        {
            _adsViewer.RemoveBanner(this);
        }
    }
}