using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Goals
{
    public class CastleView : MonoBehaviour
    {
        [SerializeField] private Castle _model;
        [SerializeField] private RectTransform _root;
        
        [SerializeField] private CastleSettings castleSettings;
        
        private List<(int, Task)> _operations = new List<(int, Task)>();
            
        public RectTransform Root => _root;
        
    }
}