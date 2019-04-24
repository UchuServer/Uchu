--------------------------------------------------------------
-- Client side script on spawned pets this script controls the 
-- icon and interactablity of the spawned pets only the player 
-- who spawned the pet to see the interact icon above it and be able to tame it

-- created by Brandi... 2/17/10
-- updated mrb... 1/17/11 - return msg
--------------------------------------------------------------


function onGetPriorityPickListType(self, msg)
    local petInfo = self:GetIsPet()
    
	-- if the pet is someones tamed pet, ignore the rest of the script
	if (not petInfo.bIsWild) or (petInfo.OwnerID == GAMEOBJ:GetControlledID()) then
		return msg
	end
	
	local myPriority = 0.8
	
    if ( myPriority > msg.fCurrentPickTypePriority ) then	
        local player = GAMEOBJ:GetControlledID()		
        
        if player:Exists() then		
			msg.fCurrentPickTypePriority = myPriority
			
			-- if the player is the player who spawned the pet, then set the lion to be interactable to that 
			if self:GetVar("localPetTamer") == player:GetID() then			
				-- Interactive pick type 	
				msg.ePickType = 14 
			end			
		else			
			--if the player is not the player who spawned the lion, the player can't interact with the lion
			msg.ePickType = -1			
		end

    end
	
    return msg	
end


-------------------------------------------------------------
-- Process server object process calls
-------------------------------------------------------------
function onNotifyClientObject(self, msg)

	if msg.name == "UpdatePicking" then
		self:RequestPickTypeUpdate()
	end	
	
end


function onScriptNetworkVarUpdate(self,msg)	
	local pettamer = msg.tableOfVars["pettamer"]
	
	if pettamer then    	
		local player = GAMEOBJ:GetControlledID()
		
		if player:GetID() == pettamer then 		
			self:SetVar("localPetTamer",player:GetID())
			self:RequestPickTypeUpdate()			
		end		    
	end
end 
