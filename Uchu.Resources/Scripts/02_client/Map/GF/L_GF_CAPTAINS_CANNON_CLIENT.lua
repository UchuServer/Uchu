--------------------------------------------------------------
-- Client side script for the Captain Jack Cannon
--
-- updated abeechler ... 1/24/11 - modified to allow for use without inventory hook equipped
-- updated abeechler ... 1/25/11 - created a server side script to handle cannon processing porperly
--------------------------------------------------------------

local hookPreconditions = "154;44"

----------------------------------------------
-- Check to see if the player can use the cannon
----------------------------------------------
function onCheckUseRequirements(self, msg)
    
    local bIsInUse = self:GetNetworkVar('bIsInUse')
	if bIsInUse then
		-- If the interact is in use, we can break immediately and report failure
		msg.bCanUse = false
	else
		-- We have set-up a precondition for this interaction (hookPrecondition)
		-- check it to ensure interaction viability
		local player = msg.objIDUser
		local check = player:CheckListOfPreconditionsFromLua{PreconditionsToCheck = hookPreconditions}
		
		if(check.bPass == false) then
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = check.IconID
				msg.Script_Reason = check.FailedReason
				msg.Script_Failed_Requirement = true
			end
			msg.bCanUse = false
		end
    end
    
    return msg
end

----------------------------------------------
-- Catch 'use' var updates to properly clamp interaction ability
----------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
    local player = GAMEOBJ:GetControlledID()
    
    -- Check to see if we have the correct message and deal with it
    if msg.tableOfVars["bIsInUse"] ~= nil then 
        self:RequestPickTypeUpdate()
    end
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetNetworkVar('bIsInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 