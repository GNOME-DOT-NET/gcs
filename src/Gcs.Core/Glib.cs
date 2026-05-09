// -*- mode: csharp; c-basic-offset: 4; indet-tabs-mode: nil; -*- */
// vim: set expandtab shiftwidth=4 tabstop=4:

// SPDX-License-Identifier: 0BSD
// Copyright (C) 2026 Adam Plejznerowski <adamplejznerowski@gmail.com>

using System.Runtime.InteropServices;

namespace Gcs.Core;

    internal static partial class GLib 
    {
        public delegate bool GSourceFunc(IntPtr data);

        [LibraryImport("glib-2.0")]
        public static partial void g_main_context_invoke(IntPtr context, GSourceFunc function, IntPtr data);

        [LibraryImport("glib-2.0")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool g_main_context_iteration(IntPtr context, [MarshalAs(UnmanagedType.Bool)] bool mayBlock);

        [LibraryImport("glib-2.0")]
        public static partial IntPtr g_main_context_default();

        [LibraryImport("glib-2.0")]
        public static partial void g_main_context_unref(IntPtr context);

        [LibraryImport("glib-2.0")]
        public static partial void g_main_context_ref(IntPtr context);

    }


