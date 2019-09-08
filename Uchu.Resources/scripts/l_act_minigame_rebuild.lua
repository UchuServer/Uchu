require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Minigame Rebuild -- Script
--//   - Resurrects the entity after the rebuild is put back together
--///////////////////////////////////////////////////////////////////////////////////////

-- this is the spawn time for NPCs based on the state of the rebuild
local completeSpawnTime = 5.0
local destroyedSpawnTime = 7.0

-- delay until the very first spawn starts
local spawnFirstStartDelay = 10.0

-- NPC template IDs
local ninjaTemplate = 2352
local ninjaFaction = 20

local pirateTemplate = 2353
local pirateFaction = 21

function onStartup(self) 

	-- set the spawn time to be the complete since the rebuild starts complete
	self:SetVar("spawnTime",completeSpawnTime)
	
	-- start a timer that will start the spawns
	GAMEOBJ:GetTimer():AddTimerWithCancel( spawnFirstStartDelay, "SpawnEntity",self )	
	
end

-- Called anytime the rebuild object's state changes
-- We use this to resurrect the object
function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
	
		-- cancel all timers
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		
	    -- set the spawn time
	    self:SetVar("spawnTime",completeSpawnTime)

	    -- resurrect the object
	    self:Resurrect()
	    
		-- restart the spawn timer
		GAMEOBJ:GetTimer():AddTimerWithCancel( completeSpawnTime, "SpawnEntity",self )		

	end
	
end     

-- called on death
function onDie(self, msg)
	
	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	
	-- set the spawn time
	self:SetVar("spawnTime",destroyedSpawnTime)

	-- break the rebuild
	self:RebuildReset()
	
	-- restart the spawn timer
	GAMEOBJ:GetTimer():AddTimerWithCancel( destroyedSpawnTime, "SpawnEntity",self )		
	
end

onTimerDone = function(self, msg)

    -- Start the scripted break process
    if msg.name == "SpawnEntity" then 

		local entityTemplateID = 0
		
		-- set the correct template based on faction
		if (self:GetFaction().faction == ninjaFaction) then
			entityTemplateID = ninjaTemplate
		elseif (self:GetFaction().faction == pirateFaction) then
			entityTemplateID = pirateTemplate
		end
		
		-- get our position and spawn the object
		if (entityTemplateID ~= 0) then
			local mypos = self:GetPosition().pos 
			RESMGR:LoadObject { objectTemplate = entityTemplateID , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, bIsSmashable = true }
		end
		
		-- start a timer that will spawn the next one
		local time = self:GetVar("spawnTime")
		GAMEOBJ:GetTimer():AddTimerWithCancel( time, "SpawnEntity",self )			

    end    

end 
