using System;
using System.IO;
using System.Threading;
using Core.FileResources.Customizing.Loaders.Base;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Loaders
{
	public class BytesLoader : IResourceLoader
	{
		private readonly int _bufferSize;
		
		public BytesLoader(int bufferSize) => _bufferSize = bufferSize;
		
		public async UniTask UploadAsync(string path, byte[] bytes, CancellationToken cancellationToken)
		{
			try
			{
				await using var fileStream = new FileStream(path,
					FileMode.OpenOrCreate,
					FileAccess.Write,
					FileShare.ReadWrite,
					_bufferSize,
					FileOptions.Asynchronous);
				
				await fileStream.WriteAsync(bytes, cancellationToken);
			}
			catch (Exception exception)
			{
				if (File.Exists(path))
					File.Delete(path);
				
				if (exception is OperationCanceledException)
					throw new OperationCanceledException(cancellationToken);
				
				throw new ApplicationException(exception.Message);
			}
		}
		
		public async UniTask<byte[]> DownloadAsync(string path, CancellationToken cancellationToken)
		{
			await using var fileStream = new FileStream(path,
				FileMode.Open,
				FileAccess.Read,
				FileShare.ReadWrite,
				_bufferSize,
				FileOptions.Asynchronous | FileOptions.SequentialScan);
			
			var buffer = new byte[fileStream.Length];
			
			int result;
			
			do
			{
				result = await fileStream.ReadAsync(buffer, cancellationToken);
			} 
			while (result > 0);
			
			return buffer;
		}
	}
}