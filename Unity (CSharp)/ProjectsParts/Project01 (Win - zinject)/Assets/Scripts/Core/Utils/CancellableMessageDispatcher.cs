using System;
using System.Threading;
using com.ootii.Messages;
using Core.Logging;
using Cysharp.Threading.Tasks;

namespace Core.Utils
{
    public static class CancellableMessageDispatcher
    {
        public static async UniTask<bool> SendMessageAsync(string rType, 
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessage(rType, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        public static async UniTask<bool> SendMessageAsync(string rType, 
            string rFilter,
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessage(rType, rFilter, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        public static async UniTask<bool> SendMessageDataAsync(string rType,
            object rData,
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessageData(rType, rData, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        public static async UniTask<bool> SendMessageAsync(object rSender, 
            string rType, 
            object rData,
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessage(rSender, rType, rData, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        public static async UniTask<bool> SendMessageAsync(object rSender, 
            object rRecipient, 
            string rType, 
            object rData,
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessage(rSender, rRecipient, rType, rData, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        public static async UniTask<bool> SendMessageAsync(object rSender,
            string rRecipient,
            string rType,
            object rData,
            float rDelayInSeconds,
            CancellationToken cancellationToken)
        {
            if (!await MakeDelayAsync(rDelayInSeconds, cancellationToken)) 
                return false;
            
            MessageDispatcher.SendMessage(rSender, rRecipient, rType, rData, EnumMessageDelay.IMMEDIATE);
            return true;
        }

        private static async UniTask<bool> MakeDelayAsync(float rDelayInSeconds, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(rDelayInSeconds), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }

            return true;
        }
    }
}