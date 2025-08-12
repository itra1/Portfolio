using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace itra.Animations
{
	public abstract class AnimationUniTask : AnimationBase
	{
		[SerializeField] private bool _stopOnDisable = true;

		private CancellationTokenSource _updateCTS;
		private CancellationTokenSource _mergeCTS;

		private void OnDisable()
		{
			if (_stopOnDisable)
				_ = Stop();
		}

		public override bool Play()
		{
			if (!base.Play())
				return false;

			_updateCTS = new();
			_mergeCTS = CancellationTokenSource.CreateLinkedTokenSource(_updateCTS.Token, destroyCancellationToken);

			_ = UpdateAsync();

			return true;
		}

		public override bool Stop()
		{
			if (!base.Stop())
				return false;
			if (_updateCTS is { IsCancellationRequested: false })
				_updateCTS.Cancel();

			return true;
		}

		protected virtual async UniTask UpdateAsync()
		{
			try
			{
				while (_isPlaying && !_mergeCTS.IsCancellationRequested)
				{
					await UniTask.Yield();
					await UpdateAnimation();
				}
			}
			catch (Exception exception)
			when (exception is not OperationCanceledException)
			{
				Debug.Log($"Exception {exception.Message} \n" +
				$"Object: {gameObject.name}");
			}
			finally
			{
				_updateCTS.Dispose();
				_updateCTS = null;
			}
		}

		protected abstract UniTask UpdateAnimation();
	}
}
