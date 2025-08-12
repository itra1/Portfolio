using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CursorChange : MonoBehaviour
{

  void Update()
  {
	 if (Input.GetKeyDown(KeyCode.H))
	 {
		Cursor.visible = !Cursor.visible;
	 }
  }
}
