using System.Collections.Generic;
using Game.Providers.Battles.Interfaces;
using UnityEngine;

namespace Game.Game.Elements.Barriers
{
	public class BoardBarriers : MonoBehaviour
	{
		[SerializeField] private Spike _spike;
		[SerializeField] private MoveBarrier _moveBarrier;
		[SerializeField] private LightBarrier _lightBarrier;
		[SerializeField] private Screw _screw;

		private readonly List<Spike> _spikesList = new();
		private int _barriersCount;
		private float _currentBarrierAngle;
		private float _maxAngle;

		public void SpikeBarriers(int count)
		{
			_spike.gameObject.SetActive(false);

			foreach (var item in _spikesList)
				item.gameObject.SetActive(false);

			if (count == 0)
				return;

			if (_spikesList.Count < count)
			{
				for (int i = _spikesList.Count; i < count; i++)
				{
					var inst = Instantiate(_spike, _spike.transform.parent);
					_spikesList.Add(inst);
				}
			}

			for (int i = 0; i < _spikesList.Count; i++)
			{
				_spikesList[i].gameObject.SetActive(true);
				_spikesList[i].transform.rotation = Quaternion.Euler(1, 1, GetNewAngle());
			}
		}

		public void MoveBarrierActive(bool isActive)
		{
			_moveBarrier.gameObject.SetActive(isActive);
			if (isActive)
				_moveBarrier.transform.rotation = Quaternion.Euler(1, 1, GetNewAngle());
		}

		public void LightBarrierActive(bool isActive)
		{
			_lightBarrier.gameObject.SetActive(isActive);
			if (isActive)
				_lightBarrier.transform.rotation = Quaternion.Euler(1, 1, GetNewAngle());
		}

		public void PositingBarriers(IBoardStageSettings stageData)
		{
			_currentBarrierAngle = Random.Range(0, 360);
			_barriersCount = stageData.Spikes;
			_barriersCount += stageData.ScrewBarrier ? 1 : 0;
			_barriersCount += stageData.MoveBarrier ? 1 : 0;

			_maxAngle = 360 / (_barriersCount != 0 ? _barriersCount : 1);

			MoveBarrierActive(stageData.MoveBarrier);
			LightBarrierActive(stageData.LoseBarrier);
			SpikeBarriers(stageData.Spikes);
			ScrewActive(stageData.ScrewBarrier);
		}

		private float GetNewAngle()
		{
			_currentBarrierAngle += Random.Range(_maxAngle * 0.15f, _maxAngle);
			return _currentBarrierAngle;
		}

		public void ScrewActive(bool isActive)
		{
			_screw.gameObject.SetActive(isActive);
		}
	}
}
