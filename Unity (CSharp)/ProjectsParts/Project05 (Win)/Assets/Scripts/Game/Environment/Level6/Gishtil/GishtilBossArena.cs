

using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level6.Gishtil
{
  /// <summary>
  /// Орена босса
  /// </summary>
  public class GishtilBossArena : MonoBehaviourBase
  {
	 [SerializeField]
	 private GishtilBattle _environment;

	 public int BattlePhase
	 {
		get => _environment.State;
	 }

	 [SerializeField]
	 private LightsData[] _lights;
	 public LightsData[] Lights { get => _lights; set => _lights = value; }
	 private bool _heartActive = false;

	 private void OnEnable()
	 {
		_environment.OnStateChangeEvent.AddListener(OnStateChange);
	 }

	 private void OnDisable()
	 {
		_environment.OnStateChangeEvent.RemoveListener(OnStateChange);
	 }

	 private void OnStateChange(int newState)
	 {
		StopAllCoroutines();

		if (newState == 1)
		  StartCoroutine(HeartCor());
	 }

	 public void EnemyActivateLight()
	 {
		bool isAll = true;
		for (int i = 0; i < _lights.Length; i++)
		{
		  if (!_lights[i]._lightsPortal.IsFireContact)
			 isAll = false;
		}
		if (isAll)
		  _environment.BattleComplete();
	 }

	 public bool InteractionReady
	 {
		get => BattlePhase == 1 && _heartActive;
	 }

	 public void Clear()
	 {
		for (int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].IsChange = false;
		  _lights[i]._lightBase.IsEnabled = false;
		  _lights[i]._lightsPortal.IsEnabled = false;
		  _lights[i]._lightsPortal.FireCintact(false);
		}
	 }

	 /// <summary>
	 /// Взиамодействие с центральной частью арены
	 /// </summary>
	 public void Interaction()
	 {
		if (BattlePhase != 1)  return;

		for (int i = 0; i < _lights.Length; i++)
		{
		  if (!_lights[i]._lightBase.IsEnabled)
		  {
			 _lights[i]._lightBase.IsEnabled = true;
			 _lights[i]._lightsPortal.IsEnabled = true;
			 break;
		  }
		}

		bool isFull = true;
		for (int i = 0; i < _lights.Length; i++)
		{
		  if (!_lights[i]._lightBase.IsEnabled)
		  {
			 isFull = false;
		  }
		}

		if (isFull)
		  _environment.AllLightingActive();

	 }


	 [System.Serializable]
	 public struct LightsData
	 {
		[HideInInspector]
		public bool IsChange;
		public GishtilLighting _lightBase;
		public GishtilLighting _lightsPortal;
	 }

	 #region Мерцание сердца
	 [Header("Мерцание сердца")]

	 [SerializeField]
	 private Material _heartMaterial;
	 [ColorUsage(false, true)]
	 [SerializeField]
	 private Color _heartLightColor;
	 [SerializeField]
	 private Light _heartLight;

	 public bool HeartActive { get => _heartActive; set => _heartActive = value; }

	 private IEnumerator HeartCor()
	 {
		while (true)
		{
		  if (BattlePhase == 1)
		  {
			 _heartActive = true;
			 for (int i = 0; i < 4; i++)
			 {
				_heartLight.DOIntensity(10, 0.5f).OnComplete(() =>
				{
				  _heartLight.DOIntensity(0, 0.5f);
				});
				DOTween.To(() => _heartMaterial.GetColor("_EmissionColor"), (x) => _heartMaterial.SetColor("_EmissionColor", x), _heartLightColor, 0.5f).OnComplete(() =>
				{
				  DOTween.To(() => _heartMaterial.GetColor("_EmissionColor"), (x) => _heartMaterial.SetColor("_EmissionColor", x), Color.black, 0.5f);
				});
				yield return new WaitForSeconds(1);
			 }
			 _heartActive = false;
		  }
		  yield return new WaitForSeconds(10);
		}
	 }

	 #endregion
  }
}