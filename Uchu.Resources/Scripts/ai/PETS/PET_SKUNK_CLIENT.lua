--------------------------------------------------------------
-- Client Script on the skunks in Nimbus Station
-- sets the pets as untamable, and allows them to be tamed after spraying them with the watergun
-- 
-- created by Michael Edwards
-- updated mrb... 1/25/11 - refactored picking
-----------------------------------------------------------
  
-- Get the pick type of the skunk and change it to pickable/non pickable as needed
function onGetPriorityPickListType(self, msg)
	if not self:GetVar("success") and self:IsPetWild{}.bIsPetWild then
		local myPriority = 0.8
		
		if ( myPriority > msg.fCurrentPickTypePriority ) then 			
			if self:GetVar("bIAmTamable") == true then	
				msg.ePickType = 14      
			else
				msg.ePickType = -1
			end	

			msg.fCurrentPickTypePriority = myPriority
		end	    
	end
	
    return msg
end

-- Intercept a network sent variable to adjust pick type
function onScriptNetworkVarUpdate(self,msg)
    self:SetVar("bIAmTamable", msg.tableOfVars["bIAmTamable"])
    self:RequestPickTypeUpdate()
end

function onNotifyPetTamingMinigame(self,msg)
	-- if player completes the taming then set to normal picking
	if msg.notifyType == "SUCCESS" then
		self:SetVar("success", true)
		self:RequestPickTypeUpdate()
	end
end

