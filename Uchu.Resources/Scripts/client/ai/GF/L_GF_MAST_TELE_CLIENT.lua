--------------------------------------------------------------
-- Client side script for the QB Pirate Mast
--
-- updated abeechler ... 1/24/11 - modified to allow for use without inventory hook equipped
-- updated abeechler ... 1/25/11 - refactored swing scripts to enforce server side functionality
--------------------------------------------------------------

----------------------------------------------
-- Check to see if the player can use the hook-swing
----------------------------------------------
function onCheckUseRequirements(self, msg)
    local player = msg.objIDUser
    local hookPreconditions = self:GetNetworkVar("hookPreconditions")
    
    -- We have set-up a precondition for this interaction (hookPrecondition)
    -- check it to ensure interaction viability
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
    
    return msg
end
