// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using System.Runtime.InteropServices;

namespace Gcs.Core;

public static class GcsBootloader
{
    private static GcsMainLoop? _loop;
    private static readonly object LockObj = new();

    public static int Initialize(IntPtr context)
    {
        lock (LockObj)
        {
            try
            {
                if (_loop != null) return -2;
                _loop = new GcsMainLoop(context);
                _loop.Start().GetAwaiter().GetResult();
                return 0;
            }
            catch { return -1; }
        }
    }
    
    public static int Restart(IntPtr context)
    {
        lock (LockObj)
        {
            try
            {
                if (_loop == null) return -2;
                _loop.DisposeAsync().AsTask().GetAwaiter().GetResult();
                _loop = new GcsMainLoop(context);
                _loop.Start().GetAwaiter().GetResult();
                return 0;
            }
            catch { return -1; }
        }
    }
}