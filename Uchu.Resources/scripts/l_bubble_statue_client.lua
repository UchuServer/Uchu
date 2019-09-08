--------------------------------------------------------------
-- Client script for bubble blower. Handles their effect
-- based on the state of the zone
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

        if (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then

            -- if no invasion play the peace effect    
            RemoveEffect(self)
            RemoveEffect2(self)
            EnableEffect(self, "statue_idle")
            EnableEffect2(self, "statue_idle2")

            -- play animation
            SetAnimation(self, "idle_light")

        elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            RemoveEffect2(self)
            
            -- show a one time only effect and delay the looping effect
            self:PlayFXEffect{ effectType = "statue_to_bubble"  }
            
            SetAnimation(self, "light_to_alarm")
            
            GAMEOBJ:GetTimer():AddTimerWithCancel(3.5 , "DelayedIdleEffect", self )
            
        elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            RemoveEffect2(self)
            EnableEffect(self, "bubble_idle")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            RemoveEffect2(self)
            EnableEffect(self, "bubble_idle")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            RemoveEffect2(self)
            EnableEffect(self, "bubble_idle")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then

            -- if no invasion play the peace effect    
            RemoveEffect(self)
            RemoveEffect2(self)

            -- play animation
            SetAnimation(self, "alarm_to_light")

        end
        
    end
    
end    


--------------------------------------------------------------
-- Plays and sets the animation
--------------------------------------------------------------
function SetAnimation(self, name)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- play animation
    self:PlayAnimation{animationID = name}
    
end


--------------------------------------------------------------
-- does the effect exist
--------------------------------------------------------------
function EffectExists(effect)

    return (effect and tostring(effect) ~= tostring(CONSTANTS["NO_OBJECT"]))

end    


--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, name)

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
	self:PlayFXEffect{ name = "bubble1", effectType = name }

    -- save the effect
	self:SetVar("currentEffect", true)
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
	if ( myEffect ) then
		self:StopFXEffect{ name = "bubble1" }
		self:SetVar("currentEffect", false)
	end

end


--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect2(self, name)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar("currentEffect2")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name = "bubble2", effectType = name }

    -- save the effect
	self:SetVar("currentEffect2", true)

end

--------------------------------------------------------------
-- Removes an effect on the spout
--------------------------------------------------------------
function RemoveEffect2(self)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("currentEffect2")
    
    -- remove the effect
	if ( myEffect ) then
		self:StopFXEffect{ name = "bubble2" }
		self:SetVar("currentEffect2", false)
	end

end


--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar("bRenderReady", true)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="ZoneStateClientObjectReady" }

end


--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("currentEffect", CONSTANTS["NO_OBJECT"])
    self:SetVar("currentEffect2", CONSTANTS["NO_OBJECT"])
    
    -- set state to No Info, waiting for state information
    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INFO"])
        
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


--------------------------------------------------------------
-- called when timer complete
--------------------------------------------------------------
function onTimerDone(self, msg)

    -- play an animation
    if (msg.name == "DelayedIdleEffect") then
        RemoveEffect(self)
        RemoveEffect2(self)
        EnableEffect(self, "bubble_idle")
    end

end


