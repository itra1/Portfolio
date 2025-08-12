namespace Core.Materials.Loading.Loader.Command.Factory
{
    public interface IMaterialDataLoadingCommandFactory
    {
        MaterialDataLoadingCommand Create();

        void Remove(MaterialDataLoadingCommand command);
    }
}