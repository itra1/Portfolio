using System.Collections.Generic;
using Elements.Windows.Video.Presenter.VideoPlayer;
using UnityEngine;
using Utils.Data;

namespace Utils
{
    public static class VideoPlayersExtensions
    {
        public static VideoPlayersWidthParameters CalculateWidthParameters(this IReadOnlyList<IVideoPlayer> players,
            Vector2 size,
            bool scaleToFitMode)
        {
            var totalVideoWidth = 0f;
            var maxVideoHeight = 0f;
			
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
				
                var videoWidth = 0f;
                var videoHeight = 0f;
				
                if (player.IsInitialized)
                {
                    var videoSize = player.OriginalVideoSize;
					
                    if (videoSize is { x: > 0, y: > 0 })
                    {
                        videoWidth = videoSize.x;
                        videoHeight = videoSize.y;
                    }
                }
				
                totalVideoWidth += videoWidth;
                maxVideoHeight = Mathf.Max(maxVideoHeight, videoHeight);
            }
			
            var scale = 1f;
			
            if (maxVideoHeight > 0f)
            {
                scale = size.y / maxVideoHeight;
                totalVideoWidth *= scale;
				
                if (scaleToFitMode && totalVideoWidth > size.x && totalVideoWidth > 0f)
                {
                    scale *= size.x / totalVideoWidth;
                    totalVideoWidth = size.x;
                }
            }
			
            return new VideoPlayersWidthParameters
            {
                TotalVideoWidth = totalVideoWidth,
                Scale = scale
            };
        }
    }
}