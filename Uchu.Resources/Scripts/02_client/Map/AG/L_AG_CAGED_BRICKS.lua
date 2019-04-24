--------------------------------------------------------------
-- caged spider
-- Created mrb... 6/7/11
--------------------------------------------------------------

local sCameshake = "npcdeathshake"
local shakefxID = 1039
local explodefxID = 2578
local preconID = "188;189"
local flagID = 74

function onStartup(self)
	local player = GAMEOBJ:GetControlledID()
	
	if not player:GetFlag{iFlagID = flagID}.bFlag then return end
	
	GAMEOBJ:DeleteObject(self)
end

function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
		msg.ePickType = 14    -- Interactive pick type
    end  
  
    return msg
end 

function onCheckUseRequirements(self, msg)
	local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preconID}

	-- We don't need to report most of this information unless this check is coming from the UI
	if not check.bPass then --check mission
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_Failed_Requirement = true
			msg.Script_Reason = check.FailedReason
			msg.Script_IconID = check.IconID
		end
		
		msg.bCanUse = false
	end
	
	return msg
end

function onClientUse(self, msg)
	local player = GAMEOBJ:GetControlledID()
	
	if msg.user:GetID() == player:GetID() then
		-- play fx 
		player:PlayFXEffect{name = "camshake", effectType = sCameshake, effectID = shakefxID}
		self:PlayFXEffect{name = "boom", effectType = "cast", effectID = explodefxID}		
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.25, "delete", self)
	end
end 

function onTimerDone(self, msg)
	if msg.name == "delete" then
		GAMEOBJ:DeleteObject(self)	
	end
end 