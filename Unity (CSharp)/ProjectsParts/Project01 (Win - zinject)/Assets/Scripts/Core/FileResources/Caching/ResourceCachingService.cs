using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Core.FileResources.Customizing;
using Core.FileResources.Customizing.Category;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.FileResources.Caching
{
	public class ResourceCachingService : IResourceCachingService
	{
		private static readonly string[] InvalidHashSymbols = { "-", " " };
		
		private readonly IResourceCustomizer _customizer;
		private readonly string _path;
		private readonly ConcurrentDictionary<string, byte> _cachingInProgress;
		
		public ResourceCachingService(IResourceCustomizer customizer)
		{
			_customizer = customizer;
			_path = Application.persistentDataPath;
			_cachingInProgress = new ConcurrentDictionary<string, byte>();
			
			CreateCacheDirectoryIfNotExists();
		}
		
		public bool IsAlreadyCached(ResourceCategory category, string url) => 
			IsAlreadyCached(category, url, out _);
		
		public bool IsAlreadyCached(ResourceCategory category, string url, out string path)
		{
			path = GetFullPath(GetDirectoryPath(_customizer.GetDirectoryName(category)), url);
			return File.Exists(path);
		}
		
		public async UniTask<string> PutIntoCacheAsync(ResourceCategory category,
			string url,
			byte[] bytes,
			CancellationToken cancellationToken)
		{
			var loader = _customizer.GetLoader(category);
			
			var path = GetFullPath(GetDirectoryPath(_customizer.GetDirectoryName(category)), url);
			
			if (!_cachingInProgress.TryAdd(path, default))
				return path;
			
			try
			{
				await loader.UploadAsync(path, bytes, cancellationToken);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
				{
					while (_cachingInProgress.ContainsKey(path) && !_cachingInProgress.TryRemove(path, out _)) { }
				}
				else
				{
					//It is not acceptable to abort an attempt to delete a key using a cancellation token,
					//as this will cause a bug the next time you try to get the resource from the cache
					await UniTask.WaitWhile(() => 
						_cachingInProgress.ContainsKey(path) && !_cachingInProgress.TryRemove(path, out _));
				}
			}
			
			return path;
		}
		
		public async UniTask<(byte[], string)> GetFromCacheAsync(ResourceCategory category,
			string url,
			CancellationToken cancellationToken)
		{
			var loader = _customizer.GetLoader(category);
			
			var path = GetFullPath(GetDirectoryPath(_customizer.GetDirectoryName(category)), url);
			
			if (_cachingInProgress.ContainsKey(path))
			{
				if (Thread.CurrentThread.IsBackground)
				{
					do
					{
						cancellationToken.ThrowIfCancellationRequested();
					}
					while (_cachingInProgress.ContainsKey(path));
				}
				else
				{
					await UniTask.WaitWhile(() => _cachingInProgress.ContainsKey(path),
						cancellationToken: cancellationToken);
				}
			}
			
			return (await loader.DownloadAsync(path, cancellationToken), path);
		}
		
		private void CreateCacheDirectoryIfNotExists()
		{
			if (!Directory.Exists(_path))
				Directory.CreateDirectory(_path);
		}
		
		private string GetDirectoryPath(string directoryName)
		{
			if (string.IsNullOrEmpty(directoryName))
				return _path;
			
			var path = $"{_path}/resources/{directoryName}";
			
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			
			return path;
		}

		private string GetFullPath(string directoryPath, string url)
		{
			if (string.IsNullOrEmpty(url))
				return directoryPath;
			
			var sha1 = new SHA1Managed();
			var hash = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(url)));
			
			for (var i = InvalidHashSymbols.Length - 1; i >= 0; i--)
				hash = hash.Replace(InvalidHashSymbols[i], string.Empty);
			
			hash = hash.ToLower();
			
			return $"{directoryPath}/{hash}";
		}
	}
}