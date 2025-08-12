using System.Collections.Generic;
using Game.Providers.Smiles.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Providers.Smiles.Handlers
{
	public class SmileHndler : ISmileHndler
	{
		public UnityEvent<bool, Sprite> OnSmileHandler { get; set; } = new();
		public List<SmileAsset> SmilesList => _smilesProvider.SmilesList;

		private ISmilesProvider _smilesProvider;

		public SmileHndler(ISmilesProvider smilesProvider)
		{
			_smilesProvider = smilesProvider;
		}

		public void EmitSmileHandler(bool isPlayer, string avatarUuid)
		{
			var asset = _smilesProvider.SmilesList.Find(x => x.Uuid == avatarUuid);
			OnSmileHandler?.Invoke(isPlayer, asset.Icone);
		}
	}
}
