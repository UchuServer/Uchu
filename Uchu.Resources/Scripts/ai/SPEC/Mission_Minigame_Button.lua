require('o_mis')
function onStartup(self)

	
	

end

-- minigame obj 4870
function onCollisionPhantom(self, msg)
    local minigame = getObjectByName(self, "minigame")
    if self:GetVar("Stored_Minigame") == nil then
    local obj = self:GetObjectsInGroup{ group = "grp_mission" }.objects
        for i = 1, #obj do      
            if obj[i]:GetLOT().objtemplate == 4870 then 
                storeObjectByName(self, "minigame", obj[i])
                self:SetVar("Stored_Minigame", true)
            end
        end
    end
    local minigame = getObjectByName(self, "minigame")
    local faction = msg.senderID:GetFaction()
    local mstate = msg.senderID:GetMissionState{missionID = 239}.missionState
    if faction and faction.faction == 1 and  self:GetVar("Stored_Minigame") and mstate == 2  and  not minigame:GetVar("running")  then
    
    	if self:GetLOT().objtemplate == 3706 then  -- blue
    		self:ShowEmbeddedEffect{type = "press"}
    		
    		local  idString = msg.senderID:GetID()
    		minigame:NotifyObject{ name = idString, param1 = 1 }
    	
    	end
    	if self:GetLOT().objtemplate == 3704 then  -- red    	
    	   self:ShowEmbeddedEffect{type = "press"}
    	   
     		local  idString = msg.senderID:GetID()
    		minigame:NotifyObject{ name = idString, param1 = 1 }   	
     	end
     	
    end
    
      

end


function onOffCollisionPhantom(self, msg)

    local minigame = getObjectByName(self, "minigame")
    local mstate = msg.senderID:GetMissionState{missionID = 239}.missionState
    local faction = msg.senderID:GetFaction()

    if faction and faction.faction == 1 and mstate == 2 and  not minigame:GetVar("running") then
    
    	if self:GetLOT().objtemplate == 3706 then  -- blue
    		self:ShowEmbeddedEffect{type = "stop"}
    		local  idString = msg.senderID:GetID()
    		minigame:NotifyObject{ name = idString, param1 = 2 }    		
    	
    	end
    	if self:GetLOT().objtemplate == 3704 then  -- red    	
    		self:ShowEmbeddedEffect{type = "stop"}
    		local  idString = msg.senderID:GetID()
    		minigame:NotifyObject{ name = idString, param1 = 2 }    		
     	end
     	
    end


end