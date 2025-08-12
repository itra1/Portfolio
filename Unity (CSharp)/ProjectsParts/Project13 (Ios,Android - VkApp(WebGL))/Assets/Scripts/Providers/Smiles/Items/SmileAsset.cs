using UnityEngine;
using Uuid;

namespace Game.Providers.Smiles.Items
{
	[CreateAssetMenu(fileName = "Smile", menuName = "App/Smiles/Smile")]
	public class SmileAsset : ScriptableObject, ISmileAsset
	{
		[UUID][SerializeField] private string _uuid;
		[SerializeField] private Sprite _icone;

		public Sprite Icone => _icone;
		public string Uuid => _uuid;
	}
}
