namespace Core.FileResources.Command.Factory
{
    public interface IResourceRequestCommandFactory
    {
        ResourceRequestCommand<TResource> Create<TResource>();

        void Remove<TResource>(ResourceRequestCommand<TResource> command);
    }
}