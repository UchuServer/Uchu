## Introduction
LU is build with the [Entity–component–system](https://en.wikipedia.org/wiki/Entity_component_system) which you may know from the Unity Engine. In short that means everything that you see on screen (and more) is a GameObject on which Components can be attached. These components give the object functionality like rendering, physics and so on. 

LU uses four different types of servers to communicate. These are: The authentication server (Uchu.Auth), which handles login and redirection to the character server (Uchu.Char). The character server then handles the character selection and redirection to the world server (Uchu.World). Now the world server is the server that you actually play on. It handles everything from movement to chat to missions. There can be multiple World servers running at once but one world server only serves one world. There can however be multiple world servers serving the same world. This happens if the maximum amount of players on a server is reached.

The address of the auth server is configured in the `boot.cfg` file in the installation directory of the client. All other servers can have any address because the previous server sends the address to the client.

Now of course these were only 3 servers but shouldn't there be four servers? - Yes there is also the UGC (User generated content) Server. This server was used for things like icons for modular rockets. As of now the UGC server is not implemented in Uchu.

## The repo structure
As you probably already noticed the Uchu repository is quite complex. Therefore this paragraph will explain what the different folders contain and what they are for.

There are 3 submodules (marked with a blue text color in GitHub) in the repo.
* [InfectedRose](https://github.com/Wincent01/InfectedRose) - A library to work with LEGO® Universe files
* [NexusLogging](https://github.com/TheNexusAvenger/Nexus-Logging) - A logging library for .NET
* [RakDotNet](https://github.com/UchuServer/RakDotNet) - A library for the RakNet protocol that LU uses

Then there are also the folders that contain the actual Uchu code. These are:

* **Uchu.Api** - It contains code for an api with which the Uchu instance can be controlled. There is a [Cli Tool](https://github.com/UchuServer/cli) that uses this api
* **Uchu.Auth** - This contains the auth server
* **Uchu.Char** - This contains the char server
* **Uchu.Core.Test** - Unit Tests for Uchu.Core
* **Uchu.Core** - Contains stuff that is used all over the project, like logging, io, database access, etc
* **Uchu.Instance** - The base for all servers that Uchu runs (like auth, char, world)
* **Uchu.Master** - This contains the main process that starts and controlls all the servers
* **Uchu.Navigation** - Code for pathfinding (used by the world server)
* **Uchu.Physics.Test**- Unit Tests for Uchu.Physics
* **Uchu.Physics** - Code for all kinds of physics calculations (used by the world server)
* **Uchu.Python** - Python interpreter for scripts
* **Uchu.Sso** - Code for lcdr's [Single-Sign-On](https://github.com/lcdr/sso_auth)
* **Uchu.StandardScripts** - C# vserions of server scripts (most of the original lua server scripts are not available)
* **Uchu.World.Test** - Unit Test for Uchu.World
* **Uchu.World** - Code for the world server

If you want to contribute to Uchu most likely Uchu.Wolrd is where you want to make your changes as it contains most of the actual game logic.

## Logging
One of the most important things for debugging is logging stuff into the console. For this you can use The `Logger` class from `Uchu.Core`. It has four logging functions: Debug, Information, Warning and Error. You can also use Log and pass the log level as an argument. 

## World Server
The main entry point for the world server is `WorldUchuServer.cs` but this is probably not very interesting to you. What's more interesting is the Handlers folder. Like many other servers the world server is event driven. That means that it acts whenever it receives packets from a client. For nearly all messages that the client sends there is a handler defined in one of the `Handler` classes which are located in the Handlers folder. A handler class contains handlers for a specific purpose. For example the `InventoryHandler` class handles all inventory related packages. However these clases do not contain that much code. Rather they call functions on the corresponding components. Components as well as GameObjects are defined in the Objects folder and its subfolders. The deffinitions for Packets are stored in the Packets folder.
