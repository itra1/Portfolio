namespace App.Parsing.Handlers.Base
{
    public abstract class CommandLineArgumentHandler
    {
        protected string Name { get; }
        
        protected CommandLineArgumentHandler(string name) =>
            Name = name;
    }
}