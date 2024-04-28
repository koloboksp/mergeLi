using UnityEngine;
using UnityEngine.UI;

namespace Core.Effects
{
    public class CollapsePointsEffectText : MonoBehaviour
    {
        [SerializeField] private Text _points;
        public string Text
        {
            set => _points.text = value;
        }
    }
}