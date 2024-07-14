using Core.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Effects
{
    public class CollapsePointsEffectText : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        
        [SerializeField] private Text _points;
        [SerializeField] private AnimationClip _pointsClip;
        
        [SerializeField] private Text _extraPoints;
        [SerializeField] private AnimationClip _extraPointsClip;

        [SerializeField] private Text _extraHatPoints;
        [SerializeField] private AnimationClip _extraHatPointsClip;
        
        [SerializeField] private AnimationClip _extraAndExtraHatPointsClip;

        public void SetPoint(PointsDesc points)
        {
            _points.text = points.Points.ToString();
            _extraPoints.text = points.ExtraPoints.ToString();
            _extraHatPoints.text = points.HatPoints.ToString();

          
            if (points.HatPoints > 0 && points.ExtraPoints > 0)
            {
                _animation.Play(_extraAndExtraHatPointsClip.name);
            }
            else if (points.HatPoints > 0)
            {
                _animation.Play(_extraHatPointsClip.name);
            }
            else if (points.ExtraPoints > 0)
            {
                _animation.Play(_extraPointsClip.name);
            }
            else
            {
                _animation.Play(_pointsClip.name);
            }
        }
    }
}