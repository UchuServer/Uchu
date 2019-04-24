--------------------------------------------------------------
-- client side script on the object to spawn the lion pet
-- this script lets the player know why they cant spawn more lions from information
-- set from the server

-- created by Brandi... 3/2/11
--------------------------------------------------------------

local TooManyPetsText = ''
local TooManyIcon = 0

--------------------------------------------------------------
-- if a script is attached, call SetVariables
--------------------------------------------------------------
function baseStartup(self,msg)
	SetVariables(self)
end

--------------------------------------------------------------
-- called when the render component is done loading
--------------------------------------------------------------
function onRenderComponentReady(self,msg)
	if TooManyPetsText == '' then
		SetVariables(self)
	end
end

--------------------------------------------------------------
-- Custom Function: pull in variables either from the script on the pet, or they can be set has config data on the object
-- the player interacts with to spawn in the pet
--------------------------------------------------------------
function SetVariables(self)
	TooManyPetsText = self:GetVar("TooManyPetsText") or "PET_SUMMON_FAIL"
	TooManyIcon = self:GetVar("TooManyPetsIcon") or 2977
end

----------------------------------------------
-- Check to see if the player can use the console
----------------------------------------------
function onCheckUseRequirements(self, msg)
	-- Determine if interaction is valid based on current use
	
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
			return msg
		end
	end
		
	-- find out if there is already too many pets out, or if the local player already has a pet out
	if(self:GetNetworkVar("TooManyPets")) or self:GetVar("playerPetAlready") then 
		
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = TooManyIcon
				msg.Script_Reason = Localize(TooManyPetsText)
				msg.Script_Failed_Requirement = true
			end
			msg.bCanUse = false 
		
	end
    return msg
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
		
		msg.ePickType = 14    -- Interactive pick type     

        return msg  
    end  
end

----------------------------------------------
-- server sends a notification about the pets spawned for the specific client
----------------------------------------------
function onNotifyClientObject(self,msg)

	if msg.name == "tooManyPets" then
		-- 1 means the player spawned a pet
		if msg.param1 == 1 then
			self:SetVar("playerPetAlready",true)
		-- otherwise the pet reserved for the player despawned
		else
			self:SetVar("playerPetAlready",false)
		end
	end
	-- update the use icon
	self:RequestPickTypeUpdate()	
end

