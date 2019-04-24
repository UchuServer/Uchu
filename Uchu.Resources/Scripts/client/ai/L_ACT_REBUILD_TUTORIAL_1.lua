--///////////////////////////////////////////////////////////////////////////////////////
--//            Rebuild Tutorial -- CLIENT Script
--///////////////////////////////////////////////////////////////////////////////////////
    
function onHelp(self, msg) 
    
	if msg.iHelpID == 0 then
		UI:DisplayToolTip{strDialogText = "Stand near the Piece and click it to pick it up.", strImageName = "", bShow=true, iTime=0}
	elseif msg.iHelpID == 1 then
		UI:DisplayToolTip{strDialogText = "Now click the blinking part on the rebuild to place the Piece.", strImageName = "", bShow=true, iTime=0}
	end
	
end
