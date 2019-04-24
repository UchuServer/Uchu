--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["TOOLTIP_ACTIVITY_SHOOTING_GALLERY_HELP_BIT"] = 3
CONSTANTS["HELP_SCREEN_TEXT"] = "SG1"


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onPlayerReady(self) 
    self:SetVar("PlayerReady", true)
	-- get local player
	local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )

	-- check the tooltip flag	
	local tooltipMsg = player:GetTooltipFlag{iToolTip = CONSTANTS["TOOLTIP_ACTIVITY_SHOOTING_GALLERY_HELP_BIT"]}
	if (tooltipMsg.bFlag == false) then
	
		-- set the tooltip?
	else
	
		-- fake the msgbox close to start the activity
		GAMEOBJ:GetZoneControlID():MessageBoxRespond{ identifier = CONSTANTS["HELP_SCREEN_TEXT"], sender = player }
		
	end
	
end

function onNotifyClientZoneObject(self,msg)
    -- UpDate Timer UI -- 
    if msg.name == "count" and self:GetVar("PlayerReady") ~= nil then
        UI:SendMessage("ChageUI", { {"sgTimer", msg.parmar1 } })
    end

end

