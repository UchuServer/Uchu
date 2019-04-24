--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_ZONEIN_POS"] = {x = 122.854, y = 278.0, z = -343.110}
CONSTANTS["ROUNDS"] = 3
CONSTANTS["ROUND_TIME"] = 360

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




--------------------------------------------------------------
-- returns player num
-- returns > 0 if valid
--------------------------------------------------------------
function GetPlayerNum(self, player)

	for pnum = 1, #PLAYERS do
		if (player:GetID() == PLAYERS[pnum]) then
			return pnum
		end
	end

	return 0

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
-- add new player for the race
--------------------------------------------------------------
function AddNewPlayerToGame(self, player)

	if (player) then

    
    end
end



--------------------------------------------------------------
-- try to stop the game
--------------------------------------------------------------
function StopGame(self)

   print("Game Stopped")



end



function onPlayerDied(self, msg)

	if msg == foo then
	
	
	end


end





--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

  
    print("Game onStartup*******************")
    self:SetVar("what_team", "A")
    self:SetVar("total_players", 0) 
  for i = 1, 8 do 
    self:SetVar("PLAYER_"..i , nil)
  end

    



  --GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "LoadFlag", self )
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if (msg.name == "StartUp") then
          local spawnPoint1 = self:GetObjectsInGroup{ group = "spawn_blue" }.objects
          local spawnPoint2 = self:GetObjectsInGroup{ group = "spawn_red" }.objects
          
            for s = 1, 4 do
              
                spawnPoint2[s]:SetVar("spawn_taken", "open")
            end
    end
    
    
    if (msg.name == "StartGame") then
		StartGame(self)
    end
    
    if (msg.name == "TimeOut") then
    	AbortGame(self)
    end
    
    if (msg.name == "EndGame") then
	    StopGame(self)
    end
  

	
end    


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)


	print ("Player Entered Dots Board: " .. msg.playerID:GetName().name)
	
		getActivityUser(self)
		
	local player = msg.playerID
    local     idString = player:GetID()
    local     finalID = "|" .. idString
        for i = 1, 8 do
            if self:GetVar("PLAYER_"..i) == nil then
                 storeObjectByName(self, "PLAYER_"..i, player)
                 self:SetVar("PLAYERNAME_"..i, msg.playerID:GetName().name ) 
                 self:SetVar("PLAYEPOINTS_"..i, 0 ) 
                 print ("Saved Data for: "..msg.playerID:GetName().name )
                 
                 if  self:GetVar("what_team") == "A" then
                 
                    local spawnPoint = self:GetObjectsInGroup{ group = "spawn_blue" }.objects
     
                        for i = 1, table.maxn (spawnPoint) do
                            if  spawnPoint[i]:GetVar("spawn_taken") == "open" then
                                local spawnpos = spawnPoint[i]:GetPosition().pos
                                spawnPoint[i]:SetVar("spawn_taken", true)           
                                player:Teleport{pos = spawnpos }
                                player:SetFaction{faction = 101}
                                self:SetVar("what_team", "B")
                                local i = self:GetVar("total_players")
                                self:SetVar("total_players",i+1) 
                                print ("Player Count: " ..self:GetVar("total_players") )
                                player:SetVar("what_team", "A")
                                player:SetNetworkVar("what_team", "A")
                                player:ServerSetUserCtrlCompPause{bPaused = true}
                                self:SetVar(tostring(msg.playerID:GetName().name), "A") 

								self:NotifyClientZoneObject{ name= player:GetName().name, param1 = 10 , param2 = 1 }
								break
                            end 
                            
                        end

                else
                    local spawnPoint = self:GetObjectsInGroup{ group = "spawn_red" }.objects
                    
                         for i = 1, table.maxn (spawnPoint) do
                            if  spawnPoint[i]:GetVar("spawn_taken") == "open" then
                                local spawnpos = spawnPoint[i]:GetPosition().pos
                                spawnPoint[i]:SetVar("spawn_taken", true)           
                                player:Teleport{pos = spawnpos }
                                player:SetFaction{faction = 100}
                                self:SetVar("what_team", "A")
                                local i = self:GetVar("total_players") 
                                self:SetVar("total_players",i+1) 
                                print ("Player Count: " ..self:GetVar("total_players") )
                                player:SetVar("what_team", "B")
                                player:SetNetworkVar("what_team", "B")
                                player:ServerSetUserCtrlCompPause{bPaused = true}
                                self:SetVar(tostring(msg.playerID:GetName().name), "B") 
                                self:NotifyClientZoneObject{ name= player:GetName().name, param1 = 10 , param2 = 2 }
                                break
                            end 
                            
                        end           
                end
                break 
         end
end	
		
		
	-- unpause player
	player:ServerSetUserCtrlCompPause{bPaused = false}
	
end


--------------------------------------------------------------
-- Gets the current activity user or returns nil
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
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onNotifyObject(self, msg)
    local whatPlayer = 0
	if msg.name == "QBuildID" then
		local Player = getObjectByName(msg.ObjIDSender, "QB_PlayerID")
		local QBGroup = msg.ObjIDSender:GetVar("grp_name")
		
		self:SetVar(QBGroup, true) 
		

		
		checkCells( self , msg.param1 ) 
	end

end


--------------------------------------------------------------
-- Set/Reset Cells
--------------------------------------------------------------


function SpawnCellOBJ(self,plnum,cell)

    	local objects = self:GetObjectsInGroup{ group = "cell_grp"}.objects
		for i = 1, table.maxn (objects) do
		    if objects[i]:GetVar("grp_name") == cell then
                local mypos = objects[i]:GetPosition().pos 
                if plnum  == 1 then
                    RESMGR:LoadObject { objectTemplate = 3704 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, bIsSmashable = false }
                elseif  plnum  == 2 then
                     RESMGR:LoadObject { objectTemplate = 3705 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, bIsSmashable = false }
                end
			end
		end

end















