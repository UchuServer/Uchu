----------------------------------------
-- Client script for the dragons golem
--
-- created mrb... 3/10/11
----------------------------------------

function onCheckUseRequirements(self, msg)
	local tagObj = self:GetNetworkVar("lootTagOwner") or false
	
	if tagObj and tagObj:Exists() then
		if tagObj:GetID() ~= msg.objIDUser:GetID() and not msg.objIDUser:TeamIsOnWorldMember{i64PlayerID = tagObj}.bResult then
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_Failed_Requirement = true
				msg.Script_Reason = Localize("NOT_YOUR_TEAMS_QUICKBUILD")
				msg.Script_IconID = 3509
			end
		
			msg.bCanUse = false
		end
	end
    
    return msg
end 