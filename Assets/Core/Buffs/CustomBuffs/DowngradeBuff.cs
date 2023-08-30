using System.Collections.Generic;
using UnityEngine;

namespace Core.Buffs
{
    public class DowngradeBuff : AreaEffect
    {
        protected override bool InnerProcessUsing()
        {
            return _gameProcessor.UseDowngradeBuff(Cost, AffectedAreas);
        }
    }
}