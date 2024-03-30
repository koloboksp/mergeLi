using System.Threading.Tasks;
using Atom;

namespace Core
{
    public interface IAnalyticsController
    {
        Task InitializeAsync(Version version);

        void TutorialStepCompleted(string stepName);
    }
}