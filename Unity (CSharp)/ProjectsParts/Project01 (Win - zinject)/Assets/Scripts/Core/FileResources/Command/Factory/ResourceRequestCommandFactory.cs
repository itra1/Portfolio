using System;
using System.Collections.Generic;
using com.ootii.Messages;
using Core.Common.Consts;
using Core.Configs;
using Core.Configs.Consts;
using Core.FileResources.Caching;
using Core.FileResources.Customizing;
using Core.Messages;
using Core.Network.Http;
using Core.Options;

namespace Core.FileResources.Command.Factory
{
    public class ResourceRequestCommandFactory : IResourceRequestCommandFactory, IDisposable
    {
        private readonly IConfig _config;
        private readonly IApplicationOptions _options;
        private readonly IResourceCustomizer _customizer;
        private readonly IHttpRequest _request;
        private readonly IResourceCachingService _caching;
        
        private IDictionary<object, Stack<IResourceRequestCommand>> _poolsByResource;
        private int _maxRequestAttempts;
        
        public ResourceRequestCommandFactory(IConfig config,
            IApplicationOptions options,
            IResourceCustomizer customizer,
            IHttpRequest request,
            IResourceCachingService caching)
        {
            _config = config;
            _options = options;
            _customizer = customizer;
            _request = request;
            _caching = caching;
            
            _poolsByResource = new Dictionary<object, Stack<IResourceRequestCommand>>();
            
            if (!config.IsLoaded)
                MessageDispatcher.AddListener(MessageType.ConfigLoad, OnConfigLoaded);
            else
                AttemptToParseMaxRequestAttempts();
        }
        
        public ResourceRequestCommand<TResource> Create<TResource>()
        {
            if (_poolsByResource == null)
                return null;
            
            if (!_poolsByResource.TryGetValue(typeof(TResource), out var pool)
                || !pool.TryPop(out var c)
                || c is not ResourceRequestCommand<TResource> command)
                command = new ResourceRequestCommand<TResource>();
            
            command.Configure(_options, _customizer, _request, _caching, _maxRequestAttempts);
            
            return command;
        }

        public void Remove<TResource>(ResourceRequestCommand<TResource> command)
        {
            if (_poolsByResource == null)
                return;
            
            var resourceType = typeof(TResource);
            
            if (!_poolsByResource.TryGetValue(resourceType, out var pool))
            {
                pool = new Stack<IResourceRequestCommand>();
                _poolsByResource.Add(resourceType, pool);
            }
            
            pool.Push(command);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            
            if (_poolsByResource == null) 
                return;
            
            _poolsByResource.Clear();
            _poolsByResource = null;
        }
        
        private void AttemptToParseMaxRequestAttempts()
        {
            if (_config.TryGetValue(ConfigKey.MaxResourceRequestAttempts, out var rawValue) && int.TryParse(rawValue, out var value))
                _maxRequestAttempts = value;
            else 
                _maxRequestAttempts = DefaultValue.MaxResourceRequestAttempts;
        }
        
        private void OnConfigLoaded(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            AttemptToParseMaxRequestAttempts();
        }
    }
}