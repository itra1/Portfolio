using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Debug = Core.Logging.Debug;
using Object = UnityEngine.Object;

namespace ScreenStreaming.Sender.Source
{
    public class ApplicationVideoStreamSource : VideoStreamSender.VideoStreamSourceImpl, IApplicationVideoStreamSource
    {
        private Camera _mainCamera;
        private MonoBehaviour _coroutineRunner;
        private Vector2 _scale;
        private Vector2 _offset;

        private RectTransform _rectTransform;

        private RenderTexture _screenTexture;
        private RenderTexture _sentTexture;

        private StringBuilder _debugLogBuffer;

        private CancellationTokenSource _updateCancellationTokenSource;

        public ApplicationVideoStreamSource(VideoStreamSender parent) : base(parent)
        {
            _mainCamera = Camera.main;
            _coroutineRunner = parent;
            _scale = Vector2.one;
            _offset = Vector2.zero;
            _debugLogBuffer = new StringBuilder();

            CreateTextures();
        }

        public override StreamSenderBase.WaitForCreateTrack CreateTrack()
        {
            var instruction = new StreamSenderBase.WaitForCreateTrack();
            var format = WebRTC.GetSupportedGraphicsFormat(SystemInfo.graphicsDeviceType);

            if (_sentTexture.graphicsFormat == format && _sentTexture.width == width && _sentTexture.height == height)
                instruction.Done(new VideoStreamTrack(_sentTexture));
            else
                instruction.Done(new VideoStreamTrack(_sentTexture, false));

            if (_updateCancellationTokenSource is { IsCancellationRequested: false })
            {
                _updateCancellationTokenSource.Cancel();
                _updateCancellationTokenSource.Dispose();
                _updateCancellationTokenSource = null;
            }

            UpdateAsync().Forget();

            return instruction;
        }

        public void SetStreamRect(RectTransform rectTransform)
        {
            if (_debugLogBuffer.Length > 0)
                _ = _debugLogBuffer.Clear();

            _rectTransform = rectTransform;

            var rect = rectTransform.rect;
            var pivot = rectTransform.pivot;

            var rectWidth = rect.width;
            var rectHeight = rect.height;
            var pivotX = pivot.x;
            var pivotY = pivot.y;

            _ = _debugLogBuffer.Append($"The stream area \"{rectTransform.gameObject.name}\" is set with the following parameters:{System.Environment.NewLine}");
            _ = _debugLogBuffer.Append($"- anchored position: {rectTransform.anchoredPosition}{System.Environment.NewLine}");
            _ = _debugLogBuffer.Append($"- width: {rectWidth.ToString(CultureInfo.InvariantCulture)}{System.Environment.NewLine}");
            _ = _debugLogBuffer.Append($"- height: {rectHeight.ToString(CultureInfo.InvariantCulture)}{System.Environment.NewLine}");

            var bottomLeftPoint = new Vector3(-rectWidth * pivotX, -rectHeight * pivotY);
            var topLeftPoint = new Vector3(-rectWidth * pivotX, rectHeight * (1f - pivotY));
            var topRightPoint = new Vector3(rectWidth * (1f - pivotX), rectHeight * (1f - pivotY));
            var bottomRightPoint = new Vector3(rectWidth * (1 - pivotX), -rectHeight * pivotY);

            unsafe
            {
                Span<Vector3> points = stackalloc[]
                {
          bottomLeftPoint,
          topLeftPoint,
          topRightPoint,
          bottomRightPoint
        };

                rectTransform.TransformPoints(points);

                _ = _debugLogBuffer.Append($"- corners in world space: {{bottom left: {bottomLeftPoint}, top left: {topLeftPoint}, top right: {topRightPoint}, bottom right: {bottomRightPoint}}}{System.Environment.NewLine}");

                bottomLeftPoint = _mainCamera.WorldToScreenPoint(points[0]);
                topLeftPoint = _mainCamera.WorldToScreenPoint(points[1]);
                topRightPoint = _mainCamera.WorldToScreenPoint(points[2]);
                bottomRightPoint = _mainCamera.WorldToScreenPoint(points[3]);
            }

            bottomLeftPoint = new Vector3((int) Mathf.Max(0, bottomLeftPoint.x), (int) Mathf.Max(0, bottomLeftPoint.y));
            topLeftPoint = new Vector3((int) Mathf.Max(0, topLeftPoint.x), (int) Mathf.Min(Screen.height, topLeftPoint.y));
            topRightPoint = new Vector3((int) Mathf.Min(Screen.width, topRightPoint.x), (int) Mathf.Min(Screen.height, topRightPoint.y));
            bottomRightPoint = new Vector3((int) Mathf.Min(Screen.width, bottomRightPoint.x), (int) Mathf.Max(0, bottomRightPoint.y));

            _ = _debugLogBuffer.Append($"- corners in screen space: {{bottom left: {bottomLeftPoint}, upper left: {topLeftPoint}, upper right: {topRightPoint}, bottom right: {bottomRightPoint}}}{System.Environment.NewLine}");

            var region = new Rect(bottomLeftPoint.x, bottomLeftPoint.y, bottomRightPoint.x - bottomLeftPoint.x, topLeftPoint.y - bottomLeftPoint.y);

            if (region is { width: > 0, height: > 0 })
            {
                _ = _debugLogBuffer.Append($"- rect: {region}{System.Environment.NewLine}");

                _scale = new Vector2(region.width / Screen.width, region.height / Screen.height);
                _offset = new Vector2(region.x / Screen.width, (Screen.height - (region.y + region.height)) / Screen.height);

                _ = _debugLogBuffer.Append($"- scale: {_scale}{System.Environment.NewLine}");
                _ = _debugLogBuffer.Append($"- offset: {_offset}{System.Environment.NewLine}");

                Debug.Log(_debugLogBuffer.ToString());
            }
            else
            {
                Debug.Log(_debugLogBuffer.ToString());
                Debug.LogError($"Failed to set the stream area: {region}");
            }

            _ = _debugLogBuffer.Clear();
        }

        public void UpdateStreamRect(RectTransform rectTransform)
        {
            if (_rectTransform == rectTransform)
                SetStreamRect(rectTransform);
        }

        public void RemoveStreamRect(RectTransform rectTransform)
        {
            if (_rectTransform == rectTransform)
                _rectTransform = null;
        }

        public override void Dispose()
        {
            if (_updateCancellationTokenSource is { IsCancellationRequested: false })
            {
                _updateCancellationTokenSource.Cancel();
                _updateCancellationTokenSource.Dispose();
                _updateCancellationTokenSource = null;
            }

            if (_debugLogBuffer != null)
            {
                if (_debugLogBuffer.Length > 0)
                    _ = _debugLogBuffer.Clear();

                _debugLogBuffer = null;
            }

            try
            {
                if (_sentTexture != null)
                {
                    _sentTexture.Release();
                    _sentTexture.DiscardContents();

                    Object.Destroy(_sentTexture);

                    _sentTexture = null;
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                if (_screenTexture != null)
                {
                    _screenTexture.Release();
                    _screenTexture.DiscardContents();

                    Object.Destroy(_screenTexture);

                    _screenTexture = null;
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                _rectTransform = null;
                _mainCamera = null;
                _coroutineRunner = null;
            }
            catch
            {
                // ignored
            }

            GC.SuppressFinalize(this);
        }

        private void CreateTextures()
        {
            var format = WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);

            _screenTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default)
            {
                antiAliasing = 1
            };

            _ = _screenTexture.Create();

            RenderTexture renderTexture;

            if (_sentTexture != null)
            {
                renderTexture = _sentTexture;

                var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(format, RenderTextureReadWrite.Default);
                var compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, FormatUsage.Render);
                var formatTexture = graphicsFormat == compatibleFormat ? graphicsFormat : compatibleFormat;

                if (renderTexture.graphicsFormat != formatTexture)
                {
                    Debug.LogWarning($"This color format \"{renderTexture.graphicsFormat}\" is not supported in unity.webrtc. Replace with a supported color format \"{formatTexture}\".");

                    renderTexture.Release();
                    renderTexture.graphicsFormat = formatTexture;
                    _ = renderTexture.Create();
                }
            }
            else
            {
                renderTexture = new RenderTexture(Screen.width / 3, Screen.height, 0, format)
                {
                    antiAliasing = 1
                };

                _ = renderTexture.Create();
            }

            _sentTexture = renderTexture;
        }

        private async UniTaskVoid UpdateAsync()
        {
            _updateCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var cancellationToken = _updateCancellationTokenSource.Token;

                while (_updateCancellationTokenSource != null)
                {
                    await UniTask.WaitForEndOfFrame(_coroutineRunner, cancellationToken);

                    if (_rectTransform == null || !_rectTransform.gameObject.activeInHierarchy)
                        continue;

                    ScreenCapture.CaptureScreenshotIntoRenderTexture(_screenTexture);
                    Graphics.Blit(_screenTexture, _sentTexture, _scale, _offset);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }

        ~ApplicationVideoStreamSource() => Dispose();
    }
}