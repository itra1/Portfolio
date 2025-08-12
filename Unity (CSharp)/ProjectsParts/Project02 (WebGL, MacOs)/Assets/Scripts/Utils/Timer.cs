using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private RectTransform _timeLineRect;
	[SerializeField] private RectTransform _maskLineRect;
	[SerializeField] private Image _lightImage;
	[SerializeField] private Sprite _defaultSprite;
	[SerializeField] private Sprite _timeBankSprite;
	[SerializeField] private Sprite _defaultLightSprite;
	[SerializeField] private Sprite _timeBankLightSprite;

	public bool IsMe;

	//#if !UNITY_WEBGL
	//	DarkTonic.MasterAudio.PlaySoundResult _loopTimerSound;
	//#endif
	private double _lastTimerSound;
	private int _lastTimerIndexTick;
	private float currentTime;
	private bool isStartTimer = false;
	private double _startDiff;
	[HideInInspector] private DateTime _endTime;
	[HideInInspector] private System.TimeSpan _startDeltaTime;

	private RectTransform _parentRect;
	private void Awake()
	{
		if (_parentRect == null)
			_parentRect = _timeLineRect.parent.GetComponent<RectTransform>();

	}

	public void StartTimer(DateTime endTime)
	{
		if (!isStartTimer || endTime != this._endTime)
		{
			StopTimer();
			_lastTimerIndexTick = 0;
			this._endTime = endTime;
			_startDiff = (_endTime - GameHelper.NowTime).TotalSeconds;
//#if !UNITY_WEBGL
			//if (IsMe && _loopTimerSound == null)
			//{
			//	_loopTimerSound = DarkTonic.MasterAudio.MasterAudio.PlaySound(StringConstants.SOUND_GAME_TIMER, 0);
			//	if (_loopTimerSound != null)
			//		_loopTimerSound.ActingVariation.VarAudio.volume = 0;



			//}
//#endif


			_startDeltaTime = _endTime - GameHelper.NowTime;
			if (_timeLineRect != null)
			{
				_timeLineRect.sizeDelta = Vector2.zero;
				_maskLineRect.sizeDelta = Vector2.zero;
			}
			//_timeLineRect.localScale = new Vector3(1, 1);

			SetVisible(true);

			currentTime = 0;
			isStartTimer = true;
		}
	}

	private void SetVisible(bool isVisible)
	{
		if (gameObject == null) return;

		if (_timeLineRect != null)
		{
			_timeLineRect.sizeDelta = Vector2.zero;
			_maskLineRect.sizeDelta = Vector2.zero;
		}
		//_timeLineRect.localScale = new Vector3(1, 1);

		gameObject.SetActive(isVisible);
	}

	private void FixedUpdate()
	{
		if (!isStartTimer) return;

		var diff = (_endTime - GameHelper.NowTime).TotalSeconds;
		if (diff <= 0)
		{
			StopTimer();
			return;
		}

		if(IsMe && Time.timeAsDouble - _lastTimerSound > 0.10){
			_lastTimerSound = Time.timeAsDouble;
			_lastTimerIndexTick++;
			if (!AppConfig.DisableAudio)
				DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_TIMER /*+ (_lastTimerIndexTick%2).ToString()*/, (float)(1 - diff / _startDiff), null, 0);
		}

		//if (_loopTimerSound != null)
		//	_loopTimerSound.ActingVariation.VarAudio.volume = (float)(1 - diff/_startDiff);

		SetTextTime();
	}

	private void SetTextTime()
	{
		var diff = (_endTime - GameHelper.NowTime).TotalSeconds;
		if (diff > 60)
		{
			TimeSpan ts = TimeSpan.FromSeconds(diff);
			timerText.text = ts.ToString(@"mm\:ss");
		}
		else
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			timerText.text = (int)diff + "s";
#endif
#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
			timerText.text = ((int)diff).ToString();
#endif
		}

		if (_timeLineRect != null)
		{
			//_timeLineRect.localScale = new Vector3((float)diff / (float)_startDeltaTime.TotalSeconds, 1);
			_timeLineRect.sizeDelta = new Vector2(Mathf.Clamp((1 - (float)diff / (float)_startDeltaTime.TotalSeconds), 0, 1) * -_parentRect.rect.width, 0);
			_maskLineRect.sizeDelta = new Vector2(Mathf.Clamp((1 - (float)diff / (float)_startDeltaTime.TotalSeconds), 0, 1) * -_parentRect.rect.width, 0);
		};
	}

	public void StopTimer()
	{
		SetVisible(false);
		_endTime = DateTime.MinValue;
		isStartTimer = false;
		//if(_loopTimerSound != null)
		//	_loopTimerSound.ActingVariation.VarAudio.volume = 0;
	}
	public void SetRedColor()
	{
		_timeLineRect.GetComponent<Image>().sprite = _timeBankSprite;
		_lightImage.sprite = _timeBankLightSprite;
	}
	public void SetResetColor()
	{
		_timeLineRect.GetComponent<Image>().sprite = _defaultSprite;
		_lightImage.sprite = _defaultLightSprite;
	}
}
