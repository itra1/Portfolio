using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Engine.Engine.Scripts.Settings;
using Game.Scripts.Providers.Addressable;
using Game.Scripts.Settings;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Controllers
{
	public class ApplicationSettingsController : IApplicationSettingsController
	{
		public UnityEvent<IApplicationSettings> OnLoadSettings { get; set; } = new();
		private const string AppSettingsFile = "ApplicationSettings";

		private readonly DiContainer _diContainer;
		private readonly IAddressableProvider _addressableProvider;
		private IApplicationSettings _applicationSettings;

		public IApplicationSettings ApplicationSettings => _applicationSettings;

		public ApplicationSettingsController(DiContainer diContainer, IAddressableProvider addressableProvider)
		{
			_diContainer = diContainer;
			_addressableProvider = addressableProvider;
		}

		public async UniTask StartAppLoad(IProgress<float> onProgress, CancellationToken cancellationToken)
		{
#if UNITY_EDITOR
			await LoadEditorFromAssetDatabase();
#else
			await LoadFromAddressable();
#endif
			OnLoadSettings?.Invoke(_applicationSettings);
		}
		private async UniTask LoadFromAddressable()
		{
			_applicationSettings = await _addressableProvider.LoadAssetAsync<ApplicationSettings>(AppSettingsFile);
		}

#if UNITY_EDITOR
		private async UniTask LoadEditorFromAssetDatabase()
		{
			_applicationSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<ApplicationSettings>($"Assets/Resources_moved/{AppSettingsFile}.asset");
			await UniTask.Yield();
		}
#endif
	}
}
