using UnityEngine;
using UnityEngine.UI;

namespace Core.Effects
{
    public class CollapsePointsEffectText : MonoBehaviour
    {
        [SerializeField] private Text _points;
        [SerializeField] private GameObject _extraPointsPanel;
        [SerializeField] private Text _extraPoints;
        [SerializeField] private GameObject _extraHatPointsPanel;
        [SerializeField] private Text _extraHatPoints;
        
        public void SetPoint(PointsDesc points)
        {
            _points.text = points.Points.ToString();
            
            if (points.ExtraPoints > 0)
            {
                _extraPointsPanel.SetActive(true);
                _extraPoints.text = points.ExtraPoints.ToString();
            }
            else
            {
                _extraPointsPanel.SetActive(false);
            }

            if (points.HatPoints > 0)
            {
                _extraHatPointsPanel.SetActive(true);
                _extraHatPoints.text = points.HatPoints.ToString();
            }
            else
            {
                _extraHatPointsPanel.SetActive(false);
            }
        }
    }
}