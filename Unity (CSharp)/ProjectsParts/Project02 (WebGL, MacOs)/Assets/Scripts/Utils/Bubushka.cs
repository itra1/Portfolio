using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Garilla
{
	public class Bubushka : MonoBehaviour
	{
		public Image FillImage;

		public void Fill(bool isActive)
		{
			FillImage.gameObject.SetActive(isActive);
		}

	}
}