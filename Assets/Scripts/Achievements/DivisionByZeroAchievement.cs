using System.Linq;
using Core.Buffs;

namespace Achievements
{
    public class DivisionByZeroAchievement : Achievement
    {
        private DowngradeBuff _downgradeBuff;
        
        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            foreach (var buff in GameProcessor.Buffs)
            {
                if (buff is DowngradeBuff downgradeBuff)
                {
                    _downgradeBuff = downgradeBuff;
                    _downgradeBuff.OnInterruptUsing(OnInterruptUsing);
                }
                   
            }
        }

        private void OnInterruptUsing()
        {
            var balls = GameProcessor.GetBalls(_downgradeBuff.AffectedAreas);
            if (balls.Count == 0)
                return;

            var canDowngradeAny = balls.Any(i => i.Points == 0);
            if (canDowngradeAny)
                Unlock();
        }
    }
}