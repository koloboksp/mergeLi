namespace Core.Tutorials
{
    public class StartTutorialEntry : TutorialEntry
    {
        public override bool CanStart(bool forceStart)
        {
            return !ApplicationController.Instance.SaveController.SaveProgress.IsTutorialComplete(Owner.Id) || forceStart;
        }
    }
}