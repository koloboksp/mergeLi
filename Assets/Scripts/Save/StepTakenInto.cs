using System;
using Core;
using Core.Gameplay;

namespace Save
{
    [Serializable]
    public class StepTakenInto
    {
        public StepTag StepTag;
        public int Count;
    }
}