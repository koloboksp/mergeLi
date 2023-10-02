namespace Core.Tutorials
{
    public class StartTutorialEntry : TutorialEntry
    {
        public override bool CanStart(bool forceStart)
        {
            return !Owner.Controller.GameProcessor.PlayerInfo.IsTutorialComplete(Owner.Id) || forceStart;
        }
    }
}