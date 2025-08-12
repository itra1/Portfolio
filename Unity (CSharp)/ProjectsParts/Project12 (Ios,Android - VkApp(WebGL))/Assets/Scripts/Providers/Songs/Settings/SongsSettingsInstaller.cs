using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Songs.Settings
{
	[CreateAssetMenu(fileName = "SongsSettingsInstaller", menuName = "Providers/Songs/Settings")]
	public class SongsSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private SongsSettings _songsSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<SongsSettings>().FromInstance(_songsSettings).AsSingle().NonLazy();
		}
	}
}
