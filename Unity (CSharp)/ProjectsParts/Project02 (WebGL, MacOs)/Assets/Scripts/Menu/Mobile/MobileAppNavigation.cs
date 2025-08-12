using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileAppNavigation : MonoBehaviour
{
	public UnityEngine.Events.UnityAction OnNewsEvent;
	public UnityEngine.Events.UnityAction OnWclEvent;
	public UnityEngine.Events.UnityAction OnMainEvent;
	public UnityEngine.Events.UnityAction OnSettingsEvent;
	public UnityEngine.Events.UnityAction OnProfileEvent;

	public void NewsTouch()
	{
		OnNewsEvent?.Invoke();
	}

	public void WclTouch()
	{
		OnWclEvent.Invoke();
	}

	public void MainTouch()
	{
		OnMainEvent?.Invoke();
	}

	public void SettingsTouch()
	{
		OnSettingsEvent?.Invoke();
	}

	public void ProfileTouch()
	{
		OnProfileEvent?.Invoke();
	}

}
