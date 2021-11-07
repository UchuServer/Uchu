# Uchu REST API reference

Representational State Transfer (REST) APIs are service endpoints that support sets of HTTP operations (methods), which provide create, retrieve, update, or delete access to the uchu server.

Default configuration in `config.xml`

  ```xml
  <Api>
    <Protocol>http</Protocol>
    <Domain>localhost</Domain>
    <Port>10000</Port>
  </Api>
  ```

## Account options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `account/new` | Takes in a `username` and `password`, and creates an account with those credentials. | http://localhost:10000/account/new?= `username`&`password` | `username`, `password` |
| `account/delete` | Takes in a `username`, and deletes the account with that username. | http://localhost:10000/account/delete?= `username` | `username` |
| `account/level` | Takes in a `username` and a `level`, and sets the account with that username to that game master level. | http://localhost:10000/account/level?= `username`&`level` | `username`, `1,2,6,9` 
| `account/ban` | Takes in a `username` and a `reason`, and bans the account with that username. | http://localhost:10000/account/ban?= `username`&`reason` | `username`, `reason`
| `account/pardon` | Takes in a `username`, and pardons the account with that username. | http://localhost:10000/account/pardon?= `username` | `username`
| `account/info` | Takes in a `username`, and returns information about the account with that username.  | http://localhost:10000/account/info?= `username` | `username`
| `account/list` | Returns a number of all accounts created. | http://localhost:10000/account/list | None

## Character options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `character/list` | Takes in the `id` of a `username`, and returns information about the characters. | http://localhost:10000/character/list?= `id` | `id` |
| `character/details` | Takes in the `id` of a `character`, and returns information about the characters. | http://localhost:10000/character/details?= `id` | `id`

## Master options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `subsidiary` | Requests information about `instance` | http://localhost:10000/subsidiary | None |
| `instance/basic` | xx | http://localhost:10000/instance/basic?= | xx |
| `instance/target` | Takes in `id` of a `instance`, and returns information about the `instance`. | http://localhost:10000/instance/target?= `id` | `id` |
| `instance/heartbeat` | Takes in the `id` of a `instance`, and sends a heart beat to the master. | http://localhost:10000/instance/heartbeat?= `id` | `id` |
| `claim/world` | Returns the `port` for the next `world instance`. | http://localhost:10000/claim/world | None |
| `claim/api` | Claims the port for the next `api instance`. | http://localhost:10000/claim/api | None |
| `instance/list` | Returns detailed list about all `instances` created. | http://localhost:10000/instance/list | None |
| `instance/list/complete` | Returns detailed list about all `instances` created successfully. | http://localhost:10000/instance/list/complete | None |
| `instance/commission` | Takes in the `id` of a `world`, and creates an `instance` with that `id`. | http://localhost:10000/instance/commission?= `id` | `id` |
| `master/decommission` | Stops the `master server` and all `instance`. | http://localhost:10000/master/decommission?= `stopping%20server.` | `stopping%20server.` |
| `instance/decommission` | Takes in the `id` of a `instance`, and stops the `instance` with that `id`. | http://localhost:10000/instance/decommission?= `id` | `id` |
| `master/seek` | Takes in the `id` of a `world`, and creates an `instance` with that `id` if none exists. | http://localhost:10000/master/seek?= `id` | `id` |
| `master/allocate` | Takes in the `id` of a `world`, and creates an `instance` with that `id`. | http://localhost:10000/master/allocate?= `id` | `id` |
| `master/status` | Returns instance `ids` and if additional world instances can be created. | http://localhost:10000/master/status | None |