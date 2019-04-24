--------------------------------------------------------------
-- Client script for teapot in aura mar
-- give the player consumable tea for tea leaves

-- created by brandi.. 10/28/10
-- updated by brandi.. 12/4/11 - updated icons for check use requirements
--------------------------------------------------------------

local numleaves = 10
		
-- checks to see if the player is eligible to use the console
function onCheckUseRequirements(self,msg)

	local player = msg.objIDUser
	if not player:Exists() then return end
	-- has the player completed the mission 976, collect tea leaves
	if player:GetMissionState{missionID = 976}.missionState < 8 then 
		msg.HasReasonFromScript = true  
		msg.Script_IconID = 4733 
		msg.Script_Reason = Localize("CP_TALK_TO_SENSEI_WU_TEAPOT") 
		msg.Script_Failed_Requirement = true  
		msg.bCanUse = false  
	-- checks that the player enough leaves for tea
	elseif player:GetInvItemCount{ iObjTemplate = 12317}.itemCount < numleaves then
		msg.HasReasonFromScript = true  
		msg.Script_IconID = 4060  
		msg.Script_Reason = Localize("CP_NOT_ENOUGH_TEA_LEAVES") 
		msg.Script_Failed_Requirement = true  
		msg.bCanUse = false 
	end
	return msg
	
end

-- This function is called when the object starts up or someone requests a pick type update
-- Handling this to set pick type on an object, which makes it able to be interactive
function onGetPriorityPickListType(self, msg)  

    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        
        msg.fCurrentPickTypePriority = myPriority 
 
        msg.ePickType = 14    -- Interactive pick type (Setting to -1 makes something non-interactive) 

    end  
  
    return msg   
	
end 

