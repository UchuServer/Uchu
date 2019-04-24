--------------------------------------------------------------
-- Description:
--
-- Server script for Guild Master NPC in FV area.
-- This NPC will react to a user interaction and display
-- the guild creation screen.


--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)
    
	-- show a dialog box
	msg.user:DisplayGuildCreateBox{bShow}
end

