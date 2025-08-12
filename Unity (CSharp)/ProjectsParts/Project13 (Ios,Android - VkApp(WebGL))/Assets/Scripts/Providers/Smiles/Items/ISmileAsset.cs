using UnityEngine;

namespace Game.Providers.Smiles.Items
{
	public interface ISmileAsset
	{
		Sprite Icone { get; }
		string Uuid { get; }
	}
}