using System;
using App.Parsing.Handlers.Base;

namespace App.Parsing.Handlers.Factory
{
    public interface ICommandLineArgumentHandlerFactory
    {
        ICommandLineArgumentHandler Create(Type type);
    }
}