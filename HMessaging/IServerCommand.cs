using System.Threading.Tasks;
using ChatProtos.Networking;

namespace CoreServer.HMessaging
{
    public interface IServerCommand
    {
        Task Execute(HConnection client, RequestMessage message);
    }
}
