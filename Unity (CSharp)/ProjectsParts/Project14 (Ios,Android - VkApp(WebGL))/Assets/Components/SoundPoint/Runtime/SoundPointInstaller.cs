using Zenject;

namespace SoundPoint {
	internal class SoundPointInstaller : MonoInstaller {

		public override void InstallBindings() {

			_ = Container.BindFactoryCustomInterface<AudioPoint, AudioPoint.Factory, IAudioPointFactory>()
			.FromPoolableMemoryPool<AudioPoint, AudioPointPool>(poolBinder => poolBinder
				.WithInitialSize(20)
				.FromComponentInNewPrefabResource("AudioPoint")
				.UnderTransformGroup("SoundPoints")
				);
		}
		public class AudioPointPool : MonoPoolableMemoryPool<IMemoryPool, AudioPoint> { }
	}
}
