using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Premiums.Saves;
using Game.Scripts.Providers.Premiums.Settings;
using Game.Scripts.Providers.Saves;

namespace Game.Scripts.Providers.Premiums
{
	public class PremiumProvider : IPremiumProvider
	{
		private PremiumSettings _settings;
		private ISaveProvider _saveProvider;
		private PremiumSave _save;

		public bool IsActive => _save.ActivePremium;

		public PremiumProvider(PremiumSettings settings, ISaveProvider saveProvider)
		{
			_settings = settings;
			_saveProvider = saveProvider;
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveProvider.GetProperty<PremiumSaveData>().Value;
			await UniTask.Yield();
		}
	}
}
