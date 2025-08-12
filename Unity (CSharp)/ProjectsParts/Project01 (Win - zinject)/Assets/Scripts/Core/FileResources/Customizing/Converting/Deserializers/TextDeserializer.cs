using System.Text;
using System.Threading;
using Core.FileResources.Customizing.Converting.Deserializers.Base;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Deserializers
{
    public class TextDeserializer : IResourceDeserializer<string>
    {
        private readonly Encoding _encoding;
		
        public TextDeserializer(Encoding encoding) => _encoding = encoding;
        
        public async UniTask<byte[]> DeserializeAsync(string resource, CancellationToken cancellationToken)
        {
            byte[] bytes;
            
            if (!Thread.CurrentThread.IsBackground)
            {
                try
                {
                    await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                    {
                        await UniTask.SwitchToThreadPool();
                        cancellationToken.ThrowIfCancellationRequested();
                        bytes = _encoding.GetBytes(resource);
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
                bytes = _encoding.GetBytes(resource);
            }
            
            return bytes;
        }
    }
}