--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')

-- @TODO: deal with players logging out in middle of race

--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_ZONEIN_POS"] = {x = 122.854, y = 278.0, z = -343.110}
CONSTANTS["PLAYER_ZONEIN_ROT"] = {w = 0.722735, x = 0, y = 0.691124, z = 0}

--CONSTANTS["PLAYER_ZONEIN_POS"] = {x = -688.0, y = 201.0, z = -147.0}
--CONSTANTS["PLAYER_ZONEIN_ROT"] = {w = 0.722735, x = 0, y = 0.691124, z = 0}
--
--CONSTANTS["PLAYER_START_POS1"] = {x = -604.0, y = 186.0, z = -154.0}
--CONSTANTS["PLAYER_START_ROT1"] = {w = 0.287, x = 0, y = 0.957, z = 0}
--
--CONSTANTS["PLAYER_START_POS2"] = {x = -601.0, y = 186.0, z = -150.0}
--CONSTANTS["PLAYER_START_ROT2"] = {w = 0.287, x = 0, y = 0.957, z = 0}
--
CONSTANTS["PLAYER_START_POS1"] = { x = -258.739,  y = 210.0, z = -457.921 }
CONSTANTS["PLAYER_START_POS2"] = { x = -254.554,  y = 210.0, z = -462.901 }
CONSTANTS["PLAYER_START_POS3"] = { x = -250.512,  y = 210.0, z = -467.708 }
CONSTANTS["PLAYER_START_POS4"] = { x = -245.919,  y = 210.0, z = -473.172 }
CONSTANTS["PLAYER_START_POS5"] = { x = -241.867,  y = 210.0, z = -477.991 }
CONSTANTS["PLAYER_START_POS6"] = { x = -237.412,  y = 210.0, z = -483.290 }
CONSTANTS["PLAYER_START_POS7"] = { x = -233.087,  y = 210.0, z = -488.434 }
CONSTANTS["PLAYER_START_POS8"] = { x = -228.770,  y = 210.0, z = -493.569 }
CONSTANTS["PLAYER_START_POS9"] = { x = -224.724,  y = 210.0, z = -498.382 }
CONSTANTS["PLAYER_START_POS10"]= { x = -220.404,  y = 210.0, z = -503.520 }

CONSTANTS["PLAYER_START_ROT"] = {w = -0.45988774, x = 0, y = 0.88797706, z = 0}

CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["CANNON_PLAYER_OFFSET"] = {x = 6.652, y = 0, z = -5.716} --@todo port back
CONSTANTS["CANNON_VELOCITY"] = 160.0
CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 1000.0
CONSTANTS["CANNON_BARREL_OFFSET"] = {x = 0, y = 4.3, z = 9}
CONSTANTS["CANNON_TIMEOUT"] = -1
CONSTANTS["CANNON_IMPACT_SKILL"] = 73

CONSTANTS["GOAL_LINE_LOT"] = 2717
CONSTANTS["CHECKPOINT_LOT"] = 2718
CONSTANTS["BOARD_LOT"] = 2250
CONSTANTS["RACE_MISSION_ID"] = 31
CONSTANTS["CANNON_MISSION_ID"] = 57
CONSTANTS["TURN_SPEED_MULT"] = 0.4				-- turn speed multiplier for track

CONSTANTS["MAX_PLAYERS"] = 10					-- number of players max
CONSTANTS["MIN_PLAYERS"] = 1					-- number of players min
CONSTANTS["MAX_LAPS"] = 3						-- number of laps for race
CONSTANTS["NUM_CHECKPOINTS"] = 0 				-- this should Auto Update
CONSTANTS["NUM_CANNONS"] = 0 					-- this should Auto Update
CONSTANTS["COUNTDOWN_TIME"] = 3.0
CONSTANTS["RACE_TIMEOUT"] = 20.0 --60.0			-- time until race auto quits with less then min players
CONSTANTS["RACE_START_TIME"] = 25.0--30.0 --30.0		-- time until race starts after min player reached
CONSTANTS["RACE_FINISH_TIME"] = 40.0 --60.0		-- time until race finishes

BOARDS = {2851, 2852, 2853, 2854}
CONSTANTS["NUM_BOARDS"] = #BOARDS

PLAYERS = {}
CHECKPOINTS = {}
CANNONS = {}					-- stores player in cannon and the cannon itself
PLAYERS_TO_REMOVE = {}			-- stores players that need to be removed from the cannon

-- local vars
LOCALS = {}
LOCALS["bWaitingForPlayers"] = false


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------

--------------------------------------------------------------
-- Pads a number with zeros on the left, to fill a field of the specified
-- length.
--------------------------------------------------------------
function ZeroPad(number, length)
	return string.rep("0", length - #tostring(number)) .. tostring(number)
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
-- checks to see if the checkPoint ID is valid, 
-- returns > 0 if valid
--------------------------------------------------------------
function IsValidCheckpoint(self, checkPoint)

	for chkp = 1, CONSTANTS["NUM_CHECKPOINTS"] do
		if (checkPoint == CHECKPOINTS[chkp]) then
			return chkp
		end
	end	
	
	return 0

end


--------------------------------------------------------------
-- checks to see if the cannon ID is valid and empty,
-- returns cannonid if valid, 0 otherwise
--------------------------------------------------------------
function GetValidCannon(self)

	for chkp = 1, CONSTANTS["NUM_CANNONS"] do
		if (CANNONS[chkp]["Player"] == 0) then
			return CANNONS[chkp]["Cannon"]
		end
	end	
	
	return 0

end

--------------------------------------------------------------
-- returns true if game is going
--------------------------------------------------------------
function IsGameStarted(self)

	return (LOCALS["GameStarted"] == true)
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
-- checks to see if a player has hit all checkpoints 
--------------------------------------------------------------
function HasPlayerHitAllCheckpoints(self, player)

	local playerData = self:GetVar(player:GetID())

	if (playerData) then

		for chkp = 1, CONSTANTS["NUM_CHECKPOINTS"] do
			if (playerData["chkpStatus" .. chkp] == false) then
				return false
			end

		end

	else
		return false
	end

	-- all checkpoints good, no failures
	return true

end


--------------------------------------------------------------
-- sends a message to display checkpoint info to the player
--------------------------------------------------------------
function DisplayCheckpointTimeToPlayer(self, player)

	if ( (player) and (IsGameStarted(self)) ) then

		-- get player data
		local playerData = self:GetVar(player:GetID())
		if (playerData) then

			-- get current time
			local theTime = GAMEOBJ:GetSystemTime()
			local strTime = tonumber(theTime) - tonumber(playerData["curLapTime"])

		end

	end

end


--------------------------------------------------------------
-- sets the place data member for a player
--------------------------------------------------------------
function SetPlayerPlace(self, player, place)

	if (player) then
		local playerData = self:GetVar(player:GetID())
		if (playerData) then
			playerData["place"] = place
			self:SetVar(player:GetID(), playerData)
		end
	end
end


--------------------------------------------------------------
-- flags a lap for a player and displays any information
--------------------------------------------------------------
function FlagLap(self, player)


	-- try to get player data
	local playerData = self:GetVar(player:GetID())
	
	-- has the player hit all the checkpoints?
	if ( (IsGameStarted(self)) and (playerData) and ( HasPlayerHitAllCheckpoints(self,player) == true ) ) then

print("Goal Hit!")

		local theTime = GAMEOBJ:GetSystemTime()
		local strTime = tonumber(theTime) - tonumber(playerData["curLapTime"])

		-- set this player's lap time
		playerData["lapTime" .. playerData["curLap"]] = strTime
		
		-- reset current lapTime
		playerData["curLapTime"] = theTime
		
		-- move the player to the next lap
		playerData["curLap"] = tonumber(playerData["curLap"]) + 1

		-- reset checkpoint flags
		for chkp = 1, CONSTANTS["NUM_CHECKPOINTS"] do
			playerData["chkpStatus" .. chkp] = false
		end
		
		-- store the data
		self:SetVar(player:GetID(), playerData)

		-- show player the lap number or end game		
		if (tonumber(playerData["curLap"]) <= CONSTANTS["MAX_LAPS"]) then
		
			player:PlaySound{ strSoundName = "lap_complete" }
					
		else
		
			-- done with race (someone finished)
			local numFinishedPlayers = IncrementVarAndReturn("NumPlayersFinished")

			-- set to first place
			SetPlayerPlace(self, player, numFinishedPlayers)
			
--			-- stun player because he is done with race
--			player:SetStunned
--			{
--				StateChangeType = PUSH,
--				bCantMove = true,
--				bCantTurn = true,
--				bCantAttack = true,
--				bCantEquip = true,
--				bCantInteract = true
--			}
--			
			-- stop the race when we are totally done
			if (numFinishedPlayers >= #PLAYERS) then
				
				StopGame(self)
				
			-- first player across the line starts the timeout timer
			elseif (numFinishedPlayers == 1) then
			
				DoFinishRaceTimer(self,player)
			
			end
			
		end
				
	end

end


--------------------------------------------------------------
-- being end race timer
--------------------------------------------------------------
function DoFinishRaceTimer(self,winner)

	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	
	local winnerName = winner:GetName().name
	
	-- display tooltip to all players in race
	for pnum = 1, #PLAYERS do
	
	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])
	    local playerData = self:GetVar(PLAYERS[pnum])
	    
	    if ((winner:GetID() == PLAYERS[pnum])) then

	        -- show tooltip to winner
	        player:DisplayTooltip{ bShow = true, strText = "You Win!\n Waiting for other players." }
	        player:PlayFXEffect{effectType = "fireworks"}

	    elseif (player and playerData) then
	    
	        -- show tooltip to others
	        player:DisplayTooltip{ bShow = true, strText = "Winner: " .. winnerName .. "\nRace ends in " .. CONSTANTS["RACE_FINISH_TIME"] .. " seconds.", iTime = 5000 }

	    end

	end
	
	-- start timer for game start
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["RACE_FINISH_TIME"], "EndGame", self )
	
end


--------------------------------------------------------------
-- flags a checkpoint for a player and displays any information
--------------------------------------------------------------
function FlagCheckpoint(self, player, checkPoint)

	-- try to get player data and checkpoint data
	local playerData = self:GetVar(player:GetID())
	local chkp = tonumber(IsValidCheckpoint(self,checkPoint))

	-- are the vars valid?
	if ( (IsGameStarted(self)) and (playerData) and ( chkp > 0) and tonumber(playerData["curLap"]) <= CONSTANTS["MAX_LAPS"]) then

print("Checkpoint " .. chkp .. " Hit!")

		-- show the checkpoint time to player
		if (playerData["chkpStatus" .. chkp] == false) then
		   player:PlaySound{ strSoundName = "checkpoint" }
		   DisplayCheckpointTimeToPlayer(self, player)
		end
	
		-- set this checkpoint to true for player
		playerData["chkpStatus" .. chkp] = true
		
		-- store the data
		self:SetVar(player:GetID(), playerData)
		
	end

end


--------------------------------------------------------------
-- get place string
--------------------------------------------------------------
function GetPlaceString(place)

	if (tonumber(place) == 1) then
		return "1st"
	elseif (tonumber(place) == 2) then
		return "2nd"
	elseif (tonumber(place) == 3) then
		return "3rd"
	else
		return (place.."th")
	end
end


--------------------------------------------------------------
-- show the summary dialog
--------------------------------------------------------------
function showSummaryDialog(self, player)

	if (player) then
	
		local playerData = self:GetVar(player:GetID())

        -- close any tooltip
        player:DisplayTooltip{ bShow = false, strText = "..." }
	        
		-- setup summary text
		local strText = ""
		strText = strText .. "Race Over!\n\n"
		
		-- show place
		local place = GetPlaceString(playerData["place"])
		if (playerData["place"] == 0) then
			strText = strText .. "<b>Not Finished!</b>\n\n"
		else
			strText = strText .. "<b>" .. place .. " Place!</b>\n\n"
		end

		-- lap times
		local totalTime = 0
		local bNotFinished = false
		for lap = 1, CONSTANTS["MAX_LAPS"] do

        	-- if no lap time, exit early
			if (tonumber(playerData["lapTime" .. lap]) <= 0) then
				bNotFinished = true
				break
			else
			
				strText = strText .. "Lap " .. lap .. ":  <b>" .. ParseTime(playerData["lapTime" .. lap]) .. "</b>\n"
				totalTime = totalTime + tonumber(playerData["lapTime" .. lap])

			end
			
		end
                                      
		
		-- show total time if finished
		if (bNotFinished == false) then
			strText = strText .. "Total:  <b>" .. ParseTime(totalTime) .. "</b>\n"
		end
		
		-- ask for retry
		strText = strText .. "\nRetry?"

		-- show the summary message box
		player:DisplayMessageBox{bShow = true, 
								 imageID = 2, 
								 callbackClient = GAMEOBJ:GetZoneControlID(), 
								 text = strText, 
								 identifier = "Racing_Summary"}
										 
	end

end


--------------------------------------------------------------
-- remove boards from a player
--------------------------------------------------------------
function RemoveBoardsFromPlayer(self, player)

	if (player) then
	
		for board = 1, CONSTANTS["NUM_BOARDS"] do
			-- unequip and remove the item
			player:RemoveItemFromInventory{ iObjTemplate = BOARDS[board] }
		end


	end

end


--------------------------------------------------------------
-- adds a random board to the player
--------------------------------------------------------------
function AddRandomBoardToPlayer(self, player)

	if (player) then
	
		-- remove all current boards
		RemoveBoardsFromPlayer(self, player)
	
	 	local board = math.random(1,CONSTANTS["NUM_BOARDS"])
	
		-- add item
		local item = player:AddNewItemToInventory{ iObjTemplate = BOARDS[board] }
		
		-- equip item
		player:EquipInventory{ itemtoequip = item.newObjID }

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
-- add new player for the race
--------------------------------------------------------------
function AddNewPlayerToGame(self, player)

	if (player) then
		local playerNum = IncrementVarAndReturn("PlayerNum")

		-----------------------------------------------------
		-- store player data
		local PlayerData = {}
		PlayerData["startTime"] = 0
		PlayerData["curLapTime"] = 0
		PlayerData["curLap"] = 1
		PlayerData["place"] = 0

		for lap = 1, CONSTANTS["MAX_LAPS"] do
			PlayerData["lapTime" .. lap] = -1
		end

		for chkp = 1, CONSTANTS["NUM_CHECKPOINTS"] do
			PlayerData["chkpStatus" .. chkp] = false
		end

		self:SetVar(player:GetID(), PlayerData)
		-----------------------------------------------------

		-- store id in local array
		PLAYERS[playerNum] = player:GetID()
		
		-- put player in right spot
		player:Teleport{pos = CONSTANTS["PLAYER_START_POS" .. playerNum],
		                x = CONSTANTS["PLAYER_START_ROT"].x,
		                y = CONSTANTS["PLAYER_START_ROT"].y,
		                z = CONSTANTS["PLAYER_START_ROT"].z,
		                w = CONSTANTS["PLAYER_START_ROT"].w,
		                bSetRotation = true}

		-- paused wait for game start
		player:ServerSetUserCtrlCompPause{bPaused = true}
		
		-- add board to player
		AddRandomBoardToPlayer(self, player)
		
		-- Try to start the game (next frame)
		local curplayers = LOCALS["PlayerNum"]
		
		if (tonumber(curplayers) == CONSTANTS["MAX_PLAYERS"]) then
		
			LOCALS["bWaitingForPlayers"] = false
			GAMEOBJ:GetTimer():CancelAllTimers( self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "StartGame", self )
			
		else
		
			-- begin timer for game start if we have the min players
			if (tonumber(curplayers) == CONSTANTS["MIN_PLAYERS"]) then
	print("minplayers")
				BeginRaceStartJoinTimer(self)
				
				player:DisplayTooltip{ bShow = true, strText = "Waiting for players...", iTime = 0 }
				
			elseif (tonumber(curplayers) < CONSTANTS["MIN_PLAYERS"]) then 
			
				-- start timeout timer
				if (LOCALS["bWaitingForPlayers"] == false) then
					GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["RACE_TIMEOUT"], "TimeOut", self )	
					LOCALS["bWaitingForPlayers"] = true
				end
				
			    -- inform player about wait with tooltip
			    player:DisplayTooltip{ bShow = true, strText = "Waiting for players...", iTime = 0 }
			    
			end
	
		end

	end
	
end


--------------------------------------------------------------
-- being the race join timer
--------------------------------------------------------------
function BeginRaceStartJoinTimer(self)

	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	
	-- display tooltip to all players in race
	for pnum = 1, #PLAYERS do
	
	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])
	    local playerData = self:GetVar(PLAYERS[pnum])
	    
	    if (player and playerData) then
	    
	        -- show tooltip
	        player:DisplayTooltip{ bShow = true, strText = "Race begins in " .. CONSTANTS["RACE_START_TIME"] .. " seconds. \nWaiting for more players." }

	    end

	end
	
	-- start timer for game start
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["RACE_START_TIME"], "StartGame", self )
	
end


--------------------------------------------------------------
-- try to start the game
--------------------------------------------------------------
function StartGame(self)

print("Game Started")

    LOCALS["bWaitingForPlayers"] = false
    
	LOCALS["GameStarted"] = true
	
	-- iterate through players
	for pnum = 1, #PLAYERS do
	
	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])
	    local playerData = self:GetVar(PLAYERS[pnum])
	    
	    if (player and playerData) then
	    
	        -- close any tooltip
	        player:DisplayTooltip{ bShow = false, strText = "..." }

	        -- trigger the Countdown on all clients
	        player:ShowActivityCountdown
			{
				bPlayCountdownSound = false,
				bPlayAdditionalSound = true,
				sndName = "metallica - fuel",
				stateToPlaySoundOn = 1
			}

	    end

	end

    -- set a timer to Unpause and Go
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["COUNTDOWN_TIME"], "Go", self )


end


--------------------------------------------------------------
-- try to stop the game
--------------------------------------------------------------
function StopGame(self)

print("Game Stopped")

	LOCALS["bWaitingForPlayers"] = false

	GAMEOBJ:GetTimer():CancelAllTimers( self )
    LOCALS["GameStarted"] = false

	-- iterate through players
	for pnum = 1, #PLAYERS do

	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])

	    if (player) then
	    
			showSummaryDialog(self,player)
			
			RemovePlayerFromGame(self, player)
			
--			-- unstun player because he is done with race
--			player:RemoveStun
--			{
--				bCantMove = false,
--				bCantTurn = false,
--				bCantAttack = false,
--				bCantEquip = false,
--				bCantInteract = false
--			}			
--
	    end

	end

    LOCALS["PlayerNum"] = 0
	LOCALS["NumPlayersFinished"] = 0

end


--------------------------------------------------------------
-- try to stop the game that hasn't started yet
--------------------------------------------------------------
function AbortGame(self)

print("Game Aborted")

	LOCALS["bWaitingForPlayers"] = false
	
	GAMEOBJ:GetTimer():CancelAllTimers( self )
    LOCALS["GameStarted"] = false

	-- iterate through players
	for pnum = 1, #PLAYERS do

	    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])

	    if (player) then
	    
			RemovePlayerFromGame(self, player)
			player:ServerSetUserCtrlCompPause{bPaused = false}
			player:DisplayTooltip{ bShow = true, strText = "Game stopped, not enough players found.", iTime = 5000 }

	    end

	end

    LOCALS["PlayerNum"] = 0
	LOCALS["NumPlayersFinished"] = 0

end


--------------------------------------------------------------
-- Get cannon number from player id
--------------------------------------------------------------
function GetCannonNumFromPlayer(self, player)

	if (player) then
	
		local playerID = player:GetID()
		
		-- loop through cannons and find it
		for cnum = 1, #CANNONS do
	
			if (CANNONS[cnum]["Player"] == playerID) then
			
				return cnum
			
			end
			
		end
		
	end	
	
	return 0

end


--------------------------------------------------------------
-- Get cannon number from cannon id
--------------------------------------------------------------
function GetCannonNumFromCannon(self, cannon)

	if (cannon) then
	
		local cannonID = cannon:GetID()
		
		-- loop through cannons and find it
		for cnum = 1, #CANNONS do
	
			if (CANNONS[cnum]["Cannon"] == cannonID) then
			
				return cnum
			
			end
			
		end
		
	end	
	
	return 0

end


--------------------------------------------------------------
-- Remove player from cannon
--------------------------------------------------------------
function RemovePlayerFromCannon(self, player)

	if (player) then
	
		local cannonNum = GetCannonNumFromPlayer(self,player)
		
		if (cannonNum > 0) then
			
			CANNONS[cannonNum]["Player"] = 0

			self:PlayerLoaded{ playerID = player }
			player:SetFaction{ faction = 1 }
--player:ServerSetUserCtrlCompPause{bPaused = false}
		
		end
				
	end	

end


--------------------------------------------------------------
-- Game Message Handlers
--------------------------------------------------------------
--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

	-- set game state
	LOCALS["PlayerNum"] = 0
	LOCALS["NumPlayersFinished"] = 0
	LOCALS["CheckpointNum"] = 0
	LOCALS["CannonNum"] = 0
	LOCALS["GameStarted"] = false

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if (msg.name == "StartGame") then
		StartGame(self)
    end
    
    if (msg.name == "TimeOut") then
    	AbortGame(self)
    end
    
    if (msg.name == "EndGame") then
	    StopGame(self)
    end
    
    if (string.starts(msg.name,"RemovePlayerFromCannon")) then
    
   		local numPlayers = #PLAYERS_TO_REMOVE
   		
		   if (numPlayers > 0) then
			
			local player = GAMEOBJ:GetObjectByID(PLAYERS_TO_REMOVE[numPlayers])
			
			if (player) then

				
				PLAYERS_TO_REMOVE[numPlayers] = nil

				-- remove the player from the cannon
				RemovePlayerFromCannon(self,player)
				
			end
		
		end
		
    end
    
    if (msg.name == "Go") then

		-- iterate through players
		for pnum = 1, #PLAYERS do

		    local player = GAMEOBJ:GetObjectByID(PLAYERS[pnum])
		    local playerData = self:GetVar(PLAYERS[pnum])

		    if (player and playerData) then

				-- get start time
				local theTime = GAMEOBJ:GetSystemTime()

				-- set times in data
				playerData["startTime"] = theTime
				playerData["curLapTime"] = theTime
				
				self:SetVar(PLAYERS[pnum], playerData)

				-- unpause
                player:ServerSetUserCtrlCompPause{bPaused = false}

		    end

	    end

	end
	
end    


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	print ("Player Entered: " .. msg.playerID:GetName().name)
	
	local player = msg.playerID
	
	-- move player to level start location
	-- @TODO: Sometimes this teleport works and sometimes it does not.....
	player:Teleport{pos = CONSTANTS["PLAYER_ZONEIN_POS"],
	                bSetRotation = false}
	                
	player:SetRacingParameters{ fTurnSpeedMult = CONSTANTS["TURN_SPEED_MULT"] }

	-- unpause player
	player:ServerSetUserCtrlCompPause{bPaused = false}
	
	-- remove all current boards
	RemoveBoardsFromPlayer(self, player)	

end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

	-- checkpoint Loaded
	if (msg.templateID == CONSTANTS["CHECKPOINT_LOT"]) then

		-- store it		
		local chkpNum = IncrementVarAndReturn("CheckpointNum")
		CHECKPOINTS[chkpNum] = msg.objectID:GetID()
		
		-- update constant
		CONSTANTS["NUM_CHECKPOINTS"] = #CHECKPOINTS
			
	end
	

	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) then
print ("cannon added")		
		-- Override the cannon shooting parameters
		msg.objectID:SetVar("ImpactSkillID",CONSTANTS["CANNON_IMPACT_SKILL"])
		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["CANNON_PLAYER_OFFSET"],
		                                      projectileVelocity = CONSTANTS["CANNON_VELOCITY"], 
		                                      cooldown =           CONSTANTS["CANNON_REFIRE_RATE"], 
		                                      muzzlePosOffset =    CONSTANTS["CANNON_BARREL_OFFSET"], 
		                                      minDistance =        CONSTANTS["CANNON_MIN_DISTANCE"], 
		                                      timeLimit =          CONSTANTS["CANNON_TIMEOUT"]}
		
		-- store it		
		local chkpNum = IncrementVarAndReturn("CannonNum")
		local CannonData = {}
		CannonData["Cannon"] = msg.objectID:GetID()
		CannonData["Player"] = 0
		CANNONS[chkpNum] = CannonData
		
		-- update constant
		CONSTANTS["NUM_CANNONS"] = #CANNONS
			
	end	
	
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- get player
    local player = msg.sender
   
	-- cancel the missions on the player for now
	player:CancelMission{ missionID = CONSTANTS["RACE_MISSION_ID"] }
	player:CancelMission{ missionID = CONSTANTS["CANNON_MISSION_ID"] }

	-- User wants to retry or start new race
	if ((msg.iButton == 1 and msg.identifier == "Racing_Summary") or
	    (msg.iButton == 1 and msg.identifier == "Race_Mission")) then

		print("trigger restart")

		if (IsGameStarted(self)) then
			print("ERROR: game is not over yet!")
		    player:DisplayTooltip{ bShow = true, strText = "The race is still running, try again after the race is over.", iTime = 5000 }

		    player:ServerSetUserCtrlCompPause{bPaused = false}
		    return
		end

		-- get current players
		local curplayers = LOCALS["PlayerNum"]

		if (tonumber(curplayers) < CONSTANTS["MAX_PLAYERS"]) then

			-- get the player Data
			local playerData = self:GetVar(player:GetID())

			-- store the *new* player data for later use
			if (not playerData) then

			   AddNewPlayerToGame(self, player)

			-- we used to be a player so we have player data but
			-- it should contain nothing
			elseif (not playerData["ShouldBeEmpty"]) then
			
			   AddNewPlayerToGame(self, player)

			else

				print("ERROR: Player already exists in game!")

			end
		else
		
   			player:DisplayTooltip{ bShow = true, strText = "The race is full, try again after the race is over.", iTime = 5000 }
		
		end

	-- User wants to quit Racing
	elseif (msg.iButton == 0 and msg.identifier == "Racing_Summary") then
		
		print("trigger leave")

		-- unpause player
		player:ServerSetUserCtrlCompPause{bPaused = false}

	-- User wants to shoot cannons
	elseif (msg.iButton == 1 and msg.identifier == "Race_Cannon_Mission") then

		print("cannon trigger")
		
		-- find an unused cannon
        local cannonID = GetValidCannon(self)

		-- no cannons available
        if (tonumber(cannonID) <= 0) then
        	player:DisplayTooltip{ bShow = true, strText = "All cannons are full, try again later.", iTime= 5000 }
            print ("no cannons found")
			return
		end
		
		-- pause player
		player:ServerSetUserCtrlCompPause{bPaused = true}

        -- setup cannon vars
		local cannon = GAMEOBJ:GetObjectByID(cannonID)
		local cannonNum = GetCannonNumFromCannon(self, cannon)
		CANNONS[cannonNum]["Player"] = player:GetID()
		
		player:SetFaction{ faction = 2 }

		-- put the player in the cannon
		cannon:RequestActivityEnter{bStart = true, userID = player}
		

	end
	
end


--------------------------------------------------------------
-- Sent from checkpoints when a player passes them
--------------------------------------------------------------
function onCollision(self, msg)

	FlagCheckpoint(self, msg.objectID, msg.senderID:GetID())

end


--------------------------------------------------------------
-- Sent from goals when a player passes them
--------------------------------------------------------------
function onOffCollision(self, msg)

    FlagLap(self, msg.objectID)

end


--------------------------------------------------------------
-- Sent from the cannon when a player starts or stops the cannon
--------------------------------------------------------------
function onRequestActivityExit(self, msg)

	-- if the user quit, port him back

	-- store the player for transport at the end of the timer
	local numPlayers = #PLAYERS_TO_REMOVE + 1
	PLAYERS_TO_REMOVE[numPlayers] = msg.userID:GetID()

	-- start timer for game start
	GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "RemovePlayerFromCannon" .. msg.userID:GetID(), self )

end