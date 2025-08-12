using UnityEngine;

namespace Platforms.RateUs.Settings
{
	[CreateAssetMenu(fileName = TDRateUsSettings.FileName, menuName = "RateUs/Settings")]
	public class TDRateUsSettings : ScriptableObject
	{
		public const string FileName = "RateUsSettings";

		[SerializeField] private TDIosSettings _ios;

		public TDIosSettings Ios => _ios;
	}
}
