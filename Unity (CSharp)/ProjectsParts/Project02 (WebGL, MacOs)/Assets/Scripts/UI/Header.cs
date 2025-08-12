using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

namespace it.UI
{
	public class Header : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _playersOnline;
		[SerializeField] private TextMeshProUGUI _welcomeMusic;
		[SerializeField] private RectTransform _titleRT;
		[SerializeField] private bool IsGame;

		private void Awake()
		{
			transform.SetParent(GetComponentInParent<UiManager>().HeaderRect);

			//var c = GetComponentInParent<Canvas>();
			//transform.SetParent(c.transform);
			//var mycanvas = gameObject.AddComponent<Canvas>();
			//mycanvas.overrideSorting = true;
			//mycanvas.gameObject.AddComponent<GraphicRaycaster>();
			//mycanvas.sortingOrder = IsGame ? 25001 : 25000;
		}

		private void OnEnable()
		{
			I2.Loc.LocalizationManager.OnLocalizeEvent -= OnLocalize;
			I2.Loc.LocalizationManager.OnLocalizeEvent += OnLocalize;
			Localization();
		}

		public void CloseButton()
		{
			var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.ExitPopup>(PopupType.Exit);

			popup.OnOk = () =>
			{
#if UNITY_STANDALONE
				StandaloneController.CloseAllTable();
				StandaloneController.Instance.CloseApplication();
#endif
			};

		}

		public void ExpandButton()
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.ExpandApplication();
#endif
		}

		public void MinimizeButton()
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.MinimizeApplication();
#endif
		}
		public void OnLocalize()
		{
			Localization();
		}
		[ContextMenu("Localize")]
		private void Localization()
		{
			//if(_title != null)
			//_title.text = LocalizationManager.GetTranslation("main.header.title");
			//if(_playersOnline != null)
			//_playersOnline.text = LocalizationManager.GetTranslation("main.header.playersOnline");
			//if(_welcomeMusic != null)
			//_welcomeMusic.text = LocalizationManager.GetTranslation("main.header.welcomeMusic");

			if (_playersOnline != null)
			{

				//RectTransform onlineRT = _playersOnline.GetComponent<RectTransform>();
				//onlineRT.sizeDelta = new Vector2(_playersOnline.preferredWidth, onlineRT.sizeDelta.y);
				//RectTransform musicRT = _welcomeMusic.GetComponent<RectTransform>();

				//float ancorX = _titleRT.anchoredPosition.x;
				//ancorX += _title.preferredWidth;
				//ancorX += 8;
				//_playersOnlineRT.anchoredPosition = new Vector2(ancorX, _playersOnlineRT.anchoredPosition.y);
				//_playersOnlineRT.sizeDelta = new Vector2(onlineRT.rect.width + 35, _playersOnlineRT.sizeDelta.y);
				//ancorX += _playersOnline.preferredWidth;
				//ancorX += 40;
				//_welcomeMusicRT.anchoredPosition = new Vector2(ancorX, _welcomeMusicRT.anchoredPosition.y);
				//_welcomeMusicRT.sizeDelta = new Vector2(musicRT.rect.width + 30, _welcomeMusicRT.sizeDelta.y);
			}
		}

		public void PointerDown()
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.WindowDragStart();
#endif
		}

		public void PointerUp()
		{
#if UNITY_STANDALONE
			StandaloneController.Instance.WindowDragStop();
#endif
		}

		public void GameRulesButton()
		{

		}
		public void SetGridTableOrderTouch()
		{
			PlayerPrefs.SetString(StringConstants.BUTTON_GRID_TABLEORDER, "");
		}

		public void SetQueueTableOrderTouch()
		{
			PlayerPrefs.SetString(StringConstants.BUTTON_QUEUE_TABLEORDER, "");
		}

	}
}