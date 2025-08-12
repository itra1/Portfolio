using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Main
{
	public class HoldemSheet : SheetBase
	{
		protected override void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.Holdem;
		}

	}
}