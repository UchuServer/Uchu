--------------------------------------------------------------
-- Generic Client interactable script that checks bInUse networkvar
-- 
-- created mrb... 2/17/11
-- edited brandi... 6/13/11 - added base check requirements so this script can be required by another on
--------------------------------------------------------------

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse
----------------------------------------------
function onCheckUseRequirements(self, msg)   
	baseCheckUseRequirements(self,msg)
end

function baseCheckUseRequirements(self,msg)
	if self:GetNetworkVar("bInUse") then		
		msg.bCanUse = false
	else
		-- Obtain preconditions
		local preConVar = self:GetVar("CheckPrecondition")
    
		if preConVar and preConVar ~= "" then
			-- We have a valid list of preconditions to check
			local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
        
			if not check.bPass then 
				-- Failed the precondition check
				if msg.isFromUI then
					msg.HasReasonFromScript = true
					msg.Script_IconID = check.IconID
					msg.Script_Reason = check.FailedReason
					msg.Script_Failed_Requirement = true
				end
			
				msg.bCanUse = false
			end
		end
    end

    return msg
end

function onScriptNetworkVarUpdate(self,msg) 
    for msgName,value in pairs(msg.tableOfVars) do
        if msgName == "bInUse" then
			if value then
				local player = GAMEOBJ:GetControlledID()
				
				if player:GetPlayerInteraction().interaction:GetID() == self:GetID() then
					-- termainate the players interaction
					player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}  
				end
			end
			
            -- update the pick type and terminate the interaction
			self:RequestPickTypeUpdate()
		end
	end
end

function onGetPriorityPickListType(self, msg)
	if self:GetNetworkVar("bInUse") then
		msg.ePickType = -1
	else
		local myPriority = 0.8
			
		if ( myPriority > msg.fCurrentPickTypePriority ) then
		   msg.fCurrentPickTypePriority = myPriority
		   msg.ePickType = 14    -- Interactive pick type
		end
	end

    return msg
end 