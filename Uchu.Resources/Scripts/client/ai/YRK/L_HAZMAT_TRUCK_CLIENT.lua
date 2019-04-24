--------------------------------------------------------------
-- Client script for the hazmat truck. Handles its effects
-- based on the state of the zone
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------

-- the name of each particle effect

CONSTANTS["FRONT_EFFECT_NAME"] = "frontLights"
CONSTANTS["LEFT_EFFECT_NAME"] = "leftLights"
CONSTANTS["RIGHT_EFFECT_NAME"] = "rightLights"

-- for testing state switch only
--CONSTANTS["TEST_TIME"] = 10


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
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar(CONSTANTS["FRONT_EFFECT_NAME"] .. "Effect", CONSTANTS["NO_OBJECT"])
	self:SetVar(CONSTANTS["LEFT_EFFECT_NAME"] .. "Effect", CONSTANTS["NO_OBJECT"])
	self:SetVar(CONSTANTS["RIGHT_EFFECT_NAME"] .. "Effect", CONSTANTS["NO_OBJECT"])
    
    -- set state to No Info, waiting for state information
    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INFO"])

end


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

            -- if no invasion, then there should be no effects on the truck    
            RemoveParticleEffects(self)

        else
			-- the skunk invasion is happening.
			-- add effects to the truck, if they aren't there already
            EnableParticleEffects(self)
            
		end
	end

end    


--------------------------------------------------------------
-- does the effect exist
--------------------------------------------------------------
function EffectExists(effect)

    return (effect and tostring(effect) ~= tostring(CONSTANTS["NO_OBJECT"]))

end    


--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar("bRenderReady", true)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="ZoneStateClientObjectReady" }
	

    if (IsActive(self) == true or self:GetLOT().objtemplate == CONSTANTS["HAZMAT_VAN_LOT"]) then
    	-- and set the animation set to the right state
        self:SetAnimationSet{strSet = ""}
	else
	    -- show this as broken if it is supposed to be
	    self:SetAnimationSet{strSet = "broken"}
    end
	
	-- TEST ONLY -- 
	--GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["TEST_TIME"], "testNoInvasion", self )

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
-- enables all the particle effects for the truck lights
--------------------------------------------------------------
function EnableParticleEffects(self)

	EnableEffect(self, CONSTANTS["FRONT_EFFECT_NAME"])
	EnableEffect(self, CONSTANTS["LEFT_EFFECT_NAME"])
	EnableEffect(self, CONSTANTS["RIGHT_EFFECT_NAME"])

end


--------------------------------------------------------------
-- Enables an effect on the truck by name unless one is already present
--------------------------------------------------------------
function EnableEffect(self, action)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar(action .. "Effect")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name = action .. "Effect", effectType = action }
    
    -- save the effect
	self:SetVar( action .. "Effect", true )

end


--------------------------------------------------------------
-- Removes all effect on the truck
--------------------------------------------------------------
function RemoveParticleEffects(self)

	RemoveEffect(self, CONSTANTS["FRONT_EFFECT_NAME"])
	RemoveEffect(self, CONSTANTS["LEFT_EFFECT_NAME"])
	RemoveEffect(self, CONSTANTS["RIGHT_EFFECT_NAME"])	

end


--------------------------------------------------------------
-- Removes a particle effect from the truck
--------------------------------------------------------------
function RemoveEffect(self, action)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar(action .. "Effect")
    
    -- remove the effect
	if ( myEffect ) then
		self:StopFXEffect{ name = action .. "Effect" }
		self:SetVar( action .. "Effect", false)
	end

end


--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then

	    -- change animations to non-broken
	    self:SetAnimationSet{strSet = ""}

	else

	    -- set animations to broken
	    self:SetAnimationSet{strSet = "broken"}
	    
	end
	
end   




