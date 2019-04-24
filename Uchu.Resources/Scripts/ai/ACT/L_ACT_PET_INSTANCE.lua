--------------------------------------------------------------
-- Description:
--
-- Server script for Shooting Gallery NPC in GF area.
-- This NPC will react to a user interaction and prompt
-- the user to start the shooting gallery. If the user 
-- presses yes, the NPC will send him to the GF SG instance.
--
--------------------------------------------------------------

--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- User wants to start Pet Ranch, send him there
	if (msg.iButton == 1 and msg.identifier == "Pet_Ranch_Start") then

	    --msg.sender:Help{ iHelpID = 0 }

	    msg.sender:TransferToZone{ zoneID = 330 }

	end

end