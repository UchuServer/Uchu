--///////////////////////////////////////////////////////////////////////////////////////
--//            Team Awesomer NPC -- SERVER Script
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Sent from a player when trying to use this object
--------------------------------------------------------------
function onUse(self, msg)

    local strText = "Want to Race?"
    
	-- show a dialog box
	msg.user:DisplayMessageBox{bShow = true,
							   imageID = 3,
							   callbackClient = self,
							   text = strText,
							   identifier = "Race_Dialog"}
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- User wants to start shooting gallery, send him there
	if (msg.iButton == 1 and msg.identifier == "Race_Dialog") then

		-- tell zone control to prep the player to race
		GAMEOBJ:GetZoneControlID():MessageBoxRespond{iButton = 1, identifier = "Race_Mission", sender = msg.sender}

	end

end