
--------------------------------------------------------------
-- Server side script watches for rebuild state changes and resets the rebuild when clicked
--------------------------------------------------------------



--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')





--------------------------------------------------------------
-- Returns true if the object is in the idle rebuild state
--------------------------------------------------------------
function IsActive(self)

    -- get the rebuild state
    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (rebuildState and tonumber(rebuildState.iState) == 3) then
        return true
    else
        return false
    end

end





--------------------------------------------------------------
-- Handle client clicking on the object on server
--------------------------------------------------------------
function onUse(self, msg) 

    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (IsActive(self) == true) then
   
        self:RebuildReset()
      
    end
    
end





--------------------------------------------------------------
-- Called when a rebuild is reset
--------------------------------------------------------------
function onRebuildReset(self, msg)

    GAMEOBJ:GetTimer():CancelAllTimers( self )

	-- hydrant was just broken to turn the water on
  
end





--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    GAMEOBJ:GetTimer():CancelAllTimers( self )
    
    -- if we just hit the idle state
	--if (msg.iState == 3) then
		-- hydrant just got repaired to turn the water off again

	--end
	
end 




--------------------------------------------------------------
-- called when a fire event msg is sent from client-side
--------------------------------------------------------------
function onFireEventServerSide( self, msg )

	if ( msg.args == "cleanPlayer" ) then

		CleanPlayer( self, msg.senderID )
	end
end





function CleanPlayer( self, target )

	-- the client-side script told us to clean off the player that's being bounced

	-- make sure it's a player
	if ( target:GetFaction().faction ~= 1 ) then
		return
	end
	
	self:CastSkill{ optionalTargetID = target, skillID = CONSTANTS["DESTINK_SKILL"] }
	
end  



