using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using DG.Tweening;

namespace it.Game.NPC.Enemyes.Boses.Hunter
{

  public class Gishtil : Enemy
  {
	 [SerializeField]
	 private PlayMakerFSM _behaviour;
	 public PlayMakerFSM Behaviour { get => _behaviour; set => _behaviour = value; }

	 [SerializeField]
	 private Collider _colliderHand;

	 [SerializeField]
	 private it.Game.Environment.Level6.Gishtil.GishtilBattle _environment;
	 public int BattlePhase
	 {
		get => _environment.State;
	 }

	 protected override void Start()
	 {
		base.Start();
		_environment.OnStateChangeEvent.AddListener(OnStateChange);
		Clear();
	 }
	 protected void OnDestroy()
	 {
		//_environment.OnStateChangeEvent.RemoveListener(OnStateChange);
	 }
	 private void OnStateChange(int newState)
	 {
		if (newState < 2)
		  GetComponent<Animator>().SetInteger("Form", 0);
	 }
	 public void SetForm1()
	 {
		GetComponent<Animator>().SetInteger("Form", 1);
	 }

	 public void Clear()
	 {
		DeactivateHandCollider();
	 }

	 public MeshStructs[] _meshStruct;

	 private bool _readyChangePhase2;
	 public bool ReadyChangePhase2 { get => _readyChangePhase2; set => _readyChangePhase2 = value; }

	 public void SetShow(bool isShow)
	 {
		Debug.Log("SetShow " + isShow);
		for(int i = 0;i < _meshStruct.Length; i++)
		{
		  if (isShow)
		  {
			 MeshStructs str = _meshStruct[i];
			 Renderer rend = str._renderer;
			 rend.material.DOFloat(1f, "_Opacity", 0);

			 //rend.material.SetFloat("_Cutoff", 0);
			 //DOTween.To(() => rend.material.GetColor("_Color"),
			 //(x) => rend.material.SetColor("_Color", x),
			 //new Color(1, 1, 1, 1), 1
			 //).OnComplete(()=> {
			 //  rend.material = str._standart;
			 //});
		  }
		  else
		  {
			 MeshStructs str = _meshStruct[i];
			 Renderer rend = str._renderer;
			 rend.material.DOFloat(.1f, "_Opacity", 0);

			 //rend.material.SetFloat("_Cutoff", 1);
			 //str._transperent.color = new Color(1, 1, 1, 1);
			 //rend.material = str._transperent;
			 //DOTween.To(() => rend.material.GetColor("_Color"),
			 //(x) => rend.material.SetColor("_Color", x),
			 //new Color(1, 1, 1, 0), 1
			 //);
		  }
		}
	 }

	 public void PhaseChangeReady()
	 {

	 }

	 public void ActivateHandCollider()
	 {
		_colliderHand.gameObject.SetActive(true);
	 }

	 public void DeactivateHandCollider()
	 {
		_colliderHand.gameObject.SetActive(false);
	 }

	 public void HandTrigger()
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, 100f/3, false);
	 }

	 public void SetPhase(int num)
	 {
		if(num == 2)
		{
		  _readyChangePhase2 = true;
		}
	 }

	 [System.Serializable]
	 public struct MeshStructs
	 {
		public Renderer _renderer;
	 }
  }
}

