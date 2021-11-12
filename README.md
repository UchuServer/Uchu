# Uchu [![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2FUchuServer%2FUchu%2Fbadge%3Fref%3Ddev&style=flat&label=build&logo=github )](https://actions-badge.atrox.dev/UchuServer/Uchu/goto?ref=dev) [![Discord](https://img.shields.io/discord/762298384979329114?label=discord&logo=discord&logoColor=white)](https://discord.gg/mrhBXVVNBD)

LEGO® Universe server written in C#

> The LEGO Group has not endorsed or authorized the operation of this game and is not liable for any safety issues in relation to its operation.

## Table of contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Setting up a client](#setting-up-a-client)
4. [Setting up a server](#setting-up-a-server)
   1. [Option A: Using a release](#option-a-using-a-release)
   2. [Option B: Building from source](#option-b-building-from-source)
   3. [Configuration](#configuration)
5. [Getting ready to play](#getting-ready-to-play)
6. [Support](#support)
7. [Python scripting](#python-scripting)
8. [Contributing](#contributing)

## Introduction
Uchu is a server implementation for The LEGO Group's 2010 MMO _LEGO® Universe_, which was shut down in January 2012.

To play LU, a client and a server are needed. The client connects to the server, and the server tells the client what to show and handles combat, NPCs, missions, and a lot more. Uchu is a server; you need the original client too (explained in detail under [Setting up a client](#setting-up-a-client)).

## Features
### Core
- Missions & achievements
- Smashables
- Quickbuilds
- Rocket building, launchpads and world teleporters
- Enemies (movement AI is slow; disabled by default)
- Vendors
- Items and skills
- Death planes
- Factions
- Vault
- Moving platforms

### Minigames
- Avant Gardens Survival
- Footraces
- Monument Race
- Combat Challenge
- Leaderboards for the above, where applicable

## Setting up a client
You need to download an **unpacked** client, so that Uchu can use its resources. A list of client downloads is available [here](https://docs.google.com/document/d/1XmHXWuUQqzUIOcv6SVVjaNBm4bFg9lnW4Pk1pllimEg/view). The recommended client is **humanoid/lcdr’s unpacked client**. This is a `.RAR` file; extract it somewhere.

After you've extracted the client, you will need to install a mod to be able to use it with the Uchu server. This mod replaces the original, outdated networking protocol used by the game. Download `mod.zip` from [this page](https://github.com/lcdr/raknet_shim_dll/releases) and extract it in your LU client folder. The result should be that a folder called `mods`, a file called `dinput8.dll` and `legouniverse.exe` are all in the same folder.

If you are on Linux or macOS, you will need [Wine](https://winehq.org) to launch the client (macOS 10.15 and later: use [this version](https://github.com/Gcenx/homebrew-wine), which has the 32-bit support you need for LU). You need to explicitly tell Wine to load the modloader by launching it using `WINEDLLOVERRIDES="dinput8.dll=n,b" wine ./legouniverse.exe`.

## Setting up a server
You can either use a release or build from source. Using a release is recommended for most users, as it is far easier than manually building from source.

### Option A: Using a release
- Download and run [Uchu Tool](https://github.com/UchuServer/UchuTool/releases/latest)
- Set the client resources path in `config.xml` as described under [Configuration](#configuration), and then run Uchu Tool again.

Whenever you run Uchu Tool, it will automatically check for updates and (when applicable) offer to install them for you.

### Option B: Building from source
In this section it is assumed that you are familiar with your operating system's terminal emulator, and know how to use it to navigate to folders and run files.

- Install [git](https://git-scm.com/downloads)
- Install [.NET 6.0 SDK](https://dotnet.microsoft.com/download) (for Linux users it will be called `.NET` without `SDK`)

**Clone the repository:**
```bash
git clone https://github.com/UchuServer/Uchu --recursive
```
Make sure you include the `--recursive` part. If you forgot to, then type `git submodule init` and `git submodule update`.

**Build the project:**
```bash
cd Uchu
dotnet build
```

**Start the server:**
```bash
cd Uchu.Master/bin/Debug/net6.0
dotnet Uchu.Master.dll
```

The first time you run the server, a configuration file called `config.xml` will be generated.

### Configuration
Open `config.xml` with a text editor. Find this text:
```xml
<GameResourceFolder>path to res folder</GameResourceFolder>
```
and insert the path to your LEGO® Universe client's `res` folder. For example, on Windows this could be:
```xml
<GameResourceFolder>C:\Users\Bob\LEGO Universe\res</GameResourceFolder>
```

Now start the server again with `dotnet Uchu.Master.dll`.

To update the server with the latest changes, navigate to Uchu's top-level directory (which is where you ran `dotnet build`, and where `Uchu.sln` is located), and run:
```bash
git pull
dotnet build
```

#### Advanced server configuration _(optional)_
**Only needed for advanced use cases:** see [this document](Configuration.md) for an explanation of all available configuration options.

## Getting ready to play
When you've got your server up and running, it's time to create a user account. If you're on Windows, find the window titled Authentication. On Linux/macOS, you just need the one window in which the server is running.

Type `/adduser <username>` and press enter to create a user (don't include the `<>`). Uchu will prompt you for a password. You can set your permissions using `/gamemaster <username> <level>`. The highest level available is **9**.

There are in-game commands available that are useful to know, such as `/complete` (completes active missions), `/smash` (you'll respawn at a safe location), and `/fly` (...it lets you fly). Type `/help` in the in-game chat for a complete list. You can also type this in the Uchu console for a list of console commands.

## Support
If you encounter issues with the installation process, take a moment to re-read all instructions carefully, and if you're still stuck you are welcome to [join our Discord server](https://discord.gg/njjfQ4W6qv) and ask your question in the `#help` channel.

## Python scripting
Uchu supports Python scripting, which allows you to make minigames and other server additions with Python. See [this document](Uchu.Python/SCRIPTING.md) to get started.

## Contributing
Contributions are always welcome! If you encounter an issue that isn't logged on the [issue board](https://github.com/UchuServer/Uchu/issues) yet, feel free to add it. And, of course, you're more than welcome to open a pull request to fix it :)  
If you are interested in contributing code to Uchu, feel free to join the [community Discord server](https://discord.gg/njjfQ4W6qv) and contact one of the developers to get an invite to the development-focused Discord server.  
See [HACKING.md](HACKING.md) to learn more about developing Uchu.
