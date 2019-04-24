require('o_mis')

function onStartup(self)
	   
	self:SetVar("Slot_1", nil )
	self:SetVar("Slot_2", nil )
	self:SetVar("Slot_3", nil )
	self:SetVar("Slot_4", nil )
    self:SetVar("Snum", 1 ) 

	
end

function onCollisionPhantom (self,msg)

	 local target = msg.objectID
 	 if  target:GetFaction().faction == 1  and not ( self:GetVar("Snum") >= 5 )  then 

	  
            for i = 1,4 do
                
                if 	self:GetVar("Slot_"..i ) == nil then
                    self:SetVar("Slot_"..i, true )
                    
                      local spawnPoint = self:GetObjectsInGroup{ group = "spawn_grp_blue" }.objects
                      local spawnpos = spawnPoint[self:GetVar("Snum")]:GetPosition().pos
                                    
                      target:Teleport{pos = spawnpos }
                      target:SetFaction{faction = 7}
                                
                      t = self:GetVar("Snum") + 1
                      self:SetVar("Snum", t ) 
                             
                      break
                end
                
            end

	 end 

end


