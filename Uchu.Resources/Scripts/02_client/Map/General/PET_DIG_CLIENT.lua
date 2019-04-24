--------------------------------------------------------------
-- Client side script on the pet dig
--
-- updated abeechler ... 6/30/11 - free trial pre-con check
--------------------------------------------------------------
local specificPetLOTs = {}
local noPetString = ""
local noPetIcon = 0
local missionRequirements = {}

function setPetVariables(passedspecificPetLOTs,passednoPetString,passednoPetIcon,passedmissionRequirements)
	specificPetLOTs = passedspecificPetLOTs
	noPetString = passednoPetString
	noPetIcon = passednoPetIcon
	missionRequirements = passedmissionRequirements
end

function onScriptNetworkVarUpdate(self,msg)			
	for k,v in pairs(msg.tableOfVars) do
        -- start the qb smash fx
        if k == "treasure_dug" and v then
			--print("treasure_dug is true!!!")
			self:PlayFXEffect{effectType = "dug_up"}
			self:SetTransparency{fAlphaValue = 0.0}	-- change to set visible false
			self:SetVar("DigDead",true)
			self:RequestPickTypeUpdate{}
			
			local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
			local pet = player:GetPetID().objID
			
			if IsLocalPet(self, pet) then			
				-- reset pickable
				pet:RequestPickTypeUpdate{}
			end
		end
	end
end

function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
	if( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
		if self:GetVar("DigDead") then
			msg.ePickType = -1
		else	
			msg.ePickType = 14    -- Interactive pick type 
		end
    end  
      
	return msg  
end

function onCheckUseRequirements( self, msg )
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	local pet = player:GetPetID().objID
    
    -- Free trial status check
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
    
	-- We don't need to report most of this information unless this check is coming from the UI
	if player:GetMissionState{missionID = 110}.missionState < 4 then --check mission
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_Failed_Requirement = true
			msg.Script_Reason = Localize("PR_DIG_TUTORIAL_04")
			msg.Script_IconID = 3069
		end
		
		msg.bCanUse = false
	elseif player:GetMissionState{missionID = 842}.missionState < 8 then --check mission
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_Failed_Requirement = true
			msg.Script_Reason = Localize("PR_DIG_TUTORIAL_02")
			msg.Script_IconID = 3069
		end
		
		msg.bCanUse = false
	elseif not pet or not pet:Exists() then
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_Failed_Requirement = true
			msg.Script_Reason = Localize("MUST_HAVE_PET_OUT")
			msg.Script_IconID = 3069
		end
		
		msg.bCanUse = false
	elseif table.maxn(specificPetLOTs) ~= 0 then
		local petLOT = pet:GetLOT().objtemplate
		local IsPet = false
		
		for k,v in ipairs(specificPetLOTs) do
			if v == petLOT then
				IsPet = true
				
				break
			end
		end
				
		if IsPet then						
			if table.maxn(missionRequirements) ~= 0 then
				for k,v in ipairs(missionRequirements) do
					if player:GetMissionState{missionID = v.ID}.missionState == v.state then --check mission
						if msg.isFromUI then
							msg.HasReasonFromScript = true
							msg.Script_Failed_Requirement = true
							msg.Script_Reason = Localize(v.string)
							msg.Script_IconID = v.icon
						end
						
						msg.bCanUse = false
					end
				end
			end
		else
			if msg.isFromUI then			
				msg.HasReasonFromScript = true  
				msg.Script_IconID = noPetIcon
				msg.Script_Reason = Localize(noPetString) 
				msg.Script_Failed_Requirement = true  
			end
			
			msg.bCanUse = false		
		end	
	end
	
	if msg.bCanUse and not pet:GetPetHasState{iStateType = 6}.bHasState then
		if msg.isFromUI then			
			msg.HasReasonFromScript = true  
			msg.Script_IconID = 3069
			msg.Script_Reason = Localize("UI_PET_WAIT_MESSAGE") 
			msg.Script_Failed_Requirement = true  
		end
		
		msg.bCanUse = false		
	end
	
	return msg
end

function IsLocalPet(self, petObj)
	-- get if pet
	local petInfo = petObj:GetIsPet()
	
	if not petInfo.bIsPet or petInfo.bIsWild then
		-- if we're not a pet or a wild pet then return false
		return false
	elseif petInfo.OwnerID:GetID() ~= GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetID() then
		-- if we're not the local players pet then return false	
		return false
	end
	
	-- this is the local players pet
	return true
end

function onCollisionPhantom(self, msg)
	if not IsLocalPet(self, msg.objectID) then return end
	
	-- set pet unpickable
	msg.objectID:SetPickType{ePickType = -1}
end


function onOffCollisionPhantom(self, msg)
	if not IsLocalPet(self, msg.objectID) then return end
	
	-- reset pickable
	msg.objectID:RequestPickTypeUpdate{}
end 