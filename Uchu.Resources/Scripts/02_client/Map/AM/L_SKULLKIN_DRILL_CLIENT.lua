--------------------------------------------------------------
-- Client side script for the skullkin drill
--
-- updated mrb... 1/6/11 - moved freezePlayer to server
--------------------------------------------------------------
local preconditionID = 143

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse
----------------------------------------------
function onCheckUseRequirements(self, msg)
    if self:GetNetworkVar('bIsInUse') then 
        msg.bCanUse = false
        
        return msg
    end
    
    local preConVar = self:GetVar("CheckPrecondition")
    
    if preConVar and preConVar ~= "" then
        local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
        
        if not check.bPass then 
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = check.IconID
				msg.Script_Reason = check.FailedReason
				msg.Script_Failed_Requirement = true
			end
			
            msg.bCanUse = false
            
            return msg
        end
    end
    
    local check = {}
    
	check = msg.objIDUser:CheckPrecondition{PreconditionID = preconditionID}
		
	if not check.bPass then 
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
function onScriptNetworkVarUpdate(self, msg)
    local player = GAMEOBJ:GetControlledID()	
	for varName,varValue in pairs(msg.tableOfVars) do
		-- check to see if we have the correct message and deal with it
		if varName == "bIsInUse" then 
			self:RequestPickTypeUpdate()
			if not varValue then				player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}			end		end
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