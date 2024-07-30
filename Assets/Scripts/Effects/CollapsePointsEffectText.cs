using Core.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.Effects
{
    public class CollapsePointsEffectText : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        
        [SerializeField] private Text _points;
        
        [SerializeField] private Text _extraPoints;
        
        [SerializeField] private Text _extraHatPoints;
        
        [SerializeField] private AnimationClip _baseClip;
        [SerializeField] private AnimationClip _baseExtraClip;
        [SerializeField] private AnimationClip _baseHatClip;
        [SerializeField] private AnimationClip _baseExtraHatClip;
        [SerializeField] private AnimationClip _hatClip;

        public void SetPoint(PointsDesc points)
        {
            _points.text = points.Points.ToString();
            _extraPoints.text = points.ExtraPoints.ToString();
            _extraHatPoints.text = points.HatPoints.ToString();
            
            if (points.Points > 0 && points.HatPoints > 0 && points.ExtraPoints > 0)
            {
                _animation.Play(_baseExtraHatClip.name);
            }
            else if (points.Points > 0 && points.HatPoints > 0 )
            {
                _animation.Play(_baseHatClip.name);
            }
            else if (points.Points > 0 && points.ExtraPoints > 0)
            {
                _animation.Play(_baseExtraClip.name);
            }
            else if (points.HatPoints > 0)
            {
                _animation.Play(_hatClip.name);
            }
            else if (points.Points > 0)
            {
                _animation.Play(_baseClip.name);
            }
        }
    }
}