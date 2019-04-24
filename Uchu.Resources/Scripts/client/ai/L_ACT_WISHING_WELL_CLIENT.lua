--------------------------------------------------------------

-- L_ACT_WISHING_WELL_CLIENT.lua

-- Client side script for the Wishing Well
-- updated abeechler ... 2/8/11 - refactored script for proper client/server behavior

--------------------------------------------------------------

local wishPrecondition = 165		-- Interact precondition = player needs 1 red Imaginite

----------------------------------------------
-- Check to see if the player can use the cannon
----------------------------------------------
function onCheckUseRequirements(self, msg)
	local player = msg.objIDUser
	
	-- We have set-up a precondition for this interaction (wishPrecondition)
	-- check it to ensure interaction viability
	local check = player:CheckPrecondition{PreconditionID = wishPrecondition}
		
	if(check.bPass == false) then
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_IconID = check.IconID
			msg.Script_Reason = check.FailedReason
			msg.Script_Failed_Requirement = true
		end
		msg.bCanUse = false
	end
    
    return msg
end

function onGetInteractionDetails(self, msg)
	msg.TextDetails = Localize("WISHING_WELL_COST")
	
	return msg
end
----------------------------------------------
-- Servers tell the client script when it is on/off
-- cooldown to appropiately set the interact state
----------------------------------------------
function onNotifyClientObject(self,msg)
	if msg.name == "StartCooldown" then
		-- Object is ON cooldown
		self:SetVar("bIsOnCooldown", true)
	elseif msg.name == "StopCooldown" then
		-- Object is OFF cooldown
		self:SetVar("bIsOnCooldown", false)
	end
	
	self:RequestPickTypeUpdate()
end

----------------------------------------------
-- Sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar("bIsOnCooldown") then
            msg.ePickType = -1
        else
			msg.ePickType = 14    -- Interactive pick type
		end
    end  
  
    return msg      
end 