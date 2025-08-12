using System.Collections.Generic;
using Engine.Scripts.Base;
using Game.Scripts.Scoring;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class CheckScorePanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private List<GameObject> _checks = new();

		private ScoreManager _scoreManager;

		[Inject]
		private void Constructor(ScoreManager scoreManager)
		{
			_scoreManager = scoreManager;

			_scoreManager.OnStarChange.AddListener(ChangeHandler);
		}

		public void ChangeHandler(int count, float value)
		{
			for (int i = 0; i < _checks.Count; i++)
				_checks[i].SetActive(i < count);
		}

		public void Show()
		{
			for (int i = 0; i < _checks.Count; i++)
				_checks[i].SetActive(false);
		}

		public void Hide()
		{
		}
	}
}
