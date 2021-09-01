# Uchu [![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2FUchuServer%2FUchu%2Fbadge%3Fref%3Ddev&style=flat&label=build&logo=github )](https://actions-badge.atrox.dev/UchuServer/Uchu/goto?ref=dev) [![Discord](https://img.shields.io/discord/762298384979329114?label=discord&logo=discord&logoColor=white)](https://discord.gg/mrhBXVVNBD)

LEGO® Universe server written in C#

> The LEGO Group has not endorsed or authorized the operation of this game and is not liable for any safety issues in relation to its operation.

## Table of contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Setting up a client](#setting-up-a-client)
   1. [Option A: Nexus LU Launcher](#option-a-nexus-lu-launcher)
   2. [Option B: Manual installation](#option-b-manual-installation)
4. [Setting up a server](#setting-up-a-server)
   1. [Option A: Using a release](#option-a-using-a-release)
   2. [Option B: Building from source](#option-b-building-from-source)
   3. [Advanced configuration (optional)](#advanced-server-configuration-optional)
5. [Getting ready to play](#getting-ready-to-play)
6. [Support](#support)
7. [Python scripting](#python-scripting)
8. [Contributing](#contributing)

## Introduction
Uchu is a server implementation for The LEGO Group's 2010 MMO _LEGO® Universe_, which was shut down in January 2012.

To play LU, a client and a server are needed. The client connects to the server, and the server tells the client what to show and handles combat, NPCs, missions, and a lot more. Uchu is a server; you need the original client too (explained in detail under [Setting up a client](#setting-up-a-client)).

To set up Uchu, you need to be able to navigate to folders using your operating system's terminal emulator. [Here](https://www.watchingthenet.com/how-to-navigate-through-folders-when-using-windows-command-prompt.html) is an introduction for Windows. For macOS, see [this](https://computers.tutsplus.com/tutorials/navigating-the-terminal-a-gentle-introduction--mac-3855) article. Linux users, you probably don't need this, but [here](https://www.redhat.com/sysadmin/navigating-filesystem-linux-terminal) is a guide.

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

There are two ways to go about this. Using Option A, Nexus LU Launcher, is recommended, as it is easier and will let you skip a configuration step when setting up the server later on.

### Option A: Nexus LU Launcher
Nexus LU Launcher is a program that helps you install, configure and launch the LEGO® Universe client.
Find the `.zip` for your operating system on [this page](https://github.com/TheNexusAvenger/Nexus-LU-Launcher/releases/latest), extract it and run the program.

Let it download and extract the client, then when that's finished go to the `Patches` menu and enable **Mod Loader** and **TCP/UDP Shim**.


### Option B: Manual installation
You need to download an **unpacked** client, so that Uchu can use its resources. A list of client downloads is available [here](https://docs.google.com/document/d/1XmHXWuUQqzUIOcv6SVVjaNBm4bFg9lnW4Pk1pllimEg/view). The recommended client is **humanoid/lcdr’s unpacked client**. This is a `.RAR` file; extract it somewhere.

After you've extracted the client, you will need to install a mod to be able to use it with the Uchu server. This mod replaces the original, outdated networking protocol used by the game. Download `mod.zip` from [this page](https://github.com/lcdr/raknet_shim_dll/releases) and extract it in your LU client folder. The result should be that a folder called `mods`, a file called `dinput8.dll` and `legouniverse.exe` are all in the same folder.

If you are on Linux or macOS, you will need [Wine](https://winehq.org) to launch the client. You need to explicitly tell Wine to load the modloader by launching it using `WINEDLLOVERRIDES="dinput8.dll=n,b" wine ./legouniverse.exe`.

## Setting up a server

### Option A: Using a release
Currently not supported. This section will be updated in the future.

### Option B: Building from source
- Install [git](https://git-scm.com/downloads)
- Install [.NET 5.0 SDK](https://dotnet.microsoft.com/download) (for Linux users it will be called `.NET` without `SDK`)

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
cd Uchu.Master/bin/Debug/net5.0
dotnet Uchu.Master.dll
```

The first time you run the server, a configuration file called `config.xml` will be generated.

**If you have installed your client with Nexus LU Launcher,** Uchu will automatically detect its resources location, and no manual setup is required. The server will start, and you can continue to [Getting ready to play](#getting-ready-to-play).

**If you have manually installed your client,** or Uchu can't find the NLUL client's resource folder, open `config.xml` with a text editor. Find this text:
```
<GameResourceFolder>path to res folder</GameResourceFolder>
```
and insert the path to your LEGO® Universe client's `res` folder. For example, on Windows this could be:
```
<GameResourceFolder>C:\Users\Bob\LEGO Universe\res</GameResourceFolder>
```

Now start the server again with `dotnet Uchu.Master.dll`.

### Advanced server configuration _(optional)_
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
If you are interested in contributing code to Uchu,  feel free to join the [community Discord server](https://discord.gg/njjfQ4W6qv) and contact one of the developers to get an invite to the development-focused Discord server.
