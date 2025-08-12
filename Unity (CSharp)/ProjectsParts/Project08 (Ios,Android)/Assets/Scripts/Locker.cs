using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Singleton<Locker> {

	public GameObject lockerObject;

	public bool isLocker;

	public int lockerCount = 0;

	public void SetLocker(bool isLock) {

		if (isLock && Tutorial.Instance.isTutorial)
			return;

		lockerCount += isLock ? +1 : -1;

		if (isLocker && lockerCount <= 0) {
			isLocker = false;
			lockerObject.gameObject.SetActive(isLocker);
		} else if (!isLocker && lockerCount > 0) {
			isLocker = true;
			lockerObject.gameObject.SetActive(isLocker);
		}

	}

}
