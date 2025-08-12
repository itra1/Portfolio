using UnityEngine;
using Zenject;

namespace Core.Engine.Settings
{

	[CreateAssetMenu(fileName = "SoundLibrary", menuName = "App/Create/Library/SoundLibrary")]
	public class SoundsLibraryObject : ScriptableObjectInstaller
	{
		[SerializeField] private SoundLibrary _soundLibrary;

		public override void InstallBindings()
		{
			_ = Container.Bind<SoundLibrary>().FromInstance(_soundLibrary).AsSingle().NonLazy();
		}
	}

	[System.Serializable]
	public class SoundLibrary
	{
		[SerializeField] private AudioClip[] _startSounds;
		[SerializeField] private AudioClip[] _winSounds;

		public AudioClip[] StartSounds => _startSounds;
		public AudioClip[] WinSounds => _winSounds;
	}
}
