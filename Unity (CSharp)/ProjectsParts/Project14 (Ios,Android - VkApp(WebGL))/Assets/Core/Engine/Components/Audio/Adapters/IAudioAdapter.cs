using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.Audio
{
	public interface IAudioAdapter
	{
		public void PlaySound(string type);
	}
}
