using Core.Elements.Windows.VideoFile.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.Video;
using Elements.FloatingWindow.Presenter.WindowAdapters.VideoFile.Components;
using Elements.Windows.Video.Presenter.VideoPlayer;
using Elements.Windows.VideoFile.Presenter;
using UnityEngine;
using Utils;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.VideoFile
{
	[MaterialData(typeof(VideoFileMaterialData))]
	public class VideoFilePresenterAdapter : VideoPresenterAdapterBase<VideoFilePresenter, VideoFileHeader>
	{
		protected override IVideoPlayer Player => Adaptee.Player;
		
		public override Vector2 OriginalContentSize
		{
			get
			{
				var originalContentSize = base.OriginalContentSize;
				var player = Player;
				
				if (player == null)
					return originalContentSize;
				
				var videoSize = player.OriginalVideoSize;
				
				return SizeCalculator.Fit(originalContentSize, videoSize);
			}
		}

		protected override void EnableComponents()
		{
			base.EnableComponents();
            
			Adaptee.FullScreenBeforeToggle += OnFullScreenBeforeToggled;
			Adaptee.FullScreenAfterToggle += OnFullScreenAfterToggled;
		}
        
		protected override void DisableComponents()
		{
			Adaptee.FullScreenBeforeToggle -= OnFullScreenBeforeToggled;
			Adaptee.FullScreenAfterToggle -= OnFullScreenAfterToggled;
			
			Header.RestoreSizeDelta();
            
			base.DisableComponents();
		}
		
		private void OnFullScreenBeforeToggled(bool isInFullScreenMode)
		{
			if (!isInFullScreenMode)
				Header.SaveSizeDelta();
		}
		
		private void OnFullScreenAfterToggled(bool isInFullScreenMode)
		{
			if (isInFullScreenMode)
				Header.RecalculateSizeDelta(Adaptee.Player.VideoDimensions.x);
			else
				Header.RestoreSizeDelta();
		}
	}
}
