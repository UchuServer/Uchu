# Releases
This document covers making releases for distribution.

## Windows Warning
The process for creating releases is highly automated and can be run on
any platform. However, while the releases built on macOS/Linux for Windows work
as expected, the releases built on Windows for macOS/Linux require changing
file permissions to run. This is because Linux file permissions don't exist
on Windows and default to not being executable on macOS and Linux. **Publishing
macOS and Linux releases built on Windows is strongly discouraged.**
If you are on Windows, a Linux virtual machine, such as [Windows Subsystem for Linux (WSL)](https://docs.microsoft.com/en-us/windows/wsl/about)
or [Oracle VirtualBox](https://www.virtualbox.org/).

## Setup
The only supported way to create releases for automation tools [like UchuTool](https://github.com/UchuServer/UchuTool)
is using the [`publish.py`](../publish.py) script. In order to use it, 2 programs
need to be installed:
1. [Microsoft .NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) for building releases.
2. [Python 3](https://www.python.org/downloads/) for running the helper script.

See any operating system-specific instructions for setup. No additional setup
of Python libraries is required to run the script. However, you will need to
make sure you have `dotnet` in your `PATH` environment variable. You can check
by opening a command prompt/terminal and entering `dotnet --version`.

## Building Releases
With Python and .NET set up, the recommended process for creating releases is
the following:
1. Open a command prompt or terminal in the root Uchu directory.
2. Run `python publish.py` or `python3 publish.py` depending on your system.

How long it takes depends on your system. At most, it should take a few minutes.
After the script completes, there will be a directory named `bin` created with
at least `Uchu-Windows-x64.zip`, `Uchu-macOS-x64.zip`, and `Uchu-Linux-x64.zip`.
These can be directly used for publishing releases.

## Adding New Runtimes
[.NET supports a lot of runtimes](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.NETCore.Platforms/src/runtime.json),
including x86, x64, ARM, and ARM64 for each of many operating systems.
Adding new operating systems or architectures, such as `linux-arm64` for the
Raspberry Pi 3 and newer, is very easy. In the [`publish.py`](../publish.py),
there is a dictionary named `PLATFORMS`. A new release type can easily
be added by adding a new entry with the first entry being the display version
of the platform and the second entry being the .NET runtime from the supported
list.

For example, adding `linux-ARM64` would look like this:
```python
PLATFORMS = [
    ["Windows-x64", "win-x64"],
    ["macOS-x64", "osx-x64"],
    ["Linux-x64", "linux-x64"],
    ["Linux-ARM64", "linux-arm64"], # Will create a 64-bit ARM release for Linux (linux-arm64) as Uchu-Linux-ARM64.zip
]
```

It is recommended to do the same for [like UchuTool](https://github.com/UchuServer/UchuTool)
as well. Be aware that the [code for downloading releases](https://github.com/UchuServer/UchuTool/blob/master/Uchu.Tool/Action/Update.cs)
may need to be updated depending on how it ends up attempting to find a version to download.
At the time of writing, anything beyond 64-bit x86 (commonly known as x64) is untested,
and releases specifically for Windows/macOS/Linux versions are untested.

If you want to add new runtimes, submit a pull request. Make sure to test the
release on the intended system and include why this new release is useful.