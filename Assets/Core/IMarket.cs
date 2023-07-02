using System;
using System.Threading.Tasks;

namespace Core
{
    public interface IMarket
    {
        public event Action<bool, string> OnBought;
        Task<bool> Buy(string inAppId);
    }
}