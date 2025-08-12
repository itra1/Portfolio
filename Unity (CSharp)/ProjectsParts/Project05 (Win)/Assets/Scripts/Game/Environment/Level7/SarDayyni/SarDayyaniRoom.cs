using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  /// <summary>
  /// Комната босса, окружение
  /// </summary>
  public class SarDayyaniRoom : MonoBehaviour
  {
	 [SerializeField]	private Light _roomLight; // освещение всей комнаты
	 [SerializeField]	private Light _crystalLight; // освещение кристала
	 [SerializeField] private Material _crystalMaterial; // Материал кристала
	 
	 [ColorUsage(false,true)]
	 private Color _crystalEmissionActive;

	 private void Awake()
	 {
		_crystalEmissionActive = _crystalMaterial.GetColor("_EmissionColor");
	 }

	 /// <summary>
	 /// Выключаем освещение в комнате
	 /// </summary>
	 public void Shadow()
	 {
		_roomLight.intensity = 0;
		_crystalLight.intensity = 0;
		_crystalMaterial.SetColor("_EmissionColor", Color.black);
	 }

	 /// <summary>
	 /// Включаем освещение
	 /// </summary>
	 public void Light()
	 {
		_crystalLight.DOIntensity(1, 1);
		_crystalMaterial.DOColor(_crystalEmissionActive, "_EmissionColor", 1);
		_roomLight.DOIntensity(1, 1).SetDelay(1);
	 }
  }
}