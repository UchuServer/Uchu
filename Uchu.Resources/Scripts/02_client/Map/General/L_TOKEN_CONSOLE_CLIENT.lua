--------------------------------------------------------------
-- client side script on the faction token console 
-- sets the console to be interactive and checks any preconditions
-- can be used as a stand alone script, or added as a require script
-- 
-- created by brandi.. 4/14/11
--------------------------------------------------------------


function onCheckUseRequirements(self, msg)    
	return baseCheckUseRequirements(self,msg)
end

function baseCheckUseRequirements(self,msg)

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

    return msg
end

function onGetInteractionDetails(self, msg)    
    return baseGetInteractionDetails(self,msg)
end

function baseGetInteractionDetails(self,msg)
	msg.TextDetails = Localize("TRADE_MAELSTROM_BRICKS_FOR_TOKENS")
	
	return msg
end

function onGetPriorityPickListType(self, msg)  
	return baseGetPriorityPickListType(self, msg)
end

-- This function is called when the object starts up or someone requests a pick type update
-- Handling this to set pick type on an object, which makes it able to be interactive
function baseGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        
        msg.fCurrentPickTypePriority = myPriority 
 
        msg.ePickType = 14    -- Interactive pick type (Setting to -1 makes something non-interactive) 

    end  
  
    return msg   	
end 
