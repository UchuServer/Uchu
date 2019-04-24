--------------------------------------------------------------

-- L_NS_DIRECTIONSIGNAGE_CLIENT.lua

-- Client side script for the NS location notification displays
-- created abeechler ... 7/21/11

--------------------------------------------------------------

local defaultProxRadius = 30      -- A stored default proximity value

----------------------------------------------------------------
-- On object instantiation, process config data based 
-- proximity sensing
----------------------------------------------------------------
function onStartup(self, msg)
    local proxRadius = self:GetVar("proxRadius") or  defaultProxRadius
    -- Set up the proximity radius
    self:SetProximityRadius {radius = proxRadius}
end

----------------------------------------------------------------
-- Catch distance based notification events
----------------------------------------------------------------
function onProximityUpdate(self, msg)
    -- Get a reference to the local player
    local player = GAMEOBJ:GetControlledID()
    local playerID = GAMEOBJ:GetControlledID():GetID()
    -- Check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    local animTime = 0
    local animName = "next"
    
    -- Pick the correct animation based on message status
    if (msg.status == "ENTER") then
        self:PlayAnimation{animationID = "next", bPlayImmediate = true}
    elseif (msg.status == "LEAVE") then 
        animName = "back"
    end    
    -- Get the animation time based on animName
    animTime = self:GetAnimationTime{animationID = animName}.time
 
    -- Cancel all timers and start one based on animName and animTime
    GAMEOBJ:GetTimer():CancelAllTimers(self)       
    GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, animName, self)
end

----------------------------------------------------------------
-- Catch timer events for sign animation processing
----------------------------------------------------------------
function onTimerDone(self, msg)
    -- Play the correct idle animation
    if msg.name == "back" then           
		self:PlayAnimation{animationID = "Sign1", bPlayImmediate = true}
    elseif msg.name == "next" then             
		self:PlayAnimation{animationID = "Sign2", bPlayImmediate = true}
    end
end 