
function onMessageBoxRespond(self, msg)
    if (msg.iButton == 1) then
         msg.sender:TransferToZone{ zoneID = 430 ,  ucInstanceType = 1 } 

    end
    
end

function onUse(self, msg)

    -- get player who clicked on us
	local player = msg.user
	self:Help{rerouteID = player, iHelpID = 0}
  
end
