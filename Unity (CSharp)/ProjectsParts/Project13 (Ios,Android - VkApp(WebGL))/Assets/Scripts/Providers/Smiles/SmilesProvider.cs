using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Smiles.Items;
using Game.Providers.Smiles.Settings;
using UnityEngine;

namespace Game.Providers.Smiles
{
	public class SmilesProvider : ISmilesProvider
	{
		private readonly ISmileSettings _smileSettings;
		private List<SmileAsset> _smilesList = new();

		public List<SmileAsset> SmilesList => _smilesList;
		public bool IsLoaded { get; private set; }

		public SmilesProvider(ISmileSettings smileSettings)
		{
			_smileSettings = smileSettings;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_smilesList = Resources.LoadAll<SmileAsset>(_smileSettings.SmilesResourcesPath).ToList();
			await UniTask.Yield();
		}
	}
}
