using System;
using System.Collections;
using UnityEngine;

namespace Core.Steps
{
    public class OperationWaiter : MonoBehaviour
    {
        public static OperationWaiter WaitForSecond(float time, Action<OperationWaiter> effectOnComplete)
        {
            GameObject go = new GameObject("Waiter");
            var operationWaiter = go.AddComponent<OperationWaiter>();
            operationWaiter.Run(time, effectOnComplete);
        
            return operationWaiter;
        }

        private void Run(float time, Action<OperationWaiter> onComplete)
        {
            StartCoroutine(Wait(time, onComplete));
        }

        private IEnumerator Wait(float time, Action<OperationWaiter> effectOnComplete)
        {
            yield return new WaitForSeconds(time);
            effectOnComplete?.Invoke(this);
        }
    }
}