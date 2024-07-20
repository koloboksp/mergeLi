using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Goals;
using UnityEngine;

namespace Core.Tutorials
{
    public class SelectCastleTutorialStep : TutorialStep
    {
        [SerializeField] private string _castleName;
        [SerializeField] private int _points;

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            Castle castle = null;
            var castleSelector = Tutorial.Controller.GameProcessor.CastleSelector;
            
            if (!string.IsNullOrEmpty(_castleName))
            {
                castle = castleSelector.Library.Castles
                    .FirstOrDefault(i => i.Id == _castleName);
            }

            if (castle == null)
            {
                castle = castleSelector.Library.Castles
                    .First();
            }

            castleSelector.SelectActiveCastle(castle.Id);
            castleSelector.ActiveCastle.SetPoints(_points, true);
            
            return true;
        }
    }
}