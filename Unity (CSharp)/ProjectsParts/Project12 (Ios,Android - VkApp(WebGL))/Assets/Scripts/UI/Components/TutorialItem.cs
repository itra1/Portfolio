using System.Collections.Generic;
using Game.Scripts.UI.Components.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	[RequireComponent(typeof(Button))]
	public class TutorialItem : MonoBehaviour, ITutorialItem
	{
		[SerializeField] private TMP_Text _nameLabel;
		[SerializeField] private TMP_Text _authorLabel;
		[SerializeField] private Button _button;

		private List<ITutorialItem> _itemsList = new();

		[Inject]
		private void Constructor()
		{
			_button.onClick.RemoveAllListeners();
			_button.onClick.AddListener(SelfTouch);

		}

		private void Build()
		{

		}

		private void SelfTouch()
		{

		}

	}
}
