using System.Collections;
using UnityEngine;

namespace Core
{
    public sealed class WaitForAny : CustomYieldInstruction
    {
        private readonly IEnumerator[] _enumerator;

        public override bool keepWaiting
        {
            get
            {
                var isWaiting = true;
                
                foreach (var enumerator in _enumerator)
                {
                    isWaiting &= enumerator.MoveNext();
                }

                return isWaiting;
            }
        }

        public WaitForAny(params IEnumerator[] enumerator)
        {
            _enumerator = enumerator;
        }
    }
}