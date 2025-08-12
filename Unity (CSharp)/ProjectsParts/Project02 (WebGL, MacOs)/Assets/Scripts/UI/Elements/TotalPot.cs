using UnityEngine;
using DG.Tweening;

namespace it.UI.Elements
{
	public class TotalPot : MonoBehaviour
	{
		[SerializeField] private Bets _bet;
		private bool _isUp;

		public void MoveUp()
		{

			if (_isUp) return;
			_isUp = true;
			_bet.Content.DOLocalMove(new Vector2(0, 15), 0.3f);
		}
		public void MoveDown()
		{

			if (!_isUp) return;
			_isUp = false;
			_bet.Content.DOLocalMove(new Vector2(0, 0), 0.3f);
		}

		public void ResetPosition()
		{
			if (!_isUp) return;
			_isUp = false;
			_bet.Content.localPosition = new Vector2(0, 0);
		}

	}
}