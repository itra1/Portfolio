using Engine.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Engine.Scripts.Installers
{
	[CreateAssetMenu(fileName = "NotesSettingsInstaller", menuName = "Installers/NotesSettingsInstaller")]
	public class NotesSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private NotesSettings _settings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<NotesSettings>().FromInstance(_settings).AsSingle().NonLazy();
		}
	}
}
