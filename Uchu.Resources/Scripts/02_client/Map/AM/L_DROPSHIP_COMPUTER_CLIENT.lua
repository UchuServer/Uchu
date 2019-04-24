---------------------------------------------
-- PROTOTYPE: Client script on the drop ship computer
-- handles the use requirements
-- 
-- created by brandi... 11/9/10
-- updated by mrb... 1/3/11 - adjusted interaction icon location
---------------------------------------------



function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
    if ( myPriority > msg.fCurrentPickTypePriority ) then
       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type
    end
    return msg
end 

-------------------------------------------
-- see if the plaque is in use before allowing the player from using it again
-------------------------------------------
function onCheckUseRequirements(self, msg)
	-- check to see if the quickbuild is built
	if self:GetRebuildState{}.iState == 2 then
		local player = msg.objIDUser
		if not player:Exists() then return end
		--check to make sure the player has at least accepted the  mission to the use the dropship computer
		if player:GetMissionState{missionID = 979}.missionState < 2 then 
			msg.HasReasonFromScript = true  
			msg.Script_IconID = 3275  
			msg.Script_Reason = Localize("CP_TALK_TO_WOUNDED_PILOT")   
			msg.Script_Failed_Requirement = true  
			msg.bCanUse = false  
			return msg	
		end
	end
end

