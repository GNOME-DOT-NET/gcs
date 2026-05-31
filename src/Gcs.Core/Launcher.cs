// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using System.Runtime;
using System.Runtime.InteropServices;

namespace Gcs.Core;

public static class Launcher
{
    private static readonly object LockObj = new();
    private static Thread? _Thread;
    private static bool _isRunning;

    // If you're looking for the [UnmanagedCallersOnly] attribute for this method, it is in Gcs.Shell. DON'T LOOK FOR IT HERE
    public static int Initialize(IntPtr context)
    {
        lock (LockObj)
        {
            try
            {
                if (_Thread != null) return -2;

                _isRunning = true;
                
                
                _Thread = new Thread(PlaceholderLoop)
                {
                    IsBackground = true,
                    Name = "PlaceholderLoop"
                };
                _Thread.Start();

                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
    
    // If you're looking for the [UnmanagedCallersOnly] attribute for this method, it is in Gcs.Shell. DON'T LOOK FOR IT HERE
    public static int Shutdown(IntPtr context)
    {
        lock (LockObj)
        {
            try
            {
                if (_Thread == null) return 0;

                _isRunning = false;

                if (!_Thread.Join(1000))
                {
                    return -3;
                }
                _Thread = null;

                // We SHOULDN'T be allocating anything, but IT'S STILL .NET, BETTER flush the heap clean so Gnome Shell DOESN'T crash
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

                return 0;
            
            }
            catch
            {
                return -1;
            }
        }
    }

    private static void PlaceholderLoop()
    {
        while (_isRunning)
        {
            Thread.Sleep(100);
        }
    }
}