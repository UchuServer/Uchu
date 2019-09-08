--------------------------------------------------------------
-- prototype: this script isn't done
-- L_NT_CONSOLE_TELEPORT_CLIENT.lua

-- Client Script for the a Teleport Console interact
-- Destination = Crux Prime
-- Created abeechler... 2/23/11

--------------------------------------------------------------
require('L_BASE_CONSOLE_TELEPORT_CLIENT')

local teleportTooltip = "CP_TOOLTIP_NEXUS_TOWER_TRANSPORT" -- need to localize

local objAlertIconID = 153		-- Object Proximity Interact Icon ID 

----------------------------------------------
-- Adjust the interact display icon on set-up
----------------------------------------------
function onStartup(self,msg)
	self:SetOverheadIconOffset{vertOffset = 3.25}
	-- Set AM Console Variables
	self:SetVar("teleportTooltip", teleportTooltip)
	self:SetVar("objAlertIconID", objAlertIconID)
	
	baseStartup(self, msg)
end

----------------------------------------------
-- Check to see if the player can use the console
----------------------------------------------
function onCheckUseRequirements(self, msg)
	baseCheckUseRequirements(self, msg)
	
	return msg
end

----------------------------------------------
-- Code script info request for the custom npc icon display
----------------------------------------------
function onGetInteractionDetails(self, msg) 
	baseGetInteractionDetails(self, msg)
	
	return msg
end

----------------------------------------------
-- Checking for distance based termination of interaction
-- to ensure proper shtudown of open interaction windows
----------------------------------------------
function onTerminateInteraction(self, msg) 
	baseTerminateInteraction(self, msg)
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	baseProximityUpdate(self, msg)
end

----------------------------------------------
-- Sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
	 local myPriority = 0.8
  
    if (myPriority > msg.fCurrentPickTypePriority) then    
        msg.fCurrentPickTypePriority = myPriority 
		
		msg.ePickType = 14    -- Interactive pick type     

        return msg  
    end  
end
