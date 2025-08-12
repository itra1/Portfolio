using System;

namespace Engine.Scripts.Base
{
	[Flags]
	public enum DifficultyTrack : byte
	{
		Normal = 1,
		Hard = 2,
		Extreme = 4
	}
}
