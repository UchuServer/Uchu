--------------------------------------------------------------
-- Client script for skunk detectors. handles state
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Get the state of the zone
--------------------------------------------------------------
function GetZoneState(self)

    return self:GetVar("ZoneState")
    
end


--------------------------------------------------------------
-- Set the state of the zone
--------------------------------------------------------------
function SetZoneState(self, state)

    -- get current state
    local prevState = GetZoneState(self)
    
    self:SetVar("ZoneState", state)

    -- perform actions based on zone state
    if (prevState and prevState ~= state) then

        DoZoneStateActions(self, state)
        
    end
    
end    


--------------------------------------------------------------
-- Perform actions based on zone state
--------------------------------------------------------------
function DoZoneStateActions(self, state)

    if (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then

        -- if no invasion remove effect
        RemoveEffect(self)

    elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then

        -- cancel any effect and play a new one
        RemoveEffect(self)
        EnableEffect(self, "red")
        
    elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then

        -- cancel any effect and play a new one
        RemoveEffect(self)
        EnableEffect(self, "red")

    elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then

        -- cancel any effect and play a new one
        RemoveEffect(self)
        EnableEffect(self, "orange")

    elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then

        -- cancel any effect and play a new one
        RemoveEffect(self)
        EnableEffect(self, "yellow")
    
    elseif (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then

        -- if no invasion remove effect
        RemoveEffect(self)
    
    end

end

--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, action)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar("currentEffect")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name = "skunk", effectType = action }

    -- save the effect
	self:SetVar( "currentEffect", true )

end


--------------------------------------------------------------
-- Removes an effect on the spout
--------------------------------------------------------------
function RemoveEffect(self)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("currentEffect")
    
    -- remove the effect
	if ( myEffect == true ) then
		self:StopFXEffect{ name = "skunk" }
		self:SetVar("currentEffect", false)
	end

end


--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar("bRenderReady", true)

    -- do actions based on state
    DoZoneStateActions(self, GetZoneState(self))

end


--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

end


--------------------------------------------------------------
-- Called when object gets a notification
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- set the state
    if (msg.name == "zone_state_change") then
        SetZoneState(self, msg.param1)
    end
    
end


