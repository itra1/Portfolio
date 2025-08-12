using System.Threading;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;

namespace Environment.Microsoft.Windows.Apps.Office.Server
{
    public interface IMsOfficeRequestAsync
    {
        UniTask<IPacket> RequestAsync(IPacket outgoingPacket, CancellationToken cancellationToken);
    }
}