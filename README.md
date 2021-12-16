# SharpStudio for Linux

SharpStudio aims to be compliant with both the original MonoDevelop and DotDevelop sources, as a fully featured IDE for .NET using Gtk.

Our goal is to be able to build and debug .NET 6 applications cross-platform.

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mono/monodevelop?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Current Status

Picking up where MonoDevelop left off, the code base is beening refactored to compile/run again on Linux and to work with .NET 6.

### What's Coming Soon

The following commitments are being made:

* Compile .NET 6 on Linux
* Clean out stale branches (_goodbye 388 branches!_)
* Unify coding standards, using spaces instead of tabs.
* Merge in outstanding Pull Requests from MonoDevelop
* Get a .DEB package
* Rebranding Project:
  * SharpIde, SharpStudio, NetDevelop, VisualDevelop, LiteDevelop, LinuxDevelop, CrossDevelop, or VS for Linux
* Ability to build Xamarin.Forms on Linux and (_crossing-fingers_) .NET MAUI
* And more!

## Compiling

### Windows 10

:warning: Currently, it is not building.

### Ubuntu 20.04

The following instructions are what you'll need to build the code from scratch on Ubuntu

```cmd
git clone -b main https://github.com/xenoinc/SharpStudio.git

cd SharpStudio/

git submodule update --init --recursive

sudo apt-get install automake

./configure --profile=gnome

make
```

## NOTE: The ReadMe contents below are outdated

The following sections are from MonoDevelop and will be updated soon.

### Directory organization

There are two main directories:

* `main`: The core MonoDevelop assemblies and add-ins (all in a single
    tarball/package).
* `extras`: Additional add-ins (each add-in has its own
    tarball/package).

### Compiling (full)

If you are building from Git, make sure that you initialize the submodules
that are part of this repository by executing:
`git submodule update --init --recursive`

If you are running a parallel mono installation, make sure to run all the following steps
while having sourced your mono installation script. (source path/to/my-environment-script)
See: [http://www.mono-project.com/Parallel_Mono_Environments]

To compile execute:
`./configure ; make`

There are two variables you can set when running `configure`:

* The install prefix: `--prefix=/path/to/prefix`
  * To install with the rest of the assemblies, use:
  `--prefix="pkg-config --variable=prefix mono"`

* The build profile: `--profile=profile-name`
  * `stable`: builds the MonoDevelop core and some stable extra add-ins.
  * `core`: builds the MonoDevelop core only.
  * `all`: builds everything
  * `mac`: builds for Mac OS X

**PS:** You can also create your own profile by adding a file to the profiles directory containing a list of the directories to build.

Disclaimer: Please be aware that the 'extras/JavaBinding' and 'extras/ValaBinding' packages do not currently work. When prompted or by manually selecting them during the './configure --select' step, make sure they stay deselected. (deselected by default)

### Running

You can run MonoDevelop from the build directory by executing:
`make run`

### Debugging

You can debug MonoDevelop using Visual Studio (on Windows or macOS) with the
`main/Main.sln` solution. Use the `DebugWin32` configuration on Windows and the
`DebugMac` configuration on macOS.

### Installing *(Optional)*

You can install MonoDevelop by running:
`make install`

Bear in mind that if you are installing under a custom prefix, you may need to modify your `/etc/ld.so.conf` or `LD_LIBRARY_PATH` to ensure that any required native libraries are found correctly.

*(It's possible that you need to install for your locale to be
correctly set.)*

### Packaging for OS X

To package MonoDevelop for OS X in a convenient MonoDevelop.app
file, just do this after MonoDevelop has finished building (with
`make`): `cd main/build/MacOSX ; make app`.
You can run MonoDevelop: `open MonoDevelop.app` or build dmg package: `./make-dmg-bundle.sh`

### Dependencies

* [Windows](https://github.com/mono/md-website/blob/gh-pages/developers/building-monodevelop.md#prerequisites-and-source)
* [Unix](http://www.monodevelop.com/developers/building-monodevelop/#linux)

### Special Environment Variables

#### BUILD_REVISION

> If this environment variable exists we assume we are compiling inside wrench.
> We use this to enable raygun only for 'release' builds and not for normal
> developer builds compiled on a dev machine with 'make && make run'.

### Known Problems

> "The type `GLib.IIcon' is defined in an assembly that is not referenced"

This happens when you accidentally installed gtk-sharp3 instead of the 2.12.x branch version.
Make sure to 'make uninstall' or otherwise remove the gtk-sharp3 version and install the older one.

xbuild may still cache a reference to assemblies that you may have accidentally installed into your mono installation,
like the gtk-sharp3 as described before. You can delete the cache in $HOME/.config/xbuild/pkgconfig-cache-2.xml

## Discussion, Bugs, Patches

_Submit bugs and patches:_

[https://github.com/xenoinc/SharpStudio/issues/new]

## References

* [MonoDevelop GitHub](https://github.com/mono/monodevelop)
* [SharpDevelop GitHub](https://github.com/icsharpcode/SharpDevelop)
* [DotDevelop GitHub](https://github.com/dotdevelop)
* [MonoDevelop website](http://www.monodevelop.com)
* [Gnome Human Interface Guidelines (HIG)](https://developer.gnome.org/hig/stable/)
* [freedesktop.org standards](http://freedesktop.org/Standards/)
