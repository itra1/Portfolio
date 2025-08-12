using Core.Engine.Components.Avatars.Common;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace Core.Engine.Components.Avatars
{
  public class AvatarsProvider : IAvatarsProvider
	{
		private readonly IAvatarsSettings _settings;
		private Dictionary<string, Texture2D> _avatars = new();

		public Dictionary<string, Texture2D> Avatars => _avatars;

		public AvatarsProvider(IAvatarsSettings avatarsSettings)
		{
			_settings = avatarsSettings;
			LoadResources();
		}

		private void LoadResources()
		{
			var textures = Resources.LoadAll<Texture2D>(_settings.AvatarsFolder);

			foreach (var texture in textures)
			{
				_avatars.Add(texture.name, texture);
			}

			AppLog.Log("Avatars = " + _avatars.Count);
		}

		public Texture2D GetTexture(string name)
		{
			return _avatars[name];
		}

		public Texture2D GetRandomTexture()
		{
			return _avatars.ElementAt(Random.Range(0, _avatars.Count)).Value;
		}

		public KeyValuePair<string,Texture2D> GetRandom()
		{
			return _avatars.ElementAt(Random.Range(0, _avatars.Count));
		}
	}
}