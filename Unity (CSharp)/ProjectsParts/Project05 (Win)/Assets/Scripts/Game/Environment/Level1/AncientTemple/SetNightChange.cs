using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using com.ootii.Messages;

namespace it.Game.Environment.Level1.AncientTemple
{

  /// <summary>
  /// Установка ночи для первого уровня
  /// </summary>
  public class SetNightChange : MonoBehaviourBase
  {
	 private Material _DaySkyBox;
	 private Material _NightSkyBox;

	 // Дневные настройки света
	 private Color _dayLightColor = new Color(224, 200, 166, 254);
	 private float _dayIntensity = 2.24f;

	 // Ночные настройки света
	 private Color _nightLightColor = new Color(0.239f, 0.239f, 0.239f, 1);
	 private float _nightIntensity = 0.47f;

	 private void Awake()
	 {
		_DaySkyBox = Resources.Load<Material>("SkyBoxes/Level1Day");
		_NightSkyBox = Resources.Load<Material>("SkyBoxes/Level1Night");
		_DaySkyBox.SetFloat("_Exposure", 1);
		_NightSkyBox.SetFloat("_Exposure", 1);
	 }

	 public void PlayChange()
	 {
		Light sun = RenderSettings.sun;

		_dayLightColor = sun.color;
		_dayIntensity = sun.intensity;


		sun.DOIntensity(_nightIntensity, 1);
		sun.DOColor(_nightLightColor, 1);

		DOTween.To(
		 () => _DaySkyBox.GetFloat("_Exposure"),
		 x => _DaySkyBox.SetFloat("_Exposure", x),
		 0, 0.5f
		  ).OnComplete(

		  () =>
		  {
			 _DaySkyBox.SetFloat("_Exposure", 1);
			 _NightSkyBox.SetFloat("_Exposure", 0);
			 RenderSettings.skybox = _NightSkyBox;
			 MessageDispatcher.SendMessage("SetNight");

			 DOTween.To(
			  () => _NightSkyBox.GetFloat("_Exposure"),
			  x => _NightSkyBox.SetFloat("_Exposure", x),
			  1, 0.5f
			 );

		  }

		  );

	 }

  }
}