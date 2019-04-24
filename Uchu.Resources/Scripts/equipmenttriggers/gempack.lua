--------------------------------------------------------------
-- Counts the number of times a player wearing a certain item gets hit and then casts a skill when they are hit enough.
-- MEdwards 6/9/11
-- MEdwards 6/16/11 Fixed the unequip message

--------------------------------------------------------------

function onFactionTriggerItemEquipped (self)
     self:SendLuaNotificationRequest{requestTarget=self:GetItemOwner().ownerID, messageName="HitOrHealResult"}
     self:SetVar("coilCount", 0)
end


function notifyHitOrHealResult( self, other, msg )
    local player = self:GetItemOwner().ownerID  
    if player:GetID() == msg.receiver:GetID() then 
        if (msg.armorDamageDealt > 0) or (msg.lifeDamageDealt > 0) then                                  
            self:SetVar("coilCount", self:GetVar("coilCount") + 1)
            if self:GetVar("coilCount") > 4 then
                player:CastSkill{skillID = 1488} 
                self:SetVar("coilCount", 0)
            end
        end    
    end
end


function onFactionTriggerItemUnequipped (self)
    self:SendLuaNotificationCancel{requestTarget=self:GetItemOwner().ownerID, messageName="HitOrHealResult"}
end