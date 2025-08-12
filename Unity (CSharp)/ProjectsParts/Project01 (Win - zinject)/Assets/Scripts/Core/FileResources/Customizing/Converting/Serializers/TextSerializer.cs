using System.Text;
using System.Threading;
using Core.FileResources.Customizing.Converting.Serializers.Base;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Serializers
{
	public class TextSerializer : IResourceSerializer<string>
	{
		private readonly Encoding _encoding;
		
		public TextSerializer(Encoding encoding) => _encoding = encoding;
		
		public async UniTask<string> SerializeAsync(string name, byte[] bytes, CancellationToken cancellationToken)
		{
			string text;
			
			if (!Thread.CurrentThread.IsBackground)
			{
				try
				{
					await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
					{
						await UniTask.SwitchToThreadPool();
						cancellationToken.ThrowIfCancellationRequested();
						text = _encoding.GetString(bytes);
					}
				}
				finally
				{
					if (Thread.CurrentThread.IsBackground)
						await UniTask.SwitchToMainThread();
				}
			}
			else
			{
				text = _encoding.GetString(bytes);
			}
			
			return text;
		}
	}
}