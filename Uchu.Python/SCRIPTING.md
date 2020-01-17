# Scripting
Uchu has two scripting systems. These work as separate systems and serve different roles on the server.

## Python (Recommended)
Python is a higher level language which makes it fit the role of the primary user scripting language for Uchu.

### Python commands
* `/python-load <relative/path/to/script.py>` to load a script.
* `/python-unload <name>` to unload a running script. The script can hereafter be loaded again with changes.
* `/python-list` to list all running scripts.
* `/python <name> <...code...>` to run python code right through the game's chat window. `\n` and `\t` will be replaced with their character counterparts.

### Python scripting
A basic understanding of Python is recommended.

This document will not include every feature or concept in Uchu, only the most prevalent once.
Uchu's Python scripting is built with IronPython, which allows you to fully interact with the Uchu and .NET Core ecosystem.
So there are no limits to what can be scripted here.

#### WARNING: Do not run scripts you don't understand!
Scripts can do everything C# could, which means they could download stuff, install software, etc. So if you're given a script from someone,
make sure you understand what it does before loading it.

<hr>

#### Functions
Define these in your scripts and the server will call them at predictable times. These should normally act as the start point of your scripts.
```python
def load(): # called once when the script is loaded.

    # Setup the script here.

    pass

def tick(): # called every server tick cycle. The server aims for this to be 20 times per second.

    # Preform operations which should happen regularly here.

    pass

def unload(): # called once when the script is going to be unloaded.

    # Clean up everything related to the script here.

    pass
```

#### Server interactions
Uchu provides multiple resources to your scripts which they can utilize to interact with the server.

<hr>

##### Global Events
* `OnStart(Object, function)` binds the function to being called when the Object is started.
* `OnDestroy(Object, function)` binds the function to being called when the Object is destroyed.
* `OnTick(Object, function)` binds the function to being called every server tick on the Object.
* `OnInteract(GameObject, function(player))` binds the function to being called when the GameObject is interacted with by a Player.
* `OnHealth(GameObject, function(health, delta, source-game-object))` binds the function to being called when the GameObject's health is changed.
* `OnArmor(GameObject, function(armor, delta, source-game-object))` binds the function to being called when the GameObject's armor is changed.
* `OnDeath(GameObject, function(smasher-game-object))` binds the function to being called when the GameObject is smashed.
* `OnChat(function(player, message))` binds the function to being called when a Player sends a chat message.

<hr>

##### Global Methods
* `Drop(lot, position, source, owner)` drops an item for a player to pick up.
* `Currency(count, postion, source, owner)` drops coin for a player to pick up.
* `Chat(player, message)` sends a chat message to a player.
* `Broadcast(message)` broadcasts a chat message to every player in the zone.
* `Distance(vector_0, vector_1)` gets the distance between two Vector3 variables.

<hr>

##### Global Properties

* `Layer` includes the standard layers included in calculating what objects should be shown to the player in the game world.
The bit operations on this mask are overwritten: `+=` adds a layer, `-=` removes a layer. `=` always sets the layer.
    * `Layer.None = 0`
    * `Layer.Default = 1`
    * `Layer.Environment = 1 << 1`
    * `Layer.Npc = 1 << 2`
    * `Layer.Smashable = 1 << 3`
    * `Layer.Player = 1 << 4`
    * `Layer.Enemy = 1 << 5`
    * `Layer.Spawner = 1 << 6`
    * `Layer.Hidden = 1 << 7` this layer should be used to hide objects from the players.
    * `Layer.All = (64-bit max value)`
<hr>

##### Zone
The `Zone` variable is set to the zone on which this script runs.

###### Properties
* `Zone.Objects` gets all the Objects in the zone, including both GameObjects and Players. (Readonly)
* `Zone.GameObjects` gets all the GameObjects in the zone, including Players. (Readonly)
* `Zone.Players` gets all the players in the zone. (Readonly)
* `Zone.DeltaTime` is calculated as `ms-passed-during-last-server-cycle / 1000`. Can be used to even out stuff like movement. (Readonly)
* `Zone.ZoneId` gets the id of the zone. (Readonly)

###### Example
Loop through all players in the zone and give toss them a coin.
```python
def give_coins():
    for player in Zone.Players:
        player.Currency += 1
```

<hr>

##### Object
A Object is any object managed by the zone.

###### Methods
* `Start(Object)` makes the zone start managing this object. Call this on any new object when you're done setting it up. Will invoke the `OnStart` event.
    ###### Example
    ```python
    def new_game_object(lot, x, y, z):
        game_object = Create(lot, Vector3(x, y, z), Quaternion(0, 0, 0, 1))
  
        Start(game_object)
        Construct(game_object)
  
        return game_object
    ```
* `Destroy(Object)` removes this object from the zone's management. Will invoke the `OnDestroyed` event.
    ###### Example
    ```python
    def destroy_object(game_object):
        Destroy(game_object)
    ```

<hr>

##### GameObject : Objects
A GameObject is any object present in the game world, visible or not.

###### Properties
* `GameObject.Name` gets the name of this GameObject. For example the name of a Player.
* `GameObject.ObjectId` gets the ObjectId of this GameObject. (Readonly)
* `GameObject.Lot` gets the Lot of this GameObject. (Readonly)
* `GameObject.Tranform` gets the Transform component on this GameObject. (Readonly)
* `GameObject.Aline` is `False` if this GameObject is no longer in the Zone. (Readonly)
* `GameObject.Viewers` gets all the Players that has this GameObject loaded. (Readonly)
* `GameObject.Layer` gets or sets the layer(s) this GameObject occupies. This determines if the GameObject should be shown to the Players or not.
    ###### Example
    ```python
    def vanish(game_object):
        old_layer = hide_object(game_object)
    
        time.sleep(10)
  
        restore_object(game_object, old_layer)

    def hide_object(game_object):
        old_layer = game_object.Layer
      
        game_object.Layer = Layer.Hidden
  
        return old_layer
  
    def restore_object(game_object, layer):
        game_object.Layer = layer
    ```

###### Methods
* `Create(Lot, Vector3, Quaternion)` gets a new GameObject from a Lot and place on world.
* `Construct(GameObject)` adds this GameObject in game world.
    ###### Example
    ```python
    def new_game_object(lot, x, y, z):
        game_object = Create(lot, Vector3(x, y, z), Quaternion(0, 0, 0, 1))
  
        Start(game_object)
        Construct(game_object)
  
        return game_object
    ```
* `Serialize(GameObject)` updates this GameObject in the game world. Should be called when you make a change to the GameObject.
    ###### Example
    ```python
    def move_game_object(game_object, x, y, z):
        game_object.Transform += Vector3(x, y, z)
        
        Serialize(game_object)
    ```
* `Destruct(GameObject)` removes this GameObject in the game world.
(This is a lower level function in Uchu. Use layers to hide GameObjects instead.)
* `GetComponent(GameObject, Type)` gets a component on this GameObject by its type name. Returns `None` if invalid type or not found.
    ###### Example
    ```python
    def remove_health(game_object, health):
        stats = GetComponent(game_object, "Stats")
        stats.Health -= health
    ```
* `AddComponent(GameObject, Type)` adds a component to this GameObject by its type name. Returns `None` if invalid type. (Limited)
* `RemoveComponent(GameObject, Type)` removes a component from this GameObject by its type name. (Limited)

<hr>
 
##### Player : GameObject
A Player in the game world.
 
###### Properties
* `Player.Currency` gets or sets the amount of coin this Player has.
* `Player.UniverseScore` gets or sets the amount of universe score this Player has.
* `Player.Level` gets or sets the level of this Player. This will set `Player.UniverseScore` to the score required for this level.
* `Player.Perspective` gets the controller for what this Player can see in the game world. (Readonly)

<hr>

##### Perspective
A controller for what a Player can see in the game world.

###### Properties
* `Perspecive.LoadedObjects` gets all the GameObjects this player views in the game world. (Readonly)
* `Perspecive.MaskFilter` gets the filter handling masking. (Readonly)

<hr>

##### MaskFilter
The filter handling the Player mask, or in other words, what layers this player will view in the game world.

###### Properties
* `MaskFilter.ViewMask` gets or sets the bitmap of layers this player will view (64-bit). The bit
operations on this mask are overwritten: `+=` adds a layer, `-=` removes a layer.

###### Example
`````python
def hide_smashables(player):
    perspective = player.Perspective

    filter = perspective.MaskFilter

    filter.ViewMask -= Layer.Smashable

def view_smashables(player):
    perspective = player.Perspective

    filter = perspective.MaskFilter

    filter.ViewMask += Layer.Smashable
`````

<hr>

##### Component : Object
A Component attached to a GameObject.

###### Properties
* `Component.GameObject` gets the GameObject this Component is attached to. (Readonly)
* `Component.Transform` gets the Transform component attached to the GameObject this Component is attached to. (Readonly)

<hr>

##### Transform : Component
A Component that holds positional, rotational, and hierarchical information about a GameObject.

###### Properties
* `Transform.Position` gets or sets the position of the GameObject.
    ###### Example
    ```python
    def move_game_object(game_object, x, y, z):
        vector = Vector3(x, y, z)
        
        game_object.Transform.Position += vector
        
        Serialize(game_object)
    ```
* `Transform.Rotation` gets or sets the rotation of the GameObject. This is a Quaternion.
* `Transform.EulerAngles` gets or sets the euler angles.
* `Transform.Scale` gets or sets the scale of the GameObject. Cannot be updated in the game world once constructed.
* `Transform.Parent` gets or sets the Transform the GameObject is a child to.
* `Transform.Children` gets all the children of this Transform. (Readonly)

###### Methods
* `Transform.Translate(Vector3)` moves the GameObject in the game world.
* `Transform.Rotate(Quaternion)` rotates the GameObject in the game world.

<hr>

##### InventoryManagerComponent : Component
A Component responsible for managing Player's inventory.

###### Methods
* `InventoryManagerComponent.FindItem(Lot)` returns the first item stack which has that Lot. `None` if none found.
* `InventoryManagerComponent.FindItems(Lot)` returns all the item stacks which has that Lot.
* `InventoryManagerComponent.AddItemAsync(Lot, Count)` adds Count amount of items of Lot to the Player's inventory.
    ###### Example
    ```python
    def give_item_to_players(lot):
        for player in Zone.Players:
            inventory = GetComponent(player, "InventoryManagerComponent")

            inventory.AddItemAsync(lot, 1)
    ```
* `InventoryManagerComponent.RemoveItemAsync(Lot, Count)` removes Count amount of items of Lot to the Player's inventory.

<hr>

##### Item : GameObject
An item stack in a Player's inventory.

###### Properties
* `Item.ItemComponent` gets the client's item information on for this object. (Readonly)
* `Item.Inventory` gets the Inventory this Item is in. (Readonly)
* `Item.Player` gets the Player this Item belongs to. (Readonly)
* `Item.Count` gets or sets the amount of items in this stack. You should normally use `InventoryManagerComponent.AddItemAsync(Lot, Count)` to add items.

###### Methods
* `Item.EquipAsync()` equips this item onto the Player.
* `Item.UnEquipAsync()` un-equips this item from the Player.

<hr>

##### Stats : Component
A Component responsible for keeping managing a GameObject's stats.

###### Properties
* `Stats.Health` gets or sets the GameObject's health.
* `Stats.MaxHealth` gets or sets the GameObject's max health.
* `Stats.Armor` gets or sets the GameObject's armor.
* `Stats.MaxArmor` gets or sets the GameObject's max armor.
* `Stats.Imagination` gets or sets the GameObject's imagination.
* `Stats.MaxImagination` gets or sets the GameObject's max imagination.

###### Methods:
* `Stats.Damage(damage, source)` does damage calculations on a GameObject.
* `Stats.Heal(damage)` does healing calculations on a GameObject.

<hr>

##### DestructibleComponent : Component
A Component where you can smash a GameObject.

###### Methods
* `DestructibleComponent.Smash(smasher, <optional> loot_owner, <optional> animation)` smashes this GameObject and drops coins and currency based upon the
GameObject's drop table. Players will drop 10% of their coin up to 10000.

###### Example
```python
def smash_object(game_object, smasher):
    component = GetComponent(game_object, "DestructibleComponent")

    component.Smash(smasher, smasher)
```

<hr>

### Practical examples

#### Prop hunting
A small mini-game where players can hide as objects in the game world.

```python
hidden_players = []


def load():

    # Setup `parse_chat` to be called when a chat message is sent.
    OnChat(parse_chat)


def parse_chat(player, message):

    # Only accept commands
    if message[0] != '/':
        return

    global hidden_players

    # Interpret player commands
    param = message.split(' ')

    if param[0] == "/hide":

        # Check if player is already hidden
        for hidden in hidden_players:
            if hidden.player == player:

                # Send notice to the player
                Chat(player, "You are already hidden! Use the /reveal command to reveal yourself.")

                return

        # Get lot
        lot = int(param[1])

        # Setup prop
        prop = Prop(player, lot)

        prop.hide()

        # Send notice to the player
        Chat(player, "You hid yourself as " + str(lot) + "!")

        hidden_players.append(prop)
    elif param[0] == "/reveal":

        # Check if player is already revealed
        for hidden in hidden_players:
            if hidden.player == player:
                # Remove prop
                hidden.reveal()

                # Send notice to the player
                Chat(player, "You revealed yourself!")

                hidden_players.remove(hidden)

                return

        # Send notice to the player
        Chat(player, "You are already revealed! Use the /hide <lot> command to hide yourself.")
    elif param[0] == "/guess":
        guess(player)


def guess(player):
    global hidden_players

    # Send notice to the player
    Chat(player, "Guessing...")

    # Check if a player has correctly found a hiding prop.
    for prop in hidden_players:
        distance = Distance(prop.player.Transform.Position, player.Transform.Position)

        if distance < 5:
            prop.reveal()

            # Send notice to the player
            Chat(prop.player, "You were revealed!")

            hidden_players.remove(prop)


def unload():
    # Reveal all hidden players
    for hidden in hidden_players:
        hidden.reveal()


class Prop:
    def __init__(self, player, lot):
        self.player = player
        self.lot = lot
        self.layer = player.Layer
        self.obj = None

    def hide(self):
        # Hide the player
        self.player.Layer = Layer.Hidden

        transform = self.player.Transform

        # Create prop object
        self.obj = Create(self.lot, transform.Position, transform.Rotation)

        # Setup prop object
        Start(self.obj)
        Construct(self.obj)

    def reveal(self):
        # Restore layer
        self.player.Layer = self.layer

        # Destroy prop object
        Destroy(self.obj)
```

## C#
Uchu can load .NET Core assemblies at runtime to fulfill lower level scripting tasks.

### Setup
1. A .NET Core library with classes deriving form `Script`.
2. Build this library every time you make changes to the Uchu's code.
3. Add a tag `<ScriptDllSource>(Library name)</ScriptDllSource>` in between the `<DllSource></DllSource>` tags.

###### Note: these libraries cannot be reloaded once loaded. Requires server restart.