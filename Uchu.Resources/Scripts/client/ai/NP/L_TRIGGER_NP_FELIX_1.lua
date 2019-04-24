--------------------------------------------------------------
-- (CLIENT SIDE) Trigger for Felix in Scene 1
--
-- Responsible for spawning felix for the player on the client
-- if the client has not completed the scene 1 felix event.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------
CONSTANTS["PLAYER_WATCH_TIMER_INTERVAL"] = 1.0


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

	-- start a local character watch timer
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["PLAYER_WATCH_TIMER_INTERVAL"], "CheckLocalCharacterExists",self )

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- check for the local character
    if (msg.name == "CheckLocalCharacterExists") then
	    
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	    
		-- restart the timer and try again
		-- if local character is not ready yet
		if (GAMEOBJ:GetLocalCharID() ~= CONSTANTS["NO_OBJECT"]) then
			-- try to spawn felix
			DoFelixSpawn(self)
		else
			GAMEOBJ:GetTimer():CancelAllTimers( self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["PLAYER_WATCH_TIMER_INTERVAL"], "CheckLocalCharacterExists",self )
		end
    
    end
	
end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- if the local character is passing through the trigger
	if (msg.objectID:GetID() == GAMEOBJ:GetLocalCharID()) then
	
		-- force felix interaction if he still exists
		local felix = getObjectByName(self, "felixObj")
		
		if (felix and felix:Exists()) then
		    felix:NotifyObject{ name = "ForceHelp" }
		end
	
	end

	-- ignore collisions
	msg.ignoreCollision = true
	return msg
  
end


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- store spawn for use later
	storeObjectByName(self, "felixObj", msg.childID)

	-- store who the parent is
	storeParent(self, msg.childID)

end


--------------------------------------------------------------
-- Check player and spawn felix if needed
--------------------------------------------------------------
function DoFelixSpawn(self)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	if ( (player) and (player:Exists()) ) then

		-- check the tooltip flag to see if he has done this event
		local tooltipMsg = player:GetTooltipFlag{ iToolTip = CONSTANTS["PLAYER_FELIX_1_FLAG_BIT"] }
		if ((tooltipMsg) and (tooltipMsg.bFlag == false)) then

			-- spawn in felix
			RESMGR:LoadObject 
			{ 
				objectTemplate	= CONSTANTS["FELIX_1_LOT"],
				x				= CONSTANTS["FELIX_1_SPAWN_POS"].x,
				y				= CONSTANTS["FELIX_1_SPAWN_POS"].y,
				z				= CONSTANTS["FELIX_1_SPAWN_POS"].z,
                rw              = CONSTANTS["FELIX_1_SPAWN_ROT"].w,
                rx              = CONSTANTS["FELIX_1_SPAWN_ROT"].x,
                ry              = CONSTANTS["FELIX_1_SPAWN_ROT"].y,
                rz              = CONSTANTS["FELIX_1_SPAWN_ROT"].z,
				owner			= self 
			}
		    
		end    
		
	end

end






