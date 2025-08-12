using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Engine.Components.Avatars
{
	public interface IAvatarsProvider
	{
		Dictionary<string, Texture2D> Avatars { get; }
		Texture2D GetTexture(string name);
		Texture2D GetRandomTexture();
		KeyValuePair<string, Texture2D> GetRandom();
	}
}
