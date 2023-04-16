# Advanced server configuration
> ⚠️ Changing any of this is OPTIONAL. The instructions in [README.md](README.md) are all you need to get a server up and running.

## Database
_`<Database>`_  
By default, Uchu uses SQLite to store data. If you already have a PostgreSQL or MySQL/MariaDB server running, you can choose to use this instead of the built-in SQLite.

To set this up, find the `<Database>` section in your config.xml. Options for `<Provider>` are `sqlite`, `postgres` and `mysql`. The rest of the options should be clear. When using SQLite, the rest of the database options are ignored.

## Logging
_`<ConsoleLogging>`, `<FileLogging>`_  
You can configure how much Uchu logs, and where, in the `<ConsoleLogging>` and `<FileLogging>` sections. Available `Level`s are `Debug`, `Information`, `Warning` and `Error`.

## Server script sources
_`<DllSource>`_  
You generally don't need to edit this. Scripts are used to implement non-core game mechanics.Uchu can load more script assemblies (in addition to `Uchu.StandardScripts.dll`), if you've written them. To do so, add a new `<ScriptDllSource>` tag (also see [SCRIPTING.md](Uchu.Python/SCRIPTING.md)).

## Game resources
_`<ResourcesConfiguration>`_  
The path in `<GameResourceFolder>` should point at an unpacked client's `res` folder.

## Networking
_`<Networking>`_
### Ports
Here you can configure which ports Uchu should use. `<CharacterPort>` for the Character server, `<AuthenticationPort>` for the Authentication server (if you change this you need to add it to the client's boot.cfg as well, e.g. `AUTHSERVERIP=0:localhost:25565`).

### Certificate
If you want your server to be publicly accessible (not just from your local PC), you need a domain name and a TLS certificate. The certificate should be a `.pfx` file. Enter the path to the certificate in `<Certificate>`, and the domain in `<Hostname>`.

## Gameplay
_`<GamePlay>`_  
`<PathFinding>` determines if enemies should have movement AI and follow players when attacking them. `<AiWander>` sets whether enemies should also wander around randomly while out of combat. At the moment, Uchu's pathfinding implementation is quite slow, so you might experience issues with it enabled.

Note: if you run Uchu on Linux, you need to have `libgdiplus` installed to use this functionality.

## API ports
_`<Api>`_  
By default, Uchu will have its API available on ports 10000 and up. If you want to use different ports, specify the port to start from in `<Port>`.

## Single Sign-On
_`<Sso>`_  
A Single Sign-On server makes it possible for users to have an account in one single place that enables them to play on all SSO-enabled LU servers. These LU servers then don't need to worry about storing account information and passwords. Uchu supports Single Sign-On with [lcdr's SSO server](https://github.com/lcdr/sso_auth). To use this, set `<Domain>` to the SSO server's domain, and under [`<Networking>`](#networking) set `<HostAuthentication>` to `false`.

## Server behaviour
_`<ServerBehaviour>`_  
If `<PressKeyToExit>` is set to `true`, when the server process exits it will wait for you to press any key. This prevents windows from disappearing too quickly to read an error message.

## Debug options
_`<Debugging>`_  
For builds in Debug mode, if `<StartInstancesAsThreads>` is set to `true`, Uchu will use threads instead of separate processes for the different server instances. This makes it possible to use .NET 6's Hot Reload. Support for this mode isn't perfect; logs will no longer include the bit indicating the log source (`[Char]`, `[Auth]` etc.) and the stopping of world won't entirely work as expected.
