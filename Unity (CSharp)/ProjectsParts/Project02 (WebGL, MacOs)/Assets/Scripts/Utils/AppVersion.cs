using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AppVersion : MonoBehaviour
{
	private void Awake()
	{
		var txtComp = GetComponent<TextMeshProUGUI>();
		txtComp.text = Application.version;
	}
}