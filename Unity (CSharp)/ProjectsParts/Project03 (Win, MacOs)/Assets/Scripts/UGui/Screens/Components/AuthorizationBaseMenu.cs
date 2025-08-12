using DG.Tweening;

using Settings.Themes;
using System;
using TMPro;
using UGui.Screens.Common;

using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace UGui.Screens.Components
{
	public class AuthorizationBaseMenu : MonoBehaviour, IZInjection
	{
		public Action OnClose;
		[HideInInspector] public UnityEvent OnLogin = new();
		[HideInInspector] public UnityEvent OnRegistration = new();

		[SerializeField] private TMP_Text _loginLabel;
		[SerializeField] private TMP_Text _registrationLabel;
		[SerializeField] private RectTransform _underlineRT;
		[SerializeField] private RectTransform _loginLineRT;
		[SerializeField] private RectTransform _registerLineRT;

		private ITheme _theme;

		[Inject]
		public void Initialize(ITheme theme)
		{
			_theme = theme;
		}

		private void OnEnable()
		{
			OnLoginButton();
			_loginLineRT.gameObject.SetActive(false);
			_registerLineRT.gameObject.SetActive(false);
		}

		private void ClearColorButtons()
		{
			_loginLabel.color = Color.white;
			_registrationLabel.color = Color.white;
		}

		private void FocusLoginButton()
		{
			ClearColorButtons();
			_loginLabel.color = _theme.FocusColor;
		}

		private void FocusRegistrationButton()
		{
			ClearColorButtons();
			_registrationLabel.color = _theme.FocusColor;
		}

		private void MoveUnderLine(RectTransform template){
			_underlineRT.transform.SetParent(template.parent);
			_underlineRT.DOSizeDelta(template.sizeDelta,0.2f);
			_underlineRT.DOAnchorPos(template.anchoredPosition, 0.2f);
		}

		#region Кнопки

		public void OnLoginButton()
		{
			FocusLoginButton();
			MoveUnderLine(_loginLineRT);
			EmitOnLogin();
		}

		public void OnRegistrationBitton()
		{
			FocusRegistrationButton();
			MoveUnderLine(_registerLineRT);
			EmitOnRegistration();
		}

		public void OnCloseButton()
		{
			EmitOnClose();
		}

		#endregion

		#region Emit событий

		public void EmitOnLogin()
		{
			OnLogin?.Invoke();
		}

		public void EmitOnRegistration()
		{
			OnRegistration?.Invoke();
		}

		public void EmitOnClose()
		{
			OnClose?.Invoke();
		}

		#endregion

	}
}
