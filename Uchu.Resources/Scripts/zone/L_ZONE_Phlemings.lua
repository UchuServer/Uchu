--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_START_POS"] = {x = -2.4, y = 153, z = -63.1}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}
CONSTANTS["NPC_START_POS"] = { x = 17.6, y = 153, z = -84 }

-- Other constants
CONSTANTS["PLAYER_FACTION"] = 1
CONSTANTS["INITIAL_PATH"] = "TestPath"
CONSTANTS["PHLEGMING"] = 2711
CONSTANTS["PHLEGMING_SAVE"] = 0.50
CONSTANTS["PLAYER_SKILL"] = 68
CONSTANTS["PLAYER_SKILL2"] = 74

--------------------------------------------------------------
-- Spawn Data
--------------------------------------------------------------
spawns = {}

spawn = {}
spawn[1] = { id = CONSTANTS["PHLEGMING"], time = 1.0, speed = 3.0, score = 50,  bwpRan = true, w1 = 1, w2 = 1, w3 = 1 }
--spawn[2] = { id = 2711, time = 2.0, speed = 5.0, score = 100, bwpRan = true, w1 = 1, w2 = 1, w3 = 1 }
spawns[1] = spawn
spawns[2] = spawn
spawns[3] = spawn
spawns[4] = spawn
spawns[5] = spawn

healthSpawns = {}
--healthSpawns[1] = { x = 34.33 , y= 270.0 , z= -12.23 }
--healthSpawns[2] = { x = 30.33 , y= 270.0 , z= -7.87 }
--healthSpawns[3] = { x = 25.29 , y= 270.0 , z= -2.68 }
--healthSpawns[4] = { x = 18.94 , y= 270.0 , z=  3.85 }
--healthSpawns[5] = { x = 12.46 , y= 270.0 , z= 10.52 }


--------------------------------------------------------------
-- Activity Constants
--------------------------------------------------------------
CONSTANTS["MAX_HEALTH"] = #healthSpawns
CONSTANTS["NUM_WAVES"] = #spawns
CONSTANTS["HEALTH_TEMPLATEID"] = 2018
CONSTANTS["FIRST_WAVE_START_TIME"] = 15.0
CONSTANTS["TIME_BETWEEN_WAVES"] = 3.0

--------------------------------------------------------------
-- String Table
--------------------------------------------------------------
strings = {}
strings["WAVE_TEXT"] = "Monkey "
strings["LIFE_LOST"] = "BBQ Lost!"


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------

--------------------------------------------------------------
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(self, varName, object)

    idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(varName, finalID)
   
end


--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(self, varName)

    targetID = self:GetVar(varName)
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end
	
end


--------------------------------------------------------------
-- try to start the game
--------------------------------------------------------------
function startGame(self, bSendRequest)

	-- get the cannon
	--local cannon = getObjectByName(self, "cannonObject")
	
	-- get the player
	--local player = getObjectByName(self, "activityPlayer")
	
	-- if we have both start it
	--if (player) then
    --		RESMGR:LoadObject { objectTemplate = 2711, 
    	--	                    bIsSmashable = true, 
    		--                    x = CONSTANTS["NPC_START_POS"].x, 
    		  --                  y = CONSTANTS["NPC_START_POS"].y, 
    		    --                z = CONSTANTS["NPC_START_POS"].z,
    		      --             owner = self }   
    		                   
		-- send request to start cannon if needed
	--	if (bSendRequest == true) then
	--		cannon:RequestActivityStartStop{bStart = true, userID = player}
	--	end

	--end
	
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
	self:SetVar("Health", CONSTANTS["MAX_HEALTH"])
	self:SetVar("DisplayWaveNum", 0)
	self:SetVar("CurSpawnNum", 0)
	self:SetVar("CurSpawnHealthNum", 0)
	self:SetVar("GameStarted", true)
	self:SetVar("NumDeadSpawns", 0)
	self:SetVar("NextWave", 0)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	self:SetVar("GameWave",0)
	
	-- spawn the health objects
	print ("Spawning Health Items")
	for k,v in pairs(healthSpawns) do SpawnHealth(k,v,self) end

	local player = getObjectByName(self, "activityPlayer")			
	
	-- start the first wave
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FIRST_WAVE_START_TIME"], "SpawnWave1",self ) 		
		
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
-- spawn an object for the game
--------------------------------------------------------------
function SpawnObject(num, spawn, self) 

	-- get the current spawn number
	local spawnNum = IncrementVarAndReturn(self,"SpawnNum")

	-- save the spawn data for the object when it is loaded
	local SpawnData = {sdTemplate = spawn.id, 
	             sdSpeed = spawn.speed, 
	             sdScore = spawn.score, 
	             sdUseRandomPath = spawn.bwpRan, 
	             sdWP1 = spawn.w1, 
	             sdWP2 = spawn.w2, 
	             sdWP3 = spawn.w3}
	

	-- store the data
	self:SetVar("SpawnData" .. spawnNum, SpawnData)

	-- set the timer to spawn the object
	local timerName = "DoSpawn" .. spawnNum
	GAMEOBJ:GetTimer():AddTimerWithCancel( spawn.time, timerName, self ) 	
	                     
end


--------------------------------------------------------------
-- spawns the health objects
--------------------------------------------------------------
function SpawnHealth(num, spawn, self) 

	-- load the object in the world
	RESMGR:LoadObject { objectTemplate = CONSTANTS["HEALTH_TEMPLATEID"], 
	                    bIsSmashable = true, 
	                    x = spawn.x, 
	                    y = spawn.y, 
	                    z = spawn.z,
	                    owner = self }                     

end


--------------------------------------------------------------
-- destroys all health objects and spawns
--------------------------------------------------------------
function DestroyAllSpawns(self) 

	for health = 1, CONSTANTS["MAX_HEALTH"] do
		local healthObject = getObjectByName(self, "healthSpawn" .. health)	
		if (healthObject) then
			print("removing health item " .. health)
			if (healthObject:IsDead().bDead == false) then
				healthObject:Die{killerID = healthObject}
			end
		end	
	end
	
	local maxSpawnNum = self:GetVar("CurSpawnNum")
	for spawn = 1, maxSpawnNum do
		local spawnObject = getObjectByName(self, "spawnObject" .. spawn)	
		if (spawnObject) then
			print("removing spawn object " .. spawn)
			if (spawnObject:IsDead().bDead == false) then
				spawnObject:Die{killerID = spawnObject}
			end
		end	
	end	
end


--------------------------------------------------------------
-- sends a message to display the current wave number to player
--------------------------------------------------------------
function DisplayWaveNumberToPlayer(self) 

	local player = getObjectByName(self, "activityPlayer")			
	if (player) then
	
		local displayWave = IncrementVarAndReturn(self,"DisplayWaveNum")
		local textString = strings["WAVE_TEXT"] .. displayWave
		
	end
end


--------------------------------------------------------------
-- show the summary dialog
--------------------------------------------------------------
function showSummaryDialog(self)

	-- get player
	local player = getObjectByName(self, "activityPlayer")
	
	if (player) then
		-- get the player's score and time
		local score = self:GetVar("GameScore")
		local time = self:GetVar("GameTime")
		local wave = self:GetVar("GameWave")
		local strText = ""
		if (score and time and wave) then
			strText = "Score/Wave: " .. score .. "/" .. wave .. ",  Retry?"
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
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end


--------------------------------------------------------------
-- Increment a saved variable and return its new value
--------------------------------------------------------------
function IncrementVarAndReturn(self,varName)
	local value = self:GetVar(varName)
	if (value) then
		value = value + 1
	else
		value = 1
	end
	self:SetVar(varName,value)
	return value
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
	self:SetVar("NumDeadSpawns", 0)
	self:SetVar("CurSpawnHealthNum", 0)
	self:SetVar("NextWave", 0)
	self:SetVar("Health", CONSTANTS["MAX_HEALTH"])
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	self:SetVar("GameWave",0)
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
			-- get the wave number from the rest of the string
			local waveNum = string.sub(msg.name,10)
			
			
			DisplayWaveNumberToPlayer(self)
			
	    
			print("Spawning Wave " .. waveNum)
			
			self:SetVar("GameWave", waveNum)
			
			-- setup spawns for wave
			for k,v in pairs(spawns[tonumber(waveNum)]) do SpawnObject(k,v,self) end
			
			-- move to the next wave
			waveNum = waveNum + 1
			
			-- repeat the last wave if we are at the max
			if (waveNum <= CONSTANTS["NUM_WAVES"]) then
				--waveNum = CONSTANTS["NUM_WAVES"];
			

			-- store next wave number			
			self:SetVar("NextWave", waveNum)
			
			-- setup next wave
			GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["TIME_BETWEEN_WAVES"], "SpawnWave" .. waveNum,self ) 
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
			
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = templateID, 
    		                    bIsSmashable = true, 
    		                    x = CONSTANTS["NPC_START_POS"].x, 
    		                    y = CONSTANTS["NPC_START_POS"].y, 
    		                    z = CONSTANTS["NPC_START_POS"].z,
    		                    owner = self }                     
		end
		
    end
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- check for health spawns, MUST BE FIRST
	if msg.templateID == CONSTANTS["HEALTH_TEMPLATEID"] then
	
		-- set the health spawn faction to same as a player
		msg.childID:SetFaction{faction = CONSTANTS["PLAYER_FACTION"]}
		
		-- get current spawn health number and increment
		local healthSpawnNum = IncrementVarAndReturn(self,"CurSpawnHealthNum")
		
		-- store health spawn for use later
		storeObjectByName(self, "healthSpawn" .. healthSpawnNum, msg.childID)		
		
	else
		-- get the current spawn number and increment it
		local spawnNum = IncrementVarAndReturn(self,"CurSpawnNum")
		
		-- get the template out of the spawn data		
		local SpawnData = self:GetVar("SpawnData" .. spawnNum)	
		
		-- check for the right template
		if msg.templateID == CONSTANTS["PHLEGMING"] then 

			-- store spawn for use later
			storeObjectByName(self, "spawnObject" .. spawnNum, msg.childID)		
		
			-- store who the parent is
			storeParent(self, msg.childID)
		    
			-- store the spawn data in the child
			msg.childID:SetVar("SpawnData", SpawnData)
		    
			-- store the waypoint data in the child
			msg.childID:FollowWaypoints{bPaused=true}
			msg.childID:SetCurrentPath{pathName=CONSTANTS["INITIAL_PATH"]}
		--	if (SpawnData.sdUseRandomPath == true) then
		--		-- do random waypoints
		--		local wp1 = math.random(1,5)
		--		local wp2 = math.random(1,5)
		--		local wp3 = math.random(1,5)
		--		
		--		msg.childID:SetVar("Waypoint1",waypoints[1][wp1])
		--		msg.childID:SetVar("Waypoint2",waypoints[2][wp2])
		--		msg.childID:SetVar("Waypoint3",waypoints[3][wp3])
		--	else
		--		-- do normal waypoints
		--		msg.childID:SetVar("Waypoint1",waypoints[1][SpawnData.sdWP1])
		--		msg.childID:SetVar("Waypoint2",waypoints[2][SpawnData.sdWP2])
		--		msg.childID:SetVar("Waypoint3",waypoints[3][SpawnData.sdWP3])
		--	end
		    
		end
	end
	
end


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	print ("Player Entered: " .. msg.playerID:GetName().name)
	
	-- move player to level start location
	-- @TODO: Sometimes this teleport works and sometimes it does not.....
	local player = msg.playerID
	
		-- store the player for later use
	local storedPlayer = getObjectByName(self, "activityPlayer")
		if (storedPlayer == nil) then
			storeObjectByName(self, "activityPlayer", player)
		end	
	
	--player:ServerSetUserCtrlCompPause{bPaused = true}
	
	player:Teleport{pos = CONSTANTS["PLAYER_START_POS"], 
	                x = CONSTANTS["PLAYER_START_ROT"].x, 
	                y = CONSTANTS["PLAYER_START_ROT"].y, 
	                z = CONSTANTS["PLAYER_START_ROT"].z, 
	                w = CONSTANTS["PLAYER_START_ROT"].w, 
	                bSetRotation = true}
	                
	player:AddSkill{skillID = CONSTANTS["PLAYER_SKILL"], temporary = true, temporaryReplaceAttack=false}
	player:AddSkill{skillID = CONSTANTS["PLAYER_SKILL2"], temporary = true, temporaryReplaceAttack=false}
	player:MapSkill{skillID = CONSTANTS["PLAYER_SKILL"], slot = 3}
	player:MapSkill{skillID = CONSTANTS["PLAYER_SKILL2"], slot = 4}

	if (self:GetVar("GameStarted") == false) then
		-- get the player
		
		-- try to start the game
		--startGame(self, true)
		
		
		player:DisplayMessageBox{bShow = true, 
								 imageID = 2, 
								 callbackClient = GAMEOBJ:GetZoneControlID(), 
								 text = "Save the monkeys! They're about to run! You need to save " .. (CONSTANTS["PHLEGMING_SAVE"]*100) .. "% of the monkeys. Are you ready?", 
								 identifier = "PhlegmingMap"}--Shooting_Gallery_Summary"}
	end
	
end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
--function onObjectLoaded(self, msg)
--
--	-- Cannon Object Loaded
--	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) then
--
--		-- Override the cannon shooting parameters
--		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["CANNON_PLAYER_OFFSET"],
--		                                      projectileVelocity = CONSTANTS["CANNON_VELOCITY"], 
--		                                      cooldown =           CONSTANTS["CANNON_REFIRE_RATE"], 
--		                                      muzzlePosOffset =    CONSTANTS["CANNON_BARREL_OFFSET"], 
--		                                      minDistance =        CONSTANTS["CANNON_MIN_DISTANCE"], 
--		                                      timeLimit =          CONSTANTS["CANNON_TIMEOUT"]}
--		
--		-- store the cannon object for use later
--		storeObjectByName(self, "cannonObject", msg.objectID)
--	
--		if (self:GetVar("GameStarted") == false) then	
--			-- try to start the game
--			startGame(self, true)
--		end
--			
--	end
--
--end


--------------------------------------------------------------
-- Sent from a ninja when he gets to his final waypoint
--------------------------------------------------------------
--function onArrived(self, msg)
--
--	if (self:GetVar("GameStarted") == true) then
--
--		-- get the health left
--		local health = self:GetVar("Health")
--		
--		if (health > 0) then
--
--			-- tell the player they lost a life
--			local player = getObjectByName(self, "activityPlayer")			
--			
--			-- remove health
--			newhealth = health - 1
--			self:SetVar("Health", newhealth)
--
--			-- destroy a health object
--			local healthObject = getObjectByName(self, "healthSpawn" .. health)	
--			if (healthObject) then
--				healthObject:Die{killerID = healthObject}
--			end
-- 
-- 			-- game over
--			if (newhealth <= 0) then
--				print("health empty, game over")
--			    stopGame(self, true)
--			end
--
--		end
--		
--	end	
--end


--------------------------------------------------------------
-- Sent from the cannon when a player starts or stops the activity
--------------------------------------------------------------
--function onRequestActivityStartStop(self, msg)
--
--	-- if the user quit, stop the game
--	if (msg.bStart == false) then
--
--		stopGame(self, false)
--		
--		showSummaryDialog(self)
--		
--	elseif (msg.bStart == true) then
--	
--		-- get the player
--		local player = getObjectByName(self, "activityPlayer")	
--		
--		-- store the player just in case
--		if (player == nil) then
--			player = msg.userID
--			storeObjectByName(self, "activityPlayer", player)
--		end
--
--		-- start the game		
--		startGame(self,false)
--		
--		DoGameStartup(self)
--		
--	end
--	
--end
--
--
----------------------------------------------------------------
---- Sent from the cannon to get a score for the player
----------------------------------------------------------------
--function onDoCalculateActivityRating(self, msg)
--    
--	-- save the score and time for later use
--	self:SetVar("GameScore",msg.score)
--	self:SetVar("GameTime",msg.time)
--	
--	-- wave should already be set
--	
--    -- also return the score as the result for the activity
--	msg.outActivityRating = msg.score
--	return msg
--	
--end
--
--
----------------------------------------------------------------
---- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- make sure this is the right player
	local player = getObjectByName(self, "activityPlayer")	
	if (player:GetID() == msg.sender:GetID()) then
		
--		-- User wants to retry
		if (msg.iButton == 1 and msg.identifier == "PhlegmingMap") then

		DoGameStartup(self)

--			startGame(self,true)
--		
--		-- User wants to quit
		elseif (msg.iButton == 0 and msg.identifier == "PhlegmingMap") then
--			
--			-- go back to the other zone
			player:ServerSetUserCtrlCompPause{bPaused = false}
			player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true }
--		
		end
--
	end
end
--
--
----------------------------------------------------------------
---- Sent from the spawn objects on death
----------------------------------------------------------------
function onUpdateMissionTask(self, msg)
	local player = getObjectByName(self, "activityPlayer")
	
	local dead = self:GetVar("NumDeadSpawns")
	local win = self:GetVar("NumFinishedSpawns")
	
	if (dead == nil) then
		dead = 0
	end
	
	if (win == nil) then
		win = 0
	end	

	if (msg.taskType == "phlegming_die") then
		dead = IncrementVarAndReturn(self,"NumDeadSpawns")
		
	end
	if (msg.taskType == "phlegming_win") then
		win = IncrementVarAndReturn(self,"NumFinishedSpawns")
		
		msg.target:FollowWaypoints{bPaused=true}
		msg.target:PlayEmote{ emoteID = "cheer" }
	end

	
--
--	local numDeadSpawns = IncrementVarAndReturn(self,"NumDeadSpawns")
--	
--	-- if all the spawns are dead or gone
--	if (numDeadSpawns == maxSpawnNum) then
--
--		-- spawn the next wave
--		local waveNum = self:GetVar("NextWave")
--		
--		-- setup next wave
--		GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["TIME_BETWEEN_WAVES"], "SpawnWave" .. waveNum,self ) 
--	end
--
end