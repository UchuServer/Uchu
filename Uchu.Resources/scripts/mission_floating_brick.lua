require('o_mis')

function onStartup(self) 

	GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "killself", self )  
	
	  self:ShowEmbeddedEffect{type = "trail"}
	  
	   local obj = self:GetObjectsInGroup{ group = "grp_mission" }.objects
        for i = 1, #obj do      
            if obj[i]:GetLOT().objtemplate == 4870 then 
                storeObjectByName(self, "minigame", obj[i])
                self:SetVar("Stored_Minigame", true)
            end
        end
   
	  
end


function onTimerDone(self,msg)


	if (msg.name == "killself") then
	
		self:Die{ killerID = self }
	end

end


function onCollision(self, msg)
	local minigame = getObjectByName(self, "minigame")
	local target = msg.objectID
	local mstate = target:GetMissionState{missionID = 239}.missionState
	local faction = target:GetFaction()
	if faction and faction.faction == 1 and mstate == 2 and  minigame:GetVar("running") then
            self:CastSkill{skillID = 13, optionalTargetID = target}
            local count = target:GetInvItemCount{itemID = 4985}.itemCount
            	if count <= 14 then
            		target:AddNewItemToInventory{ iObjTemplate = 4985 }
            	end
            self:Die{ killerID = msg.playerID}
            
            
            local count = target:GetInvItemCount{itemID = 4985}.itemCount
            minigame:NotifyClientZoneObject{name = "sendToclient_bubble" , paramStr = tostring(count), paramObj = target }         
                    
            
            
	end       	
	msg.ignoreCollision = true
  return msg
end


