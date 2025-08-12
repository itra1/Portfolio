using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Main
{
	public class AllOrNothingSheet : SheetBase
	{

		protected override void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.PLO_4_hi;
		}
	}
}