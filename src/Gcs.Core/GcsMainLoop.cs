// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Gcs.Core;

public sealed class GcsMainLoop : IAsyncDisposable
{
    private readonly IntPtr _ctx;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cts = new();
    private readonly TaskCompletionSource _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly ConcurrentDictionary<IntPtr, GCHandle> _activeHandles = new();
    private int _isDisposing;

    public bool IsDisposing => Volatile.Read(ref _isDisposing) == 1;
    private static readonly GLib.GSourceFunc WakeupCallback = _ => false;

    public GcsMainLoop(IntPtr context)
    {
        _ctx = context == IntPtr.Zero ? GLib.g_main_context_default() : context;
        if (context != IntPtr.Zero) GLib.g_main_context_ref(_ctx);
        _thread = new Thread(RunInternal) { IsBackground = true, Name = "GCS-MainLoop" };
    }

    public Task Start() { _thread.Start(); return _tcs.Task; }

    internal bool RegisterHandle(IntPtr ptr, GCHandle handle)
    {
        if (IsDisposing) return false;
        _activeHandles[ptr] = handle;
        if (IsDisposing && _activeHandles.TryRemove(ptr, out _)) return false;
        return true;
    }

    internal void UnregisterHandle(IntPtr ptr)
    {
        if (_activeHandles.TryRemove(ptr, out var handle)) handle.Free();
    }

    private void RunInternal()
    {
        try
        {
            SynchronizationContext.SetSynchronizationContext(new GcsSyncContext(_ctx, this));
            _tcs.SetResult();
            while (!_cts.IsCancellationRequested) GLib.g_main_context_iteration(_ctx, true);
        }
        catch (Exception ex) { if (!_tcs.Task.IsCompleted) _tcs.SetException(ex); }
        finally { if (_ctx != IntPtr.Zero && _ctx != GLib.g_main_context_default()) GLib.g_main_context_unref(_ctx); }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposing, 1) == 1) return;
        _cts.Cancel();
        GLib.g_main_context_invoke(_ctx, WakeupCallback, IntPtr.Zero); 
        await _tcs.Task;
        _cts.Dispose();
        foreach (var handle in _activeHandles.Values) handle.Free();
        _activeHandles.Clear();
    }
}