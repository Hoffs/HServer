using System.Threading.Tasks;

namespace CoreServer.HMessaging
{
    public interface IMessageProcessor
    {
        Task ProcessMessageTask(HConnection connection, byte[] message);
    }
}