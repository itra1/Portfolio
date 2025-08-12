using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBarrierAttack : ExEvent.EventBehaviour {
	
	public SpecialBarriersTypes type;      // Тип препядствия
	protected System.Action<bool> UseFunction;
	protected List<GameObject> bulletList = new List<GameObject>();
	public System.Action EndBarrier;

	public abstract void Generate(bool isActive, SpecialBarriersTypes? type);

}


/// <summary>
/// Специальный тип атак
/// </summary>
public enum SpecialBarriersTypes {
	airStone,                                           // Камнепад
	airArrow,                                           // Артелерийская аттака
	stickyFlor                                          // Липкий пол
}
