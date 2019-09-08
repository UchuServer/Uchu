--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in GF area.
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------
require('c_CannonInstancer')

function onStartup(self)

    Initialize(self)
    self:SetProximityRadius{radius = 15}

end








function onProximityUpdate(self, msg)

	if msg.status == "LEAVE" then
			local player = msg.objId
			local localPlayer = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID())
			
			if player:GetName().name == localPlayer:GetName().name then
				UI:SendMessage("HideHT", {{"HIDE",  true }} )
			end

	end
	
end


function onNotifyClientObject(self,msg)


	if msg.name == "Clicked" and GAMEOBJ:GetLocalCharID() == msg.paramObj:GetID() then
	
		local player = msg.paramObj:GetID()
	    local finalID = "|" .. player
	    UI:SendMessage("EnterSG", {{"user", finalID  }} )

	end
	if msg.name == "PreconditionFail" and GAMEOBJ:GetLocalCharID() == msg.paramObj:GetID() then
	
		msg.paramObj:DisplayTooltip{ bShow = true, strText = "Collect cannonballs to play the Shooting Gallery!", iTime = 0 }
	
	end



end

function onGetOverridePickType(self, msg)
	msg.ePickType = 14	--Interactive type
	return msg

end
