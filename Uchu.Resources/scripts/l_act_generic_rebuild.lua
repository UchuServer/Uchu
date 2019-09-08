require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Generic Rebuild -- Script
--//   - Creates a spawned entity to trigger the break on the rebuild
--///////////////////////////////////////////////////////////////////////////////////////

-- TODO: need to create offset based on rotation of the object
local detector_anim_delay = 7.0
local spawnDistance = -20.0
local entityTemplateID = 2223

-- This rebuild resets as soon as it is spawned
function onStartup(self) 
	GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "BreakSelf",self )
end

-- Called anytime the rebuild object's state changes
-- We use this to kick off the NPC break process on completion of the activity
function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
	
		-- start a timer that will start the process
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "BeginScriptedProcess",self )

	end
	
end     

-- Store the parent in the child
function onChildLoaded(self, msg)
	
	if msg.templateID == entityTemplateID then 
	    storeParent(self, msg.childID)
	end
end

onTimerDone = function(self, msg)

	if msg.name == "BreakSelf" then
		self:RebuildReset()
	end
	
    -- Start the scripted break process
    if msg.name == "BeginScriptedProcess" then 

       --local anim_time = self:GetAnimationTime{  animationID = "rebuild-complete" }.time
       self:PlayFXEffect{effectType = "rebuild-complete"}	

		-- start a timer that will spawn a rebuild break entity
		GAMEOBJ:GetTimer():AddTimerWithCancel( detector_anim_delay, "SpawnRebuildBreakEntity",self )
    end    

    -- Spawns the break entity into the world nearby
    if msg.name == "SpawnRebuildBreakEntity" then 

		-- get the heading and create a vector using spawn distance
		-- ninja detector animation needs the vector to be opposite the heading		
		local heading = getHeading(self)
		
		heading.x = heading.x * spawnDistance
		heading.y = heading.y * spawnDistance
		heading.z = heading.z * spawnDistance

		-- add some offset
		local mypos = self:GetPosition().pos 
		mypos.x = mypos.x + heading.x
		mypos.y = mypos.y + heading.y
		mypos.z = mypos.z + heading.z
		
		RESMGR:LoadObject { objectTemplate = entityTemplateID , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self }
    end
end 
