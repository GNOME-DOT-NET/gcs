# GCS (GNOME C-Sharp)
**Alpha - Active development. Not yet suitable for production use.**

GCS is a native C# runtime for GNOME Shell, replacing GJS and its JavaScript engine with a NativeAOT-based execution model. It embeds directly into GNOME Shell and exposes C# bindings generated from GIR files.

GCS is not a drop-in replacement for GJS. Extensions must be rewritten in C# and compiled to native `.so` shared libraries. In exchange, the entire desktop stack runs without a JavaScript runtime and .NET runtime dependency.

## Who is GCS for?

**Extension developers** who want to write GNOME Shell extensions in C# instead of JavaScript, and are comfortable with a native compilation toolchain.

**Distribution maintainers** who want a more predictable, fully native desktop stack without a runtime scripting dependency.

**End users** who want a GNOME desktop running on a native execution model. GCS is not yet installable, this will change as the project matures.

## How it works

GCS generates C# bindings from GIR introspection files, then compiles the GNOME Shell runtime to a native binary using .NET NativeAOT. There is no .NET runtime at execution time.

Extensions are written in C# and compiled separately to native `.so` shared libraries, which GCS loads at runtime via `dlopen`. This preserves GNOME's dynamic extension loading model while keeping the runtime fully native.

## Requirements

> [!CAUTION]
> **Build Requirements & Dependency Map**
>
> You need GNOME 48, Debian 13 Trixie (stable) or a Debian-based derivative.
> GCS requires low-level access to the GNOME stack. Missing headers will cause Source Generator failure.
> Ensure you have the following tree (or equivalents) installed:
>
> **Quick Install (Debian/Ubuntu)**
> ```bash
> sudo apt install build-essential clang zlib1g-dev libicu-dev pkg-config \
> libglib2.0-dev libgirepository-1.0-dev libcairo2-dev libpango1.0-dev \
> libgraphene-1.0-dev libgdk-pixbuf-2.0-dev libgtk-4-dev libadwaita-1-dev \
> libmutter-16-dev libclutter-1.0-dev libcogl-dev libatk1.0-dev \ libdbus-1-dev libxml2-utils
>```

## Writing extensions

Extensions are written in C# and compiled to native `.so` libraries using the .NET SDK with NativeAOT support. GCS loads them at runtime via `dlopen`.

**Requirements for extension developers:**

- .NET SDK with NativeAOT toolchain

- Familiarity with C# and native interop

- Adherence to the Gcs.Generator and Gcs.Bindings

> **Note:** There is currently no official SDK, project templates, or scaffolding tooling for extensions. This is planned for future releases.

## Status
GCS is in **alpha**. Core infrastructure is being written. Nothing is installable or runnable yet.

**Project Roadmap**
- [x] **Architecture defined**
- [x] **Gcs.Core: NativeAOT Bootloader** – Configuring the project as `NativeLibrary` and implementing `[UnmanagedCallersOnly]` for `gcs_init`.
- [x] **Gcs.Core: GLib MainLoop** – Mapping `g_main_context` and taking control of the Shell event loop directly in C#.
- [ ] **Gcs.Generator: GIR XML Parser** – Engine parsing `.gir` files from /usr/share/gir-1.0/ to extract structures, methods, and signals.
- [ ] **Gcs.Generator: Type Mapping & Bridge** – Building a type translator (C → .NET) and `[LibraryImport]` attribute generator.
- [ ] **Gcs.Bindings:** – Generating mappings for the stack: 
  - [ ] GLib-2.0, GObject-2.0, GModule-2.0, Gio-2.0 **(Fundamentals)**
  - [ ] Atk-1.0, Pango-1.0, Cairo-1.0, GdkPixbuf-2.0, Graphene-1.0 **(Graphic)**
  - [ ] Cogl-16, CoglPango-16, Clutter-16 **(Rendering)**
  - [ ] Meta-16, Shell-0.1, St-1.0 **(Shell)**
  - [ ] Gtk-4.0, Adw-1 **(UI)**
- [ ] **GNOME Shell embedding** – Forking `gnome-shell`: modify `main.c`, remove GJS and add `dlopen` for `libgcs.so`.
- [ ] **Build pipeline** – Build automation: Generator → Bindings → Core → Final `.so`.
- [ ] **Gcs.Shell: Resource Embedder** – Packaging `.ui` (XML) files as `EmbeddedResource` inside a binary library.
- [ ] **Gcs.Shell: UI Component Logic** – Implementation of UI component logic classes managing native widgets.
- [ ] **Gcs.Services: Native DBus Services** – Implementation of Dbus interfaces
- [ ] **Extension loading via dlopen** – Mechanism for dynamically loading extensions in the form of external `.so` libraries.
- [ ] **First working shell session** – Starting a GNOME session driven by a C# binary instead of JavaScript.
- [ ] **Extension developer tooling** – `GCS-CLI`: Tool for creating templates and building native extensions.

## License

[0BSD](https://github.com/GNOME-DOT-NET/gcs/blob/main/license.txt)
