using Garilla.Games;
using it.UI.Elements;
using System.Collections;
using TMPro;
using UnityEngine;

namespace it.Game.Panels
{
	public class StraddlePanel : MonoBehaviour
	{
		[SerializeField] private GraphicButtonUI _straddleButton;
		public GameControllerBase GameManager { get => _gameManager; set => _gameManager = value; }

		private Garilla.Games.GameControllerBase _gameManager;
		private bool _isReauest;

		private void Awake()
		{
			SetVisibleButton(false);
		}

		/// <summary>
		/// Клик по кнопке интерфейса
		/// </summary>
		public void StraddleButtonTouch()
		{
			_straddleButton.interactable = false;
			SetVisibleButton(false);
			TableApi.Straddle(_gameManager.SelectTable.id, (result) =>
			{
				if (!result.IsSuccess)
				{
					_straddleButton.interactable = true;
					SetVisibleButton(true);
					return;
				}
				SetVisibleButton(false);

			});
		}

		/// <summary>
		/// Установка видимости кнопки
		/// </summary>
		/// <param name="isVisible">Показать</param>
		public void SetVisibleButton(bool isVisible){
			_straddleButton.gameObject.SetActive(isVisible);
			if(isVisible)
				_straddleButton.interactable = true;

		}

	}
}