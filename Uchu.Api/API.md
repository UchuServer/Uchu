# Uchu REST API reference

Representational State Transfer (REST) APIs are service endpoints that support sets of HTTP operations (methods), which provide create, retrieve, update, or delete access to the uchu server.

Default configuration in `config.xml`

  ```
  <Api>
    <Protocol>http</Protocol>
    <Domain>localhost</Domain>
    <Port>10000</Port>
  </Api>
  ```

## All available options

| Name | Description | Example | Expected Values |
|-|-|-|-|
| `account/new` | Takes in a `username` and `password`, and creates an account with those credentials. | http://localhost:10000/account/new?=`username`&`password` | `username`, `password` |
| `account/delete` | Takes in a `username`, and deletes the account with that username. | http://localhost:10000/account/delete?=`username` | `username` |
| `account/level` | Takes in a `username` and a `level`, and sets the account with that username to that game master level. | http://localhost:10000/account/level?=`username`&`level` | `username`, `1,2,6,9` 
| `account/ban` | Takes in a `username` and a `reason`, and bans the account with that username. | http://localhost:10000/account/ban?=`username`&`reason` | `username`, `reason`
| `account/pardon` | Takes in a `username`, and pardons the account with that username. | http://localhost:10000/account/pardon?=`username` | `username`
| `account/info` | Takes in a `username`, and returns information about the account with that username.  | http://localhost:10000/account/info?=`username` | `username`
| `account/list` | Returns a number of all accounts created. | http://localhost:10000/account/list | None