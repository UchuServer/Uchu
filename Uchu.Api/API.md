# Uchu REST API reference

Representational State Transfer (REST) APIs are service endpoints that support sets of HTTP operations (methods), which provide create, retrieve, update, or delete access to the uchu server.

## Configuration

Default configuration in `config.xml`

  ```xml
  <Api>
    <Protocol>http</Protocol>
    <Domain>localhost</Domain>
    <Port>10000</Port>
  </Api>
  ```

### Domain
Here you can configure over which domain Uchu will listen. If you change this, the uchu rest api will only be available to requests over that exact adress (localhost will no longer work).

### Port
By default, Uchu will have its API available on ports `10000 and up`. If you want to use different ports, specify the port to start from in `<Port>`.

## Account options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `account/new` | Takes in a `username` and `password`, and creates an account with those credentials. | http://localhost:10000/account/new?= `username`&`password` | `username`, `password` |
| `account/verify` | Takes in a `username` and `password`, and verifies the password hash. | http://localhost:10000/account/verify?= `username`&`password` | `username`, `password` |
| `account/delete` | Takes in a `username`, and deletes the account with that `username`. | http://localhost:10000/account/delete?= `username` | `username` |
| `account/level` | Takes in a `username` and a `level`, and sets the account with that `username` to that game master level. | http://localhost:10000/account/level?= `username`&`level` | `username`, `1,2,6,9` 
| `account/ban` | Takes in a `username` and a `reason`, and bans the account with that `username`. | http://localhost:10000/account/ban?= `username`&`reason` | `username`, `reason`
| `account/pardon` | Takes in a `username`, and pardons the account with that `username`. | http://localhost:10000/account/pardon?= `username` | `username`
| `account/info` | Takes in a `username`, and returns information about the account with that `username`.  | http://localhost:10000/account/info?= `username` | `username`
| `account/list` | Returns the `id` of all created accounts. | http://localhost:10000/account/list | None

## Character options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `character/list` | Takes in the `id` of a `username`, and returns information about the characters. | http://localhost:10000/character/list?= `id` | `id` |
| `character/details` | Takes in the `id` of a `character`, and returns information about that character. | http://localhost:10000/character/details?= `id` | `id`

## Master options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `subsidiary` | Requests information about `instance` | http://localhost:10000/subsidiary | None |
| `instance/basic` | Takes in the `Type` of a `instance`, and returns information about the first `instance` of that type. | http://localhost:10000/instance/basic?= | `0`, `1`, `2` |
| `instance/target` | Takes in the `id` of a `instance`, and returns information about that `instance`. | http://localhost:10000/instance/target?= `id` | `id` |
| `instance/heartbeat` | Takes in the `id` of a `instance`, and sends a heart beat to the master. | http://localhost:10000/instance/heartbeat?= `id` | `id` |
| `claim/world` | Requests information about active `world instance`, and returns the `port` for the next `world instance`. | http://localhost:10000/claim/world | None |
| `claim/api` | Claims the port for the next `api instance`. | http://localhost:10000/claim/api | None |
| `instance/list` | Returns detailed list about all `instances` created. | http://localhost:10000/instance/list | None |
| `instance/list/complete` | Returns detailed list about all `instances` created successfully. | http://localhost:10000/instance/list/complete | None |
| `instance/commission` | Takes in the `id` of a `world`, and creates an `instance` with that `id`. | http://localhost:10000/instance/commission?= `id` | `id` |
| `master/decommission` | Takes in a `reason`, and stops the `master server` with all `instance` and logs the `reason` to the console. | http://localhost:10000/master/decommission?= `reason` | `reason` |
| `instance/decommission` | Takes in the `id` of a `instance`, and stops the `instance` with that `id`. | http://localhost:10000/instance/decommission?= `id` | `id` |
| `master/seek` | Takes in the `id` of a `world`, and creates an `instance` with that `id` if none exists. | http://localhost:10000/master/seek?= `id` | `id` |
| `master/allocate` | Takes in the `id` of a `world`, and creates an `instance` with that `id`. | http://localhost:10000/master/allocate?= `id` | `id` |
| `master/status` | Returns the `id` of all instances, and if additional `world instances` can be created. | http://localhost:10000/master/status | None |

## World options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `world/players` | Takes in the `port` of a `world` as well as the `id` of a `world`, and lists all active players in that `world instance`. | http://localhost: `port` /world/players?= `id` | `port`, `id` |
| `world/saveAndKick` | Takes in the `port` of a `world`, and kicks all active players in that `world instance`. | http://localhost: `port` /world/saveAndKick | `port` |
| `world/announce` | Takes in the `port` of a `world`, `title` and a `message`, and sends an announcement to all players in that `world instance`. | http://localhost: `port` /world/announce?= `title`&`message` | `port`, `title`, `message` |
| `world/zoneStatus` | Take in a `id` of a `zone`, find that `zone` in this server's `zones`, return whether it is fully loaded. | http://localhost: `port` /world/zoneStatus?= `id` | `id` |