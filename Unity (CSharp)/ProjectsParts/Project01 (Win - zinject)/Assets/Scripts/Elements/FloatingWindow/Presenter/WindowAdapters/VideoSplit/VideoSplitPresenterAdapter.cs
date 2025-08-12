using System.Linq;
using Core.Elements.Windows.VideoSplit.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.FloatingWindow.Presenter.WindowAdapters.Video;
using Elements.Windows.Video.Presenter.VideoPlayer;
using Elements.Windows.VideoSplit.Presenter;
using UnityEngine;
using Utils;
using Zenject;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.VideoSplit
{
	[MaterialData(typeof(VideoSplitMaterialData))]
	public class VideoSplitPresenterAdapter : VideoPresenterAdapterBase<VideoSplitPresenter, WindowHeader>
	{
		protected override IVideoPlayer Player => Adaptee.Players?.FirstOrDefault();
		
		[Inject]
		private void Initialize() => Adaptee.SetScaleToFitMode();
		
		public override Vector2 OriginalContentSize
		{
			get
			{
				var originalContentSize = base.OriginalContentSize;
				var players = Adaptee.Players;
				
				if (players.Length == 0)
					return originalContentSize;
				
				var totalVideoSize = new Vector2();
				
				foreach (var player in players)
				{
					if (player == null)
						continue;
					
					var videoSize = player.OriginalVideoSize;
					
					totalVideoSize.x += videoSize.x;
					totalVideoSize.y = Mathf.Max(totalVideoSize.y, videoSize.y);
				}
				
				return SizeCalculator.Fit(originalContentSize, totalVideoSize);
			}
		}
		
		public override void UpdateContent() => Adaptee.AlignPlayers();
	}
}
