function AddPlayerToTeam(self,msg)

   local player = msg.playerID
       
   if foo ~= nil then     
		local player = msg.playerID
        --------------------------------------------
        -- Add Player to MiniGame
        --------------------------------------------
       self:MiniGameAddPlayer{playerID = player}
        
	    --------------------------------------------
        -- Add Player to Team
        -------------------------------------------- 	 
 		local TID1 = #self:MiniGameGetTeamPlayers{teamID = 1}.objects 
        local TID2 = #self:MiniGameGetTeamPlayers{teamID = 2}.objects 
        
        
        if TID1 > TID2 then
        
        	self:MiniGameSetTeam{playerID = player , teamID = 2}
        	
        elseif TID1 < TID2 then
        
        	self:MiniGameSetTeam{playerID = player, teamID = 1}
        	
        elseif TID1 == TID2 then
        
        	self:MiniGameSetTeam{playerID = player, teamID = 1}
        
        end
        
      
        
        --------------------------------------------
        -- Sets Player Respan Locations
        --------------------------------------------  
        SetPlayerRespawnLoc(self, msg)
        --------------------------------------------
        -- Gets TeamID 
        --------------------------------------------  
 		local TID = self:MiniGameGetTeam{ playerID = player}.teamID

        --------------------------------------------
        -- Gets TeamID 
        --------------------------------------------  

        player:SetPVPStatus{ bOn = true }
         -- Set Teams and Factions
        if TID == 1 then
          
             setPlayerScores(self, player)
              -- Send MSG CLIENT             
             self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_2") }
              -- Set Gui Color
              GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetBkGrndColor" , param1 = 1 , rerouteID = player } 
              RoutToPlayer{msg, name = "UISetObjective" , param1 = nil , param2 = nil , paramStr  = self:GetVar("Con.TeamRoll_1") , paramObj = player, playerID = player }
             
            
           
        elseif TID == 2 then

             setPlayerScores(self, player)

             -- Send MSG CLIENT   
             self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_2") }
             RoutToPlayer{msg, name = "UISetObjective" , param1 = nil , param2 = nil , paramStr  = self:GetVar("Con.TeamRoll_2") , paramObj = player, playerID = player }
             -- Set Gui Color
             GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetBkGrndColor" , param1 = 2 , rerouteID = player }
            
           
            
        end
        --------------------------------------------
        -- Set the players Attr 
        --------------------------------------------         

		if self:GetVar("Set.CustomPlayer") then
		
			player:SetMaxImagination{imagination = self:GetVar("Set.Imagination")}
			player:SetMaxHealth{health = self:GetVar("Set.Health")}
			player:SetMaxArmor{armor = self:GetVar("Set.Armor")}
			player:SetImagination{imagination = self:GetVar("Set.Imagination")}
			player:SetHealth{health = self:GetVar("Set.Health")}
			player:SetArmor{armor = self:GetVar("Set.Armor")}
			
        end 
        
        TeleportPlayer(self,msg)
        
        if self:GetVar("Con.GameStarted") then
	
			  self:NotifyClientZoneObject{name = "FreezPlayer", paramObj = player }
			  
        end
        
        --------------------------------------------
        -- Start Game if we have our players
        --------------------------------------------      
        if self:GetVar("Con.Players_loaded") == self:GetVar("Set.Minimum_Players_to_Start")   then
         
          GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "resetCaptured"} 
          self:SetVar("Con.Pre_Counter",self:GetVar("Set.Notify_Team_Objectives")  )
          self:SetVar("TimeToBeat" , self:GetVar("Set.DefendTime") )
          self:NotifyClientZoneObject{name = "TimeToBeat" ,paramStr = self:GetVar("TimeToBeat")  }
          GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.Notify_Team_Objectives") , "GateTime", self ) 
          self:NotifyClientZoneObject{name = "Send_State", paramStr = "Game Pre Start"}
		

    	end    
        
        if not self:MiniGameGetTeam{playerID = player }.teamID then
        
        	print("!! PLAYER "..player:GetName().name.." FAILD TO JOIN A TEAM WTF !!")
        
        else
        
        	
        	
        	for i = 1, 4 do
        		local playerOBJ = self:MiniGameGetTeamPlayers{teamID = self:MiniGameGetTeam{playerID = player }.teamID}.objects[i]
        		if playerOBJ then
        			
        			if playerOBJ:GetName().name == player:GetName().name then    		
        				local TID = self:MiniGameGetTeam{playerID = player }.teamID
        				self:NotifyClientZoneObject{name = "SetPlayerName" ,paramStr = player:GetName().name, param1 =TID , param2 = i }
        				self:NotifyClientZoneObject{name = "SetClientColor" ,paramStr = player:GetName().name, param1 =TID , param2 = i }
        				break
        			end
        		end
        		
        	end
        	
        	


        end
  
  end      
	
end


-- Set Player Attributes
function SetPlayerAttributes(player)

        player:SetMaxImagination{imagination = self:GetVar("Set.Imagination")}
        player:SetMaxHealth{health = self:GetVar("Set.Health")}
        player:SetMaxArmor{armor = self:GetVar("Set.Armor")}
        player:SetImagination{imagination = self:GetVar("Set.Imagination")}
        player:SetHealth{health = self:GetVar("Set.Health")}
        player:SetArmor{armor = self:GetVar("Set.Armor")}
end



function SetPlayerRespawnLoc(self, player)

	
    local TID = self:MiniGameGetTeam{playerID = player }.teamID
    
    player:SetPlayerAllowedRespawn{dontPromptForRespawn = true}
    
    if TID == 1 then
      player:SetRespawnGroup{findClosest=true, respawnGroup='RespawnTeamA'}
  
    elseif  TID == 2 then
      player:SetRespawnGroup{findClosest=true, respawnGroup='RespawnTeamb'}
   
    else
    
    	print("!! PLAYER "..player:GetName().name.." IS NOT ON A TEAM !!")
    end
    
    
end
function SetPlayerRespawnLocPlayer(self, player , team)


  
    
    player:SetPlayerAllowedRespawn{dontPromptForRespawn = true}
    
    if team == 1 then
      player:SetRespawnGroup{findClosest=true, respawnGroup='RespawnTeamA'}
  
    elseif  team == 2 then
      player:SetRespawnGroup{findClosest=true, respawnGroup='RespawnTeamb'}
   
    else
    
    	print("!! PLAYER "..player:GetName().name.." IS NOT ON A TEAM !!")
    end
    
    
end