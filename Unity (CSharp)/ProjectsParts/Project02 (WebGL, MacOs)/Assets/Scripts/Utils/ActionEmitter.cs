using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmitter : MonoBehaviour
{
	[SerializeField] private string _keyAction;
	[SerializeField] private List<string> _options;

	public void Emit()
	{
		ActionController.Instance.Emit(_keyAction, _options);
	}
}