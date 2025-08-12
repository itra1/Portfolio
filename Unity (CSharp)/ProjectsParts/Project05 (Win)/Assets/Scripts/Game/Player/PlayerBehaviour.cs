using UnityEngine;
using RootMotion.FinalIK;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;
using it.Game.Player.MotionControllers.Motions;
using it.Game.Managers;
using it.Game.Player.Interactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player
{

  public class PlayerBehaviour : MonoBehaviourBase, IPlayer
  {
	 public const string ANIM_EVENT_STOP_CATSCENE = "CatAnimComplete";
	 public const string EVT_PLAYER_ANIM_NAME = "PLAYER_ANIM_NAME";
	 public const string ANIM_CRV_ITERACTION = "crv_interaction";
	 public const string ANIM_CRV_RIGHT_HAND_ITERACTION = "crv_right_hand_interation";
	 public const string ANIM_CRV_LEFT_HAND_ITERACTION = "crv_left_hand_interation";
	 public const string ANIM_CRV_RIGHT_FOOT = "crv_right_foot";
	 public const string ANIM_CRV_LEFT_FOOT = "crv_left_foot";

	 private readonly string hipBoneName = "root/Root_M";
	 private readonly string headBoneName = "root/Root_M/Spine1_M/Spine2_M/Chest_M/Neck_M/Head_M";

	 [SerializeField] private GameObject _startVfx;
	 [SerializeField] private LayerMask _collisionMask = -1;

	 private Transform _platform;
	 public Transform Platform { get => _platform; set => _platform = value; }

	 public int Form
	 {
		get => MotionController.CurrentForm;
	 }

	 public static PlayerBehaviour Instance
	 {
		get
		{
		  if (_Instance == null)
			 _Instance = (PlayerBehaviour)FindObjectOfType(typeof(PlayerBehaviour));
		  return _Instance;
		}
	 }
	 public FullBodyBipedIK FullBodyBipedIK
	 {
		get
		{
		  if (_fullBodyBipedIK == null)
			 _fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();

		  return _fullBodyBipedIK;
		}
	 }
	 private Vector3 _wind = Vector3.zero;
	 /// <summary>
	 /// Ветер
	 /// </summary>
	 public Vector3 Wind
	 {
		get => _wind;
		set
		{
		  _wind = value;
		}
	 }
	 private ActorController _ActorController;
	 public ActorController ActorController
	 {
		get
		{
		  if (_ActorController == null)
			 _ActorController = GetComponent<ActorController>();
		  return _ActorController;
		}
		set
		{
		  _ActorController = value;
		}
	 }

	 private MotionController _MotionController;
	 public MotionController MotionController
	 {
		get
		{
		  if (_MotionController == null)
			 _MotionController = GetComponent<MotionController>();
		  return _MotionController;
		}
		set
		{
		  _MotionController = value;
		}
	 }

	 private Interactions.InteractionSystem _interactionSystem;
	 public Interactions.InteractionSystem InteractionSystem
	 {
		get
		{
		  if (_interactionSystem == null)
			 _interactionSystem = GetComponent<Interactions.InteractionSystem>();
		  if (_interactionSystem == null)
			 _interactionSystem = GetComponentInChildren<Interactions.InteractionSystem>();
		  return _interactionSystem;
		}
		set => _interactionSystem = value;
	 }

	 private com.ootii.Actors.BoneControllers.BoneController _boneController;
	 public com.ootii.Actors.BoneControllers.BoneController BoneController
	 {
		get
		{
		  if (_boneController == null)
			 _boneController = GetComponentInChildren<com.ootii.Actors.BoneControllers.BoneController>();
		  return _boneController;
		}
		set => _boneController = value;
	 }

	 private Transform _hipBone;
	 public Transform HipBone {
		get
		{
		  if(_hipBone == null)
			 _hipBone = transform.Find(hipBoneName);
		  return _hipBone;
		}
		set
		{
		  _hipBone = value;
		}
	 }
	 private Transform _headBone;
	 public Transform HeadBone
	 {
		get
		{
		  if (_headBone == null)
			 _headBone = transform.Find(headBoneName);
		  return _headBone;
		}
		set
		{
		  _headBone = value;
		}
	 }
	 [SerializeField] private Light[] _tailsLights;
	 public Light[] TailsLights { get => _tailsLights; set => _tailsLights = value; }

	 protected static PlayerBehaviour _Instance;
	 private FullBodyBipedIK _fullBodyBipedIK;


	 
	 
	 [SerializeField] [ColorUsage(false, true)] private Color _tailsDefaultEmissionColor;
	 [SerializeField] [ColorUsage(false, true)] private Color _tailsRedEmissionColor;

	 [SerializeField] [HideInInspector] private PlayerDress _dress;
	 private PlayerDress Dress
	 {
		get
		{
		  if (_dress == null)
			 _dress = GetComponent<PlayerDress>();
		  return _dress;
		}
		set
		{
		  _dress = value;
		}
	 }

	 public void RegisterVars()
	 {
		var variablePlayMaker = HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Player");
		variablePlayMaker.Value = gameObject;
		ChangeSkin();
	 }

	 public void OnTriggerEnter(Collider other)
	 {
		var collisions = other.GetComponents<IPlayerTriggerEnter>();
		for (int i = 0; i < collisions.Length; i++)
		{
		  var cond = (collisions[i] as Component).GetComponent<IInteractionCondition>();
		  if(cond != null)
		  {
			 if(cond.InteractionReady())
				collisions[i].OnPlayerTriggerEnter();
		  }
		  else
			 collisions[i].OnPlayerTriggerEnter();
		}
	 }

	 public void OnTriggerExit(Collider other)
	 {
		var collisions = other.GetComponents<IPlayerTriggerEnter>();
		for (int i = 0; i < collisions.Length; i++)
		{
		  collisions[i].OnPlayerTriggerExit();
		}
	 }

	 public void HidePlayerCamera(bool isHide)
	 {
		HoldItem interaction = MotionController.GetMotion<HoldItem>();
		if (interaction.HoldenItem != null)
		  interaction.HoldenItem.gameObject.SetActive(!isHide);
	 }

	 public void SetNote(string uuidNote)
	 {
		var notePrefab = it.Game.Managers.GameManager.Instance.Inventary.GetPrefab(uuidNote);
		GameObject inst = it.Game.Managers.GameManager.Instantiate(notePrefab);
		var target = inst.GetComponentInChildren<it.Game.Player.Interactions.InteractionTarget>();
		var effector = FullBodyBipedIK.solver.GetEffector(target.effectorType);

		Vector3 parentPosition = effector.bone.position;
		Quaternion rotation = effector.bone.rotation;

		effector.bone.transform.position = target.transform.position;
		effector.bone.transform.rotation = target.transform.rotation;
		inst.transform.SetParent(effector.bone.transform);
		effector.bone.transform.position = parentPosition;
		effector.bone.transform.rotation = rotation;
		var poser = effector.bone.GetComponent<RootMotion.FinalIK.HandPoser>();
		poser.poseRoot = target.transform;
		poser.weight = 100;
		var motion = MotionController.GetMotion<Interaction>();
		motion.Item = inst.GetComponent<it.Game.Items.IInteraction>();
		motion.IsGetItem = true;
		//inst.GetComponent<InventaryItem>().CheckExistsAndDeactive = false;
		inst.SetActive(true);
	 }

	 private void Update()
	 {

		if (ActorController.BodyShapes != null && ActorController.BodyShapes.Count > 0)
		{
		  var shape = ActorController.BodyShapes[0] as com.ootii.Actors.BodyCapsule;

		  RaycastHit[] hits = new RaycastHit[15];
		  UnityEngine.Physics.CapsuleCastNonAlloc(transform.position, transform.position + shape.EndOffset, shape.Radius, ActorController._Transform.forward, hits, 0, _collisionMask);

		  for (int i = 0; i < hits.Length; i++)
		  {
			 if (hits[i].collider != null)
				CheckCollision(hits[i].collider);
		  }
		}

		EnergyChange();

	 }
	 public void ChangeSkin()
	 {
		Dress.Change();
	 }

	 public bool IsDead { get => Game.Managers.GameManager.Instance.UserManager.Health.IsDead; }

	 public it.Game.Items.HoldenItem GetHoldItem()
	 {
		MotionController lMotionController = GetComponent<MotionController>();
		if (lMotionController == null) { return null; }

		MotionControllers.Motions.HoldItem lMotion = lMotionController.GetMotion<MotionControllers.Motions.HoldItem>();
		if (lMotion == null) { return null; }

		return lMotion.HoldenItem;

	 }

	 private bool _isPlayerControl = true;

	 /// <summary>
	 /// Управление у игрока
	 /// </summary>
	 public bool PlayerControl
	 {
		get
		{
		  return _isPlayerControl;
		}
		set
		{
		  if (_isPlayerControl == value)
			 return;
		  _isPlayerControl = value;
		  ConfirmChangePlayerControl();
		}
	 }


	 private void OnDestroy()
	 {
		try
		{
		  if (GameManager._instance != null)
			 AudioListenerBehaviour.Instance.transform.SetParent(GameManager.Instance.transform);
		  AudioListenerBehaviour.Instance.transform.localPosition = Vector3.zero;
		  AudioListenerBehaviour.Instance.transform.localRotation = Quaternion.identity;
		}
		catch { }
	 }

	 private void Start()
	 {
		Debug.LogWarning("Start");
		var actor = GetComponent<MotionController>();
		actor.InputSource = Game.Managers.GameManager.Instance.GameInputSource;

		AudioListenerBehaviour.Instance.transform.SetParent(transform);
		AudioListenerBehaviour.Instance.transform.localPosition = Vector3.up * actor.ActorController.Height;
		AudioListenerBehaviour.Instance.transform.localRotation = Quaternion.identity;
	 }
	 private void CheckCollision(Collider collider)
	 {
		CheckDamage(collider);
	 }

	 private void CheckDamage(Collider collider)
	 {
		var damage
		  = collider.GetComponent<Handlers.IPlayerDamage>();

		if (damage == null)
		  return;

		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, damage);
	 }

	 public static void Damage(float value)
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(Instance, value);
	 }

	 private void ConfirmChangePlayerControl()
	 {
		var components = GetComponentsInChildren<IPlayerControl>();

		foreach (var elem in components)
		{
		  (elem as MonoBehaviourBase).enabled = _isPlayerControl;
		}
	 }

	 public void PortalJump(Vector3 jumpPoint, bool effect = true)
	 {
		ActorController.SetPosition(jumpPoint);
		ActorController.SetRelativeVelocity(Vector3.zero);
		ActorController.SetVelocity(Vector3.zero);

		if (effect)
		{
		  GameObject vfx = Instantiate(_startVfx, transform.position, Quaternion.identity);
		  vfx.GetComponent<UnityEngine.VFX.VisualEffect>().SendEvent("OnPlay");
		  Destroy(vfx, 5);
		}
	 }
	 public void PortalJump(Transform jumpPoint, bool effect = true)
	 {
		PortalJump(jumpPoint.position, effect);
	 }

	 #region Изменение энергии

	 private bool _decrementEnergy = false;
	 private float _speedEnergyChange = 0;

	 public void SetEnergyChange(bool isEnable, float speed = 0)
	 {
		_decrementEnergy = isEnable;
		_speedEnergyChange = speed;
	 }

	 private void EnergyChange()
	 {
		if (!_decrementEnergy)
		  return;

		GameManager.Instance.EnergyManager.Subtract(_speedEnergyChange * Time.deltaTime);

		if(GameManager.Instance.EnergyManager.Percent <= 0)
		{
		  DressChange dc = MotionController.GetMotion<DressChange>();
		  dc.SetDress(0);
		  MotionController.ActivateMotion(dc);
		}
	 }

	 #endregion

	 /// <summary>
	 /// Проверка На коллизию с игроком
	 /// </summary>
	 /// <param name="col"></param>
	 /// <returns></returns>
	 public static bool IsPlayerCollider(Collider col)
	 {
		PlayerBehaviour player = col.GetComponent<PlayerBehaviour>();
		
		if(player == null)
		  player = col.GetComponentInParent<PlayerBehaviour>();

		if (player == null)
		  return false;
		return true;
	 }
  }

}