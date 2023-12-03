using System.Threading.Tasks;

namespace Core
{
    public interface ISocialService
    {
        Task Authentication();
        
        bool IsAuthenticated();
    }
}