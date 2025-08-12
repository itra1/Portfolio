using System.Collections;
using UnityEngine;

namespace Garilla
{
	public class LinkOpener : MonoBehaviour
	{
		[SerializeField] private string _link;

		public void Open()
		{
			Garilla.LinkManager.OpenUrl(_link);
		}

	}
}