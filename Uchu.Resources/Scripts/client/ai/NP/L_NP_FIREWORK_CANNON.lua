require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//  Client side script for Static Cannon
--//  - Fires periodically on the client within an interval and after a cooldown
--//  - Only fires if the player is within specified distance
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Parameters and Constants
--------------------------------------------------------------
-- interval of time a fire can occur in (int seconds)
local minInterval = 5
local maxInterval = 10

-- cooldown from firing, will not fire again until cooldown is complete (float seconds)
local fireCooldown = 5.0

-- player must be within min distance or firing won't happen
local proxRadius = 240.0


--------------------------------------------------------------
-- Startup of object
--------------------------------------------------------------
function onStartup(self)

	SetIsActive(self, false)
		
end


--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

	self:SetProximityRadius { radius = proxRadius }

end


--------------------------------------------------------------
-- Called when an entity gets within proximity of the object
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	-- if the local player is close enough to us
	if (msg.status == "ENTER") and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then

		SetIsActive(self, true)
		SetupFireTimer(self)

	elseif (msg.status == "LEAVE") and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then
	
		-- cancel all timers
		SetIsActive(self, false)
		GAMEOBJ:GetTimer():CancelAllTimers( self )

	end

end



--------------------------------------------------------------
-- Setup timer for firing
--------------------------------------------------------------
function SetupFireTimer(self)

	if (IsActive(self) == true) then

		local ran = math.random(minInterval,maxInterval)
		GAMEOBJ:GetTimer():AddTimerWithCancel( ran, "DoFire",self )

	end
		
end


--------------------------------------------------------------
-- Cannon plays effects
--------------------------------------------------------------
function DoFireEffect(self)

	if (IsActive(self) == true) then

		self:PlayFXEffect{ effectType = "fire"  }
		--self:PlayFXEffect{ effectType = "onfire2_large" }
		--self:PlayFXEffect{ effectType = "environment_fire" }
	
		GAMEOBJ:GetTimer():AddTimerWithCancel( fireCooldown, "FireCooldown",self )
	
	end

end


--------------------------------------------------------------
-- Get IsActive state
--------------------------------------------------------------
function IsActive(self)
	return self:GetVar("IsActive")
end


--------------------------------------------------------------
-- Set IsActive State
--------------------------------------------------------------
function SetIsActive(self, bActive)
	self:SetVar("IsActive", bActive)
end


--------------------------------------------------------------
-- Called when timers complete
--------------------------------------------------------------
onTimerDone = function(self, msg)

    if (msg.name == "DoFire") then
		DoFireEffect(self)
    end
    
    if (msg.name == "FireCooldown") then
		SetupFireTimer(self)		
    end    
    
end    