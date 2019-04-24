--------------------------------------------------------------
-- Server side script maintains the state of the bubble statue 
-- if statue is not enabled, we can bubble the player
--------------------------------------------------------------
	
	
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Get the enable state
--------------------------------------------------------------
function GetEnableState(self)

    return self:GetVar("StatueEnabled")
    
end


--------------------------------------------------------------
-- Set the enable state
--------------------------------------------------------------
function SetEnableState(self, state)

    -- calc new state and set
    local bIsEnabled = (state == CONSTANTS["ZONE_STATE_NO_INVASION"])
    self:SetVar("StatueEnabled", bIsEnabled)

end    


--------------------------------------------------------------
-- On startup
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetProximityRadius{ radius = CONSTANTS["BUBBLE_STATUE_RADIUS"]}

    -- stores if the statue mode is currently enabled
    self:SetVar("StatueEnabled", true) 
    
end


--------------------------------------------------------------
-- Called when proximity is updated
--------------------------------------------------------------
function onProximityUpdate(self, msg)

    -- a player enters proximity and we are Not in statue mode
    if ( msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 and GetEnableState(self) == false) then
    
		-- if the player that just entered proximity had skunk stink, clean it off
		BubblePlayer( self, msg )
	
	end

end


--------------------------------------------------------------
-- try to put player in bubble and destink
--------------------------------------------------------------
function BubblePlayer( self, msg )

    self:CastSkill{ optionalTargetID = msg.objId, skillID = CONSTANTS["DESTINK_SKILL"] }
	msg.objId:ActivateBubbleBuff{}

end


--------------------------------------------------------------
-- Notification to object
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- set the state
    if (msg.name == "zone_state_change") then
        SetEnableState(self, msg.param1)
	end
    
end   