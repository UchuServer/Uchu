--------------------------------------------------------------
-- Makes the blood ravens fly away when you get close.
-- 
-- created mrb... 7/23/10
--------------------------------------------------------------

local sOnProximityAnim = "fly1"
local sIdleProximityAnim = "fly2"
local sOffProximityAnim = "fly3"

--------------------------------------------------------------
-- Sent when the script is started.
--------------------------------------------------------------
function onStartup(self,msg)
    -- set up the proximity radius, and pulse the proximity
    self:SetProximityRadius{radius = 45, name = "BloodRaven"}
    self:GetProximityObjects{name = "BloodRaven"}
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    -- is this the correct proximity
    if msg.name == "BloodRaven" then
        local sAnim = sOnProximityAnim
        
        -- set bAway
        if msg.status == "ENTER" then              
            self:SetVar("bAway", true)
        elseif msg.status == "LEAVE" then 
            sAnim = sOffProximityAnim            
            self:SetVar("bAway", false)
        end               
                   
        -- start animating if we're ready to
        if not self:GetVar("bAnimating") then
            self:PlayAnimation{animationID = sAnim, bTriggerOnCompleteMsg = true}
            self:SetVar("bAnimating", true)
        end
    end    
end 

function onAnimationComplete(self, msg)
    -- done animating
    self:SetVar("bAnimating", false)
    
    if msg.animationID == sOnProximityAnim then -- done flying away, decide to go back or idle away
        if not self:GetVar("bAway") then
            self:PlayAnimation{animationID = sOffProximityAnim, bTriggerOnCompleteMsg = true}
            self:SetVar("bAnimating", true)
        else
            self:PlayAnimation{animationID = sIdleProximityAnim} 
        end  
    elseif msg.animationID == sOffProximityAnim then -- done flying back, should we fly away
        if self:GetVar("bAway") then
            self:PlayAnimation{animationID = sOnProximityAnim, bTriggerOnCompleteMsg = true}
            self:SetVar("bAnimating", true)
            self:SetVar("bAway", false)
        end            
    end    
end 