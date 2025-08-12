using Engine.Scripts.Common.Structs;
using UnityEngine;

namespace Engine.Scripts.Settings
{
	[CreateAssetMenu(fileName = "RhythmSettings", menuName = "Dypsloom/Rhythm Timeline/RhythmSettings", order = 1)]
	public class RhythmSettings : ScriptableObject
	{
		[SerializeField] private FloatRange _spawnTimeRange = new() { Max = 9, Min = 3 };

		public FloatRange SpawnTimeRange => _spawnTimeRange;
	}
}
