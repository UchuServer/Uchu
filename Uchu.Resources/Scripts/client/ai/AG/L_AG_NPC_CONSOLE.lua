--------------------------------------------------------------
-- Description:
--
-- Client script for playing effect on battlefield outpost console
-- created mrb... 1/22/10
-- modified by brandi 9/8/10.. took out the picktype stuff, since the console is a mission giver, that system handles all of it
--------------------------------------------------------------

function onClientUse(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check if this is the local palyer or not
    if playerID ~= msg.user:GetID() then return end    
    
    self:PlayFXEffect{name = "onInteract",effectType = "interact"}
end

function onTerminateInteraction(self,msg)
    self:StopFXEffect{name = "onInteract"}
end

