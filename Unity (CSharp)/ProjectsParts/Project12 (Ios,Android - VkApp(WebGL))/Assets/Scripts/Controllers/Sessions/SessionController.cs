using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Controllers.Sessions.Common;

namespace Game.Scripts.Controllers.Sessions
{
	public class SessionController : ISessionController
	{
		private ISession _session;
		private readonly IApplicationSettingsController _applicationSessingsController;

		public SessionController(ISession session, IApplicationSettingsController applicationSessingsController)
		{
			_session = session;
			_applicationSessingsController = applicationSessingsController;
		}

		public async UniTask StartAppLoad(IProgress<float> onProgress, CancellationToken cancellationToken)
		{
			_session.SceneVisibleMoveSet(_applicationSessingsController.ApplicationSettings.SceneVisibleMode);
			_session.GameMissMoveSet(_applicationSessingsController.ApplicationSettings.GameMissMode);
			await UniTask.Yield();
		}
	}
}
