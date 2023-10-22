using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Goals
{
    public class CastlePartDesc : MonoBehaviour
    {
        [SerializeField] private int _cost;
        [SerializeField] private Sprite _sprite;
        
        public int Cost => _cost;
        public Sprite Image => GetComponent<Image>().sprite;
        
    }
}