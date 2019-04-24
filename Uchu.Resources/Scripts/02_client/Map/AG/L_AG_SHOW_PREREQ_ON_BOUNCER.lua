--------------------------------------------------------------
-- Client script to display required message on the bouncer

-- created mrb... 6/2/11
--------------------------------------------------------------
function onStartup(self)
	-- Obtain preconditions
	local preConVar = self:GetVar("CheckPrecondition")

	if preConVar and preConVar ~= "" then
		-- We have a valid list of preconditions to check
		local check = GAMEOBJ:GetControlledID():CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}

		if check.bPass then 
			self:SetVar("bPass", true)
		end
	end
end

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse
----------------------------------------------
function onCheckUseRequirements(self, msg)    
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
		else
			self:SetVar("bPass", true)
			self:RequestPickTypeUpdate()
		end
    end
	
    return msg
end

function onGetInteractionDetails(self, msg)
	msg.IconID = 3572
	msg.TextDetails = Localize("Preconditions_244_FailureReason")
    
    return msg
end 

function onGetPriorityPickListType(self, msg)
	if self:GetVar("bPass") then
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