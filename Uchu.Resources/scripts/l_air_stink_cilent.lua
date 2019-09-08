--------------------------------------------------------------
-- Client script for air stink. Handles its effect
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
-- for testing state switch only
--CONSTANTS["TEST_TIME"] = 10




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

        if ( state == CONSTANTS["ZONE_STATE_NO_INVASION"] or 
		state == CONSTANTS["ZONE_STATE_TRANSITION"] ) then
 
            RemoveEffect(self)

        else

            EnableEffect(self, "add")
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
	self:PlayFXEffect{ name = "stink", effectType = name }
    -- save the effect
	self:SetVar( "currentEffect", true )

end

--------------------------------------------------------------
-- Removes the effect
--------------------------------------------------------------
function RemoveEffect(self)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("currentEffect")
    
    -- remove the effect
	if ( EffectExists(myEffect) ) then
		self:StopFXEffect{ name = "stink" }
		self:SetVar("currentEffect", false )
	end

end




--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar("bRenderReady", true)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="ZoneStateClientObjectReady" }
	
	-- instantly hide the render object, we only care to see the effect
    self:SetVisible{visible = false, fadeTime = 0.0}
	
	
	-- TEST ONLY -- 
	--GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["TEST_TIME"], "testNoInvasion", self )

end


--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)
	
	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("currentEffect", CONSTANTS["NO_OBJECT"])
    
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




--[[
--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

    print ("Timer name: "..msg.name)
    
    local time = CONSTANTS["TEST_TIME"]

	if ( msg.name == "testNoInvasion" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INVASION"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testTransition", self )
	
	elseif ( msg.name == "testTransition" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_TRANSITION"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testHigh", self )
	
	elseif ( msg.name == "testHigh" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_HIGH_ALERT"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testMedium", self )
	
	elseif ( msg.name == "testMedium" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_MEDIUM_ALERT"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testLow", self )
	
	elseif ( msg.name == "testLow" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_LOW_ALERT"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testDone", self )
	
	elseif ( msg.name == "testDone" ) then
		SetZoneState(self, CONSTANTS["ZONE_STATE_DONE_TRANSITION"])
		GAMEOBJ:GetTimer():AddTimerWithCancel(time, "testNoInvasion", self )
	
	end
	
end

]]--
