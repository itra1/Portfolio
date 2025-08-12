using System.Collections.Generic;
using Game.Game.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class StageIndicatorView : MonoBehaviour, IPoolable<int, bool, IMemoryPool>
	{
		[SerializeField] private Image _image;
		[SerializeField] private List<TypeData> _typeData;

		private SignalBus _signalBus;
		private GameSession _gameSession;
		private IMemoryPool _pool;
		private int _index;
		private bool _isBoss;

		[Inject]
		public void Constructor(SignalBus signalBus, GameSession gameSession)
		{
			_signalBus = signalBus;
			_gameSession = gameSession;
		}

		public void OnDespawned()
		{
			//_signalBus.Unsubscribe<NewStageSignal>(ConfirmView);
		}

		public void Destroy()
		{
			_pool.Despawn(this);
		}

		public void OnSpawned(int index, bool isBoss, IMemoryPool pool)
		{
			_pool = pool;
			_index = index;
			_isBoss = isBoss;
			//_signalBus.Subscribe<NewStageSignal>(ConfirmView);
			ConfirmView();
		}

		private void ConfirmView()
		{
			var prop = _isBoss ? _typeData[1] : _typeData[0];
			_image.sprite = _gameSession.StageIndex > _index ? prop.Complete : prop.NoComplete;
		}

		public class Factory : PlaceholderFactory<int, bool, StageIndicatorView> { }

		[System.Serializable]
		public struct TypeData
		{
			public Sprite NoComplete;
			public Sprite Complete;
			public float Width;
		}

	}
}
