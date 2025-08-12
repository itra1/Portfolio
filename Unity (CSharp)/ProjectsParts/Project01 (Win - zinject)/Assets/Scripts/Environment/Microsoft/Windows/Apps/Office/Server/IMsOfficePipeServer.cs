using System;
using Core.Base;
using Cysharp.Threading.Tasks;

namespace Environment.Microsoft.Windows.Apps.Office.Server
{
    public interface IMsOfficePipeServer : IMsOfficePipeServerState, ILateInitialized, IDisposable
    {
        UniTask<bool> ConnectAsync(string serverName);
        UniTask<bool> DisconnectAsync();
    }
}