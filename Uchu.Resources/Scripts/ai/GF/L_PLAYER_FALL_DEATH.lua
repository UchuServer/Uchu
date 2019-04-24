--------------------------------------------------------------
-- Script for the falling volumes
-- updated Brandi... 1/29/10
--------------------------------------------------------------
require('o_mis')

local effectPlayed = false

function onCollisionPhantom(self, msg)
	local player = msg.objectID
	
	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--setting a player flag so if a player falls though two+ volumes, the camera only gets set once
		if effectPlayed == false then
			self:SendLuaNotificationRequest{requestTarget=player, messageName="Resurrect"}
			self:SendLuaNotificationRequest{requestTarget=player, messageName="PlayerReady"}

			effectPlayed = true
            
            CAMERA:SetToPrevGameCam()
            
			local config = { {"objectID", msg.objectID:GetID()}, {"leadIn", 0.1}, {"leadOut", 0}, {"lag", 0.1}, {"lockPos", true} }

			msg.objectID:AddCameraEffect{ effectType = "lookAt", effectID = "lookatFall", configData = config }
			storeObjectByName(self, "player", player)

		end
	end

end

function notifyResurrect(self, player, msg)
        effectPlayed = false
        player:RemoveCameraEffect{ effectID = "lookatFall" }
		self:SendLuaNotificationCancel{requestTarget=player, messageName="Resurrect"}
end

function notifyPlayerReady(self, player, msg)
        effectPlayed = false
        player:RemoveCameraEffect{ effectID = "lookatFall" }
        self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function onShutdown(self)
	local player = getObjectByName(self, "player")
	if player then
		player:RemoveCameraEffect{ effectID = "lookatFall" }
    end
end

