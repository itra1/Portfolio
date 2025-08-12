using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;
using DG.Tweening;
using com.ootii.Messages;

namespace it.Game.Environment.Level1.AncientTemple
{
  public class AncientTemple : Environment
  {
	 [SerializeField]
	 private GhostBlue[] _ghostBlueArr;

	 [SerializeField]
	 private Light[] _lights;

	 protected override void Awake()
	 {
		base.Awake();
		_DaySkyBox = Resources.Load<Material>("SkyBoxes/Level1Day");
		_NightSkyBox = Resources.Load<Material>("SkyBoxes/Level1Night");
		_DaySkyBox.SetFloat("_Exposure", 1);
		_NightSkyBox.SetFloat("_Exposure", 1);
	 }

	 /// <summary>
	 /// Установка ночи
	 /// </summary>
	 [ContextMenu("Play  to night change")]
	 public void SetNight()
	 {
		if (State == 1)
		  return;
		State = 1;
		Save();
		ConfirmState();
	 }


	 private void SetGhost()
	 {
		foreach (var el in _ghostBlueArr)
		  el.Visible();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State > 0)
		{
		  if (isForce)
			 ChangeNightForce();
		  else
			 ChangeNight();
		}
	 }

	 #region SetNight

	 private Material _DaySkyBox;
	 private Material _NightSkyBox;

	 // Дневные настройки света
	 private Color _dayLightColor = new Color(224, 200, 166, 254);
	 private float _dayIntensity = 2.24f;

	 // Ночные настройки света
	 private Color _nightLightColor = new Color(0.239f, 0.239f, 0.239f, 1);
	 private float _nightIntensity = 0.47f;

	 private void ChangeNightForce()
	 {

		SetGhost();
		_NightSkyBox.SetFloat("_Exposure", 1);
		RenderSettings.sun.intensity = _nightIntensity;
		RenderSettings.sun.color = _nightLightColor;
		RenderSettings.skybox = _NightSkyBox;

	 }

	 private void ChangeNight()
	 {
		SetGhost();

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

			 Managers.GameManager.Instance.DialogsManager.ShowDialog("9630f28b-65f0-4440-a3b3-d1de4c4fa9ce", null,
		  null,
		  null);

		  }

		  );

	 }

	 #endregion


#if UNITY_EDITOR

	 [ContextMenu("Find all blue Lights")]
	 private void FingBlueLights()
	 {
		_ghostBlueArr = transform.GetComponentsInChildren<GhostBlue>();
	 }

#endif

  }
}