using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using it.Network.Rest;

public class PasswordPopup : MonoBehaviour
{
	public TMP_InputField ValueField;
	public TMP_Text ErrorText;
	private Action<bool> callback;
	private Table table;

	public void Show(Table table, Action<bool> callback)
	{
		this.table = table;
		this.callback = callback;
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		callback(false);
	}

	public void Submit()
	{
		if (ValueField.text.Length > 0)
		{
			GameHelper.ObserveTable(table, ValueField.text, (table) =>
			{
				callback(true);
				Hide();
			}, (error) =>
			{

				//var data = (ErrorRest)it.Helpers.ParserHelper.Parse(typeof(ErrorRest), error);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);
				if (data.errors[0].id == "user_is_already_player")
				{
					Hide();
					callback(true);
					return;
				}

				ErrorText.text = "Incorrect password";
			});
		}
		else
		{
			ErrorText.text = "Not valid password";
		}
	}
}

