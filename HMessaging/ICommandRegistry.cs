using System.Threading.Tasks;

namespace HServer.HMessaging
{
    public interface ICommandRegistry<T> where T : class
    {
        Task RegisterCommand(HCommandIdentifier identifier, T command);

        Task<T> GetCommand(HCommandIdentifier identifier);
    }
}