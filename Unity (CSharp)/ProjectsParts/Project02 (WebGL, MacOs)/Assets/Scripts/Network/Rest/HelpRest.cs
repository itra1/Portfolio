using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Network.Rest
{

	public class RegionResponse
	{
		public List<Region> data;
	}

	public class Region
	{
		public ulong id;
		public ulong gp_id;
		public string title;
		public string short_title;
		public bool can_register_with;
		public string flag;
	}
}