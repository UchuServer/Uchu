--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes, this is called.
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)    
	if (msg.iState == 2) then
	     -- Set to Darkling hated smashable faction
	    self:SetFaction{ faction = 16 }        
	    
        local MortarList  = self:GetObjectsInGroup{group = "Mortar_"..self:GetVar("target")}.objects
        
        local awesome = self:GetVar("target")
   
        
        for i = 1, #MortarList do 
            MortarList[i]:CastSkill{skillID = 318 ,optionalTargetID = self }
		end
        
	end	
end