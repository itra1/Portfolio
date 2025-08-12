using Settings.Themes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMPro;

using UGui.Screens.Common;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace UGui.Elements
{
	public class RegistrationTypeToggle : MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _label;
		[SerializeField] private GameObject _icone;
		[SerializeField] private bool _defaultSelect;

		private Toggle _toggle;
		private ITheme _theme;

		[Inject]
		private void Initiate(ITheme theme){
			_theme = theme;
		}

		private void Awake()
		{
			_toggle = GetComponent<Toggle>();

			_toggle.onValueChanged.RemoveAllListeners();
			_toggle.onValueChanged.AddListener(VisibleChange);

			VisibleChange(_defaultSelect);
		}

		private void VisibleChange(bool isSelect){
			_label.color = isSelect ? Color.white : _theme.PlaceholderColor;
			_icone.gameObject.SetActive(isSelect);
		}

	}
}
