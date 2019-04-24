

--------------------------------------------------------------
-- reroute client message to a player            -------------
--------------------------------------------------------------
function RoutToPlayer(msg, name , param1 , param2 , paramStr , paramObj , playerID )

   GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = msg.name , paramStr = msg.paramStr ,param1 = msg.param1 ,param2 = msg.param2, paramObj = msg.paramObj  , rerouteID = msg.playerID }

end
--------------------------------------------------------------
-- reroute client message to a Team              -------------
--------------------------------------------------------------
function RoutToTeam(msg, name , param1 , param2 , paramStr , paramObj , team )

	for x = 1,  #GAMEOBJ:GetZoneControlID():GetAllActivityUsers{}.objects do  
		local player = GAMEOBJ:GetZoneControlID():GetAllActivityUsers{}.objects[x]
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

--------------------------------------------------------------
-- Teleport Player to there start Pos
--------------------------------------------------------------
function TeleportPlayer(self,player)

		
		print("Player  "..player:GetName().name)
		for i = 1, 6 do
			if self:GetVar("Con.Start_Pos_"..i) == "open" then
				self:SetVar("Con.Start_Pos_"..i , "closed")
				local spawn =  getObjectByName(self, "Blue_Mark_"..i)

				-- Set Player Pos on the Line Up.
				
				-- SetActivityValue(self, player, 4, i  ) -- Set Starting Pos 

				local Markpos = spawn:GetPosition().pos 
				local Markrot = spawn:GetRotation()
				
				local post = {x=Markpos.x  ,y=Markpos.y  ,z=Markpos.z }
				player:Teleport{pos = post,  bIgnoreY = false}
				
				CreateVehicle(self, player, i, Markpos, Markrot)
				
				break
			end
		end
end

function CreateVehicle(self, player,pos, Markpos, Markrot)
	if 0==1 then
	--------------------------------------------------
		-- Move this into the race instance script	
		local storeddata = player:GetStoredConfigData{optionalKey="matching.droppedItem"}.configData["matching.droppedItem"]
		if storeddata ~= nil then
			print ("Activate player vehicle:"..storeddata)
		else
			print "No dropped item found!"
		end
	else
    local idString = player:GetID()
    local finalID = "|" .. idString
			
		local config = { {"RaceDriver", finalID }, {"SpawnMark", "Blue_Mark_"..pos } , {"StartPos", pos }  }
		RESMGR:LoadObject { objectTemplate = self:GetVar("Set.Car_Object") , x = Markpos.x ,
				y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
				rz = Markrot.z, owner = self , configData = config};
	end
end

----------------------------------------------------------------
-- GetLeaderboard Data message
----------------------------------------------------------------
function GetLeaderboardData( self, player, activityID )
    

    -- get the leaderboard data for the user and update summary screen if it exists
    self:RequestActivitySummaryLeaderboardData{ user = player, target = self, queryType = 7, gameID = activityID } 
end
----------------------------------------------------------------
-- Reset Activity Vars
----------------------------------------------------------------
function setPlayerScores(self, player)


		SetActivityValue(self, player, 0,  0   )
		SetActivityValue(self, player, 1,  0   )
		SetActivityValue(self, player, 2,  0   )
		SetActivityValue(self, player, 3,  1   ) -- Lab Number
		SetActivityValue(self, player, 4,  0   )



end


