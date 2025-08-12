using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logger : Singleton<Logger> {

	public RectTransform content;
	public LoggerElement loggerElement;
	public Text textgui;

	private string test;

	private float startPosition = 0;
	

	protected override void Awake() {
		base.Awake();
		//Application.logMessageReceived += AddLogger;
		//Application.logMessageReceivedThreaded += AddLogger;
	}

	public void AddLogger(string condition, string stackTrace, LogType type) {
		
		try {
			textgui.text = textgui.text + '\n' + condition;
			content.sizeDelta = new Vector2(content.sizeDelta.x, 50);
		}
		catch {
		}
		
		/*
		GameObject inst = Instantiate(loggerElement.gameObject);
		inst.SetActive(true);

		LoggerElement newLog = inst.GetComponent<LoggerElement>();
		RectTransform newLogRect = inst.GetComponent<RectTransform>();
		newLogRect.SetParent(content);
		newLogRect.localScale = Vector3.one;
		newLogRect.anchoredPosition = new Vector2(0, -startPosition);
		startPosition += newLog.SetData(condition, stackTrace) + 10;
		content.sizeDelta = new Vector2(content.sizeDelta.x, startPosition);
		*/
	}

	public class LoggedElement {
		public string condition;
		public string stackTrace;
	}
	
}
