using Providers.SystemMessage.Common;

using UGui.Screens.Elements;

namespace Providers.SystemMessage
{
	public class SystemMessageProvider : ISystemMessage
	{
		private ISystemMessageSettings _settings;
		private ISystemMessagesParent _parent;
		public SystemMessageProvider(ISystemMessagesParent parent, ISystemMessageSettings settings)
		{
			_settings = settings;
			_parent = parent;
		}
	}
}
