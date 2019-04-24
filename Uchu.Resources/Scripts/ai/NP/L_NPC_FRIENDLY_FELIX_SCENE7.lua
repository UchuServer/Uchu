--------------------------------------------------------------
-- Description:
--
-- Server script for Felix 7. Ports player to next zone
--------------------------------------------------------------


--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)

    local strText = "Would you like to travel to Youreeka?"
    
	-- show a dialog box
	msg.user:DisplayMessageBox{bShow = true,
							   imageID = 2,
							   callbackClient = self,
							   text = strText,
							   identifier = "Travel_To_Youreeka"}
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- User wants to start shooting gallery, send him there
	if (msg.iButton == 1 and msg.identifier == "Travel_To_Youreeka") then

	    msg.sender:TransferToZone{ zoneID = 312 }

	end

end