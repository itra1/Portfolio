using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButton
{
	UnityEngine.Events.UnityEvent OnClickPointer { get; set; }
}
