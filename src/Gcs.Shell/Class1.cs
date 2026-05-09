// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using Gcs.Core;

namespace Gcs.Shell;

public static class ExportClasses
{

    [UnmanagedCallersOnly(EntryPoint = "gcs_init")]
    public static int Initialize(IntPtr context)
    {
        return global::Gcs.Core.GcsBootloader.Initialize(context);
    }

    [UnmanagedCallersOnly(EntryPoint = "gcs_restart")]
    public static int Restart(IntPtr context)
    {
        return global::Gcs.Core.GcsBootloader.Restart(context);
    }
}


