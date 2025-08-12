using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Nicknames.Settings;
using UnityEngine;

namespace Game.Providers.Nicknames
{
	public class NicknamesProvider : INicknamesProvider
	{
		private NicknameSettings _nicknamesSettings;
		private List<string> _nicknames = new();

		public bool IsLoaded { get; private set; }

		public NicknamesProvider(NicknameSettings nicknameSettings)
		{
			_nicknamesSettings = nicknameSettings;
		}

		public string GetRandom()
		{
			return _nicknames[UnityEngine.Random.Range(0, _nicknames.Count)];
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			var textAssets = await Resources.LoadAsync<TextAsset>(_nicknamesSettings.FilePath).ToUniTask(progress: OnProgress, cancellationToken: cancellationToken);

			_nicknames = (textAssets as TextAsset).text.Split("\n").ToList();
		}
	}
}
