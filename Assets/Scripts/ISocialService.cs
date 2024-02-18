using System.Threading.Tasks;

namespace Core
{
    public interface ISocialService
    {
        bool IsAutoAuthenticationAvailable();
        Task Authenticate();
        bool IsAuthenticated();
      
    }
}