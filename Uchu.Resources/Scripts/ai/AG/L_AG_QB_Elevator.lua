--------------------------------------------------------------
-- Server-side script for the monument elevators
-- 
-- mrb... 8/24/09 -- added in qb smash fx
--------------------------------------------------------------

-- constants
local endTime = 4
local startTime = 8
local killTime = 10
local proxRadius = 5

-- when the QB is finished being built by a player
function onRebuildComplete( self, msg )
    -- start the proximity radius to watch for players getting on the elevator
    self:SetProximityRadius{name = "elevatorProx", radius = proxRadius} 
    -- set the player who built the QB
    self:SetVar("qbPlayer", "|" .. msg.userID:GetID())
    
    local delayTime = killTime - endTime 
    
    if delayTime < 1 then
        delayTime = 1
    end
    
    -- add a timer that will kill the QB if no players get on in the killTime
    GAMEOBJ:GetTimer():AddTimerWithCancel(killTime, "startKillTimer", self )
    self:StopPathing()
end

function onProximityUpdate(self, msg)    
    -- make sure we haven't already started pathing.
    if self:GetVar("qbPlayerRdy") then return end
    
    if msg.status == "ENTER" then    
        local player = msg.objId
            
        --print('player entered')
        -- If a player collided with me, then do our stuff
        if player:IsCharacter().isChar and not self:GetVar("qbPlayerRdy") then  
            local qbPlayer = self:GetVar("qbPlayer") or ""
        
            GAMEOBJ:GetTimer():CancelTimer('KillTimer', self)
            
            if player:GetID() == qbPlayer then
                -- the builder has entered so cancel the start timer and just start moving
                --print('Builder entered')
                self:SetVar("qbPlayerRdy", true)
                GAMEOBJ:GetTimer():CancelAllTimers(self)	
                self:UnsetProximityRadius{name = "elevatorProx"}
                self:StartPathing()
            elseif not self:GetVar("StartTimer") then
                -- non-builder player entered so fire off the start timer incase the builder doesn't get on
                self:SetVar("StartTimer", true)
                GAMEOBJ:GetTimer():AddTimerWithCancel(startTime, "StartElevator", self )
            end
        end
    end
end

function onPlatformAtLastWaypoint(self, msg)
    -- moving platform reached the end of the path so start the kill timer
    killTimerStartup(self)
end

function killTimerStartup(self)
    GAMEOBJ:GetTimer():CancelAllTimers(self)
    GAMEOBJ:GetTimer():AddTimerWithCancel(endTime, "KillTimer", self )
    -- start the blink effect by sending to the client  
    self:SetNetworkVar("startEffect", endTime)
end

-- timers...
function onTimerDone(self, msg)
    if msg.name == "StartElevator" then
        -- start pathing the builder isn't coming
        GAMEOBJ:GetTimer():CancelAllTimers(self)	
        self:UnsetProximityRadius{name = "elevatorProx"}
        self:StartPathing()
    elseif msg.name == "startKillTimer" then
        -- start the kill timer, the objects been around too long
        killTimerStartup(self)
    elseif msg.name == "KillTimer" then
        -- destroy the mover
        self:Die{killerID = self}
    end
end 