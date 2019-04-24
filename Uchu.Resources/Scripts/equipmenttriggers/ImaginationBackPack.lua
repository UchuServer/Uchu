--------------------------------------------------------------
-- Casts the buff check on the player equipped with the imagination backpack to see if the player should play the effects for the pack
-- MEdwards 6/16/11, NFoster 6/20/11
--------------------------------------------------------------

function onFactionTriggerItemEquipped (self, msg)
	self:SendLuaNotificationRequest{requestTarget=self:GetItemOwner().ownerID, messageName="PlayerResurrectionFinished"}
end

function notifyPlayerResurrectionFinished( self, other, msg )
    local player = self:GetItemOwner().ownerID 
    player:CastSkill{skillID = 1334} 
end

function onFactionTriggerItemUnequipped (self, msg)
    self:SendLuaNotificationCancel{requestTarget=self:GetItemOwner().ownerID, messageName="PlayerResurrectionFinished"}
end