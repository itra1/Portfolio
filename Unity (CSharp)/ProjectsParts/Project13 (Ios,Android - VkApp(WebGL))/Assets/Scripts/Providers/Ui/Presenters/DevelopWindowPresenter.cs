using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Presenters.Base;
using TMPro;
using UnityEngine;

namespace Game.Providers.Ui.Presenters
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.Develop)]
	public class DevelopWindowPresenter : WindowPresenter
	{
		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _description;
		[SerializeField] private string _titleText;
		[SerializeField] private string _descriptionText;

		public void SetTitle(string text) => _title.text = text;

		public void SetDescription(string text) => _description.text = text;

		public void SetDefault()
		{
			SetTitle(_titleText);
			SetDescription(_descriptionText);
		}

		public void OkButtonTouch()
		{
			Hide();
		}
	}
}
