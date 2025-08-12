using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Decorations {

  public class Coral: MonoBehaviourBase {

    [SerializeField]
    private GameObject[] m_Tentacle;

    //private void Start() {
    //  UpdateTentacles();
    //}



    [ContextMenu("Update tentacles")]
    private void UpdateTentacles() {

			foreach (var elem in m_Tentacle)
				elem.SetActive(Random.value < 0.5f);
    }

  }

}