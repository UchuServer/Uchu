--------------------------------------------------------------
-- (CLIENT SIDE) Trigger for Help
--
-- Responsible for showing help information to player when
-- he is in the trigger for a certain period of time.
-- Same script works for all triggers via c_NimbusPark.lua
-- constants.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 
	GAMEOBJ:GetTimer():CancelAllTimers( self )
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
    if (msg.name == "ShowHelp") then

        -- show help information based on this object's LOT
        if (CONSTANTS["HELP_TRIGGER_DATA"][self:GetLOT().objtemplate] ~= nil) then
            UI:DisplayToolTip
            {
                strDialogText = CONSTANTS["HELP_TRIGGER_DATA"][self:GetLOT().objtemplate], 
                strImageName = "", 
                bShow=true, 
                iTime=0
            }
        end
    
    end
	
end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- if the local character is passing through the trigger
	if (msg.objectID:GetID() == GAMEOBJ:GetLocalCharID()) then

        -- start a timer to trigger help	
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["HELP_TRIGGER_IDLE_TIME"], "ShowHelp",self )
	
	end

	-- ignore collisions
	msg.ignoreCollision = true
	return msg
  
end


--------------------------------------------------------------
-- Off Collision
--------------------------------------------------------------
function onOffCollision(self, msg)

	-- if the local character is leaving the trigger
	if (msg.senderID:GetID() == GAMEOBJ:GetLocalCharID()) then
	
	    -- cancel all timers
    	GAMEOBJ:GetTimer():CancelAllTimers( self )
    	
    	-- close the tooltip
        UI:DisplayToolTip
        {
            strDialogText = "...", 
            strImageName = "", 
            bShow=false, 
            iTime=0
        }
        	
	end
  
end







