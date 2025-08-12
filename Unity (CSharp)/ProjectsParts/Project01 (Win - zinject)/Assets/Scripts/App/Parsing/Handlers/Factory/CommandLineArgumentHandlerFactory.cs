using System;
using App.Parsing.Handlers.Base;
using Zenject;

namespace App.Parsing.Handlers.Factory
{
    public class CommandLineArgumentHandlerFactory : ICommandLineArgumentHandlerFactory
    {
        private readonly DiContainer _container;
        
        public CommandLineArgumentHandlerFactory(DiContainer container) => _container = container;
        
        public ICommandLineArgumentHandler Create(Type type) => 
            (ICommandLineArgumentHandler) _container.Instantiate(type);
    }
}