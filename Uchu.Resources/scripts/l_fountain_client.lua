--------------------------------------------------------------
-- Client script for fountain. Handles their effect
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
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)
            EnableEffect(self, 1, "peaceTime1")
            EnableEffect(self, 2, "peaceTime2")
            EnableEffect(self, 3, "peaceTime3")
            EnableEffect(self, 4, "peaceTime4")

            -- play animation
            SetAnimation(self, "normal")

        elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then

            -- disable the effect in this state
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)

            -- play animation with optional delay
            SetAnimation(self, "alertUp")
            
        elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)
            EnableEffect(self, 1, "alarmTime")

            -- play animation
            SetAnimation(self, "normal_up")

        elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)
            EnableEffect(self, 1, "alarmTime")

            -- play animation
            SetAnimation(self, "normal_up")

        elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)
            EnableEffect(self, 1, "alarmTime")

            -- play animation
            SetAnimation(self, "normal_up")

        elseif (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then

            -- if no invasion play the peace effect    
            RemoveEffect(self, 1)
            RemoveEffect(self, 2)
            RemoveEffect(self, 3)
            RemoveEffect(self, 4)

            -- play animation
            SetAnimation(self, "alertDown")

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
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, num, action)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar("currentEffect" .. num)
	if ( myEffect == true ) then
		print("fountain on already")
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name = "fountain" .. num, effectType = action }

    -- save the effect
	self:SetVar( "currentEffect" .. num, true )

end

--------------------------------------------------------------
-- Removes an effect on the spout
--------------------------------------------------------------
function RemoveEffect(self, num)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("currentEffect" .. num)
    
    -- remove the effect
	if ( myEffect ) then
		self:StopFXEffect{ name = "fountain" .. num }
		self:SetVar("currentEffect" .. num , false)
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
    
    self:SetVar("currentEffect1", CONSTANTS["NO_OBJECT"])
    self:SetVar("currentEffect2", CONSTANTS["NO_OBJECT"])
    self:SetVar("currentEffect3", CONSTANTS["NO_OBJECT"])
    self:SetVar("currentEffect4", CONSTANTS["NO_OBJECT"])
    
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




