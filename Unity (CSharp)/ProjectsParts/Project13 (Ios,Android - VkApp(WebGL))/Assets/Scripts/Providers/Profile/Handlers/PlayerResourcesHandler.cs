using System.Collections.Generic;

namespace Game.Providers.Profile.Handlers {
	public class PlayerResourcesHandler {
		private Dictionary<string, IPlayerResourceHandler> _handlers = new();

		public Dictionary<string, IPlayerResourceHandler> Handlers { get => _handlers; set => _handlers = value; }

		public PlayerResourcesHandler(CoinsHandler coinsHandler, DollarHandler dollarHandler, ExperienceHandler experienceHandler) {
			_handlers.Add(coinsHandler.ResourceType, coinsHandler);
			_handlers.Add(dollarHandler.ResourceType, dollarHandler);
			_handlers.Add(experienceHandler.ResourceType, experienceHandler);
		}

		public IPlayerResourceHandler GetHandler(string type) {
			if (!_handlers.ContainsKey(type))
				return null;
			return _handlers[type];
		}
	}
}
