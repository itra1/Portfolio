using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers.Base;
using ModestTree;

namespace Game.Scripts.Managers.Dialogs
{
	/// <summary>
	/// Вывод диалоговых сообщенний по очереди при возможности
	/// </summary>
	public class DialogVisibleOrderHelper : IDialogVisibleOrderHelper
	{
		private bool _allowed;
		private readonly Queue<Func<UniTask>> _jobOrder = new();

		private readonly IGameHandler _gameHandler;
		private readonly IGamePlaying _gamePlaying;
		private readonly IUiNavigator _uiHelper;
		private bool _processVisible;

		/// <summary>
		/// Permission to execute
		/// </summary>
		public bool Allowed
		{
			get => _allowed; set
			{
				_allowed = value;

				if (VisibleAllowed)
					_ = ProcessJobsIfNeed();
			}
		}

		/// <summary>
		/// Permission to launch the line of dialogs
		/// </summary>
		private bool VisibleAllowed
		{
			get
			{
				// Another ban
				if (!Allowed)
					return false;

				// Game process
				if (_gamePlaying.IsPlaying)
					return false;

				// The process has already been launched
				if (_processVisible)
					return false;

				// The list is empty
				if (_jobOrder.IsEmpty())
					return false;

				return true;
			}
		}

		public DialogVisibleOrderHelper(IGameHandler gameHandler, IGamePlaying gamePlaying, IUiNavigator uiHelper)
		{
			_gameHandler = gameHandler;
			_gamePlaying = gamePlaying;
			_uiHelper = uiHelper;

			_gameHandler.OnGameChangeEvent.AddListener(GameChangeEvent);
			_uiHelper.OnPresenterVisibleChange.AddListener(PresenterChangeEvent);
		}

		private void PresenterChangeEvent(bool isVisible, IWindowPresenterController controller)
		{
			if (!isVisible)
				return;

			Allowed = controller.WindowPresenter.GetType().GetInterface(nameof(IDialogAllowed)) != null;
		}

		private void GameChangeEvent(RhythmTimelineAsset track, bool isGame)
		{

		}

		public void AddJob(Func<UniTask> actionVisibleDialog)
		{
			_jobOrder.Enqueue(actionVisibleDialog);

			_ = ProcessJobsIfNeed();
		}

		private async UniTask ProcessJobsIfNeed()
		{
			if (!VisibleAllowed)
				return;

			_processVisible = true;

			while (_jobOrder.Count > 0)
			{
				var currentItem = _jobOrder.Dequeue();
				await currentItem();
			}

			_processVisible = false;
		}
	}
}