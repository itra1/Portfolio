using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Items
{
  public interface IHoldParent
  {

	 void BeforeStartHold();
	 void AfterStartHold();

	 void BeforeEndHold();
	 void AfterEndHold();

  }
}