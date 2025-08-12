using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class UUIDBase : MonoBehaviourBase
{
  [UUID(drawnNewButton = true)]
  [SerializeField]
  private string _uuid;

  public string Uuid => _uuid;

  public void GenerateUuid()
  {
	 _uuid = GetNewUUID();
  }
}
