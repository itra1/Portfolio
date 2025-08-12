using UnityEngine;

namespace Game.Providers.Smiles.Settings
{
	[System.Serializable]
	public class SmileSettings : ISmileSettings
	{
		[SerializeField] private string _smilesResourcesPath = "Smiles";
		public string SmilesResourcesPath => _smilesResourcesPath;
	}
}
