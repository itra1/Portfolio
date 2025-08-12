using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WWWCachier : Singleton<WWWCachier>
{
	[SerializeField]
	string url;
	static string filesPatch => Application.persistentDataPath + "/" + "CahedFiles" + "/";
	static Dictionary<string, Sprite> _casheSprite = new Dictionary<string, Sprite>();
	static Dictionary<string, Texture2D> _casheTextures = new Dictionary<string, Texture2D>();
	static Dictionary<string, byte[]> _casheBytes = new Dictionary<string, byte[]>();

#if UNITY_STANDALONE_WIN
	// Start is called before the first frame update
	void Start()
	{
		CreateDirectoryForFiles();
	}

	private void CreateDirectoryForFiles()
	{
		//filesPatch = Application.persistentDataPath + "/" + "CahedFiles" + "/";
		it.Logger.Log(filesPatch);
		if (!Directory.Exists(filesPatch))
		{
			Directory.CreateDirectory(filesPatch);
		}
	}
	public WWWCachier DownloadToImage(Image targ)
	{
		StartCoroutine(DownloadToImage(url, targ));
		return this;
	}
#endif
	public WWWCachier SetUrl(string url)
	{
		this.url = url;
		return this;
	}
#if UNITY_STANDALONE_WIN
	private IEnumerator DownloadToImage(string url, Image target)
	{
		Uri ur = new Uri(url);
		var filename = CreateMD5(url);// Path.GetFileName(ur.AbsoluteUri);
		if (!File.Exists(filesPatch + filename))
		{
			var www = UnityWebRequestTexture.GetTexture(url);
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				it.Logger.LogError(www.error);
			}
			else
			{
				var tex = DownloadHandlerTexture.GetContent(www);
				//it.Logger.Log(tex.width+" "+tex.height);
				var loadedsprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
				SetSpriteToImage(loadedsprite, ref target);

				File.WriteAllBytes(filesPatch + filename, tex.EncodeToPNG());

				//it.Logger.Log($"file saved: {filesPatch + filename}");
			}
		}
		else
		{
			//it.Logger.Log($"file in cache {filesPatch + filename}");
			var dat = File.ReadAllBytes(filesPatch + filename);
			var tex = new Texture2D(2, 2);
			tex.LoadImage(dat);
			var loadedsprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
			SetSpriteToImage(loadedsprite, ref target);
			yield break;

		}

	}
#endif
	void SetSpriteToImage(Sprite sprite, ref Image target)
	{
		if (target == null) return;
		target.sprite = sprite;
	}
	public static string CreateMD5(string str)
	{
		using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
		{

			byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
			byte[] hash = md5.ComputeHash(data);
			return BitConverter.ToString(hash).Replace("-", "");
		}

	}

	public bool CasheExistsTexture(string url)
	{
		var filename = CreateMD5(url);
		return _casheTextures.ContainsKey(filename);
	}
	public Texture2D LoadFromCasheTexture(string url)
	{
		var filename = CreateMD5(url);
		return _casheTextures[filename];
	}
	public void SaveTextureCashe(Texture2D data, string url)
	{
		var filename = CreateMD5(url);

		if (!_casheTextures.ContainsKey(filename))
		{
			_casheTextures.Add(filename, data);
		}
	}

	public Sprite GetSpriteFromCashe(string url)
	{

		var filename = CreateMD5(url);
		return _casheSprite.ContainsKey(filename) ? _casheSprite[filename] : null;
	}

	public void AddSpriteToCashe(string url, Sprite sprite)
	{

		var filename = CreateMD5(url);
		if (_casheSprite.ContainsKey(filename))
			return;
		_casheSprite.Add(filename, sprite);
	}


	public bool CasheExistsBytes(string url)
	{
		var filename = CreateMD5(url);
		return _casheBytes.ContainsKey(filename);
	}
	public byte[] LoadFromCasheBytes(string url)
	{
		var filename = CreateMD5(url);
		return _casheBytes[filename];
	}
	public void SaveByte(byte[] data, string url)
	{
#if UNITY_STANDALONE_WIN
		var filename = CreateMD5(url);
		File.WriteAllBytes(filesPatch + filename, data);
#endif
	}
	public void SaveByteCashe(byte[] data, string url)
	{
		var filename = CreateMD5(url);
		_casheBytes.Add(filename, data);
	}
	public byte[] LoadByte(string url)
	{
#if UNITY_STANDALONE_WIN
		var filename = CreateMD5(url);
		var dat = File.ReadAllBytes(filesPatch + filename);
		_casheBytes.Add(filename, dat);
		return dat;
#else
		return null;
#endif
	}



	public bool FileExist(string url)
	{
#if UNITY_STANDALONE_WIN
		var filename = CreateMD5(url);
		return File.Exists(filesPatch + filename);
#else
		return false;
#endif
	}
	public Texture2D LoadTexture(string url)
	{
#if UNITY_STANDALONE_WIN
		var filename = CreateMD5(url);
		var dat = File.ReadAllBytes(filesPatch + filename);
		var tex = new Texture2D(2, 2);
		tex.LoadImage(dat);
		_casheTextures.Add(filename, tex);
		return tex;
#else
		return null;
#endif
	}

}
