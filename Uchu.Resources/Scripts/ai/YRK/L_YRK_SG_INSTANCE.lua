--------------------------------------------------------------
-- Description:
--
-- Server script for Shooting Gallery NPC in ZP area.
-- This NPC will react to a user interaction and prompt
-- the user to start the shooting gallery. If the user 
-- presses yes, the NPC will send him to the ZP SG instance.
--
--------------------------------------------------------------

function onStartup(self)
	self:SetProximityRadius{radius = 15}
end

--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)

    local strText = "Start Shooting Gallery?"
    
	-- show a dialog box
	msg.user:DisplayMessageBox{bShow = true,
							   imageID = 3,
							   callbackClient = self,
							   text = strText,
							   identifier = "Shooting_Gallery_Start"}
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- User wants to start shooting gallery, send him there
	if (msg.iButton == 1 and msg.identifier == "Shooting_Gallery_Start") then

	    msg.sender:TransferToZone{ zoneID = 335, ucInstanceType = 1 }

	end

end

function onProximityUpdate(self, msg)
	if msg.status == "LEAVE" then
			msg.objId:DisplayMessageBox{bShow = false,
							   --imageID = 3,
							   callbackClient = self,
							   --text = strText,
							   identifier = "Shooting_Gallery_Start"}
	end
end