// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using System.Runtime.InteropServices;

namespace Gcs.Core;

public sealed class GcsSyncContext : SynchronizationContext
{
    private readonly IntPtr _ctx;
    private readonly GcsMainLoop _owner;

    public GcsSyncContext(IntPtr ctx, GcsMainLoop owner)
    {
        _ctx = ctx;
        _owner = owner;
    }

    private static readonly GLib.GSourceFunc PostCallback = stateHandle =>
    {
        var handle = GCHandle.FromIntPtr(stateHandle);
        if (handle.Target is Tuple<SendOrPostCallback, object?, GcsMainLoop> tuple)
        {
            var (callback, state, owner) = tuple;
            try { callback(state); }
            catch (Exception ex) { Environment.FailFast("Krytyczny błąd w pętli GCS", ex); }
            finally { owner.UnregisterHandle(stateHandle); }
        }
        return false;
    };

    public override void Post(SendOrPostCallback d, object? state)
    {
        if (_owner.IsDisposing) return;

        var payload = Tuple.Create(d, state, _owner);
        var handle = GCHandle.Alloc(payload);
        var ptr = GCHandle.ToIntPtr(handle);

        if (!_owner.RegisterHandle(ptr, handle))
        {
            handle.Free();
            return;
        }

        GLib.g_main_context_invoke(_ctx, PostCallback, ptr);
    }
}

