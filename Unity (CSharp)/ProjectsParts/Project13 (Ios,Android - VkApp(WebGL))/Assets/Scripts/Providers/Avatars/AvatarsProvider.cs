using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Avatars.Common;
using UnityEngine;

namespace Game.Providers.Avatars
{
	public class AvatarsProvider : IAvatarsProvider
	{
		private readonly IAvatarsSettings _settings;
		private Dictionary<string, Texture2D> _avatars = new();

		public Dictionary<string, Texture2D> Avatars => _avatars;

		public bool IsLoaded { get; private set; }

		public AvatarsProvider(AvatarsSettings avatarsSettings)
		{
			_settings = avatarsSettings;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			var textures = Resources.LoadAll<Texture2D>(_settings.AvatarsFolder);
			foreach (var texture in textures)
			{
				_avatars.Add(texture.name, texture);
			}
			await UniTask.Yield();
		}

		public Texture2D GetTexture(string name)
		{
			return !string.IsNullOrEmpty(name) && _avatars.ContainsKey(name) ? _avatars[name] : GetRandomTexture();
		}

		public bool ExistsAvatar(string name)
		{
			return _avatars.ContainsKey(name);
		}
		public string GetRandomKeyExclude(string name)
		{

			var result = name;
			while (result == name)
			{
				result = _avatars.ElementAt(UnityEngine.Random.Range(0, _avatars.Count)).Key;
			}

			return result;
		}

		public string GetRandomKey()
		{
			return _avatars.ElementAt(UnityEngine.Random.Range(0, _avatars.Count)).Key;
		}
		public Texture2D GetRandomTexture()
		{
			return _avatars.ElementAt(UnityEngine.Random.Range(0, _avatars.Count)).Value;
		}

		public KeyValuePair<string, Texture2D> GetRandom()
		{
			return _avatars.ElementAt(UnityEngine.Random.Range(0, _avatars.Count));
		}
	}
}