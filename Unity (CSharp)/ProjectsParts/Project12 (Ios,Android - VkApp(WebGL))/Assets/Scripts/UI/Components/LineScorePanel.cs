using Engine.Scripts.Base;
using Game.Scripts.Scoring;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class LineScorePanel : MonoBehaviour, IInjection
	{
		[SerializeField] private RectTransform _progressRectTransform;

		private ScoreManager _scoreManager;
		private float _lineWidth;

		[Inject]
		private void Constructor(ScoreManager scoreManager)
		{
			_scoreManager = scoreManager;

			Build();

			_scoreManager.OnScoreChange.AddListener(ChangeHandler);
		}

		~LineScorePanel()
		{
			_scoreManager.OnScoreChange.RemoveListener(ChangeHandler);
		}

		private void Build()
		{
			_lineWidth = _progressRectTransform.rect.width;
		}

		public void ChangeHandler(float value)
		{
		}
	}
}
