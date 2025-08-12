using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.App;
using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles
{
	public interface IProfileProvider : IApplicationLoaderItem
	{
		public UnityEvent<IProfile> OnProfileChange { get; }
		IProfile Profile { get; }
		bool IsFirstLogin { get; }

		void FirstRun();
		void SetAvatar(string avatar);
	}
}