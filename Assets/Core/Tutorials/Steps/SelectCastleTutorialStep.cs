using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class SelectCastleTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var castle = Tutorial.Controller.GameProcessor.CastleSelector.Library.Castles.First();
            Tutorial.Controller.GameProcessor.CastleSelector.SelectActiveCastle(castle.Id);
           
            return true;
        }
    }
}