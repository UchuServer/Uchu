function onPlayerDied( self, msg)
 	
 	
 	--[[
 	
 		kills =   1
 		deaths =  2
 		Capts =   3
 		Returns = 4
 		points =  5
 		rebuilds = 6
 		
 	--]]
 	
 	
	 local player = msg.playerID
	 local killer = msg.killerID
 
     GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "HideDropButton" ,rerouteID = player }
	 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "ReSpawnTimer" , param1 = self:GetVar("Set.RespawnTime"),  rerouteID = player }
         
        
        self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = killer:GetName().name..self:GetVar("Set.Info_Text_1")..player:GetName().name.."!" }
         
         
			player:UnEquipItem{bImmediate = true}

            self:MiniGameAddPlayerScore{playerID = killer, scoreType = 1, score = 1 } -- kills
            self:MiniGameAddPlayerScore{playerID = player, scoreType = 2, score = 1 } -- deaths  
         --   self:NotifyClientZoneObject{name = "Update_Player_Scores"}   
            
             GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "reSetAnimationSet" , paramObj = player }
             

            if self:MiniGameGetTeam{ playerID = player}.teamID == 2 then
                 local FlagFound = removeflags(self, player)
    
                 if FlagFound then
                 
                 	  self:MiniGameAddPlayerScore{playerID = msg.killerID, scoreType = 9, score = 1 } -- kill flag carry 
                 	  self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_5") }
                      local bluepos =   player:GetPosition().pos 
                      local config = { {"AtHomePoint", false }, {"taken_name", FlagFound}   }
                      RESMGR:LoadObject { owner = self, objectTemplate = self:GetVar("Set.QB_Object_LOT") , x= bluepos.x, y= bluepos.y , z= bluepos.z, configData = config  }        
                end
           end
       
           UpdateScore(self)
           GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.RespawnTime") , player:GetName().name , self )	
end 
