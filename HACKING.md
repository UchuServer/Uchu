## Introduction
LU is build with the [Entity–component–system](https://en.wikipedia.org/wiki/Entity_component_system) which you may know from the Unity Engine. In short that means everything that you see on screen (and more) is a GameObject on which Components can be attached. These components give the object functionality like rendering, physics and so on. 

LU used two different types of servers to communicate. These are: 
* The authentication server (Uchu.Auth), which handles login and redirection to the world server. 
* The world server, which handles two things
  * Character selection (Uchu.Char)
  * Main game (Uchu.World)

**Note:** A world server in Uchu only handles one world but there can be multiple world servers in one Uchu instance. They can even serve the same map if one world server has reached the maximum amount of player. 

The address of the auth server is configured in the `boot.cfg` file in the installation directory of the client. All other servers can have any address because the previous server sends the address to the client.

There were also two other server types that aren't implemented in Uchu. These are
* The Chat server which was never used because the world sever handles chat messages
* The UGC (user generated content) server. This handles stuff like custom icons for modular rockets and cars. The UGC Server address is also set in `boot.cfg`

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

If you want to contribute to Uchu most likely Uchu.World is where you want to make your changes as it contains most of the actual game logic.

## Logging
One of the most important things for debugging is logging stuff into the console. For this you can use The `Logger` class from `Uchu.Core`. It has four logging functions: Debug, Information, Warning and Error. You can also use Log and pass the log level as an argument. 

## World Server
The main entry point for the world server is `WorldUchuServer.cs` but this is probably not very interesting to you. What's more interesting is the Handlers folder. Like many other servers the world server is event driven. That means that it acts whenever it receives packets from a client. For nearly all messages that the client sends there is a handler defined in one of the `Handler` classes which are located in the Handlers folder. A handler class contains handlers for a specific purpose. For example the `InventoryHandler` class handles all inventory related packages. However these classes do not contain that much code. Rather they call functions on the corresponding components. Components as well as GameObjects are defined in the Objects folder and its sub-folders. The definitions for Packets are stored in the Packets folder.

### Game Messages
Most of the Packages that the client and world server exchange are Game Messages (GM for short). They're used to communicate all kinds of things. GMs are described in the [lu_packet_structs](https://docs.google.com/document/d/1v9GB1gNwO0C81Rhd4imbaLN7z-R0zpK5sYJMbxPP3Kc) document and in lcdr's [lu_packets](https://lcdruniverse.org/lu_packets/lu_packets/).

Uchu stores the struct definitions for Game Messages in `Packets/GameMessages`. There are two sub-folders here. One for messages sent by the client and one for messages sent by the server. The code for them looks like this (EquipItemMessage as an example)

```c#
namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct EquipItemMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.EquipInventory;
        public bool IgnoreCooldown { get; set; }
        public bool OutSuccess { get; set; }
        public Item Item { get; set; }
    }
}
```
The first two Properties (`Associate` and `GameMessageId`) are always present. Uchu will automatically create GM structs when it receives a GM based on `GameMessageId`. The `GameMessageId` type is an enum with an integer parameter that is defined in the `Enums` folder.

Have a look at some GM structs if you want to learn more.

### Handlers
Handlers for Game Messages are defined in `Uchu.World.Handlers.GameMessages`.

```c#
namespace Uchu.World.Handlers.GameMessages
{
    public class MyHandler : HandlerGroup
    {
        [PacketHandler]
        public void MyNewHandler(GameMessageToHandle message, Player player)
        {
            // Do stuff
        }
    }
}
```

As you can see all handlers are inside a class that extends `HandlerGroup`. The actual handling functions are annotated with `PacketHandler` and should always contain two arguments: The message to handle and the player that the message came from.

Uchu will automatically find the right handler for a certain Game Message and invoke it so just have to define it.

### Sending Game Messages
To send a Game Message you need access to a player object which you can send the message to. Just create a new Game Message with all the arguments and call `Message` with it.

```c#
player.Message(new PlayerReadyMessage
{
    Associate = player
});
```

### Components
Components don't exist by themselves. They are always attached to a GameObject. You can get components from a GameObject by using one of those functions:
* `GetComponent(Type type)`
* `GetComponents(Type type)`
* `GetComponent<T>()`
* `GetComponents<T>()`
* `GetAllComponents()`
* `TryGetComponent(Type type, out Component result)`
* `TryGetComponents(Type type, out Component[] result)`
* `TryGetComponent<T>(out T result)`
* `TryGetComponents<T>(out T[] result)`

The TryGet functions return a bool that says if a component of the requested type is present. Every component has a reference to it's GameObject, called `GameObject` that you can use to get access to other components.
