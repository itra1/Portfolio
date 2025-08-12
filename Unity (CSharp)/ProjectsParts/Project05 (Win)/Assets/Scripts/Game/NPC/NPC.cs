using UnityEngine;
using System.Collections;
using it.Game.Environment.All.Animals;

namespace it.Game.NPC
{
  public class NPC : UUIDBase
  {
	 public string Name { get => _name; protected set => _name = value; }

	 [SerializeField]
	 private string _name;
	 /// <summary>
	 /// Название
	 /// </summary>

	 private Animator _animator;

	 protected bool _isDead = false;
	 public bool IsDead { get => _isDead; set => _isDead = value; }

	 public Animator Animator
	 {
		get
		{
		  if (_animator == null)
			 _animator = GetComponent<Animator>();
		  if (_animator == null)
			 _animator = GetComponentInChildren<Animator>();
		  if (_animator == null)
			 _animator = GetComponentInParent<Animator>();
		  return _animator;
		}
		set => _animator = value;
	 }

#if UNITY_EDITOR
	 public string Title { get => _title; protected set => _title = value; }

	 [SerializeField]
	 private string _title;
#endif

	 private Vector3 _startPosition;
	 public Vector3 StartPosition { get => _startPosition; set => _startPosition = value; }

	 private NpcPhase _phase;
	 public NpcPhase Phase {
		get
		{
		 return  _phase;
		}
		set => 
		  _phase = value; }

	 /// <summary>
	 /// Состояние
	 /// </summary>
	 protected int _state = 0;
	 public virtual int State
	 {
		get {
		  return _state;
		}
		set
		{
		  _state = value;
		  PhaseChange();
		}
	 }

	 protected int _animStateID;
	 protected int _animTransitionID;

	 protected virtual void Awake() { }

	 protected virtual void Start()
	 {
		StartCoroutine(FindPlayer());
	 }

	 protected virtual void OnEnable()
	 {
		StartCoroutine(FindPlayer());
		_isDead = false;
		StartPosition = transform.position;
	 }

	 protected virtual void Update()
	 {
		GetAnimData();
	 }

	 IEnumerator FindPlayer()
	 {
		bool isSet = false;

		while (!isSet)
		{
		  if (it.Game.Player.PlayerBehaviour.Instance != null)
		  {
			 PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
			 for (int i = 0; i < fsms.Length; i++)
			 {
				var player = fsms[i].FsmVariables.GetFsmGameObject("Player");
				if (player != null)
				{
				  player.Value = it.Game.Player.PlayerBehaviour.Instance.gameObject;
				}
			 }

			 //var trees = gameObject.GetComponents<BehaviorDesigner.Runtime.BehaviorTree>();

			 //for (int i = 0; i < trees.Length; i++)
			 //{

				//var playerVar = trees[i].GetVariable("Player");
				//if (playerVar != null)
				//{
				//  playerVar.SetValue(it.Game.Player.PlayerBehaviour.Instance.gameObject);
				//}
			 //}


			 isSet = true;
		  }
		  yield return null;
		}

	 }

	 public virtual void SetState(int state)
	 {
		this.State = state;
	 }

#if UNITY_EDITOR
	 [ContextMenu("Replace spawner")]
	 private void ReplaceToSpawner()
	 {
		GameObject obj = new GameObject();
		obj.transform.position = transform.position;
		if (transform.parent != null)
		  obj.transform.parent = transform.parent;
		else
		  obj.transform.parent = AnimalSpawnManager.Instance.transform;

		AnimalSpaner spawner = obj.AddComponent<AnimalSpaner>();
		spawner.GenerateUuid();
		spawner.AnimaUuid = Uuid;
		spawner.AnimalTitle = Title;
		spawner.Rotation = transform.rotation.eulerAngles;
		spawner.ScaleSpan = new AnimalSpaner.FloatSpan
		{
		  min = transform.lossyScale.x,
		  max = transform.lossyScale.x
		};
		spawner.Rename();
		AnimalSpawnManager.SceneReady();
		DestroyImmediate(gameObject);
	 }
#endif

	 protected void GetAnimData()
	 {
		//_animStateID = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		//_animTransitionID = Animator.GetAnimatorTransitionInfo(0).fullPathHash;
	 }
	 /// <summary>
	 /// Изменение фазы
	 /// </summary>
	 protected virtual void PhaseChange() { }

	 public void Step(string foot)
	 {

	 }

  }

  [System.Flags]
  public enum NpcPhase
  {
	 none = 0,
	 free = 1,
	 idle = 2,
	 hold = 4

  }


}