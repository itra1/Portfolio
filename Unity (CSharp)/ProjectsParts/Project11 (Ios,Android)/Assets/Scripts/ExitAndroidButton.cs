using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAndroidButton : MonoBehaviour {

	public void Click() {
		GameService.Instance.Logout();
	}
}
