using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("you_sat_at_the_table", "Плеер садится за стол")]
	public class YouSatAtTheTable : SocketIn
	{
		//public ulong TableId;

		public override void Parse()
		{
			//TableId = (ulong)JSource.GetProperty("shared_data").GetProperty("table_id").GetInt64();
		}

		public override void Process()
		{
			UserController.Instance.ActiveTableManager.TableListChange();
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}

	[SocketAction("you_left_the_table", "Плеер поднимается за стол")]
	public class YouLetTheTable : SocketIn
	{
		//public ulong TableId;

		public override void Parse()
		{
			//TableId = (ulong)JSource.GetProperty("shared_data").GetProperty("table_id").GetInt64();
		}

		public override void Process()
		{
			UserController.Instance.ActiveTableManager.TableListChange();
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}


}