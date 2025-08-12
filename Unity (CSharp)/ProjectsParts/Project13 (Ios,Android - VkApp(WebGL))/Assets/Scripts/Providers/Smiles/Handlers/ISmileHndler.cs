using System.Collections.Generic;
using Game.Providers.Smiles.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Providers.Smiles.Handlers
{
	public interface ISmileHndler
	{
		UnityEvent<bool, Sprite> OnSmileHandler { get; set; }
		List<SmileAsset> SmilesList { get; }

		void EmitSmileHandler(bool isPlayer, string avatarUuid);
	}
}