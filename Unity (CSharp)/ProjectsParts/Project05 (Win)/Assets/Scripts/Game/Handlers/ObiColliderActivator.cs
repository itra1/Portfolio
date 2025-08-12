using UnityEngine;
using System.Collections;

public class ObiColliderActivator : MonoBehaviour
{
  public Obi.ObiCollider _collider; 

  public void Activate()
  {
	 _collider.enabled = true;
  }

  public void Deactivate()
  {
	 _collider.enabled = false;
  }
}
