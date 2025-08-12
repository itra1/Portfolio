using System;
using System.Threading;
using Core.Utils.Enums;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core.Utils
{
    public static class TextureExtensions
    {
        public static async UniTask<byte[]> ToBytesAsync(this Texture source,
            CancellationToken cancellationToken,
            FileEncodingFormat format)
        {
            byte[] bytes;
            
            var graphicsFormat = source.graphicsFormat;
            var width = source.width;
            var height = source.height;
            var buffer = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            try
            {
                var request = await AsyncGPUReadback.RequestIntoNativeArray(ref buffer, source);
                
                cancellationToken.ThrowIfCancellationRequested();
                
                if (!request.done || request.hasError)
                    throw new InvalidOperationException("Failed to request native array when trying to encode resource");

                if (format == FileEncodingFormat.RawTexture)
                {
                    bytes = buffer.ToArray();
                }
                else
                {
                    await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                    {
                        await UniTask.SwitchToThreadPool();
                        
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        using var encodedBytes = format == FileEncodingFormat.Png
                            ? ImageConversion.EncodeNativeArrayToPNG(buffer, graphicsFormat, (uint) width, (uint) height)
                            : ImageConversion.EncodeNativeArrayToJPG(buffer, graphicsFormat, (uint) width, (uint) height);
                        
                        bytes = encodedBytes.ToArray();
                    }
                }
            }
            finally
            {
                if (format != FileEncodingFormat.RawTexture && Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
                
                if (buffer.IsCreated)
                    buffer.Dispose();
            }

            return bytes;
        }
    }
}