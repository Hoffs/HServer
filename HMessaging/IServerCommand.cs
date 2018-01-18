using System.Threading.Tasks;
using HServer.Networking;

namespace HServer.HMessaging
{
    public interface IServerCommand
    {
        Task Execute(HConnection client, RequestMessage message);
    }
}
