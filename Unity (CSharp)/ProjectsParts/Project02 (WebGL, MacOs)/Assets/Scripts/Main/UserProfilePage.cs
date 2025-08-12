using System.Collections;
using UnityEngine;
using it.UI.Profiles;
using it.Animations;

namespace it.UI
{
	public class UserProfilePage : MainContentPage
	{

		private void OnEnable()
		{
			IVisibleAnimation[] animComponents = GetComponentsInChildren<IVisibleAnimation>();
			for(int i = 0; i < animComponents.Length;i++)
				animComponents[i].PlayAnimation();
		}

	}
}