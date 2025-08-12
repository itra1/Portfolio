using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditGame : MonoBehaviour
{
	[SerializeField] private List<Button> images;

	private void Start()
	{

	}

	public void SetColorImage(int index)
	{
		for (int i = 0; i < images.Count; i++)
		{
			if (i == index)
			{



				images[i].image.color = new Color(1, 1, 1);

			}
			else
			{

				images[i].image.color = new Color(0.2169811f, 0.209612f, 0.209612f);



			}
		}
	}
}
