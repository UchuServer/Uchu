require('State')

function SpawnTeams(self)
   
      if self:GetVar("Set.Game_Type") == "CTF" then 
          -- spawn Blue Team 
            for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do
                local Mark = getObjectByName(self, "Blue_Spawn_"..i)
                local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
                if Mark ~= nil and player:Exists() then
                       
                    local Markpos = Mark:GetPosition().pos 
                    local Markrot = Mark:GetRotation()                    
                    player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
                    SetPlayerRespawnLoc(self,player)
                 
                end
            end
           -- spawn Red Team 
            for i = 1, #self:MiniGameGetTeamPlayers{teamID = 2}.objects  do
                local Mark = getObjectByName(self, "Red_Spawn_"..i)
                local player =  self:MiniGameGetTeamPlayers{teamID = 2}.objects[i]
                if Mark ~= nil and player:Exists() then
               
                    local Markpos = Mark:GetPosition().pos 
                    local Markrot = Mark:GetRotation()                    
                    player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
                    SetPlayerRespawnLoc(self,player)
                  
                end
            end     
      end
      
end   
function removeBarrels(self)

		for i = 1, 3 do
			if getObjectByName(self, "Barrel_"..i ) then
			
				getObjectByName(self,"Barrel_"..i ):NotifyObject{name = "killself"}
				self:SetVar( "Barrel_"..i , nil)
				
			end
		end
        SpawnRebuilds(self)
end


function RoutToPlayer(msg, name , param1 , param2 , paramStr , paramObj , playerID )

   GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = msg.name , paramStr = msg.paramStr ,param1 = msg.param1 ,param2 = msg.param2, paramObj = msg.paramObj  , rerouteID = msg.playerID }

end
function RoutToTeam(msg, name , param1 , param2 , paramStr , paramObj , team )

	for x = 1,  #GAMEOBJ:GetZoneControlID():MiniGameGetTeamPlayers{teamID =  msg.team}.objects do  
		local player = GAMEOBJ:GetZoneControlID():MiniGameGetTeamPlayers{teamID =  msg.team}.objects[x]
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = msg.name , paramStr = msg.paramStr ,param1 = msg.param1 ,param2 = msg.param2, paramObj = player  , rerouteID = player }
	end
     
	
end
--------------------------------------------------------------
-- Gets the current activity user or returns nil -------------
--------------------------------------------------------------
function getActivityUser(self)

    local targetID = self:GetActivityUser().userID
    if (targetID == 0 or targetID == nil) then
		return nil
	else
		return targetID
	end
	
end
--------------------------------------------------------------
-- Send Message to All Clients ( Bubble ) --------------------
--------------------------------------------------------------
function sendchatbubble(self, msg)

    

end


--------------------------------------------------------------
-- remove player from game
--------------------------------------------------------------
function RemovePlayerFromGame(self, player)

	if (player) then
	
	    local playerNum = GetPlayerNum(self, player)

		-- reset data
		PLAYERS[playerNum] = -1
		local NoData = {}
		self:SetVar(player:GetID(), NoData)
		
		-- remove all boards
		RemoveBoardsFromPlayer(self, player)

		player:Teleport{pos = CONSTANTS["PLAYER_ZONEIN_POS"],
		                bSetRotation = false}

	end

end

--------------------------------------------------------------
-- parses time to a string
--------------------------------------------------------------
function ParseTime(numTime)

	local newTime = tonumber(numTime)
	
	local min = math.floor(newTime / 1000 / 60)
	newTime = newTime - (min * 1000 * 60)
	
	local sec = math.floor(newTime / 1000)
	newTime = newTime - (sec * 1000)
	
	local msec = math.floor(newTime)
	
	local strTime = ""
	if (min > 0) then
		strTime = ZeroPad(min,2) .. ":" .. ZeroPad(sec,2) .. "." .. ZeroPad(msec,3)
	else
		strTime = ZeroPad(sec,2) .. "." .. ZeroPad(msec,3)
	end

	return strTime

end
-- Removes Any Flags from the player Inventory
function removeflags(self, player)

        for i =0, player:GetInventorySize{inventoryType = 1 }.size  do
            if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:GetLOT{}.objtemplate == self:GetVar("Set.QB_Loot_Object") then
                   local item = player:GetInventoryItemInSlot{slot = 1,inventoryType = 4 }.itemID
                   player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:UnEquipItem{bImmediate = true}
                   player:UnEquipInventory{ itemtounequip = item}
                   taken = player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:GetVar("taken_name")
                   print("taken = "..taken)
                   player:RemoveItemFromInventory{ eInvType = 4 ,iObjTemplate = self:GetVar("Set.QB_Loot_Object") }
                   
                   DoObjectAction(player, "stopeffects", "godlight")
                   player:SetSkillRunSpeed{Modifier = 500 }
                   
                   return taken
                end
            end
        end

end


function setPlayerScores(self, player)

    self:MiniGameSetPlayerScore{playerID = player, scoreType = 0, score = 0 }
    self:MiniGameSetPlayerScore{playerID = player, scoreType = 1, score = 0 }
    self:MiniGameSetPlayerScore{playerID = player, scoreType = 2, score = 1 } -- Lap Number
    self:MiniGameSetPlayerScore{playerID = player, scoreType = 3, score = 0 }
    self:MiniGameSetPlayerScore{playerID = player, scoreType = 4, score = 0 }


end




function setTeamUI(self, team)

       local num = #self:MiniGameGetTeamPlayers{teamID = 1}.objects

        if num > 1 then
            for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do
                local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
                player:TeamAddPlayerMsg{name =  self:MiniGameGetTeamPlayers{teamID = 1}.objects[i+1]}
                player:RequestTeamUIUpdate{}
            end
        
        end

end

function TeleportAllPlayers(self)

	
    		for i = 1, self:GetVar("Set.Number_Of_Teams") do

                  for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
                        local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]

                        local team = self:MiniGameGetTeam{ playerID = player}.teamID
                            if team == 1 then
                                local blueMax = GAMEOBJ:GetZoneControlID():GetVar("Con.Blue_Spawners") - 1
                                local ran = math.random(1,blueMax)
                                local spawn =  getObjectByName(self, "Blue_Spawn_"..ran)
                                local Markpos = spawn:GetPosition().pos 
                                local Markrot = spawn:GetRotation()                    
                                player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
                            
                           
                            else
                                local redMax = self:GetVar("Con.Red_Spawners") - 1
                                local ran = math.random(1,redMax)
                                local spawn =  getObjectByName(self, "Red_Spawn_"..ran)
                                local Markpos = spawn:GetPosition().pos 
                                local Markrot = spawn:GetRotation()                    
                                player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }  
                         
                                  
                            end
                  end
		    end

end
function TeleportPlayer(self,player)

		
		local TID = self:MiniGameGetTeam{ playerID = player}.teamID

			if TID == 1 then
				local blueMax = GAMEOBJ:GetZoneControlID():GetVar("Con.Blue_Spawners") - 1
				local ran = math.random(1,blueMax)
				local spawn =  getObjectByName(self, "Blue_Spawn_"..ran)
				local Markpos = spawn:GetPosition().pos 
				local Markrot = spawn:GetRotation()                    
				player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
			

			else
				local redMax = self:GetVar("Con.Red_Spawners") - 1
				local ran = math.random(1,redMax)
				local spawn =  getObjectByName(self, "Red_Spawn_"..ran)
				local Markpos = spawn:GetPosition().pos 
				local Markrot = spawn:GetRotation()                    
				player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }  
				

			end
       

end


function SpawnRebuilds(self)
	GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "resetCaptured"} 
	    for i = 1, 3 do
			self:SetVar("Barrel_"..i, nil) 
		end	
		
		
		
	for i = 1, self:GetVar("Con.Blue_Point") - 1 do

		local config = { {"AtHomePoint", true} , {"taken_name", "Blue_Point_"..i} }
		local loc = getObjectByName(self, "Blue_Point_"..i ):GetPosition().pos
		getObjectByName(self, "Blue_Point_"..i ):SetVar("taken", true)
		RESMGR:LoadObject {  owner = self ,objectTemplate =  self:GetVar("Set.QB_Object_LOT")  , x= loc.x , y=  loc.y , z=  loc.z  , configData = config  } 
	  
	end


end
 -- Checks for any Seige Objects and removes them  
 function RemoveSiegeObjects(self, player)    
         
 	for i = 1, 2 do
 
 		   for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
 
 				local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
 				
 				for i = 0, player:GetInventorySize{inventoryType = 1 }.size  do
 					if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:Exists() then
 						if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:GetLOT{}.objtemplate == self:GetVar("Set.QB_Loot_Object") then
 						   local item = player:GetInventoryItemInSlot{slot = 1,inventoryType = 4 }.itemID
 						   player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:UnEquipItem{bImmediate = true}
 
 						   taken = player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:GetVar("taken_name")
 						   print("taken = "..taken)
 						   player:RemoveItemFromInventory{ eInvType = 4 ,iObjTemplate = self:GetVar("Set.QB_Loot_Object") }
 						  
 						end
 					end
         		end
 				
 		   end
      end
         
 end

 
 function UpdateScore(self,swap)
     
     for i = 1, 2 do
 
 		   for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
 		   
 			   local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
 	
 			   local kills =    tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 1}.score)
 			   local deaths =   tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 2}.score)
 			   local Capts =    tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 3}.score)
 			   local Returns =  tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 4}.score)
 			   
 			   local FlagCarry =  tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 9}.score)
 			   
 			   local Build =    tostring(self:MiniGameGetPlayerScore{playerID= player,   scoreType = 7}.score)
 			   local DestroyQB =tostring(self:MiniGameGetPlayerScore{playerID= player,   scoreType = 8}.score)
 			   
 
 			   local name = player:GetName().name 	
 			   print(tostring(FlagCarry))
 			   
 			   local addPoints = ((kills * self:GetVar("Set.Kills")) + (FlagCarry * self:GetVar("Set.KillObjCarrier")) +  (Capts * self:GetVar("Set.CapturObj"))  + (Returns * self:GetVar("Set.RetrunObj")) + (Build * self:GetVar("Set.Build"))  + (DestroyQB * self:GetVar("Set.DestroyQB")) - (deaths * self:GetVar("Set.Deaths")) )	
 			   self:MiniGameSetPlayerScore{playerID = player, scoreType = 5, score = addPoints } 	
 				
 			 
 			
 				
 			   local points =   tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 5}.score) 
 			   local txt = ""..kills..","..deaths..","..Capts..","..Returns..","..points..","..name..""
 
 				if self:GetVar("Con.swapTeams") == true then
 					
 					if i == 1 then
 						i = 2
 					else
 						i = 1
 					end
 				
 				end
 
 
                GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SetPlayerPoints" , paramStr = txt , param1 = i ,param2 = x  }
                  
 		   end
     end
 end


function sendLeaderBoardData(self)

     for i = 1, 2 do
 
 		   for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
 		   
 			   local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
 	
 			   local kills =    tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 1}.score)
 			   local deaths =   tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 2}.score)
 			   local value1Var =    tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 3}.score)
 			   local Returns =  tostring(self:MiniGameGetPlayerScore{playerID= player, scoreType = 4}.score)

			   scoreVar = ((kills * self:GetVar("Set.Kills") ) + (value1Var * self:GetVar("Set.CapturObj"))  + (Returns * self:GetVar("Set.RetrunObj")) - (deaths * self:GetVar("Set.Deaths")) )	



			   if i ==1 and self:GetVar("Con.Round_1_Win") == true then
			   		
			   		value2Var = 1
			   		lost = 0
			   		scoreVar = scoreVar * self:GetVar("Set.WonMatchMultiplier") 
			   else
			   		value2Var = 0
			   		lost = 1
			   
			   end
			   	
			   if i == 2 and self:GetVar("Con.Round_2_Win") == true then
			        scoreVar = scoreVar * self:GetVar("Set.WonMatchMultiplier") 
			        value2Var = 1 
			        lost = 0
			   else
			   		value2Var = 0
			   		lost = 1
			   end

			   	
		 		StopActivity(self,player, scoreVar, value1Var, value2Var )
		 		

				self:NotifyClientZoneObject{name = "SiegeRewardsHide", paramObj = player, rerouteID = player} 
			
		  end
		  
 	end

end

----------------------------------------------------------------
function StopActivity(self,player, scoreVar, value1Var, value2Var )
   -- local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

	-- store the time as activity rating [1]
	GAMEOBJ:GetZoneControlID():SetActivityUserData{ userID = player, typeIndex = 0, value = scoreVar }
	print("SetActivityUserData0 Data ="..scoreVar)

	GAMEOBJ:GetZoneControlID():SetActivityUserData{ userID = player, typeIndex = 1, value = value1Var }
	print("SetActivityUserData1 Data ="..value1Var)

	GAMEOBJ:GetZoneControlID():SetActivityUserData{ userID = player, typeIndex = 2, value = value2Var }
	print("SetActivityUserData2 Data ="..value2Var)
	
	
	---GAMEOBJ:GetZoneControlID():UpdateActivityLeaderboard{ userID = player }
	-- distribute rewards        
	GAMEOBJ:GetZoneControlID():DistributeActivityRewards{ userID = player, bAutoAddCurrency = true, bAutoAddItems = true }      



	-- Update Leaderboards for this user
	GAMEOBJ:GetZoneControlID():UpdateActivityLeaderboard{ userID = player }

	-- get the leaderboard data for the user and update summary screen if it exists
	GAMEOBJ:GetZoneControlID():RequestActivitySummaryLeaderboardData{ user = player, queryType = 7 } 

	-- remove the user from activity
	GAMEOBJ:GetZoneControlID():RemoveActivityUser{ userID = player }      
 

 

end

----------------------------------------------------------------
-- GetLeaderboard Data message
----------------------------------------------------------------
function GetLeaderboardData( self, player, activityID )
    

    -- get the leaderboard data for the user and update summary screen if it exists
    self:RequestActivitySummaryLeaderboardData{ user = player, target = self, queryType = 7, gameID = activityID } 
end
