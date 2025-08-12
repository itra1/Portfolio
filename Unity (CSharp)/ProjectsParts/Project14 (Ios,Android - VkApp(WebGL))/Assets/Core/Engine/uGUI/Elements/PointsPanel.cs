using Core.Engine.Components.User;

using System.Collections;

using TMPro;

using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Elements
{
	public class PointsPanel : MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _userNameLabel;

		private IUserProvider _userProvider;

		[Inject]
		public void Initiate(IUserProvider userProvider)
		{
			_userProvider = userProvider;
		}

		private void OnEnable()
		{
			_userNameLabel.text = _userProvider.PointsCount.ToString();
		}
	}
}