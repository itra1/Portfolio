using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Prices;
using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	[CreateAssetMenu(fileName = "WelcomeBundle", menuName = "Providers/Shop/Products/WelcomeBundle")]
	public class WelcomeBundleProductProprty : ProductProperty<RealPrice>
	{
		[SerializeField] private int _gemsCount;
		[SerializeField] private int _boxCount;
		[SerializeField] private List<RhythmTimelineAsset> _songsList;
		public override string Type => ProductType.WelcomeBundle;
		public int SongCount => _songsList.Count;
		public int GemsCount => _gemsCount;
		public int BoxCount => _boxCount;
	}
}
