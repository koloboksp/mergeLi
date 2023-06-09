﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class RemoveGeneratedItemsOperation : Operation
    {
        private readonly List<Vector3Int> _itemsPositions;
        private readonly IField _field;

        private readonly List<Ball> _balls = new();
        
        public RemoveGeneratedItemsOperation(IEnumerable<Vector3Int> itemsPositions, IField field)
        {
            _itemsPositions = new List<Vector3Int>(itemsPositions);
            _field = field;
        }
    
        protected override void InnerExecute()
        {
            foreach (var itemPosition in _itemsPositions)
                _balls.AddRange(_field.GetSomething<Ball>(itemPosition));
            _field.DestroyBalls(_balls);
            
            Complete(null);
        }
    }
}