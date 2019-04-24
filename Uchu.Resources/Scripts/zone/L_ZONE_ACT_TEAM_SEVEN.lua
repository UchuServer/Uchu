--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_START_POS"] = {x = -500.542480, y = 229.773178, z = -900.577438}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}

-- cannon constants
CONSTANTS["PINBALL_TEMPLATEID"] = 2706
CONSTANTS["PINBALL_PLAYER_OFFSET"] = {x = 6.652, y = 0, z = -5.716}
CONSTANTS["PINBALL_VELOCITY"] = 160.0
CONSTANTS["PINBALL_TIMEOUT"] = -1

-- cannon impact skills
CONSTANTS["CANNON_IMPACT_SKILL"] = {34, 34, 61, 62}


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------
--------------------------------------------------------------
-- try to start the game
--------------------------------------------------------------
function startGame(self, bSendRequest)

	-- get the cannon
	local cannon = getObjectByName(self, "cannonObject")
	
	-- get the player
	local player = getObjectByName(self, "activityPlayer")
	
	-- if we have both start it
	if ((cannon) and (player)) then

		-- send request to start cannon if needed
		if (bSendRequest == true) then
			cannon:RequestActivityStartStop{bStart = true, userID = player}
		end

	end
	
end


--------------------------------------------------------------
-- try to stop the game
--------------------------------------------------------------
function stopGame(self, bSendRequest)

	-- get the cannon
	local cannon = getObjectByName(self, "cannonObject")
	
	-- get the player
	local player = getObjectByName(self, "activityPlayer")
	
	-- if we have both stop it if we need to
	if ((bSendRequest == true) and (cannon) and (player)) then
		cannon:RequestActivityStartStop{bStart = false, userID = player}
	end		

	DoGameShutdown(self)
	
end


--------------------------------------------------------------
-- handle all the game startup data
--------------------------------------------------------------
function DoGameStartup(self)

	-- set game state and vars
	self:SetVar("SpawnNum", 0)
	self:SetVar("CurSpawnNum", 0)
	self:SetVar("GameStarted", true)
	self:SetVar("ThisWave", 0)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	
	-- start the first wave
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FIRST_WAVE_START_TIME"], "SpawnWave1",self ) 		

	-- set the cannon reticule back to start	
	setCannonReticuleSize(self, 1)
	
end


--------------------------------------------------------------
-- handle all the game shutdown data
--------------------------------------------------------------
function DoGameShutdown(self)

	self:SetVar("GameStarted", false)
	
	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	
	-- despawn all spawns
	DestroyAllSpawns(self)
		
end


--------------------------------------------------------------
-- Get a random path
--------------------------------------------------------------
function GetRandomPath(spawn)

	-- pick a random 
	local ran = math.random(1,#spawn.path)
	return spawn.path[ran]	
	
end


--------------------------------------------------------------
-- spawn an object for the game
--------------------------------------------------------------
function SpawnObject(num, spawn, self, bSpawnNow) 

	-- get the current spawn number
	local spawnNum = IncrementVarAndReturn(self,"SpawnNum")

	-- save the spawn data for the object when it is loaded
	local SpawnData = {sdTemplate = spawn.id, 
				 sdRespawn = spawn.bRespawn,
	             sdSpeed = spawn.speed, 
	             sdScore = spawn.score, 
	             sdPath = GetRandomPath(spawn),
	             sdnum = num,
	             sdChangeSpeed = spawn.bChangeSpeed,
	             sdSpeedChance = spawn.speedChangeChance,
	             sdMinSpeed = spawn.minSpeed,
	             sdMaxSpeed = spawn.maxSpeed,
	             sdMovingPlat = spawn.bMovingPlatform,
	             sdDespawnTime = spawn.despawnTime,
	             sdTimeScore = spawn.timeScore,
	             bSpawned = false}
	
	-- store the data
	self:SetVar("SpawnData" .. spawnNum, SpawnData)

	-- set the timer to spawn the object
	local timerName = "DoSpawn" .. spawnNum
	
	if (bSpawnNow == true) then
		if (spawn.initSpawnTime > 0) then
		print ("spawning with time " .. spawn.initSpawnTime)
			GAMEOBJ:GetTimer():AddTimerWithCancel( spawn.initSpawnTime, timerName, self ) 	
		else
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, timerName, self ) 	
		end
	elseif (spawn.bRespawn == true) then
		-- pick a random spawn time
		local ranSpawnTime = (math.random() * (spawn.maxTime - spawn.minTime)) + spawn.minTime
		GAMEOBJ:GetTimer():AddTimerWithCancel( ranSpawnTime, timerName, self ) 	
	end
	                     
end


--------------------------------------------------------------
-- destroys all spawns
--------------------------------------------------------------
function DestroyAllSpawns(self) 

	local maxSpawnNum = self:GetVar("CurSpawnNum")
	for spawn = 1, maxSpawnNum do
		local spawnObject = getObjectByName(self, "spawnObject" .. spawn)	
		if (spawnObject) then
			if (spawnObject:Exists() and not spawnObject:IsDead().bDead) then
				print("removing spawn object " .. spawn)
				-- @TODO: need a better way to clear a wave, deletequeue message
				--        creates tons of other crap messages
				spawnObject:Die{killerID = spawnObject}
				--spawnObject:MoveToDeleteQueue{}
			end
		end	
	end	
	-- reset vars
	self:SetVar("SpawnNum", 0)
	self:SetVar("CurSpawnNum", 0)
end

--------------------------------------------------------------
-- look through spawn data to find the most recent data
-- that matches the template, returns nil or data
--------------------------------------------------------------
function GetLatestSpawnDataByTemplate(self,templateID)

	local spawnNum = self:GetVar("SpawnNum")
	while (spawnNum > 0) do
		
		-- get the data
		local SpawnData = self:GetVar("SpawnData" .. spawnNum)
		
		-- check spawn flag and template
		if (SpawnData.bSpawned == false and templateID == SpawnData.sdTemplate) then
			
			-- set spawned flag
			SpawnData.bSpawned = true
			
			-- re-save data
			self:SetVar("SpawnData" .. spawnNum, SpawnData)
			
			-- return the good data
			return SpawnData
		end
		
		-- try prev spawn data
		spawnNum = spawnNum - 1		
	
	end
	
	return nil

end

--------------------------------------------------------------
-- show the summary dialog
--------------------------------------------------------------
function showSummaryDialog(self)

	-- get player
	local player = getObjectByName(self, "activityPlayer")
	
-- TODO: Customize score window
	
	if (player) then
		-- get the player's score and time
		local score = self:GetVar("GameScore")
		local time = self:GetVar("GameTime")
		local strText = ""
		if (score and time) then
			strText = "Score: " .. score .. ",  Retry?"
		else
			strText = "Retry?"
		end

		-- show the summary message box
		player:DisplayMessageBox{bShow = true, 
								 imageID = 2, 
								 callbackClient = GAMEOBJ:GetZoneControlID(), 
								 text = strText, 
								 identifier = "Shooting_Gallery_Summary"}
	end

end



--------------------------------------------------------------
-- Add more time to the current wave timer
--------------------------------------------------------------
function AddTimeToWave(self, timeToAdd)

	-- get wave number
	local waveNum = self:GetVar("ThisWave")
	
	-- check on next wave
	waveNum = waveNum + 1
	
	-- get correct timer name
	local timerName = "GameOver"
	if (waveNum <= CONSTANTS["NUM_WAVES"]) then
		timerName = "SpawnWave" .. waveNum
	end
	
	-- get the time left
	local theTime = GAMEOBJ:GetTimer():GetTime(timerName,self )
	if (theTime > 0.0) then
	
		-- cancel timer and add a new one with more time
		GAMEOBJ:GetTimer():CancelTimer(timerName, self)
		theTime = theTime + timeToAdd
		GAMEOBJ:GetTimer():AddTimerWithCancel( theTime, timerName, self ) 
		
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
	self:SetVar("GameStarted", false)
	self:SetVar("CurSpawnNum", 0)
	self:SetVar("ThisWave", 0)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    -- parse the name to get out the wave number
    -- use the wave number to select the spawns
    -- of format "SpawnWaveXXX" where XXX is the spawn number
    
    if (string.starts(msg.name,"SpawnWave")) then

		if (self:GetVar("GameStarted") == true) then
		
			-- get rid of current spawns
			DestroyAllSpawns(self)
			
			-- cancel all timers
			GAMEOBJ:GetTimer():CancelAllTimers( self )
			
			-- get the wave number from the rest of the string
			local waveNum = string.sub(msg.name,10)
	    
			print("Spawning Wave " .. waveNum)
			
			-- store this wave number			
			self:SetVar("ThisWave", waveNum)			
			
			-- display wave number to player
			
			
			-- change cannon reticule
			setCannonReticuleSize(self, waveNum)
			
			-- setup spawns for wave
			for k,v in pairs(spawns[tonumber(waveNum)]) do SpawnObject(k,v,self,true) end
			
			-- move to the next wave
			waveNum = waveNum + 1
			
			-- there are no more waves left
			-- so stop game after this wave
			if (waveNum > CONSTANTS["NUM_WAVES"]) then
			
				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( waves[tonumber(waveNum) - 1].timeLimit, "GameOver", self ) 
				
			else
			
				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( waves[tonumber(waveNum) - 1].timeLimit, "SpawnWave" .. waveNum,self ) 
		
			end

			
		end
    end    
    
    -- parse the name to get out the spawn number
    -- use the spawn number to select the template id
    -- load the object
    -- of format "DoSpawnXXX" where XXX is the spawn number
    
    if (string.starts(msg.name,"DoSpawn")) then

		if (self:GetVar("GameStarted") == true) then

			-- get the spawn number from the rest of the string
			local spawnNum = string.sub(msg.name,8)
			
			-- get the template out of the spawn data		
			local SpawnData = self:GetVar("SpawnData" .. spawnNum)
			local templateID = SpawnData.sdTemplate
			
			print("spawning " .. spawnNum)
			
			-- get the position of the first waypoint
			local startPos = GAMEOBJ:GetWaypointPos( SpawnData.sdPath, 1 )
			
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = templateID, 
    		                    bIsSmashable = true,
    		                    x = startPos.x, 
    		                    y = startPos.y, 
    		                    z = startPos.z,
    		                    owner = self }                     
		end
		
    end
    
    -- end the game
    
    if (msg.name == "GameOver") then
		stopGame(self, true)
    end
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- look through spawn data to find the most recent data
	-- that matches the template
	local SpawnData = GetLatestSpawnDataByTemplate(self,msg.templateID)
	
	if (SpawnData) then
		
		local curSpawnNum = IncrementVarAndReturn(self,"CurSpawnNum")
	
		-- store spawn for use later
		storeObjectByName(self, "spawnObject" .. curSpawnNum, msg.childID)		
	
		-- store who the parent is
		storeParent(self, msg.childID)
	    
		-- store the spawn data in the child
		msg.childID:SetVar("SpawnData", SpawnData)
		
		msg.childID:SetPathingSpeed{ speed = SpawnData.sdSpeed }
		
		if (SpawnData.sdMovingPlat == true) then
			msg.childID:SetMovingPlatformParams{ wsPlatformPath = SpawnData.sdPath, iStartIndex = 0 }		
		else
			-- assign child's waypoint
			msg.childID:SetVar("attached_path",SpawnData.sdPath)
			msg.childID:SetVar("attached_path_start",0)
			
			-- start child on path
			msg.childID:FollowWaypoints()
		end
	   
	else
		-- error
		print("Error: Spawned object " .. msg.templateID .. " but saved data not found for it!")
	end
	
end


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	print ("Player Entered: " .. msg.playerID:GetName().name)
	
	-- stun and move player to level start location
	-- @TODO: Sometimes this teleport works and sometimes it does not.....
	local player = msg.playerID
	
	player:ServerSetUserCtrlCompPause{bPaused = true}
	
--	player:Teleport{pos = CONSTANTS["PLAYER_START_POS"], 
--	                x = CONSTANTS["PLAYER_START_ROT"].x, 
--	                y = CONSTANTS["PLAYER_START_ROT"].y, 
--	                z = CONSTANTS["PLAYER_START_ROT"].z, 
--	                w = CONSTANTS["PLAYER_START_ROT"].w, 
--	                bSetRotation = true}

	if (self:GetVar("GameStarted") == false) then
		-- get the player
		local player = getObjectByName(self, "activityPlayer")	
		
		-- store the player for later use
		if (player == nil) then
			storeObjectByName(self, "activityPlayer", msg.playerID)
		end
		
		-- try to start the game
		startGame(self, true)
	end
	
end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

	-- Cannon Object Loaded
	if (msg.templateID == CONSTANTS["PINBALL_TEMPLATEID"]) then
		
		-- Override the cannon shooting parameters
		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["PINBALL_PLAYER_OFFSET"],
		                                      projectileVelocity = CONSTANTS["PINBALL_VELOCITY"], 
		                                      timeLimit =          CONSTANTS["PINBALL_TIMEOUT"]}
		
		-- store the cannon object for use later
		storeObjectByName(self, "cannonObject", msg.objectID)
	
		if (self:GetVar("GameStarted") == false) then	
			-- try to start the game
			startGame(self, true)
		end
			
	end

end


--------------------------------------------------------------
-- Sent from the cannon when a player starts or stops the activity
--------------------------------------------------------------
function onRequestActivityStartStop(self, msg)

	-- if the user quit, stop the game
	if (msg.bStart == false) then

		stopGame(self, false)
		
		showSummaryDialog(self)

	elseif (msg.bStart == true) then
	
		-- get the player
		local player = getObjectByName(self, "activityPlayer")	
		
		-- store the player just in case
		if (player == nil) then
			player = msg.userID
			storeObjectByName(self, "activityPlayer", player)
		end

		-- start the game		
		startGame(self,false)
		
		DoGameStartup(self)
		
	end
	
end


--------------------------------------------------------------
-- Sent from the cannon to get a score for the player
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)
    
	-- save the score and time for later use
	self:SetVar("GameScore",msg.fValue1)
	self:SetVar("GameTime",msg.fValue2)
	
    -- also return the score as the result for the activity
	msg.outActivityRating = msg.fValue1
	return msg
	
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- make sure this is the right player
	local player = getObjectByName(self, "activityPlayer")	
	if (player:GetID() == msg.sender:GetID()) then
		
		-- User wants to retry
		if (msg.iButton == 1 and msg.identifier == "Shooting_Gallery_Summary") then
		
			startGame(self,true)
		
		-- User wants to quit
		elseif (msg.iButton == 0 and msg.identifier == "Shooting_Gallery_Summary") then
			
			-- go back to the other zone
			player:ServerSetUserCtrlCompPause{bPaused = false}
			--player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true }
		
		end

	end
end


--------------------------------------------------------------
-- Sent from the spawn objects on death
--------------------------------------------------------------
function onUpdateMissionTask(self, msg)

	-- get this wave number			
	local waveNum = self:GetVar("ThisWave")

	if (self:GetVar("GameStarted") == true) and (waveNum) and (waveNum > 0) then	

		-- get the spawn data
		local spawnData = msg.target:GetVar("SpawnData")
		
		-- spawn the right object
		if (spawnData) and (spawnData.sdRespawn == true) then
			SpawnObject(spawnData.sdnum,spawns[waveNum][spawnData.sdnum],self,false)
		end
		
		-- check for time adding
		if (spawnData.sdTimeScore > 0.0) then
			AddTimeToWave(self,spawnData.sdTimeScore)
		end
	
	end
	
end