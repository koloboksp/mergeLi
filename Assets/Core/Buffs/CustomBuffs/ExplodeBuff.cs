namespace Core.Buffs
{
    public class ExplodeBuff : AreaEffect
    {
        protected override bool InnerProcessUsing()
        {
            _gameProcessor.UseExplodeBuff(Cost, AffectedAreas);
            return true;
        }
    }
}