
//#define BOSS_DISABLE // тключаем босса

using UnityEngine;
using System.Collections;
using DG.Tweening;
using com.ootii.Actors;
using Aura2API;
using it.Game.Items;
using UnityEngine.VFX;
using Slate;

namespace it.Game.Environment.Level4
{
  public class KarmamancerBossArena : Environment
  {
	 /*
	  * Состояния:
	  * 0 - не активное
	  * 1 - появление босса
	  * 2 - босс побежден
	  * 
	  */

	 [SerializeField]
	 private NPC.Enemyes.Boses.Karmamancer.TheKarmamancer _boss;
	 [SerializeField]
	 private Obelisc[] _obelisks;

	 [SerializeField]
	 private AuraVolume _fog;
	 [SerializeField]
	 private ParticleSystem _particlesFog;
	 private ParticleSystem.EmissionModule _particlesFogEmission;

	 [SerializeField]
	 private Katana _katana;
	 [SerializeField]
	 private InfectedCrystal _infectedCrystal;

	 [SerializeField]
	 private GameObject _forceField;
	 [SerializeField]
	 private GameObject _gateField;

	 private Vector3 _katanaPos;
	 private Quaternion _katanaRot;
	 private Vector3 _crystalPos;
	 private Quaternion _crystalRot;

	 [SerializeField]
	 private VisualEffect _baseFog;
	 [SerializeField]
	 private AtmosphericHeightFog.HeightFogOverride _fogHeight;

	 [SerializeField]
	 public Cutscene _catanaScene;
	 [SerializeField]
	 public Cutscene _closegateCutScene;

	 /// <summary>
	 /// Стратовая катсцена
	 /// </summary>
	 [SerializeField]
	 private PlayMakerFSM _firstCutScene;

	 protected override void Start()
	 {
		base.Start();
		_particlesFogEmission = _particlesFog.emission;
		_katanaPos = _katana.transform.position;
		_katanaRot = _katana.transform.rotation;
		_crystalPos = _infectedCrystal.transform.position;
		_crystalRot = _infectedCrystal.transform.rotation;
		_particlesFogEmission.enabled = false;
	 }

	 /// <summary>
	 /// Запуск катсцены закрытия двери
	 /// </summary>
	 public void CloseGate()
	 {
		if (_gateField.activeInHierarchy)  return;

#if UNITY_EDITOR
		_gateField.SetActive(true);
		return;
#endif


		Managers.GameManager.Instance.GameInputSource.enabled = false;
		_closegateCutScene.Play(() =>
		{
		  Managers.GameManager.Instance.GameInputSource.enabled = true;
		  _gateField.SetActive(true);
		});
	 }

	 private void Clear()
	 {
		_forceField.gameObject.SetActive(true);
		_gateField.gameObject.SetActive(false);
		_baseFog.gameObject.SetActive(false);
		_fogHeight.gameObject.SetActive(false);
		GetComponentInChildren<TheKarmamancerGate>().Clear();
		_fogHeight.fogIntensity = 0;
	 }

	 /// <summary>
	 /// Запускаем PlayMaker первоначальную анимацию
	 /// </summary>
	 public void PlayStartVideo()
	 {
		_firstCutScene.SendEvent("StartFSM");
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		_particlesFogEmission = _particlesFog.emission;

		if (isForce)
		{
		  if (State == 0)
		  {
			 Clear();
			 _fog.densityInjection.strength = 0;
			 _particlesFogEmission.enabled = false;
			 _particlesFog.Stop();
			 _boss.gameObject.SetActive(false);
			 for (int i = 0; i < _obelisks.Length; i++)
			 {
				_obelisks[i].ResetStateBossArena();
			 }
			 if (_katanaPos != Vector3.zero)
			 {
				_katana.transform.position = _katanaPos;
				_katana.transform.rotation = _katanaRot;
				_infectedCrystal.transform.position = _crystalPos;
				_infectedCrystal.transform.rotation = _crystalRot;

			 }
		  }
		  if (State == 2)
		  {
			 _boss.gameObject.SetActive(false);
		  }
		  _forceField.SetActive(State == 0);
		  //_gateField.SetActive(State != 0);
		}
	 }

	 [ContextMenu("Show gate")]
	 public void ShowGate()
	 {
		GetComponentInChildren<TheKarmamancerGate>().Show();
	 }

	 [ContextMenu("Destroy Boss")]
	 public void DestroyBoss()
	 {
		_boss.FsmBehaviour.SendEvent("OnDead");
	 }

	 /// <summary>
	 /// Подтверждение удаления босса
	 /// </summary>
	 public void CompleteDestroyBoss()
	 {
		_boss.gameObject.SetActive(false);
		DOVirtual.DelayedCall(1, ShowGate);
	 }


	 /// <summary>
	 /// Вход игрока на арену
	 /// </summary>
	 public void PlayerEnterArena()
	 {
		if (!_isActived) return;

		if (State > 0)
		  return;

		State = 1;

		var player = it.Game.Player.PlayerBehaviour.Instance;

		PlayStartVideo();

	 }

	 /// <summary>
	 /// Активация тумана
	 /// 
	 /// Вызывается из PlayMakera
	 /// </summary>
	 public void ActiveFog()
	 {
		//_gateField.gameObject.SetActive(true);
		_baseFog.gameObject.SetActive(true);
		_fogHeight.gameObject.SetActive(true);
		DOTween.To(() => _baseFog.GetFloat("EmitionRate"), (x) => _baseFog.SetFloat("EmitionRate", x), 1000, 5);
		DOTween.To(() => _fogHeight.fogIntensity, (x) => _fogHeight.fogIntensity = x, 1, 5);

		//StartCoroutine(FogActive());
	 }

	 /// <summary>
	 /// Стартовый телепорт босса за спину игрока
	 /// 
	 /// Вызывается из PlayMakera
	 /// </summary>
	 public void BossStartShow()
	 {
#if !BOSS_DISABLE
		_boss.gameObject.SetActive(true);
		_boss.FsmBehaviour.SendEvent("FistShow");
#endif
	 }

	 /// <summary>
	 /// Закончен показ босса
	 /// </summary>
	 public void BossShowComplete()
	 {
		_boss.FsmBehaviour.SendEvent("Battle");
	 }

	 [ContextMenu("Play catana cutscene")]
	 private void PlayCatanaCutscene()
	 {
		_catanaScene.Play(()=> {
		  _forceField.gameObject.SetActive(false);

		});
	 }

	 IEnumerator FogActive()
	 {
		_particlesFogEmission.enabled = true;
		while (_fog.densityInjection.strength < 2)
		{
		  yield return null;
		  _fog.densityInjection.strength += 0.3f * Time.deltaTime;
		}
	 }

	 [ContextMenu("Activate obelisk")]
	 public void ActivateObelisk()
	 {
		int count = CheckCountComplete();

		if (count == 2)
		{
		  _boss.FsmBehaviour.SendEvent("OnBigCast");
		}
		if (count == _obelisks.Length)
		{
		  PlayCatanaCutscene();
		  //_boss.IsDead = true;
		}
	 }

	 private int CheckCountComplete()
	 {
		int count = 0;
		for (int i = 0; i < _obelisks.Length; i++)
		{
		  if (_obelisks[i].IsComplete)
			 count++;
		}
		return count;
	 }

#if UNITY_EDITOR

	 [ContextMenu("Play first cutscene")]
	 private void PlayVideoCatScene()
	 {
		PlayStartVideo();
	 }

	 private void EnemyTeleportToBackPlayer()
	 {

	 }

#endif

  }
}