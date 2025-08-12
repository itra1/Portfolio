using System;
using System.Collections.Generic;
using com.ootii.Messages;
using Core.Common.Consts;
using Core.Configs;
using Core.Configs.Consts;
using Core.Materials.Parsing;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Http;
using Core.Options;
using Core.Workers.Material.Coordinator;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Loading.Loader.Command.Factory
{
    public class MaterialDataLoadingCommandFactory : IMaterialDataLoadingCommandFactory, IDisposable
    {
        private readonly IConfig _config;
        private readonly IApplicationOptions _options;
        private readonly IHttpRequest _request;
        private readonly IMaterialDataParser _parser;
        private readonly IMaterialDataStorage _materials;
        private readonly IMaterialWorkerCoordinator _workerCoordinator;
        
        private Stack<MaterialDataLoadingCommand> _pool;
        private int _maxRequestAttempts;

        public MaterialDataLoadingCommandFactory(IConfig config,
            IApplicationOptions options, 
            IHttpRequest request, 
            IMaterialDataParser parser,
            IMaterialDataStorage materials,
            IMaterialWorkerCoordinator workerCoordinator)
        {
            _config = config;
            _options = options;
            _request = request;
            _parser = parser;
            _materials = materials;
            _workerCoordinator = workerCoordinator;
            
            _pool = new Stack<MaterialDataLoadingCommand>();
            
            if (!config.IsLoaded)
                MessageDispatcher.AddListener(MessageType.ConfigLoad, OnConfigLoaded);
            else
                AttemptToParseMaxRequestAttempts();
        }

        public MaterialDataLoadingCommand Create()
        {
            if (_pool == null)
            {
                Debug.LogError("Commands pool is disposed");
                return null;
            }
            
            if (!_pool.TryPop(out var command))
                command = new MaterialDataLoadingCommand();
            
            command.Configure(_options, _request, _parser, _materials, _workerCoordinator, _maxRequestAttempts);

            return command;
        }
        
        public void Remove(MaterialDataLoadingCommand command) => _pool?.Push(command);
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);

            if (_pool != null)
            {
                _pool.Clear();
                _pool = null; 
            }
        }
        
        private void AttemptToParseMaxRequestAttempts()
        {
            if (_config.TryGetValue(ConfigKey.MaxMaterialRequestAttempts, out var rawValue) && int.TryParse(rawValue, out var value))
                _maxRequestAttempts = value;
            else 
                _maxRequestAttempts = DefaultValue.MaxMaterialRequestAttempts;
        }
        
        private void OnConfigLoaded(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            AttemptToParseMaxRequestAttempts();
        }
    }
}