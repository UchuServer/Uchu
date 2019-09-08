require('o_mis')


function onStartup(self)
    self:SetVar("INUSE", false)
         
end


function onCollision(self, msg)
   

		local target = msg.objectID	
		local pet = target:GetPetID().objID

		if target:BelongsToFaction{factionID = 1}.bIsInFaction and self:GetVar("INUSE") == false then
			
			 local firstWP = GAMEOBJ:GetWaypointPos( "Bridge_Path", 1)
			pet:Teleport{ pos = firstWP }
            storeObjectByName(self, "pet", pet)
			pet:SetVar("AiDisabled", false)
			pet:AddPetState{iStateType = 10 } 
			pet:SetVar("PathingActive", true ) 
			pet:SetVar("WPEvent_NUM", 1)
			pet:SetVar("attached_path", "Bridge_Path" )
			pet:SetVar("attached_path_start", 0 ) 
			pet:FollowWaypoints()
			self:SetVar("INUSE", true)
			 GAMEOBJ:GetTimer():AddTimerWithCancel( 20  , "reset", self )

		end
      
       	
end 

onTimerDone = function (self, msg)
    if msg.name == "reset" then
        self:SetVar("INUSE", false)
        local pet =  getObjectByName(self, "pet")
        pet:RemovePetState{ iStateType = 10 }
    end 
end 


onRebuildNotifyState = function(self, msg)
	if (msg.iState) == 1 then
	
	        getObjectByName(self, "p1"):SetPosition {pos = {x=-399,y=-399,z=-399}   }
	        getObjectByName(self, "p2"):SetPosition {pos = {x=-399,y=-399,z=-399}   }
		 

	end
	if (msg.iState) == 4 then -- break
	        print(msg.iState)
	        local parts = self:GetObjectsInGroup{ group = "bridge" }.objects
            for i = 1, table.maxn (parts) do 
            
                        if ( parts[i]:GetLOT().objtemplate == 3915 ) then
                        
                             local pos = parts[i]:GetPosition().pos
                             local rot  = parts[i]:GetRotation()
                                    
                                    if self:GetVar("bridge_1") == nil then
                                    
                                        bridge_1 = {}
                                        bridge_1['x']   = pos.x 
                                        bridge_1['y']   = pos.y 
                                        bridge_1['z']   = pos.z  
                                        self:SetVar("bridge_1",bridge_1)
                                       
                                         storeObjectByName(self, "p1", parts[i])
                                    elseif self:GetVar("bridge_2") == nil then 
                                    
                                        bridge_2 = {}
                                        bridge_2['x']   = pos.x  
                                        bridge_2['y']   = pos.y 
                                        bridge_2['z']   = pos.z  
                                        storeObjectByName(self, "p2", parts[i])   
                                        self:SetVar("bridge_2",bridge_2)                   
                                        
                                   
                                    end
                            
                        end 
            end
        	
	     
	end
    if (msg.iState) == 4 then 
    	getObjectByName(self, "p1"):SetPosition{pos = self:GetVar("bridge_1")}
	    getObjectByName(self, "p2"):SetPosition{pos = self:GetVar("bridge_2")}
    
    end
end 

