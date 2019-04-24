--///////////////////////////////////////////////////////////////////////////////////////
--//            Rebuild Tutorial NPC -- CLIENT Script
--///////////////////////////////////////////////////////////////////////////////////////

CONSTANTS = {}
CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"] = 0
CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"] = 1
CONSTANTS["MINIFIG_BODY_TORSO"] = 1
CONSTANTS["MINIFIG_BODY_LEGS"] = 2

function onRenderComponentReady(self, msg) 

	-- change torso (red plain shirt)
	self:SwapDecalAndColor{decalIndex = 0, 
	                       color = 0, 
	                       bodyPiece = CONSTANTS["MINIFIG_BODY_TORSO"]}

	-- change legs (blue pants)
	self:SwapDecalAndColor{decalIndex = 0, 
	                       color = 2, 
	                       bodyPiece = CONSTANTS["MINIFIG_BODY_LEGS"]}
	
end

function onHelp(self, msg)

	if msg.iHelpID == CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"] then
		UI:DisplayToolTip{strDialogText = "Click this to start rebuilding.", strImageName = "", bShow=true, iTime=0}
	elseif msg.iHelpID == CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"] then
		UI:DisplayToolTip{strDialogText = "Follow the path and build the next one. You must build it in order!", strImageName = "", bShow=true, iTime=7000}
	end
	
end


--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 4
	return msg

end