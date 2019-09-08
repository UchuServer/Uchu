----------------------------------------------------------------
-- level specific Server script for Property Pushback in AG small property
-- this script requires a base script
-- this script should be in the zone script in the DB
----------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_BASE_PROPERTY_SERVER')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables

--GROUPS, set in Happy Flower on objects
local Group = {
				ClaimMarker		= "ClaimMarker", -- claimmarker object that the player rebuilds
				Generator 		= "Generator", -- object the player smashes to get the claimmarker quickbuild
				Guard			= "Guard", -- mission giver npc
				PropertyPlaque	= "PropertyPlaque", -- make sure this matching the client script
				PropertyVendor	= "PropertyVendor", -- the object the player actually rents the property from
				Spots			= "Spots", -- the fx on the ground that don't damage the player and say until the player places a model
				MSClouds		= "maelstrom", -- the damaging maelstrom cloud FX around the property
				Enemies			= "strombies", -- all the enemies on the map, no matter what spawner network they are in
				FXManager		= "FXObject" -- the hidden object underground (small yellow box) that controls all the env fx for the map
				}

--Spawner networks, set in happy flower
local Spawners = {
					enemy 			= { "StrombieWander","Strombies" }, -- this can be as many spawner networks as necessary, 
												--but all spawner networks with enemies should be listed
					claimMarker 	= "ClaimMarker", --the spawner network for the claim marker, should only be one node
					generator		= "Generator", --the spawner network for the generator, should only be one node
					DamageFX		= "MaelstromFX", -- the spawner network for the damaging maelstrom clouds
					FXSpots 		= "MaelstromSpots", -- the spawner network for the non-damaging fx spots
					PropMG  		= "PropertyGuard" -- spawns the mission giver for this property
				}
-- player flags. These have to be different for each property map. these are set up in the db
local flags = { 
				defeatedProperty 	= 71,  -- when the player builds the claim marker, this flag is set
				placedModel 		= 73	-- when a player places a model for the first time, this flag is set
			  }

----------------------------------------------------------------
-- leave the functions below alone
----------------------------------------------------------------


function onStartup(self)
	
	setGameVariables(Group,Spawners,flags)
	baseStartup(self,msg,newMsg)
end
----------------------------------------------------------------
-- Called when the player fully loads into the map, passes the variables set above,
-- Sets up the map for maelstrom if the player has not defeated this map before
----------------------------------------------------------------			
function onPlayerLoaded(self, msg)
    
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
