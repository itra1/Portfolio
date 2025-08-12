using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Base;
using Game.Providers.Battles;
using Game.Providers.Battles.Settings;
using Game.Providers.Battles.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TournamentResultListView : MonoBehaviour, IInjection
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private RectTransform _noDataIcone;
		[SerializeField] private RectTransform _waitBlock;
		[SerializeField] private RectTransform _completeBlock;

		private IBattleProvider _provider;
		private TournamentResultView.Factory _tournamentFactory;
		private SignalBus _signalBus;
		private readonly float _separate = 10;
		private List<TournamentResultView> _listItems = new();

		[Inject]
		public void Constructor(SignalBus signalBus, IBattleProvider tournamentProvider, TournamentResultView.Factory tournamentResultFactory)
		{
			_provider = tournamentProvider;
			_tournamentFactory = tournamentResultFactory;
			_signalBus = signalBus;
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<BattleResultChangeSignal>(MakeList);
			MakeList();
			StopAllCoroutines();
			_ = StartCoroutine(UpdateCoroutine());
		}
		public void OnDisable()
		{
			_signalBus.Unsubscribe<BattleResultChangeSignal>(MakeList);
			StopAllCoroutines();
		}

		private IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(60);
				MakeList();
			}
		}

		private void MakeList()
		{

			_listItems.ForEach(x => x.Despawned());
			_listItems.Clear();

			_noDataIcone.gameObject.SetActive(_provider.Results.Count <= 0);
			_scrollRect.gameObject.SetActive(_provider.Results.Count > 0);

			_scrollRect.movementType = ScrollRect.MovementType.Clamped;

			float summaryHeight = 0;

			var waitBlock = PrintRecordsItems(_waitBlock, (from list in _provider.Results
																										 orderby list.ExistsReward descending, list.TimeComplete descending
																										 where list.WaitComplete
																										 select list).ToList());

			if (waitBlock == 0)
			{
				_waitBlock.gameObject.SetActive(false);
			}
			else
			{
				_waitBlock.gameObject.SetActive(true);
				_waitBlock.anchoredPosition = new(_waitBlock.anchoredPosition.x, -summaryHeight);
				_waitBlock.sizeDelta = new(_waitBlock.sizeDelta.x, waitBlock);
				summaryHeight += 10;
			}
			summaryHeight += waitBlock;

			var completeBlock = PrintRecordsItems(_completeBlock, (from list in _provider.Results
																														 orderby list.ExistsReward descending, list.TimeComplete descending
																														 where !list.WaitComplete
																														 select list).ToList());

			if (completeBlock == 0)
			{
				_completeBlock.gameObject.SetActive(false);
			}
			else
			{
				_completeBlock.gameObject.SetActive(true);
				_completeBlock.anchoredPosition = new(_completeBlock.anchoredPosition.x, -summaryHeight);
				_completeBlock.sizeDelta = new(_completeBlock.sizeDelta.x, waitBlock);
			}
			summaryHeight += completeBlock;

			_scrollRect.content.sizeDelta = new(_scrollRect.content.sizeDelta.x, summaryHeight);
		}

		private float PrintRecordsItems(RectTransform parent, List<BattleResult> records)
		{

			if (records.Count == 0)
				return 0;

			float height = 135;

			for (var i = 0; i < records.Count; i++)
			{
				var inst = _tournamentFactory.Create(records[i]);
				_listItems.Add(inst);
				inst.transform.SetParent(parent);
				var providerRect = inst.transform as RectTransform;
				providerRect.localScale = Vector3.one;
				providerRect.transform.localPosition = Vector3.zero;
				providerRect.sizeDelta = new(0, providerRect.sizeDelta.y);
				providerRect.anchoredPosition = new Vector2(0, -135 + (-(providerRect.rect.height + _separate) * i));
				height += providerRect.rect.height + _separate;
			}
			return height;
		}

	}
}
