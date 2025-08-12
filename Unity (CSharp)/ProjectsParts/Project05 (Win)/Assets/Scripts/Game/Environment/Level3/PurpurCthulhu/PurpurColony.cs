using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player;
using it.Game.Managers;

namespace it.Game.Environment.GreenCthulhu
{
  public class PurpurColony : Environment
  {
	 public const string DUNGEON_COMPLETE_ITEM = "d4321328-af2d-48f2-8436-a56b143a00fa";

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onComplete;

	 [SerializeField]
	 private it.Game.NPC.Enemyes.Cthulhu.Cthulhu _shaman;

	 [SerializeField]
	 private it.Game.Environment.Handlers.PegasusController _pegasusColony;

	 /// <summary>
	 /// Позиции мпавна cthulhu
	 /// </summary>
	 [SerializeField]
	 private Transform _startSpawnCthulhu;
	 [SerializeField]
	 private Transform _playerChtulhuQuestComplete;

	 /// <summary>
	 /// Камень перекрывающий врата
	 /// </summary>
	 [SerializeField]
	 public Transform _gateStone;
	 /// <summary>
	 /// Позиция игрока после прохождения подземелья
	 /// </summary>
	 [SerializeField]
	 private Transform _playerPositionAfterGate;
	 [SerializeField]
	 private it.Game.Environment.Handlers.PegasusController _pegasusOpenGate;
	 [SerializeField]
	 private Light _portalLight;
	 [SerializeField]
	 private ParticleSystem _portalParticles;

	 [SerializeField]
	 private Game.NPC.Enemyes.Cthulhu.Cthulhu[] _cthulhu;

	 protected override void Start()
	 {
		base.Start();

		ActiveAllPlayMakerFsm(false);

		Game.Events.EventDispatcher.AddListener(it.Game.Managers.UserManager.ENT_PLAYER_INSTANT, PLayerInstance);
	 }

	 protected override void OnDestroy()
	 {
		base.OnDestroy();
		Game.Events.EventDispatcher.AddListener(it.Game.Managers.UserManager.ENT_PLAYER_INSTANT, PLayerInstance);
	 }

	 private void PLayerInstance(com.ootii.Messages.IMessage message)
	 {
		CheckExistsItem();
	 }

	 [ContextMenu("Chaman Call")]
	 public void ShamanLookQuest()
	 {
		State = 2;
		Save();
		_pegasusColony.Activate(() =>
		{
		  GameManager.Instance.GameInputSource.enabled = false;
		  DOVirtual.DelayedCall(1f, () =>
		  {
			 GameManager.Instance.GameInputSource.enabled = true;
			 _pegasusColony.Deactivate();
			 //QuestComplete();
		  });

		});
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State <= 1)
		{
		  _shaman.gameObject.SetActive(true);
		  ActivateAllCthulhu(false);
		  _shaman.GetFsm("Behaviour").SendEvent("StartFSM");

		}
		else if(State == 2)
		{
		  _shaman.gameObject.SetActive(true);
		  ActivateAllCthulhu(false);
		  _shaman.GetFsm("Behaviour").SendEvent("FirstComplete");

		}
		else	if (State == 3)
		{
		  ActivateAllCthulhu(true);
		  _shaman.gameObject.SetActive(true);
		  _shaman.GetFsm("Behaviour").SendEvent("OnMoveToPortal");

		  _portalLight.intensity = 0;
		  _portalParticles.Stop();

		  //StartCoroutine(ShowCamera());
		  _gateStone.localPosition = Vector3.zero;
		}
		else if(State > 3)
		{
		  ActivateAllCthulhu(true);

		  for (int i = 0; i < _cthulhu.Length; i++)
		  {
			 _cthulhu[i].ActiveAllPlayMakerFsm(false);
			 _cthulhu[i].ActivePlayMakerFsm("Behaviour");
		  }
		  _gateStone.localPosition = new Vector3(0, -14f, 0);
		}

		_portalParticles.Stop();
	 }

	 [ContextMenu("CheckExistsItem")]
	 private void CheckExistsItem()
	 {
		CheckDungeonComplete();
	 }

	 private void ActivateAllCthulhu(bool isActive)
	 {

		for (int i = 0; i < _cthulhu.Length; i++)
		{
		  _cthulhu[i].gameObject.SetActive(isActive);

		}
	 }

	 IEnumerator ShowCamera()
	 {
		yield return new WaitForEndOfFrame();
		CheckDungeonComplete();
	 }

	 private void CheckDungeonComplete()
	 {
		if (!Managers.GameManager.Instance.Inventary.ExistsItem(PurpurColony.DUNGEON_COMPLETE_ITEM))
		  return;

		Managers.GameManager.Instance.Inventary.Remove(PurpurColony.DUNGEON_COMPLETE_ITEM);
		DOVirtual.DelayedCall(1f, () => { QuestComplete(); });
		//QuestComplete();

	 }

	 [ContextMenu("QuestComplete")]
	 public void QuestComplete()
	 {
		State = 3;
		Save();

		_pegasusOpenGate.FromCameraPosition = false;
		_pegasusOpenGate.Activate(() =>
		{
		  //_gateStone.gameObject.SetActive(false);
		  _gateStone.DOLocalMove(new Vector3(0, -14f, 0), 1).OnComplete(() =>
		  {
			 _portalLight.DOIntensity(6f, 1);
			 _portalParticles.Play();
			 DOVirtual.DelayedCall(1.5f, () =>
			 {
				StartCoroutine(VisibleCthulhu());
			 });
		  });
		  //_pegasusOpenGate.Deactivate();

		});

		//_shaman.DeactiveAllPlayMakerFsm();
		//_shaman.ActivePlayMakerFsm("Move to portal");
	 }

	 private IEnumerator VisibleCthulhu()
	 {
		for(int i = 0; i < _cthulhu.Length; i++)
		{
		  _cthulhu[i].transform.position = _startSpawnCthulhu.position;
		  _cthulhu[i].ActiveAllPlayMakerFsm(false);
		  _cthulhu[i].ActivePlayMakerFsm("Move out");
		  _cthulhu[i].gameObject.SetActive(true);
		  yield return new WaitForSeconds(Random.Range(0.7f,1.2f));
		}

		DOVirtual.DelayedCall(1, PlayerMoveComplete);

		PlayerBehaviour.Instance.PortalJump(_playerChtulhuQuestComplete);

		//var playerFsm = GetFsm("PlayerMoveOutGate");
		//playerFsm.FsmVariables.GetFsmGameObject("Player").Value = PlayerBehaviour.Instance.gameObject;
		//playerFsm.FsmVariables.GetFsmGameObject("Region").Value = gameObject;
		//playerFsm.enabled = true;
	 }
	 [ContextMenu("PlayerMoveComplete")]
	 public void PlayerMoveComplete()
	 {
		_pegasusOpenGate.Deactivate();
		_shaman.GetFsm("Behaviour").SendEvent("OnMoveToPortal");
		//_shaman.ActiveAllPlayMakerFsm(false);
		//_shaman.ActivePlayMakerFsm("Move to portal");

		for (int i = 0; i < _cthulhu.Length; i++)
		{
		  _cthulhu[i].ActiveAllPlayMakerFsm(false);
		  _cthulhu[i].ActivePlayMakerFsm("Behaviour");
		}

		
	 }

	 public void PortalComplete()
	 {
		_onComplete?.Invoke();
		State = 4;
		Save();
		Managers.GameManager.Instance.UserManager.PlayerProgress.Save();
	 }

  }
}