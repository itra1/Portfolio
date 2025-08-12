using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using UI.MusicPlayer.Controller.Converter.Data;

namespace UI.MusicPlayer.Controller.Converter
{
    public class AudioClipInfoResolver : IAudioClipInfoResolver
    {
        public async UniTask<AudioClipInfo> ResolveAsync(AudioMaterialData material,
            byte[] bytes,
            CancellationToken cancellationToken)
        {
            return material.File.Mimetype switch
            {
                "audio/mpeg" => await ConvertMp3BytesToAudioClipInfoAsync(material, bytes, cancellationToken),
                "audio/wav" => ConvertWavBytesToAudioClipInfo(material, bytes),
                _ => ConvertBytesToAudioClipInfo(material, bytes)
            };
        }
        
        private async UniTask<AudioClipInfo> ConvertMp3BytesToAudioClipInfoAsync(AudioMaterialData material,
            byte[] bytes,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await using var mp3Stream = new MemoryStream(bytes);
            await using var mp3Audio = new Mp3FileReader(mp3Stream);
            await using var waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Audio);
            await using var outputStream = new MemoryStream();
            await using var waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat);
            
            var buffer = new byte[waveStream.Length];
            
            waveStream.Position = 0;

            int result;
            
            do
            {
                result = await waveStream.ReadAsync(buffer, 0, Convert.ToInt32(waveStream.Length), cancellationToken);
            } 
            while (result > 0);
            
            await waveFileWriter.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            await waveFileWriter.FlushAsync(cancellationToken);
            
            var wav = new Wav(outputStream.ToArray());
            var data = CombineChannels(wav.LeftChannel, wav.RightChannel);
            
            return new AudioClipInfo(material, wav.SampleCount, wav.ChannelCount, wav.Frequency, data);
        }
        
        private AudioClipInfo ConvertWavBytesToAudioClipInfo(AudioMaterialData material, byte[] bytes)
        {
            var wav = new Wav(bytes);
            var data = CombineChannels(wav.LeftChannel, wav.RightChannel);
            
            return new AudioClipInfo(material, wav.SampleCount, wav.ChannelCount, wav.Frequency, data);
        }
        
        private AudioClipInfo ConvertBytesToAudioClipInfo(AudioMaterialData material, byte[] bytes)
        {
            var data = new float[bytes.Length * 4];
            
            Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);
            
            return new AudioClipInfo(material, bytes.Length, 2, 48000, data);
        }

        private float[] CombineChannels(IReadOnlyList<float> leftChannel, IReadOnlyList<float> rightChannel)
        {
            var leftChannelLength = leftChannel?.Count ?? 0;
            var rightChannelLength = rightChannel?.Count ?? 0;
            
            var channels = new float[leftChannelLength + rightChannelLength];
            
            var pointer = 0;
            var leftChannelPointer = 0;
            var rightChannelPointer = 0;
            
            while (pointer < channels.Length)
            {
                if (leftChannel != null)
                {
                    channels[pointer] = leftChannel[leftChannelPointer];
                    leftChannelPointer++;
                    pointer++;
                }
                
                if (rightChannel != null)
                {
                    channels[pointer] = rightChannel[rightChannelPointer];
                    rightChannelPointer++;
                    pointer++; 
                }
            }
            
            return channels;
        }
    }
}