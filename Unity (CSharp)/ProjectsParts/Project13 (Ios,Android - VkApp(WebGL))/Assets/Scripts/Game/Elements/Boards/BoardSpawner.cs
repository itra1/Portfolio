using System.Collections.Generic;
using Game.Game.Elements.Barriers;
using Game.Game.Elements.Bonuses;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Scenes;
using Game.Game.Settings;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using Game.Providers.Battles.Interfaces;
using UnityEngine;

namespace Game.Game.Elements.Boards
{
	public class BoardSpawner
	{
		private BoardFactory _factory;
		private BarrierFactory _barrierFactory;
		private BonusFactory _bonusFactory;
		private AudioHandler _audioHandler;
		private int _bonusIndex;
		private IGameScene _gameScene;
		public BoardSpawner(
			BoardFactory factory,
			BarrierFactory barrierFactory,
			BonusFactory bonusfactory,
			AudioHandler audioHandler,
			IGameScene gameScene
		)
		{
			_gameScene = gameScene;
			_factory = factory;
			_barrierFactory = barrierFactory;
			_bonusFactory = bonusfactory;
			_audioHandler = audioHandler;
		}

		public void Despawn(IBoard board)
		{
			if (board != null && board is Component boardComponent)
				boardComponent.gameObject.SetActive(false);
		}
		public Board SpawnNew(string boardName, IBoardStageSettings stageData)
		{
			var board = _factory.GetInstance(boardName, _gameScene.BoardPoint);
			board.transform.localPosition = Vector3.zero;

			var rotationcomponent = board.GetComponent<BoardRotation>();
			rotationcomponent.enabled = false;
			rotationcomponent.ResetRotate();
			board.SetData(stageData);

			//if (stageData.Formation.Count > 0)
			//{
			//	SpawnCustomBarrier(ref board, stageData.Formation);
			//	SpawnCustomBonuses(ref board, stageData.Formation);
			//}
			//else
			//{
			//	var existsList = SpawnRandomBarrier(ref board, stageData.Barriers);
			//	SpawnRandomBonuses(ref board, existsList, stageData.Bonuses);
			//}

			rotationcomponent.enabled = true;
			rotationcomponent.RandomRotate();
			board.GetComponent<BoardRotation>().SetRotation(stageData.BoardRotation);
			rotationcomponent.enabled = true;
			board.gameObject.SetActive(true);
			return board;
		}
		public string GetRandomKey() => _factory.GetRandomKey();

		private List<Vector3> SpawnRandomBarrier(ref Board board, BoardBarrier barrierData)
		{
			var startAngle = UnityEngine.Random.Range(0, 360f);
			var count = UnityEngine.Random.Range(barrierData.Counts.Min, barrierData.Counts.Max + 1);

			List<Vector3> positions = new();

			if (count == 0)
				return positions;
			for (var i = 0; i < count; i++)
			{
				var barrier = barrierData.Types.Count > 0 ? _barrierFactory.GetRandomInstance(barrierData.Types, null) : _barrierFactory.GetRandomInstance(null);
				barrier.transform.rotation = Quaternion.identity;
				barrier.FixedOnBoard(board);
				barrier.transform.localScale = Vector3.one;
				barrier.transform.eulerAngles = new(0, 0, startAngle);
				barrier.transform.localPosition = barrier.transform.up * -0.93f;
				positions.Add(barrier.transform.localPosition);
				barrier.gameObject.SetActive(true);
				barrier.ClearVelocity();
				startAngle += UnityEngine.Random.Range(barrierData.Distances.Min, barrierData.Distances.Max);
			}
			return positions;
		}

		private void SpawnCustomBarrier(ref Board board, List<BoardFormationItem> barrierData)
		{
			var items = barrierData.FindAll(x => x.Type == GameCollisionName.Barrier);
			var count = items.Count;

			if (count == 0)
				return;
			for (var i = 0; i < items.Count; i++)
			{
				var barrier = _barrierFactory.GetInstance(items[i].SybType, null);
				barrier.FixedOnBoard(board);
				barrier.transform.localScale = items[i].LocalScale;
				barrier.transform.localEulerAngles = items[i].LocalRotation;
				barrier.transform.localPosition = items[i].LocalPosition;
				barrier.ClearVelocity();
				barrier.gameObject.SetActive(true);
			}
		}

		private void SpawnRandomBonuses(ref Board board, List<Vector3> positions, BonusStruct bonusData)
		{
			var startAngle = UnityEngine.Random.Range(0, 360f);
			var count = UnityEngine.Random.Range(bonusData.Counts.Min, bonusData.Counts.Max + 1);
			_bonusIndex = -1;
			if (count == 0)
				return;
			for (var i = 0; i < count; i++)
			{
				var bonus = bonusData.Types.Count > 0 ? _bonusFactory.GetRandomInstance(bonusData.Types, null) : _bonusFactory.GetRandomInstance(null);
				bonus.gameObject.SetActive(true);
				bonus.transform.rotation = Quaternion.identity;
				bonus.FixedOnBoard(board);
				bonus.transform.localScale = Vector3.one;

				bonus.OnDestroy = () =>
				{
					_ = _audioHandler.PlayIndexClip(SoundNames.Bonus, ++_bonusIndex);
				};

				do
				{
					startAngle += 5;
					bonus.transform.eulerAngles = new(0, 0, startAngle);
					bonus.transform.localPosition = bonus.transform.up * 0.93f;
				} while (positions.Exists(x => (x - bonus.transform.localPosition).magnitude < 0.5f));
				positions.Add(bonus.transform.localPosition);
				startAngle += UnityEngine.Random.Range(bonusData.Distances.Min, bonusData.Distances.Max);
			}
		}

		private void SpawnCustomBonuses(ref Board board, List<BoardFormationItem> barrierData)
		{
			var items = barrierData.FindAll(x => x.Type == GameCollisionName.Bonus);
			var count = items.Count;

			if (count == 0)
				return;
			for (var i = 0; i < items.Count; i++)
			{
				var inst = _bonusFactory.GetRandomInstance(null);
				inst.FixedOnBoard(board);
				inst.transform.localScale = items[i].LocalScale;
				inst.transform.localEulerAngles = items[i].LocalRotation;
				inst.transform.localPosition = items[i].LocalPosition;
			}
		}
	}
}
