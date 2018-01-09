namespace CoreServer.HMessaging
{
    public interface ICommandRegistry
    {
        void RegisterCommand(HCommandIdentifier identifier, IServerCommand command);

        IServerCommand GetCommand(HCommandIdentifier identifier);
    }
}