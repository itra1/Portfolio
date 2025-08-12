using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionPrint : MonoBehaviour
{
	public TMPro.TextMeshProUGUI TextLabel;

	private void Awake()
	{
		TextLabel.text = Application.version;
	}

}
