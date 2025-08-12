 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoLine : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI _textLabel;
	[SerializeField] private Image _back;

	private bool _timer;
	private Coroutine _timerCoroutine;

	public void SetWait()
	{
		if (_timerCoroutine != null)
			StopCoroutine(_timerCoroutine);

#if UNITY_STANDALONE
		_textLabel.fontSize = 14.5f;		
#endif
		_textLabel.text = "game.blackLine.waitForPlayers".Localized();
		_back.gameObject.SetActive(true);
	}

	public void StartTimer(float time)
	{
		_back.gameObject.SetActive(true);
		_timerCoroutine = StartCoroutine(Timer(time));
	}

	IEnumerator Timer(float time)
	{
#if UNITY_STANDALONE
		_textLabel.fontSize = 30f;	
#endif
		while (time > 0)
		{
			_textLabel.text = ((int)time).ToString();
			yield return new WaitForSeconds(1);
			time--;
		}
		_textLabel.text = "game.blackLine.gameStart".Localized();
		yield return new WaitForSeconds(1);
		_back.gameObject.SetActive(false);
	}

	public void SetValue(string value)
	{
		StopAllCoroutines();
		_textLabel.text = value;
		_back.gameObject.SetActive(true);
	}

	public void DisableLabel()
	{
		_back.gameObject.SetActive(false);
	}

}
