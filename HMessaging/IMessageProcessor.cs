using System.Threading.Tasks;

namespace HServer.HMessaging
{
    public interface IMessageProcessor
    {
        Task ProcessMessageTask(HConnection connection, byte[] message);
    }
}