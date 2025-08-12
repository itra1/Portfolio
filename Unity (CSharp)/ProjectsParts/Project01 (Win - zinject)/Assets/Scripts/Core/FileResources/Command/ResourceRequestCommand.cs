using System;
using System.Threading;
using Core.FileResources.Caching;
using Core.FileResources.Customizing;
using Core.FileResources.Info;
using Core.Network.Http;
using Core.Options;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Core.FileResources.Command
{
	public class ResourceRequestCommand<TResource> : IResourceRequestCommand
	{
		private const string ErrorMessageFormat = "{0}.{1}Error: {2}{1}{3}";

		private IApplicationOptions _options;
		private IResourceCustomizer _customizer;
		private IHttpRequest _request;
		private IResourceCachingService _caching;
		private int _maxRequestAttempts;
		private ResourceInfo _info;
		private Action<TResource, string> _onCompleted;
		private Action<string> _onFailure;
		private Action _onCanceled;
		private int _currentRequestAttempt;
		private CancellationTokenSourceProxy _cancellationTokenSource;

		public bool InProgress { get; private set; }
		public bool IsCanceled => _cancellationTokenSource?.IsCancellationRequested ?? false;

		public string Url => _info.Url;

		public object Resources { get; private set; }

		public string FilePath { get; private set; }

		public event Action<ResourceRequestCommand<TResource>> Disposed;

		public void Configure(IApplicationOptions options,
			IResourceCustomizer customizer,
			IHttpRequest request,
			IResourceCachingService caching,
			int maxRequestAttempts)
		{
			_options = options;
			_customizer = customizer;
			_request = request;
			_caching = caching;
			_maxRequestAttempts = maxRequestAttempts;
		}

		public void Initialize(in ResourceInfo info,
			Action<TResource, string> onCompleted,
			Action<string> onFailure,
			Action onCanceled,
			CancellationTokenSource cancellationTokenSource = null)
		{
			InProgress = true;

			_info = info;
			_onCompleted = onCompleted;
			_onFailure = onFailure;
			_onCanceled = onCanceled;

			_cancellationTokenSource = cancellationTokenSource != null
				? new CancellationTokenSourceProxy(cancellationTokenSource)
				: new CancellationTokenSourceProxy();
		}

		public void Execute()
		{
			if (_cancellationTokenSource is { IsCancellationRequested: false })
			{
				if (_options is { IsManagersLogEnabled: true })
					Debug.Log($"Request for resource {_info} has been initialized");

				if (_customizer.IsCacheable(_info.Category) && _caching.IsAlreadyCached(_info.Category, _info.Url))
					AttemptToGetFromCacheAsync().Forget();
				else
					AttemptToMakeHttpRequest();
			}
			else
			{
				Dispose();
			}
		}

		public void Cancel()
		{
			if (_cancellationTokenSource is { IsCancellationRequested: false })
				_cancellationTokenSource.Cancel();
		}

		private void Dispose()
		{
			if (_cancellationTokenSource is { IsDisposed: false })
			{
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}

			_maxRequestAttempts = 0;
			_currentRequestAttempt = 0;

			if (_options is { IsManagersLogEnabled: true })
				Debug.Log($"Request for resource {_info} has been disposed");

			_options = null;
			_customizer = null;
			_request = null;
			_caching = null;
			_info = default;
			_onCompleted = null;
			_onFailure = null;
			_onCanceled = null;

			InProgress = false;

			Disposed?.Invoke(this);
		}

		private bool CheckForCancellation()
		{
			if (_cancellationTokenSource is not { IsCancellationRequested: true })
				return false;

			ReportCancellation();
			return true;
		}

		private void ReportCompletion(TResource resource, string filePath = null)
		{
			if (_options is { IsManagersLogEnabled: true })
				Debug.Log($"Request for resource {_info} has been completed");
			Resources = resource;
			FilePath = filePath;
			_onCompleted?.Invoke(resource, filePath);

			Dispose();
		}

		private void ReportFailure(string message)
		{
			Debug.LogError(message);
			_onFailure?.Invoke(message);
			Dispose();
		}

		private void ReportCancellation()
		{
			if (_options is { IsManagersLogEnabled: true })
				Debug.Log($"Request for resource {_info} has been cancelled");

			_onCanceled?.Invoke();

			Dispose();
		}

		private void AttemptToMakeHttpRequest()
		{
			if (CheckForCancellation())
				return;

			var category = _info.Category;

			if (_customizer.IsRequestable(category))
			{
				_customizer.GetRequester<TResource>(category)
					.Make(_request, _info.Url, OnRequestCompletedAsync, ReportFailure);
			}
			else
			{
				ReportFailure($"The resource {_info} is not customized for requesting when trying to make it");
			}
		}

		private async UniTaskVoid AttemptToGetFromCacheAsync()
		{
			if (CheckForCancellation())
				return;

			byte[] bytes;
			string filePath;

			try
			{
				var cancellationToken = _cancellationTokenSource.Token;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					cancellationToken.ThrowIfCancellationRequested();
					(bytes, filePath) = await _caching.GetFromCacheAsync(_info.Category, _info.Url, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				ReportCancellation();
				return;
			}
			catch (Exception exception)
			{
				OnGettingFromCacheFailure(exception);
				return;
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}

			OnGettingFromCacheCompletedAsync(bytes, filePath).Forget();
		}

		private async UniTask<string> AttemptToPutIntoCacheAsync(byte[] bytes)
		{
			if (CheckForCancellation())
				return null;

			string filePath;

			try
			{
				var cancellationToken = _cancellationTokenSource.Token;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					cancellationToken.ThrowIfCancellationRequested();
					filePath = await _caching.PutIntoCacheAsync(_info.Category, _info.Url, bytes, cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				ReportCancellation();
				return null;
			}
			catch (Exception exception)
			{
				OnPuttingToCacheFailure(exception);
				return null;
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}

			return filePath;
		}

		private async UniTask<TResource> AttemptToSerializeAsync(byte[] bytes)
		{
			if (CheckForCancellation())
				return default;

			TResource resource;

			try
			{
				resource = await _customizer.GetSerializer<TResource>(_info.Category)
					.SerializeAsync(_info.Name, bytes, _cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				ReportCancellation();
				return default;
			}
			catch (Exception exception)
			{
				OnSerializationFailure(exception);
				return default;
			}

			return AttemptToValidate(resource) ? resource : default;
		}

		private async UniTask<byte[]> AttemptToDeserializeAsync(TResource resource)
		{
			if (CheckForCancellation())
				return default;

			byte[] bytes;

			try
			{
				bytes = await _customizer.GetDeserializer<TResource>(_info.Category)
					.DeserializeAsync(resource, _cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				ReportCancellation();
				return default;
			}
			catch (Exception exception)
			{
				OnDeserializationFailure(exception);
				return default;
			}

			return bytes is { Length: > 0 } ? bytes : default;
		}

		private bool AttemptToValidate(TResource resource)
		{
			var category = _info.Category;

			if (_customizer.IsValidatable(category) && !_customizer.GetValidator<TResource>(category).IsValid(resource))
			{
				if (_customizer.IsDestructible(category))
					_customizer.GetDestructor<TResource>(category).Destruct(resource);

				if (_currentRequestAttempt++ < _maxRequestAttempts)
				{
					AttemptToMakeHttpRequest();
					return false;
				}

				OnExceededRequestAttemptsFailure();
				return false;
			}

			return true;
		}

		private async UniTaskVoid OnRequestCompletedAsync(TResource resource)
		{
			if (CheckForCancellation())
				return;

			string filePath = null;

			if (_customizer.IsConvertable(_info.Category) && !_caching.IsAlreadyCached(_info.Category, _info.Url, out filePath))
				filePath = await AttemptToPutIntoCacheAsync(await AttemptToDeserializeAsync(resource));

			if (CheckForCancellation())
				return;

			ReportCompletion(resource, filePath);
		}

		private async UniTaskVoid OnGettingFromCacheCompletedAsync(byte[] bytes, string filePath)
		{
			var resource = await AttemptToSerializeAsync(bytes);

			if (CheckForCancellation())
				return;

			if (resource != null)
				ReportCompletion(resource, filePath);
		}

		private void OnGettingFromCacheFailure(Exception exception)
		{
			var description = $"An error occurred while trying to get a resource {_info} from the cache";
			ReportFailure(string.Format(ErrorMessageFormat, description, Environment.NewLine, exception.Message, exception.StackTrace));
		}

		private void OnPuttingToCacheFailure(Exception exception)
		{
			var description = $"An error occurred while trying to put a resource {_info} to the cache";
			ReportFailure(string.Format(ErrorMessageFormat, description, Environment.NewLine, exception.Message, exception.StackTrace));
		}

		private void OnSerializationFailure(Exception exception)
		{
			var description = $"An error occurred while trying to serialize a resource {_info}";
			ReportFailure(string.Format(ErrorMessageFormat, description, Environment.NewLine, exception.Message, exception.StackTrace));
		}

		private void OnDeserializationFailure(Exception exception)
		{
			var description = $"An error occurred while trying to deserialize a resource {_info}";
			ReportFailure(string.Format(ErrorMessageFormat, description, Environment.NewLine, exception.Message, exception.StackTrace));
		}

		private void OnExceededRequestAttemptsFailure()
		{
			ReportFailure($"Attempts to request a resource {_info} exceeded. Maximum attempts is {_maxRequestAttempts}");
		}
	}
}