# Uchu [![appveyor](https://ci.appveyor.com/api/projects/status/of25uoxf8um5ejc3?svg=true)](https://ci.appveyor.com/project/jettford/uchu) [![discord](https://img.shields.io/discord/762298384979329114?label=Discord&logo=discord&logoColor=white)](https://discord.gg/mrhBXVVNBD)

LEGO Universe server written in C#

## Disclaimer
> The LEGO Group has not endorsed or authorized the operation of this game and is not liable for any safety issues in relation to its operation.

## Contributions
Contributions are always welcome! Feel free to open pull requests or issues to help with the continued development of Uchu.

## Python scripting
Uchu supports [Python scripting](https://github.com/UchuServer/Uchu/blob/master/Uchu.Python/SCRIPTING.md), which you can use to code minigames, new game features, and a lot more! This is the perfect way for those of you with less programming experience to contribute.

## Releases
Check out the [release page](https://github.com/UchuServer/Uchu/releases) for standalone binary releases of Uchu.

## Prerequisites
This project will be built and run using a CLI. You will also require a text editor capable of editing raw text files. These are requirements of setting up this project so if you are not familiar with either it is recommended to research them in advance.
Users should also understand that Uchu is not a game in and of itself, but a server which tells a program on your computer - the client - what to do.
Uchu is built with .NET 5.0 (see below for installation details) which is compatible with Windows, Linux, and MacOS.

### .NET 5.0
Install .NET 5.0 SDK for your OS using the instructions found on [here](https://dotnet.microsoft.com/download/dotnet/5.0).

### Database
Uchu uses SQLite as its database provider by default. This can be changed in the config file to either MySQL or PostgreSQL.

### Redis (optional)
Uchu uses Redis as its Cache service provider. If you decide to skip this step, the server will fall back to the database for caching. The latest version of Redis is only natively supported on Linux and MacOS so setting it up on Windows requires some workarounds.

Recommended to install when setting up for hosting.

#### Linux (Debian/Ubuntu)
```
sudo apt install redis-server
```

#### MacOS
```
brew install redis
```

##### Windows
There's a [package on chocolatey](https://chocolatey.org/packages/redis-64), although fairly outdated and may cause issues.

### LEGO Universe Client
You can find a list of available clients [here](https://docs.google.com/document/d/1XmHXWuUQqzUIOcv6SVVjaNBm4bFg9lnW4Pk1pllimEg), it is recommended you download humanoid/lcdr's **unpacked** client.

Uchu does not contain all of the information the server needs to run LEGO Universe and requires resources from the client in order to run. If you do choose a packed client, you will have to unpack the files yourself using [lcdr's utilities](https://github.com/lcdr/utils).

### TcpUdp Mod
The underlying network library this server (now) uses does not have support for the original RakNet protocol the game used. Because of this you will have to download [this client mod](https://github.com/lcdr/raknet_shim_dll/releases) made by lcdr and extract it next to the game's executable.

There are several reasons for not supporting the original protocol anymore with the major one being security, if you would like to get more info please contact us on discord (or via mail if that's your thing).

#### Linux and MacOS
Unlike on Windows, Wine does not automatically load dll files placed in the same directory as the executable. You will have to run the client like this in order for it to use the shim's dinput8:

```sh
WINEDLLOVERRIDES="dinput8.dll=n,b" wine ./legouniverse.exe
```

### Building from source

#### Git

##### Linux
```
sudo apt install git
```
##### MacOS
Included in macos dev tools, running `git` from a terminal should prompt you to install them.

##### Windows
You can download git from [the official website](https://git-scm.com/).

#### Clone the repository
You can append `--depth 1` to the following command if you don't care about commit history and/or have slow internet.

```
git clone https://github.com/UchuServer/Uchu --recursive -b dev
```

#### Building
Run the following command in the project root directory (where Uchu.sln is located)
```
dotnet build
```

### Configuration

#### Config file
Before we can configure the server, it needs to generate a config file.
1. Navigate to where you built the Uchu.Master project. This is commonly `bin/Debug/net5.0/`, relative to the Uchu.Master path, but may differ.
2. Run `dotnet Uchu.Master.dll`. This will throw errors.
3. Close the process when it says it has generated a default config file.

#### Configure Servers
Uchu is not a LEGO Universe repository and you will have to supply your own game resource files for the servers to work with.

1. Navigate to where you have LEGO Universe installed and copy the full path to the `res` directory.
2. Find the `config.default.xml` file generated earlier and rename it to `config.xml`
3. Open the config, go to `<GameResourceFolder></GameResourceFolder>` and paste your LEGO Universe's `res` directory path between the tags.

No quotation marks (`""`) should be used.

##### Database

You should check if you've filled in the right database credentials in the config file, it will not run without a database connection.

##### Define Uchu.Instance and Uchu.StandardScripts
You have to tell Uchu where it can find the different libraries it will utilize at runtime.

1. Open the config file and find the `<DllSource></DllSource>` section.
2. If `dotnet` is not accessible as a global command, copy the path to `dotnet(.exe)` in between the `<DotNetPath></DotNetPath>` tags.
3. Copy the path to Uchu.Instance.dll in between the `<Instance></Instance>` tags. This is commonly `/bin/Debug/net5.0`, relative to the Uchu.Instance path, but may differ.
4. Copy the path to Uchu.StandardScripts.dll in between the `<ScriptDllSource></ScriptDllSource>` tags. This is commonly `/bin/Debug/net5.0`, relative to the Uchu.StandardScripts path, but may differ.

No quotation marks (`""`) should be used.

##### Network ports (optional)
If your operating system does not allow you to host a server on any specific network port, or you run other services that might occupy any of the network ports used by Uchu, you can change the config to bind to different ports.

###### Servers
1. Open the config file and find the `<Networking></Networking>` section.
2. To rebind the character network port, add a xml element like this (for network port 4000), `<CharacterPort>4000</CharacterPort>`.
3. World servers will incrementally bind to network 2003(+). This might not be feasible for when you have to port-forward for every world server. You can therefore add ANY NUMBER of xml element like this (for network port 10000), `<WorldPort>10000</WorldPort>`, to tell Uchu where it can bind world servers.

**IMPORTANT!** If Uchu runs out of specified world ports, additional world servers will **not** work.

###### API
1. Open the config file and find the `<Api></Api>` section.
2. APIs will incrementally bind to network 10000(+) by default. This can be changed by setting the number in the `<Port></Port>` tags.

### Infrastructure

#### Single-Sign-On authentication
If you are using a Single-Sign-On (SSO) authentication server (https://github.com/lcdr/sso_auth), you need to specify the domain it is located on.
1. Open the config file and find the `<Sso></Sso>` section.
2. Input the domain where the SSO authentication server is hosted in between the `<Domain></Domain>` tags.
3. Set the `<HostAuthentication></HostAuthentication>` variable in the `<Networking></Networking>` section to `false`.

#### Creating a user
In the Uchu.Master console, type `/adduser <username>` and press enter. You will than have to enter a password which will be displayed as stars (\*), and when you are done, press enter.

#### Admin commands
In the Uchu.Master console, type `/gamemaster <username> 2`. This will make your account an Admin and will give you access to a lot of commands. Type "/" using the ingame chat to have your options displayed to you.

#### Experimental features
These features are experiential and may be unstable. These can be enabled under the `<GamePlay></GamePlay>` section in the config file.

### Troubleshooting

#### Old config file
If you have used an older version of Uchu, you might have to reset your config file to accomodate new additions. Do this by deleting your old config file and have the server generate a new one.

#### Code does not compile
Make sure the `RakDotNet/` and `InfectedRose/` directories are not empty. If they are you have to pull those submodules with git.

If they are not empty but the code still does not compile, they might be out of date. Pull those submodules with git to update them.

#### Nothing happens when I run Uchu.Master.dll
If the server did not output a line saying the api is ready, attempt to rebind the API port. See Network ports -> API

#### The server is saying `Invalid local resources (Invalid path or no .luz files found). Please ensure you are using an unpacked client.`
Please ensure you are using an unpacked client. See Prerequisite -> LEGO Universe Client

#### Two windows open and immediately close
Attempt to specify a different character port. See Network ports -> Servers.
If that doesn't work double check that the Uchu.Instance and Uchu.StandardScripts paths are correct. See Configuration -> Define Uchu.Instance and Uchu.StandardScripts.

#### Cannot load into world
Attempt the specify a couple of world server ports. See Network ports -> Servers

#### Cannot create a character
Make sure the `CDClient.db` is not corrupted. You might have to manually copy it to the output directory from Uchu.Core.

#### Cannot progress
Uchu is far from complete, and a lot of features just are not implemented. You can run the `/complete` command in chat to complete all your active missions.
