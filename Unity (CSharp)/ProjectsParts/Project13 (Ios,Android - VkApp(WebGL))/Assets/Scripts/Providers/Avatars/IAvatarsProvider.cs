using System.Collections.Generic;
using Game.Base.AppLaoder;
using UnityEngine;

namespace Game.Providers.Avatars
{
	public interface IAvatarsProvider : IAppLoaderElement
	{
		Dictionary<string, Texture2D> Avatars { get; }
		Texture2D GetTexture(string name);
		Texture2D GetRandomTexture();
		KeyValuePair<string, Texture2D> GetRandom();
		bool ExistsAvatar(string name);
		string GetRandomKey();
	}
}
