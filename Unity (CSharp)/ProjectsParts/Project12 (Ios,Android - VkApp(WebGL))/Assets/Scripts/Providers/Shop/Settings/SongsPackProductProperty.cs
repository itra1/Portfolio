using System.Collections.Generic;
using Engine.Scripts.Base;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Prices;
using StringDrop;
using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	[CreateAssetMenu(fileName = "SongsPack", menuName = "Providers/Shop/Products/SongsPack")]
	public class SongsPackProductProperty : ProductProperty<RealPrice>
	{
		public override string Type => ProductType.SongsPack;
		[SerializeField][StringDropList(typeof(ColorTypes))] private string _color;
		[SerializeField][StringDropList(typeof(MusicalGenre))] private string _genre;

		[SerializeField] private List<RhythmTimelineAsset> _songs;

		public int SongCount => _songs.Count;

		public string Genre => _genre;

		public string Color => _color;
	}
}
