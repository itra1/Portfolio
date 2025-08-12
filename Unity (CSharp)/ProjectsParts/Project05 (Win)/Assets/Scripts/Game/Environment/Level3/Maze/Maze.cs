using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level3.Maze {


  /// <summary>
  /// Лабиринт на 3м уровне
  /// </summary>
  public class Maze : Environment
  {
	 /*
	  * Стате
	  * 0 - обычное состояние
	  * 1 - открыты врата 1
	  * 2 - открыты врата 2
	  */

	 [SerializeField]
	 private Renderer _gateShield1;
	 [SerializeField]
	 private Renderer _gateShield2;

	 private it.Game.NPC.Enemyes.FireGolemMaze[] _golems;

	 protected override void Awake()
	 {
		base.Awake();
		_golems = GetComponentsInChildren<it.Game.NPC.Enemyes.FireGolemMaze>();
	 }

	 protected override void Start()
	 {
		base.Start();
		_gateShield1.material = Instantiate(_gateShield1.material);
		_gateShield2.material = Instantiate(_gateShield2.material);
	 }

	 public void OpenGate1()
	 {
		_gateShield1.material.DOFloat(0, "_Dissolve", 1f).OnComplete(()=> {
		  _gateShield1.gameObject.SetActive(false);
		});

		for(int i = 0; i < _golems.Length; i++)
		  _golems[i].Activate();

		State = 1;
		Save();
	 }

	 public void OpenGate2()
	 {

		_gateShield2.material.DOFloat(0, "_Dissolve", 1f).OnComplete(() => {
		  _gateShield2.gameObject.SetActive(false);
		});
		for (int i = 0; i < _golems.Length; i++)
		  _golems[i].Deactivate();
		State = 2;
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if(State == 0)
		  {
			 _gateShield1.gameObject.SetActive(true);
			 _gateShield1.material.SetFloat("_Dissolve", 1);
			 _gateShield2.gameObject.SetActive(true);
			 _gateShield2.material.SetFloat("_Dissolve", 1);
			 for (int i = 0; i < _golems.Length; i++)
				_golems[i].Deactivate();
		  }
		  if(State == 1)
		  {
			 _gateShield1.gameObject.SetActive(false);
			 _gateShield2.gameObject.SetActive(true);
			 for (int i = 0; i < _golems.Length; i++)
				_golems[i].Activate();
		  }
		  if (State == 2)
		  {
			 _gateShield1.gameObject.SetActive(false);
			 _gateShield2.gameObject.SetActive(false);
			 for (int i = 0; i < _golems.Length; i++)
				_golems[i].Deactivate();
		  }
		}

		//_gateShield1.gameObject.SetActive(State <= 0);
		//_gateShield1.material.SetFloat("_Dissolve", (State <= 0 ? 1 : 0));
		//_gateShield2.gameObject.SetActive(State <= 1);
		//_gateShield2.material.SetFloat("_Dissolve", (State <= 1 ? 1 : 0));

	 }

  }
}