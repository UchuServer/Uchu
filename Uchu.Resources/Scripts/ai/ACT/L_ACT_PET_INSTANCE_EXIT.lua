--------------------------------------------------------------
-- Description:
--
-- This NPC allows the player to get out of the pet instance and back to YouReeka
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)

    local strText = "Exit Pet Ranch?"
    
	-- show a dialog box
	msg.user:DisplayMessageBox{bShow = true,
							   imageID = 3,
							   callbackClient = self,
							   text = strText,
							   identifier = "Pet_Ranch_Exit"}
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- User wants to start shooting gallery, send him there
	if (msg.iButton == 1 and msg.identifier == "Pet_Ranch_Exit") then

	    msg.sender:TransferToZone{ zoneID = 312, pos_x = -199, pos_y = 302, pos_z = -103 }

	end

end