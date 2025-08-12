using Core.Engine.Components.Skins;
using Zenject;

namespace Scripts.Workers
{
	public class SkinWorker
	{
		private readonly SignalBus _signalBus;
		private readonly ISkinProvider _skinProvider;
		public SkinWorker(SignalBus signalBus, ISkinProvider skinProvider)
		{
			_skinProvider = skinProvider;
			_signalBus = signalBus;

			_signalBus.Subscribe<SetSkinSignal>(OnSetSkinSignal);
			OnSetSkinSignal();
		}

		private void OnSetSkinSignal()
		{
			var activeSkin = _skinProvider.GetActiveSkin(SkinType.Mesh);
			activeSkin.Confirm();
		}
	}
}
