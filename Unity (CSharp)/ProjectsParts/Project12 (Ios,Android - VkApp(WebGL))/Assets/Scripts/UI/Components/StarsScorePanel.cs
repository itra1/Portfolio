using System.Collections.Generic;
using Engine.Scripts.Base;
using Game.Scripts.Custom;
using Game.Scripts.Scoring;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class StarsScorePanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private List<SpecialPointUi> _stars = new();

		private ScoreManager _scoreManager;

		[Inject]
		private void Constructor(ScoreManager scoreManager)
		{
			_scoreManager = scoreManager;

			_scoreManager.OnStarChange.AddListener(ChangeHandler);
		}

		~StarsScorePanel()
		{
			_scoreManager.OnStarChange.RemoveListener(ChangeHandler);
		}

		public void ChangeHandler(int count, float value)
		{
			for (int i = 0; i < _stars.Count; i++)
			{
				if (i < count)
					_stars[i].SetFillValue(1);
				if (i == count)
					_stars[i].SetFillValue(value);
				if (i > count)
					_stars[i].SetFillValue(0);
			}
		}

		public void Show()
		{
			for (int i = 0; i < _stars.Count; i++)
				_stars[i].SetFillValue(0);
		}

		public void Hide()
		{
		}
	}
}
