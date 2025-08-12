using System.Collections;

using UnityEngine;

namespace Assets.Scripts.UI.Elements
{
	public class IosObjectDisable : MonoBehaviour
	{
#if UNITY_IOS
		private void Awake()
		{
			gameObject.SetActive(false);
		}
#endif
	}
}