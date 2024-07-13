using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class AsyncExtensions
    {
        public static async Task WaitForSecondsAsync(float time, CancellationToken cancellationToken)
        {
            var timer = 0.0f;

            while (timer < time)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                timer += Time.deltaTime;
                await Task.Yield();
            }
        }
        public static async Task WaitForSecondsAsync(float time, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
        {
            var timer = 0.0f;

            while (timer < time)
            {
                cancellationToken1.ThrowIfCancellationRequested();
                cancellationToken2.ThrowIfCancellationRequested();

                timer += Time.deltaTime;
                await Task.Yield();
            }
        }
        
        public static async Task WaitForConditionAsync(Func<bool> predicate, CancellationToken cancellationToken)
        {
            while (predicate())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Yield();
            }
        }
    }
}