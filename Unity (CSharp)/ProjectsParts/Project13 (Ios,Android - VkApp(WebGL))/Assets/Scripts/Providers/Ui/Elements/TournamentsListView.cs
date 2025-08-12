using System.Collections.Generic;
using Game.Base;
using Game.Game.Common;
using Game.Providers.Battles;
using Game.Providers.Battles.Helpers;
using Game.Providers.Battles.Settings;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TournamentsListView : MonoBehaviour, IInjection
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private RectTransform _noDataIcone;
		[SerializeField] private RegistrationPanelView _registrationPanel;
		[SerializeField] private RectTransform _tournamentsTitle;

		private IBattleProvider _provider;
		private TournamentView.Factory _tournamentFactory;
		private IBattleHelper _battleHelper;
		private SignalBus _signalBus;
		private GameSession _gameSession;
		private readonly float _separate = 30;
		private readonly List<TournamentView> _listItems = new();

		[Inject]
		public void Constructor(SignalBus signalBus,
		IBattleProvider battleProvider,
		TournamentView.Factory tournamentFactory,
		GameSession gameSession,
		IBattleHelper tournamentHelper)
		{
			_signalBus = signalBus;
			_provider = battleProvider;
			_tournamentFactory = tournamentFactory;
			_gameSession = gameSession;
			_battleHelper = tournamentHelper;
		}

		public void OnEnable()
		{
			MakeList();
		}

		private void MakeList()
		{
			_listItems.ForEach(x => x.Despawned());
			_listItems.Clear();

			_scrollRect.movementType = ScrollRect.MovementType.Clamped;
			float height = 0;

			//var tournamentList = (from list in _provider.Items
			//											where list.Type == BattleType.Duel
			//											select list).ToList();

			//_noDataIcone.gameObject.SetActive(tournamentList.Count <= 0);
			//_scrollRect.gameObject.SetActive(tournamentList.Count > 0);

			//for (var i = 0; i < tournamentList.Count; i++)
			//{
			//	height += SpawnItem(++index, tournamentList[i]);
			//	height += _separate;
			//}
			_scrollRect.content.sizeDelta = new(_scrollRect.content.sizeDelta.x, height);
		}

		private float SpawnItem(int index, DuelItemSettings item)
		{
			var inst = _tournamentFactory.Create(item);
			_listItems.Add(inst);
			inst.transform.SetParent(_scrollRect.content);
			inst.transform.localScale = Vector3.one;
			inst.transform.localPosition = Vector3.zero;
			inst.OnClick = (tourn) =>
			{
				_battleHelper.RunSolo(tourn, inst.transform as RectTransform);
			};
			var providerRect = inst.transform as RectTransform;
			providerRect.anchoredPosition = new Vector2(0, -40 - (providerRect.rect.height + _separate) * index);
			providerRect.sizeDelta = new(0, providerRect.sizeDelta.y);
			return providerRect.rect.height;
		}
	}
}
