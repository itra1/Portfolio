using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BaseGameManager<T> where T : IInitializable, new()
{
    protected BaseGameManager()
    {
    }

    private static T _instance;

    // ReSharper disable StaticMemberInGenericType
    private static Task<bool> _blockInstanceTask;
    // ReSharper restore StaticMemberInGenericType

    public sealed class InstanceBlocker : IDisposable
    {
        private readonly TaskCompletionSource<bool> _blockTcs = new TaskCompletionSource<bool>();

        public InstanceBlocker()
        {
            _blockInstanceTask = _blockTcs.Task;
        }

        private void UnBlockInstance()
        {
            _blockTcs.SetResult(true);
        }

        public void Dispose()
        {
            UnBlockInstance();
        }
    }

    private static async Task<T> CreateInstance()
    {
        var instance = new T();
        await instance.Setup();
        return instance;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public static async Task<T> GetInstanceAsync()
    {
        if (_blockInstanceTask == null)
        {
            var blockInstanceTcs = new TaskCompletionSource<bool>();
            _blockInstanceTask = blockInstanceTcs.Task;
            _instance = await CreateInstance();
            blockInstanceTcs.SetResult(true);

        }

        await _blockInstanceTask;
        return _instance;
    }

    public static async Task<T> InstanceReSetup()
    {
        if (_instance == null)
        {
            return await GetInstanceAsync();
        }

        using (BlockInstance())
        {
            await _instance.Setup();
            return _instance;
        }
    }

    protected static InstanceBlocker BlockInstance()
    {
        return new InstanceBlocker();
    }

    public virtual Task Setup()
    {
        return Task.CompletedTask;
    }
}
