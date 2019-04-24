
function onCollision(self, msg)
	local target = msg.objectID

	local faction = target:GetFaction()
	

	if faction and  (faction.faction == 101) then
	 
	 	    self:ShowEmbeddedEffect{type = "pickup"}
		  
	        local itemMsg =  target:AddNewItemToInventory{ iObjTemplate = 2956 }
	        
		    target:EquipInventory{ itemtoequip = itemMsg.newObjID }   
		
		
            self:Die{ killerID = msg.playerID, killType = "SILENT" }
	
	    
	end
    if faction and  (faction.faction == 100)  and not self:GetVar("home") then




    end   	
	-- Ignore this on the server actually. More of a test than needed
	msg.ignoreCollision = true
	-- ONly do this once
  return msg
end

function onStartup(self)
      self:SetVar("home", true)
end

