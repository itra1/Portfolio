using it.Main;
using System.Collections;
using UnityEngine;
using TMPro;

namespace it.Main
{
	public class VipGameSheet : SheetBase
	{
		[SerializeField] private TMP_InputField _searchField;

		protected override void OnEnable()
		{
			_searchField.text = "";
			base.OnEnable();
			if (_searchField != null)
			{
				_searchField.onValueChanged.RemoveAllListeners();
				_searchField.onValueChanged.AddListener((vel) =>
				{
					Search(_searchField.text);
				});
			}

		}

		public void CreateTable()
		{
			TableManager.Instance.CreateTable();
		}

	}
}