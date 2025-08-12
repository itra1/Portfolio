using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// Обработчик полосы жизней
/// </summary>
public class EnemyLiveLine : MonoBehaviour {

	/// <summary>
	/// Полоса с текущим значением жизней
	/// </summary>
	public RectTransform liveValue;
  public RectTransform blackRectValue;
  private float _maxValue;
	private float _activeValue;

	public Image shieldYellow;
	public Image shieldBlue;
	public Image bootIcon;
	public Image demonIcon;
	public Image backLineYellow;
	public Image backLineBlue;
	public Image liveImage;
  public Image backRectImage;

  private void OnDisable() {
		StopAllCoroutines();
		_color1Sec = false;
		_color3Sec = false;
	}

	/// <summary>
	/// Установка текущего размера полосы значения жизней
	/// </summary>
	/// <param name="maxValue">Mаксимальное значение</param>
	/// <param name="value">Текущее значение</param>
	public void SetValue(Enemy parent) {

    liveValue.GetComponent<RectMask2D>().PerformClipping();


    blackRectValue.sizeDelta = new Vector2(284-(284 * ((parent.liveNow / parent.startLive >= 0 ? parent.liveNow / parent.startLive : 0))), blackRectValue.sizeDelta.y);


    backLineBlue.gameObject.SetActive(parent.isShield);
		backLineYellow.gameObject.SetActive(!parent.isShield);
		demonIcon.gameObject.SetActive(parent.isAngry);
		bootIcon.gameObject.SetActive(parent.isBaffSpead);
		shieldBlue.gameObject.SetActive(parent.isShield && !parent.isAngry && !parent.isBaffSpead);
		shieldYellow.gameObject.SetActive(!parent.isShield && parent.armor > 0 && !parent.isAngry && !parent.isBaffSpead);
		liveImage.gameObject.SetActive(true);
    backRectImage.gameObject.SetActive(true);

    if (parent.isAngry) {
			demonIcon.color = new Color(1, 1, 1, 1);
		}

		if (parent.isBaffSpead) {
			bootIcon.color = new Color(1, 1, 1, 1);
		}

		if (parent.isShield && !parent.isAngry && !parent.isBaffSpead) {
			shieldBlue.color = new Color(1, 1, 1, 1);
		}

		if (!parent.isShield) {
			if (parent.armor > 0)
				shieldYellow.color = new Color(1, 1, 1, 1);
		}

		if (parent.startLive == parent.liveNow || this._activeValue == parent.liveNow) {
			if (liveImage.color.a <= 0) {
				backLineBlue.gameObject.SetActive(false);
				backLineYellow.gameObject.SetActive(false);
				liveImage.gameObject.SetActive(false);
        backRectImage.gameObject.SetActive(false);

      }

			if(!_color1Sec && !_color3Sec)
				_sec1Cor = StartCoroutine(ColorChange1Sec());

		} else if (parent.liveNow > 0) {

			if (parent.isShield) 
				backLineBlue.color = new Color(1, 1, 1, 1);
			else
				backLineYellow.color = new Color(1, 1, 1, 1);
			
			liveImage.color = new Color(1, 1, 1, 1);
      backRectImage.color = new Color(0, 0, 0, 1);


      if (_color1Sec) {
				StopCoroutine(_sec1Cor);
				_color1Sec = false;
			}

			if(!_color3Sec && !_color1Sec)
				_sec3Cor = StartCoroutine(ColorChange3Sec());
		}

		this._maxValue = parent.startLive;
		this._activeValue = parent.liveNow;

	}

	private bool _color1Sec;
	private bool _color3Sec;
	private Coroutine _sec1Cor;
	private Coroutine _sec3Cor;
	
	IEnumerator ColorChange3Sec() {
		_color3Sec = true;

		while (shieldYellow.color.a > 0) {
			yield return null;

			shieldYellow.color = new Color(1, 1, 1, shieldYellow.color.a - 0.34f*Time.deltaTime);
			shieldBlue.color = new Color(1, 1, 1, shieldBlue.color.a - 0.34f * Time.deltaTime);
			backLineYellow.color = new Color(1, 1, 1, backLineYellow.color.a - 0.34f * Time.deltaTime);
			backLineBlue.color = new Color(1, 1, 1, backLineBlue.color.a - 0.34f * Time.deltaTime);
			liveImage.color = new Color(1, 1, 1, liveImage.color.a - 0.34f * Time.deltaTime);
      backRectImage.color = new Color(0, 0, 0, backRectImage.color.a - 0.34f * Time.deltaTime);

      bootIcon.color = new Color(1, 1, 1, bootIcon.color.a - 0.34f * Time.deltaTime);
			demonIcon.color = new Color(1, 1, 1, demonIcon.color.a - 0.34f * Time.deltaTime);
		}
		_color3Sec = false;
	}

	IEnumerator ColorChange1Sec() {
		_color1Sec = true;

		while (shieldYellow.color.a > 0) {
			yield return null;
			shieldYellow.color = new Color(1, 1, 1, shieldYellow.color.a - 1f * Time.deltaTime);
			shieldBlue.color = new Color(1, 1, 1, shieldBlue.color.a - 1f * Time.deltaTime);

			bootIcon.color = new Color(1, 1, 1, bootIcon.color.a - 1f * Time.deltaTime);
			demonIcon.color = new Color(1, 1, 1, demonIcon.color.a - 1f * Time.deltaTime);
		}
		_color1Sec = false;
	}

}
