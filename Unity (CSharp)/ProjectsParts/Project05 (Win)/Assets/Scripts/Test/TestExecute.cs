using UnityEngine;
using System.Collections;

public class TestExecute : MonoBehaviour
{
  public void OnPreRender()
  {
	 gameObject.SetActive(false);
	 //Debug.Log("OnPreRender");
  }
  public void OnBecameVisible()
  {
	 Debug.Log("OnBecameVisible");
  }

  public void OnBecameInvisible()
  {
	 Debug.Log("OnBecameInvisible");
  }


  public void Start()
  {
	 Debug.Log("Start");
  }

}
