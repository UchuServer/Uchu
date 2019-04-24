--------------------------------------------------------------
-- Client Zone script to display spinjitzu required message

-- created mrb... 1/21/11
--------------------------------------------------------------
function onStartup(self, msg)
	spinjitzuSetIcon(self)
end

function spinjitzuSetIcon(self)
	self:SetProximityRadius{radius = 8, iconID = 86}
end

function onGetInteractionDetails(self, msg)
	local spinMsg = spinjitzuGetInteractionDetails(self, msg)
    
    return spinMsg
end 

function spinjitzuGetInteractionDetails(self, msg)
	msg.IconID = 3384
	msg.TextDetails = Localize("Preconditions_153_FailureReason")
    
    return msg
end 