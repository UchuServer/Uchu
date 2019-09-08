----------------------------------------------------------------
-- level specific Server script for Property Pushback in NS small property
-- this script requires a base script
-- this script should be in the zone script in the DB
-- updated mrb... 9/7/10 - added brickLinkMissionID
-- updated abeechler... 2/22/11 - added "BankObj" PropObjs Spawner
----------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_BASE_PROPERTY_SERVER')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables

--GROUPS, set in Happy Flower on objects
local Group = {
				ClaimMarker		= "Rhino", -- claimmarker object that the player rebuilds
				Generator 		= "Generator", -- object the player smashes to get the claimmarker quickbuild
				Guard			= "Guard", -- mission giver npc
				PropertyPlaque	= "PropertyPlaque", -- make sure this matching the client script
				PropertyVendor	= "PropertyVendor", -- the object the player actually rents the property from
				Spots			= "Spots", -- the fx on the ground that don't damage the player and say until the player places a model
				MSClouds		= "Clouds", -- the damaging maelstrom cloud FX around the property
				Enemies			= "Enemies", -- all the enemies on the map, no matter what spawner network they are in
				FXManager		= "FXManager", -- the hidden object underground (small yellow box) that controls all the env fx for the map
				ImagOrb			= "Orb",
				GeneratorFX		= "GeneratorFX"
				}

--Spawner networks, set in happy flower
local Spawners = {
					Enemy 			= { "StrombieWander","StrombieGen","PirateWander","PirateGen","RoninGen" }, -- this can be as many spawner networks as necessary, 
												--but all spawner networks with enemies should be listed
					ClaimMarker 	= "ClaimMarker", --the spawner network for the claim marker, should only be one node
					Generator		= "Generator", --the spawner network for the generator, should only be one node
					DamageFX		= "MSClouds", -- the spawner network for the damaging maelstrom clouds
					FXSpots 		= "Spots", -- the spawner network for the non-damaging fx spots
					PropMG  		= "Guard", -- spawns the mission giver for this property
					ImagOrb			= "Orb",
					GeneratorFX		= "GeneratorFX",
					Smashables		= "Smashables", -- smashables to give the player imagination if they run out
					FXManager		= "FXManager", -- the hidden object underground (small yellow box) that controls all the env fx for the map
					PropObjs		= "BankObj", -- spawns objects that player's can interact with once a property is claimed
					AmbientFX		= { "Rockets"},  -- the ambient happy effects for the property, they are on by default and are turned off if maelstrom is spawned
					BehaviorObjs	= { "Cage", "Platform","Door" } -- Behavior scenerio object
				}
-- player flags. These have to be different for each property map. these are set up in the db
local flags = { 
				defeatedProperty 	= 97,  -- when the player builds the claim marker, this flag is set
				placedModel 		= 105,	-- when a player places a model for the first time, this flag is set
				guardMission		= 872, -- last mission for the guard
				password			= "s3kratK1ttN", -- behavior password build qb object with behaviors
				generatorID			= 11031, --lot id of the generator
				orbID				= 10226, -- lot id of the orb
				behavQBID			= 11009, -- lot id of the behavior platform quickbuild
				brickLinkMissionID  = 948           -- Achievement ID to complete on property rental
			  }

----------------------------------------------------------------
-- leave the functions below alone
----------------------------------------------------------------


----------------------------------------------------------------
-- Called when the player fully loads into the map, passes the variables set above,
-- Sets up the map for maelstrom if the player has not defeated this map before
----------------------------------------------------------------			
function onPlayerLoaded(self, msg)
    setGameVariables(Group,Spawners,flags)
	basePlayerLoaded(self,msg,newMsg)
end

----------------------------------------------------------------
-- called when the player rents a zone, turns on the property border
----------------------------------------------------------------
function onZonePropertyRented(self, msg)
	baseZonePropertyRented(self,msg,newMsg)
end

----------------------------------------------------------------
-- called when the player places a model, the first time it turns off the spots and sets a player flag
----------------------------------------------------------------
function onZonePropertyModelPlaced(self, msg)
	baseZonePropertyModelPlaced(self,msg,newMsg)	
end

----------------------------------------------------------------
-- called from the generator object and the claimmarker object when they die
----------------------------------------------------------------
function notifyDie(self,other,msg)
	baseNotifyDie(self,other,msg)
end

-------------------------------------------------------
-- called when a player exits the zone
----------------------------------------------------------------
function onPlayerExit(self,msg)
	basePlayerExit(self,other,msg)
end

----------------------------------------------------------------
-- called from the quickbuild behavior model when its done rebuilding
----------------------------------------------------------------
function notifyRebuildComplete(self,other,msg)
    baseNotifyRebuildComplete(self,other,msg)
end

----------------------------------------------------------------
-- called when orb is collided with
----------------------------------------------------------------
function notifyCollisionPhantom(self,other,msg)
    baseNotifyCollisionPhantom(self,other,msg)
end

----------------------------------------------------------------
-- called when notify object message is recieved
----------------------------------------------------------------
function onNotifyObject(self,msg)
	baseNotifyObject(self,msg,newMsg)
end

----------------------------------------------------------------
-- called when timers are done
----------------------------------------------------------------
function onTimerDone(self,msg)
	baseTimerDone(self,msg,newMsg)
end
