--------------------------------------------------------------
-- Counts the number of times a player wearing a certain item gets hit and then casts a skill when they are hit enough.
-- MEdwards 11/12/10

--------------------------------------------------------------

function onFactionTriggerItemEquipped (self)
     self:SendLuaNotificationRequest{requestTarget=self:GetItemOwner().ownerID, messageName="OnHit"}
     self:SetVar("coilCount", 0)
end

function notifyOnHit( self, other, msg )
    local player = self:GetItemOwner().ownerID                                      
    self:SetVar("coilCount", self:GetVar("coilCount") + 1)
    if self:GetVar("coilCount") > 4 then
        player:CastSkill{skillID = 1001}  
        self:SetVar("coilCount", 0)
    end
end


function onFactionTriggerItemUnequipped (self)
    self:SendLuaNotificationCancel{requestTarget=self:GetItemOwner().ownerID, messageName="OnHit"}
end