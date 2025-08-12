using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Items
{
  /// <summary>
  /// Итем
  /// </summary>
  public abstract class Item : UUIDBase, ILightItem
  {

	 /// <summary>
	 /// Иконка в инвентаре
	 /// </summary>
	 public Sprite Icon => _icon;

	 public string Title => _title;
	 [Tooltip("Иконка")]
	 [SerializeField]
	 private Sprite _icon;

	 [Tooltip("Заголовок")]
	 [SerializeField]
	 private string _title;

	 [Tooltip("Описание")]
	 [SerializeField]
	 private string _deascription;

	 [ColorUsage(true)]
	 [SerializeField]
	 protected Color _colorHide = Color.cyan;

	 [SerializeField]
	 protected GameObject _backLightPrefab;

	 [SerializeField]
	 private bool _lightingReady;
	 private bool _isLighting;
	 private GameObject _backLight;

	 public void SetLight(bool isLight)
	 {
		if (!_lightingReady)
		  return;
		if (_isLighting == isLight)
		  return;

		_isLighting = isLight;

		// Отключаем подсветку
		if (!isLight)
		{
		  if (_backLight != null) {
			 HideLightObject(false);
		  }
		  return;
		}

		// Включаем подсветку
		if (_backLight == null)
		{
		  if (_backLightPrefab != null)
			 _backLight = Instantiate(_backLightPrefab, transform.position, Quaternion.identity);
		}

		if (_backLight != null)
		  HideLightObject(true);

	 }

	 public void SetIcon(Sprite sprite)
	 {
		_icon = sprite;
	 }

	 /// <summary>
	 /// Установка заголовка для предмета
	 /// </summary>
	 /// <param name="title"></param>
	 public void SetTitle(string title)
	 {
		_title = title;
	 }

	 public void ColorHide()
	 {
		ColorHide(null);

		if (_backLight != null)
		{
		  _backLight.transform.parent = null;
		  Destroy(_backLight, 4);
		}

	 }

	 public void ColorHide(UnityEngine.Events.UnityAction onComplete)
	 {
		//var comp = gameObject.AddComponent<Game.Handles.ColorHide>();

		//comp.StartAnim(_colorHide, 1 / 0.5f, true, () =>
		//{
		//  onComplete?.Invoke();
		//  gameObject.SetActive(false);
		//});

		var material =  gameObject.GetComponentInChildren<Renderer>().material;

		material.DOFloat(1, "_DissolveAmount", 1).OnComplete(() => {
		  onComplete?.Invoke();
		  gameObject.SetActive(false);
		});

		//DOTween.To(() => material.GetFloat("_DissolveAmount"), (x) => material.SetFloat("_DissolveAmount", x), 1f, 1f).OnComplete(()=> {
		//  onComplete?.Invoke();
		//  gameObject.SetActive(false);
		//});

		if(_backLight != null)
		{
		  HideLightObject(false);
		}

	 }

	 private void HideLightObject(bool isActive)
	 {
		_backLight.transform.position = transform.position;
		ParticleSystem[] particles = _backLight.GetComponentsInChildren<ParticleSystem>();

		for (int i = 0; i < particles.Length ; i++){
		  var emissionModule =  particles[i].emission;
		  emissionModule.enabled = isActive;
		}
	 }

	 public void ColorShow(UnityEngine.Events.UnityAction onComplete = null)
	 {
		var comp = gameObject.AddComponent<Game.Handles.ColorHide>();
		comp.StartAnim(_colorHide, 1 / 0.5f, false, () =>
		{
		  onComplete?.Invoke();
		});
	 }

  }
}