require('o_mis')

function onStartup(self)
    self:SetVar("Total_players", 0 )
	self:SetVar("running", false)
	self:MiniGameSetParameters{numTeams = 1 ,playersPerTeam = 2 }       
   GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "storeObj", self )  
end
--[[

    <attr type="LWOOBJID" name="ObjIDSender" />
    <attr type="std::wstring" name="name" />
    <attr type="int" name="param1" />
    <attr type="int" name="param2" />
--]]

function onNotifyObject(self, msg)

	if (msg.param1 == 1) then -- Add Player 
	
		
		 
    		local player = GAMEOBJ:GetObjectByID(msg.name)
	        self:MiniGameAddPlayer{playerID = player, teamID = 1}
	        self:SetVar("Total_players", self:GetVar("Total_players") + 1)
	        self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = player:GetName().name.." Joined!! Total Players = "..self:GetVar("Total_players") }
	        if self:GetVar("Total_players") == 2 and  not self:GetVar("running") then
	        
                self:NotifyClientZoneObject{name = "sendToAllclients_bubble" ,  paramStr ="Starting in 5 Seconds" }
                self:NotifyClientZoneObject{name = "FreezAllPlayers" ,  param1 = 1 }
                self:SetVar("running", true)
                GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "Start", self )  
	        	-- teport -69 200 277  (( -70 200 335 ))
	        	
	        end
	end
	
	if (msg.param1 == 2) then -- Remove Player
	        local player = GAMEOBJ:GetObjectByID(msg.name)
	        self:MiniGameRemovePlayer{playerID= player}
	        self:SetVar("Total_players", self:GetVar("Total_players") - 1)
	        self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = player:GetName().name.." Left!! Total Players = "..self:GetVar("Total_players") }
	    
	    	    local spawnerl = getObjectByName(self, "spawnloc")
	        	spawnerl:NotifyObject{ name="stop" } 
	        
	end
	if (msg.param1 == 3) then 
	
	
	end
	
	if (msg.param1 == 4) then
	
	
	end
	
	if (msg.param1 == 5) then
	
	
	end


end


function onTimerDone(self, msg) 


    if (msg.name == "Start") then
                 
                  for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do  
                         local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
                        self:NotifyClientZoneObject{name = "unFreezAllPlayers" ,  param1 = 1 }
                        
                         if i == 1 then
                            
                            local poss = { x=-69 , y=200 , z =277}
                            player:Teleport{pos = poss }
                  
                         else
                            local poss = { x=-70 , y=200 , z =335}
                            player:Teleport{pos = poss }
                     
                         end
                         
                    self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "You have 45 Seconds to complete Mission!!" }
                        GAMEOBJ:GetTimer():AddTimerWithCancel( 45, "missionTime", self ) 
                        GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "missionTime2", self ) 
                  end
    
    
    	        local spawnerl = getObjectByName(self, "spawnloc")
	        	spawnerl:NotifyObject{ name="spawn" } 
    
    end
     if (msg.name == "missionTime2") then
        self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Collect 15 energy crystal and rebuild your half of the portal." }
     end
    if (msg.name == "missionTime") then
    
    	    	 local spawnerl = getObjectByName(self, "spawnloc")
	        	 spawnerl:NotifyObject{ name="stop" } 
	        	 self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Timer has Expired!!" }
                for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do  
                        local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
                        local count = player:GetInvItemCount{itemID = 4985}.itemCount
                        if count ~= nil then
                            for x = 1, count do
                                player:RemoveItemFromInventory{iObjTemplate =4985 }
                                if player ~= nil then
                                    self:MiniGameRemovePlayer{playerID= player}
                                end
                            end    
                        end
                        self:SetVar("running", false)
                  end
                                  for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do  
                        local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
                        local count = player:GetInvItemCount{itemID = 4985}.itemCount
                        if count ~= nil then
                            for x = 1, count do
                                player:RemoveItemFromInventory{iObjTemplate =4985 }
                                if player ~= nil then
                                    self:MiniGameRemovePlayer{playerID= player}
                                end
                            end    
                        end
                        self:SetVar("running", false)
                  end
                   self:SetVar("Total_players", 0)
    end
    
    if (msg.name == "storeObj") then
    
	local obj = self:GetObjectsInGroup{ group = "grp_mission" }.objects
	for i = 1, #obj do      
		if obj[i]:GetLOT().objtemplate == 3706 then -- blue button 
			storeObjectByName(self, "blue_button", obj[i])
			storeObjectByName(obj[i], "minigame",self )
					
		end
		if obj[i]:GetLOT().objtemplate == 3704 then -- red button 
			storeObjectByName(self, "red_button", obj[i])
			storeObjectByName(obj[i], "minigame",self )
		end		
		if obj[i]:GetLOT().objtemplate == 3554 then -- Cylinder
			storeObjectByName(self, "Cylinder", obj[i])
			storeObjectByName(obj[i], "minigame", self)
		end			
		
		if obj[i]:GetLOT().objtemplate == 4064 then -- rebuild two
		    if self:GetVar("rebuild1") == nil then
                storeObjectByName(self, "rebuild1", obj[i])
                storeObjectByName(obj[i], "minigame",self )
            else
                storeObjectByName(self, "rebuild2", obj[i])
                storeObjectByName(obj[i], "minigame",self ) 
            end
            
		end		
		if obj[i]:GetLOT().objtemplate == 4934 then 
			if self:GetVar("spawnloc") == nil then
				storeObjectByName(self, "spawnloc", obj[i])
				storeObjectByName(obj[i], "minigame",self )
			else
				storeObjectByName(self, "spawnloc", obj[i])
				storeObjectByName(obj[i], "minigame",self ) 
			end

		end	

		
	 end  
    
    end


end


 
 
