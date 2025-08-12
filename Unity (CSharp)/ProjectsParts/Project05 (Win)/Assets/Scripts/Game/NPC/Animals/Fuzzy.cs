using UnityEngine;
using System.Collections;
//using Kalagaan.HairDesignerExtension;

namespace it.Game.NPC.Animals
{
  public class Fuzzy : Animal
  {
	 [SerializeField]
	 private int _skin = 0;


	 protected override void OnEnable()
	 {
		base.OnEnable();

		//StartCoroutine(WairAndRun());
	 }

	 //IEnumerator WairAndRun()
	 //{

		//HairDesigner hair = GetComponentInChildren<HairDesigner>();
		//hair.enabled = false;
		//yield return new WaitForEndOfFrame();
		//hair.enabled = true;

		//for (int i = 0; i < hair.m_generators.Count; ++i)
		//  hair.GetLayer(i).SetActive(i == _skin);
		//hair.Update();
	 //}

  }
}