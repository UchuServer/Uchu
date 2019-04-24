--------------------------------------------------------------
-- Server side script maintains the state of the spouts using 
-- player proximity
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

    return self:GetVar("SpoutEnabled")
    
end


--------------------------------------------------------------
-- Set the enable state
--------------------------------------------------------------
function SetEnableState(self, state)

    -- calc new state and set
    local bIsEnabled = (state == CONSTANTS["ZONE_STATE_NO_INVASION"])
    self:SetVar("SpoutEnabled", bIsEnabled)

end    


--------------------------------------------------------------
-- On startup
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("PlayerOnMe", false )    -- @TODO: leaveprox does not work on logout

    self:SetProximityRadius{ radius = CONSTANTS["radius"]}

    -- stores if the spout is currently enabled
    self:SetVar("SpoutEnabled", true) 
    
end


--------------------------------------------------------------
-- Called when proximity is updated
--------------------------------------------------------------
function onProximityUpdate(self, msg)

    -- a player enters proximity while no players are on it
    if ( msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 and self:GetVar("PlayerOnMe") == false ) then
    
		-- if the player that just entered proximity had skunk stink, clean it off
		CleanPlayer( self, msg )
		self:SetVar( "PlayerOnMe", true )
		
	
	elseif ( msg.status == "LEAVE" and msg.objId:GetFaction().faction == 1 ) then
	
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "ProxCheck",self )
	
	end

end


--------------------------------------------------------------
-- cleans skunk stink off the player that just entered proximity
--------------------------------------------------------------
function CleanPlayer( self, msg )

    if (GetEnableState(self) == true) then
	    print( "SPOUT:CleanPlayer" )

        self:CastSkill{ optionalTargetID = msg.objId, skillID = CONSTANTS["DESTINK_SKILL"] }
    end

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
	
	-- check for players in proximity
    if (msg.name == "ProxCheck") then
	    
	    -- no one left
        if (ArePlayersInProximity(self) == false) then
        
            self:SetVar( "PlayerOnMe", false )
            
		end

    end
	
end


--------------------------------------------------------------
-- Determines if any players are currently in proximity
--------------------------------------------------------------
function ArePlayersInProximity(self)

	local objs = self:GetProximityObjects().objects
	local index = 1

	while index <= table.getn(objs)  do

		local target = objs[index]
		local faction = target:GetFaction()

		if faction and faction.faction == 1 then
			return true;
		end
		index = index + 1

	end
	return false;

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