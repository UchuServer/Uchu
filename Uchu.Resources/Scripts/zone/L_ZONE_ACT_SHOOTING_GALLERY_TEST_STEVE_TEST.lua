--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('o_ShootingGallery')

-- @TODO: Add Path Changing on waypoint, need to get [Closest Waypoint on New Path]
--        Also need [Number of Waypoints on Path]
-- @TODO: Optimize

--------------------------------------------------------------
-- Locals and Constants
LOCALS = {}
CONSTANTS = {}
--------------------------------------------------------------

-- Start Location for the Zone
CONSTANTS["PLAYER_START_POS"] = {x = -908.542480, y = 229.773178, z = -7.577438}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}

-- cannon constants
CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["CANNON_PLAYER_OFFSET"] = {x = 6.652, y = 0, z = -1.188}
CONSTANTS["CANNON_VELOCITY"] = 160.0
CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 800.0
CONSTANTS["CANNON_BARREL_OFFSET"] = {x = 0, y = 4.3, z = 9}
CONSTANTS["CANNON_TIMEOUT"] = -1
CONSTANTS["CANNON_FOV"] = 58.6
CONSTANTS["CANNON_USE_LEADERBOARDS"] = true

-- for Animations
CONSTANTS["VALID_ACTORS"] = {3109, 3110, 3111, 3112, 3125, 3126}
ACTORS = {}					-- stores actors for the instance

-- for Effects
CONSTANTS["VALID_EFFECTS"] = {3122}
EFFECTS = {}					-- stores effects for the instance


-- cannon impact skills
--CONSTANTS["CANNON_IMPACT_SKILL"] = {34, 34, 61, 62}
--CONSTANTS["CANNON_RETICULE_EFFECT"] = {"inRange", "inRange", "inRange2", "inRange3"}


--------------------------------------------------------------
-- Wave Data
waves = {}
PLAYER_SCORE = {}
--------------------------------------------------------------

-- Syntax:     [Time]   [Text to Show Player]
--------------------------------------------------------------
AddWave(waves,	30.0,	"Wave;One"   )		
AddWave(waves,	30.0,	"Wave;Two" )
AddWave(waves,	30.0,	"Wave;Three" )


-- wave constants
CONSTANTS["NUM_WAVES"] = #waves
CONSTANTS["FIRST_WAVE_START_TIME"] = 4.0
CONSTANTS["IN_BETWEEN_WAVE_PAUSE"] = 7.0

--------------------------------------------------------------
-- Spawn Data
spawns = {}
SPAWN_DATA = {}

-- AddPath - adds a path to a spawn. Can have one or more.
-- Syntax:  AddPath(paths, "[Path Name]")


-- AddSpawn - configures data for the spawn and adds it. Can
--            only have one.
-- Syntax: AddSpawn(spawn, paths, [Template ID],
--                  [Initial Spawn Time Min], [Initial Spawn Time Max], [Does Respawn],
--                  [Respawn Time Min], [Respawn Time Max],
--                  [Initial Speed], [Score], [Change Speed at Waypoints],
--                  [Chance to Change Speed], [Min Speed], [Max Speed],
--                  [Is Moving Platform], [Despawn On Last Waypoint], [Time Score])
--
-- NOTE: All times are in Seconds (i.e. 3.50 = 3 and a half seconds)
--------------------------------------------------------------

--------------------------------------------------------------
-- Wave 1 Spawns
spawn = {}
--------------------------------------------------------------

	-- Duck
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Duck_1")
	AddPath(paths,	"Wave_1_Duck_1-2")
	AddSpawn(spawn,paths,
		 2970,	0.0,	0.0,	true,	0,	0,	2.0,	100,		false,	0.0,	1.0,	1.0,	false,		false,		0.0)

	-- Ship
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ship_1")
	AddSpawn(spawn,paths,
		 2974,	0.0,	0.0,	true,	0,	0,	2.0,	250,		false,	0.0,	1.0,	1.0,	false,		false,		0.0)

	-- Sub
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Sub_1")
	AddPath(paths,	"Wave_1_Sub_1-2")
	AddSpawn(spawn,paths,
		 2972,	0.0,	0.0,	true,	0,	0,	2.0,	500,		false,	0.0,	1.0,	1.0,	false,		false,		0.0)

	-- Ness
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Ness_1")
	AddPath(paths,	"Wave_1_Ness_1-2")
	AddSpawn(spawn,paths,
		 2166,	0.0,	0.0,	true,	0,	0,	2.0,	1000,		false,	0.0,	1.0,	1.0,	false,		false,		0.0)

	-- Pearl
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Pearl")
	AddSpawn(spawn,paths,
		 2976,	9.0,	29.0,	false,	0,	0,	2.0,	0,		false,	0.0,	1.0,	1.0,	true,		true,		5.0)


-- Add the spawns for the wave
AddSpawnsForWave(spawns,spawn)


--------------------------------------------------------------
-- Wave 2 Spawns
spawn = {}
--------------------------------------------------------------

	-- Duck
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_1_Duck_1")
	AddPath(paths,	"Wave_1_Duck_1-2")
	AddSpawn(spawn,paths,
		 2970,	0.0,	0.0,	true,	0,	4.0,	1,	200,		true,	0.50,	1.0,	2,	false,		false,		0.0)

	-- Duck Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Duck_2")
	AddSpawn(spawn,paths,
		 2969,	0.0,	0.0,	true,	0,	4.0,	2,	400,		true,	0.50,	2,	3.5,	false,		false,		0.0)

	-- Ship
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ship_1")
	AddSpawn(spawn,paths,
		 2974,	0.0,	0.0,	true,	0,	4.0,	1,	300,		true,	0.50,	1.0,	2,	false,		false,		0.0)

	-- Ship Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ship_2")
	AddSpawn(spawn,paths,
		 2973,	0.0,	0.0,	true,	0,	4.0,	2,	750,		true,	0.50,	2,	3.50,	false,		false,		0.0)

	-- Sub
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Sub_1")
	AddSpawn(spawn,paths,
		 2972,	0.0,	0.0,	true,	0,	4.0,	1,	400,		true,	0.50,	1.0,	2,	false,		false,		0.0)

	-- Sub Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Sub_2")
	AddSpawn(spawn,paths,
		 2971,	0.0,	0.0,	true,	0,	4.0,	2,	1250,		true,	0.50,	2,	3.5,	false,		false,		0.0)

	-- Ness
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ness_1")
	AddPath(paths,	"Wave_3_Ness_1-2")
	AddSpawn(spawn,paths,
		 2565,	0.0,	0.0,	true,	0,	4.0,	4,	5000,		true,	0.50,	4,	6,	true,		true,		0.0)

	-- Pearl
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Pearl")
	AddSpawn(spawn,paths,
		 2976,	5.0,	55.0,	false,	0,	0,	2.0,	0,		false,	0.0,	1.0,	1.0,	true,		true,		5.0)


-- Add the spawns for the wave
AddSpawnsForWave(spawns,spawn)


--------------------------------------------------------------
-- Wave 3 Spawns
spawn = {}
--------------------------------------------------------------

	-- Duck
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Duck_1")
	AddSpawn(spawn,paths,
		 2970,	0.0,	0.0,	true,	0,	4.0,	1.0,	200,		true,	0.50,	1.0,	1.75,	false,		false,		0.0)

	-- Duck Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Duck_2")
	AddSpawn(spawn,paths,
		 2969,	0.0,	0.0,	true,	0,	4.0,	1.75,	400,		true,	0.50,	1.75,	2.50,	false,		false,		0.0)

	-- Ship
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ship_1")
	AddSpawn(spawn,paths,
		 2974,	0.0,	0.0,	true,	0,	4.0,	1.0,	1000,		true,	0.50,	1.0,	1.75,	false,		false,		0.0)

	-- Ship Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_3_Ship_2")
	AddSpawn(spawn,paths,
		 2973,	0.0,	0.0,	true,	0,	4.0,	1.75,	1500,		true,	0.50,	1.75,	2.50,	false,		false,		0.0)

	-- Sub
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_4_Sub_1")
	AddPath(paths,	"Wave_4_Sub_1-2")
	AddSpawn(spawn,paths,
		 2984,	0.0,	0.0,	true,	0,	4.0,	1.0,	2500,		true,	0.50,	1.0,	1.75,	true,		true,		0.0)

	-- Sub Fast
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_4_Sub_2")
	AddPath(paths,	"Wave_4_Sub_2-2")
	AddSpawn(spawn,paths,
		 2983,	0.0,	0.0,	true,	0,	4.0,	2,	5000,		true,	0.50,	2,		3,		true,		true,		0.0)

	-- Ness
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Wave_4_Ness_1")
	AddPath(paths,	"Wave_4_Ness_2")
	AddPath(paths,	"Wave_4_Ness_3")
	AddPath(paths,	"Wave_4_Ness_4")
	AddPath(paths,	"Wave_4_Ness_5")
	AddPath(paths,	"Wave_4_Ness_6")
	AddPath(paths,	"Wave_4_Ness_7")
	AddPath(paths,	"Wave_4_Ness_8")
	AddPath(paths,	"Wave_4_Ness_9")
	AddSpawn(spawn,paths,
		 3118,	0.0,	0.0,	true,	0,	0,	1.0,	10000,		false,	0,		0,		0,		true,		true,	0.0)

	-- Pearl
	--------------------------------------------------------------
	paths = {}
	--------------------------------------------------------------
	AddPath(paths,	"Pearl")
	AddSpawn(spawn,paths,
		 2976,	5.0,	55.0,	false,	0,	0,	2.0,	0,		false,	0.0,	1.0,	1.0,	true,		true,		5.0)


-- Add the spawns for the wave
AddSpawnsForWave(spawns,spawn)


--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------
--------------------------------------------------------------
-- play effects on all effects in the scene
--------------------------------------------------------------
function PlaySceneEffect(effectName)

	-- trigger effects on effect objects
	for effectID = 1, #EFFECTS do

		local effect = GAMEOBJ:GetObjectByID(EFFECTS[effectID])
		effect:PlayFXEffect{ effectType = effectName }

	end

end


--------------------------------------------------------------
-- play animations on all actors in the scene, and others
--------------------------------------------------------------
function PlaySceneAnimation(anim, bPlayCannon, bPlayPlayer)

	-- play animation on actors
	for actorID = 1, #ACTORS do

		local actor = GAMEOBJ:GetObjectByID(ACTORS[actorID])
		actor:PlayAnimation{ animationID = anim }

	end

	-- play on cannon
	if (bPlayCannon == true) then

		local cannon = getObjectByName("cannonObject")
		cannon:PlayAnimation{ animationID = anim }

	end

	-- play on player
	if (bPlayPlayer == true) then

		local player = getObjectByName("activityPlayer")
		player:PlayAnimation{ animationID = anim }

	end

end

--------------------------------------------------------------
-- return if template is a valid actor
--------------------------------------------------------------
function IsValidActor(templateID)

	for actors = 1, #CONSTANTS["VALID_ACTORS"] do
		if (templateID == CONSTANTS["VALID_ACTORS"][actors]) then
			return true
		end
	end

	return false

end


--------------------------------------------------------------
-- return if template is a valid effect
--------------------------------------------------------------
function IsValidEffect(templateID)

	for effects = 1, #CONSTANTS["VALID_EFFECTS"] do
		if (templateID == CONSTANTS["VALID_EFFECTS"][effects]) then
			return true
		end
	end

	return false

end


--------------------------------------------------------------
-- put the player in the cannon, does not start the cannon
--------------------------------------------------------------
function enterCannon(self)

	-- get the cannon
	local cannon = getObjectByName("cannonObject")

	-- get the player
	local player = getObjectByName("activityPlayer")

	-- if we have both start it
	if ((cannon) and (player)) then
		cannon:RequestActivityEnter{bStart = false, userID = player}
	end

end


--------------------------------------------------------------
-- try to start the game
--------------------------------------------------------------
function startGame(self)

	-- get the cannon
	local cannon = getObjectByName("cannonObject")

	-- get the player
	local player = getObjectByName("activityPlayer")
	-- if we have both start it
	if ((cannon) and (player)) then

		-- put the player in if we have to, otherwise start
		if ((cannon:GetActivityUser().userID):GetID() ~= player:GetID()) then
			print("wrong player")
			cannon:RequestActivityEnter{bStart = true, userID = player}
		else
			print("starting game")
			cannon:ActivityStart{ rerouteID = player }
		end

		DoGameStartup(self)

	end

end


--------------------------------------------------------------
-- try to stop the game
--------------------------------------------------------------
function stopGame(self, bCanceling)

	-- get the cannon
	local cannon = getObjectByName("cannonObject")

	-- get the player
	local player = getObjectByName("activityPlayer")

	-- if we have both stop it if we need to, but dont exit
	if ((cannon) and (player)) then

		print("stopping game")

		cannon:ActivityStop{rerouteID = player, bExit = false, bUserCanceled = bCanceling}

		DoGameShutdown(self)

		-- if we are not exiting, shwo the summary and allow for retry
		if (bCanceling == false) then
			showSummaryDialog(self)
		end

	end

end


--------------------------------------------------------------
-- handle all the game startup data
--------------------------------------------------------------
function DoGameStartup(self)

	-- set game state and vars
	LOCALS["SpawnNum"] = 0
	LOCALS["CurSpawnNum"] = 0
	LOCALS["GameStarted"] = true
	LOCALS["ThisWave"] = 0
	LOCALS["GameScore"] = 0
	LOCALS["GameTime"] = 0
	LOCALS["NumShots"] = 0
	LOCALS["NumKills"] = 0
	LOCALS["MaxStreak"] = 0

	-- reset scores
	for waveNum = 1, tonumber(CONSTANTS["NUM_WAVES"]) do
		PLAYER_SCORE[waveNum] = 0
	end

	-- start the first wave
	LOCALS["ThisWave"] = 1
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FIRST_WAVE_START_TIME"], "SpawnWave1",self )

	-- Scene animations and wave number DISABLED for start, we use 3/2/1/Go instead
	
	-- play wave animation on actors, cannon, and player
	--PlaySceneAnimation("wave" .. LOCALS["ThisWave"], true, true)

	-- display wave number to player
	--DisplayWaveNumberToPlayer(self, LOCALS["ThisWave"])

	-- set the cannon reticule back to start
	setCannonReticuleSize(self, 1)

end


--------------------------------------------------------------
-- handle all the game shutdown data
--------------------------------------------------------------
function DoGameShutdown(self)

	print("game shutdown")

	LOCALS["GameStarted"] = false

	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )

	-- despawn all spawns
	DestroyAllSpawns(self)

end


--------------------------------------------------------------
-- Remove the player from the zone
--------------------------------------------------------------
function RemovePlayerFromZone(self, player)

	player:ServerSetUserCtrlCompPause{bPaused = false}

	-- UNCOMMENT to go back to the other zone
	player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true }

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
	local spawnNum = IncrementVarAndReturn("SpawnNum")

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
	SPAWN_DATA[tonumber(spawnNum)] = SpawnData

	-- set the timer to spawn the object
	local timerName = "DoSpawn" .. spawnNum

	-- spawn now and use initial spawn times
	if (bSpawnNow == true) then
		if (spawn.initSpawnTimeMin > 0 and spawn.initSpawnTimeMax > 0) then
			local ranSpawnTime = (math.random() * (spawn.initSpawnTimeMax - spawn.initSpawnTimeMin)) + spawn.initSpawnTimeMin
			print ("init spawning with time " .. ranSpawnTime)
			GAMEOBJ:GetTimer():AddTimerWithCancel( ranSpawnTime, timerName, self )
		else
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, timerName, self )
		end

	-- respawn, use respawn times
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

	local maxSpawnNum = LOCALS["CurSpawnNum"]
	for spawn = 1, maxSpawnNum do
		-- @TODO: (Optimize this)
		local spawnObject = getObjectByName("spawnObject" .. spawn)
		if (spawnObject) then
			if (spawnObject:Exists() and not spawnObject:IsDead().bDead) then
				print("removing spawn object " .. spawn)
				spawnObject:Die{killerID = spawnObject, killType = "SILENT"}
			end
		end
	end

	-- reset vars
	LOCALS["SpawnNum"] = 0
	LOCALS["CurSpawnNum"] = 0
end


--------------------------------------------------------------
-- sends a message to display the current wave text to player
--------------------------------------------------------------
function DisplayWaveNumberToPlayer(self, waveNum)

	-- @TODO: (Optimize)
	local player = getObjectByName("activityPlayer")
	if (player) then
		UI:SendMessage("ToggleFlashingText", { {"visible",  true }, {"text", waves[tonumber(waveNum)].waveStr}})

	end
end


--------------------------------------------------------------
-- look through spawn data to find the most recent data
-- that matches the template, returns nil or data
--------------------------------------------------------------
function GetLatestSpawnDataByTemplate(self,templateID)

	local spawnNum = LOCALS["SpawnNum"]
	while (spawnNum > 0) do

		-- get the data
		local SpawnData = SPAWN_DATA[tonumber(spawnNum)]

		-- check spawn flag and template
		if (SpawnData.bSpawned == false and templateID == SpawnData.sdTemplate) then

			-- set spawned flag
			SpawnData.bSpawned = true

			-- re-save data
			SPAWN_DATA[tonumber(spawnNum)] = SpawnData

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
	local player = getObjectByName("activityPlayer")

	if (player) then

		-- do summary dialog

		-- Wave Score
		local score = LOCALS["GameScore"]
		--local strText = "Score:\n\n"

		--for wavenum = 1, tonumber(CONSTANTS["NUM_WAVES"]) do
		--	strText = strText .. "Wave " .. wavenum .. " = " .. PLAYER_SCORE[tonumber(wavenum)] .. "\n"
		--end

		-- Total Score
		--strText = strText .. "\nTotal: " .. score .. "\n"

		-- Accuracy
		--strText = strText .. "\nAccuracy: " .. LOCALS["NumKills"] .. "/" .. LOCALS["NumShots"] .. "\n"

		-- Highest Streak
		--strText = strText .. "\Highest Streak: " .. LOCALS["MaxStreak"] .. "\n"


		local cannon = getObjectByName("cannonObject")
		local msgReward = cannon:GetActivityReward{ playerID = player }
		
		print("rewardMoney = " .. tostring(msgReward.rewardMoney))

		player:ShowActivitySummary{	totalScore				= score,
									waveScore1				= PLAYER_SCORE[1],
									waveScore2				= PLAYER_SCORE[2],
									waveScore3				= PLAYER_SCORE[3],
									waveScore4				= PLAYER_SCORE[4],
									numShots				= LOCALS["NumShots"],
									numKills				= LOCALS["NumKills"],
									longestStreak			= LOCALS["MaxStreak"],
									leaderboard_yourRank	= 1,					--@TODO: We don't have leaderboards, replace this when we do!
									leaderboard_topScore	= 100,					--@TODO: We don't have leaderboards, replace this when we do!
									leaderboard_yourTopScore= 100,					--@TODO: We don't have leaderboards, replace this when we do!
									rewardMoney				= msgReward.rewardMoney,
									rewardItem1Name			= msgReward.rewardItem1Name,
									rewardItem1Image		= msgReward.rewardItem1Image,
									rewardItem1StackSize	= msgReward.rewardItem1StackSize,
									rewardItem2Name			= msgReward.rewardItem2Name,
									rewardItem2Image		= msgReward.rewardItem2Image,
									rewardItem2StackSize	= msgReward.rewardItem2StackSize,
									callbackObj				= GAMEOBJ:GetZoneControlID()}

		if (cannon) then
			cannon:RequestActivitySummaryLeaderboardData{ user = player }
		end

	end

end


--------------------------------------------------------------
-- set the cannon's reticule size and skill based on wave
--------------------------------------------------------------
function setCannonReticuleSize(self, waveNum)

	-- Disabled 6-11-08 : vtran

	---- get the cannon
	--local cannon = getObjectByName("cannonObject")
--
	--if (cannon) then
--
		--cannon:SetActiveProjectileSkill{ skillID = CONSTANTS["CANNON_IMPACT_SKILL"][tonumber(waveNum)] }
		--cannon:SetVar("ImpactSkillID",CONSTANTS["CANNON_IMPACT_SKILL"][tonumber(waveNum)])
		--cannon:SetShootingGalleryReticuleEffect{ type = CONSTANTS["CANNON_RETICULE_EFFECT"][tonumber(waveNum)] }
--
	--end
--
end


--------------------------------------------------------------
-- Add more time to the current wave timer
--------------------------------------------------------------
function AddTimeToWave(self, timeToAdd)

	-- get wave number
	local waveNum = LOCALS["ThisWave"]

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

		-- get the cannon
		local cannon = getObjectByName("cannonObject")
        cannon:AddActivityTime{ rerouteID =  getObjectByName("activityPlayer"), timeToAdd = timeToAdd }
	end

end

function RecordPlayerWaveScore(self)

	-- get wave number
	local waveNum = tonumber(LOCALS["ThisWave"])

	-- get the cannon
	local cannon = getObjectByName("cannonObject")

	if (waveNum) and (waveNum > 0) and (cannon) then

		-- get total current score
		local score = tonumber(cannon:GetActivityPoints().points)

		-- to get the wave score we must subtract prior waves from it
		for waves = 1, waveNum - 1 do
			score = score - PLAYER_SCORE[waves]
		end

		-- store the new score
		PLAYER_SCORE[waveNum] = score

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
	LOCALS["GameStarted"] = false
	LOCALS["CurSpawnNum"] = 0
	LOCALS["ThisWave"] = 0
	LOCALS["GameScore"] = 0
	LOCALS["GameTime"] = 0
	LOCALS["NumShots"] = 0
	LOCALS["NumKills"] = 0
	LOCALS["MaxStreak"] = 0

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)

    -- parse the name to get out the wave number
    -- use the wave number to select the spawns
    -- of format "SpawnWaveXXX" where XXX is the spawn number
    if (string.starts(msg.name,"SpawnWave")) then

		if (LOCALS["GameStarted"] == true) then

			-- cancel all timers
			GAMEOBJ:GetTimer():CancelAllTimers( self )

			-- store this wave number
			waveNum = LOCALS["ThisWave"]

			print("Spawning Wave " .. waveNum)

			-- setup spawns for wave
			for k,v in pairs(spawns[tonumber(waveNum)]) do SpawnObject(k,v,self,true) end

			-- move to the next wave
			--waveNum = waveNum + 1

			-- there are no more waves left
			-- so stop game after this wave
			if (tonumber(waveNum) >= CONSTANTS["NUM_WAVES"]) then

				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( waves[tonumber(waveNum)].timeLimit, "GameOver", self )

			else

				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( waves[tonumber(waveNum)].timeLimit, "EndWave" .. waveNum,self )

			end

			local cannon = getObjectByName("cannonObject")
			local player = getObjectByName("activityPlayer")
	    	if cannon and player then
	    		cannon:StartActivityTime{ rerouteID = player, startTime = waves[tonumber(waveNum)].timeLimit }
				cannon:ActivityPause{ rerouteID = player, bPause = false }
		    end



		end
    end

	-- called when a wave ends, contains the number of the next wave
    if (string.starts(msg.name,"EndWave")) then

		if (LOCALS["GameStarted"] == true) then

			-- get rid of current spawns
			DestroyAllSpawns(self)

			-- cancel all timers
			GAMEOBJ:GetTimer():CancelAllTimers( self )

			-- record the score
			RecordPlayerWaveScore(self)

			-- get the wave number from the rest of the string
			local waveNum = string.sub(msg.name,8)

			-- store the next wave number
			waveNum = tonumber(waveNum) + 1
			LOCALS["ThisWave"] = waveNum

			-- @TODO: Do whatever we need to during the pause, wave begins after pause
			-- play wave animation on actors, cannon, and player
			PlaySceneAnimation("wave" .. LOCALS["ThisWave"], true, true)

			-- display wave number to player
			DisplayWaveNumberToPlayer(self, waveNum)

			-- there are no more waves left
			-- so stop game after this wave
			if (waveNum > CONSTANTS["NUM_WAVES"]) then

				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "GameOver", self )

			else

				-- setup next wave
				GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["IN_BETWEEN_WAVE_PAUSE"], "SpawnWave" .. waveNum,self )

			end

			local player = getObjectByName("activityPlayer")
			local cannon = getObjectByName("cannonObject")
	    	if player and cannon then
				cannon:ActivityPause{rerouteID = player, bPause = true}
		    end

		end
    end

    -- parse the name to get out the spawn number
    -- use the spawn number to select the template id
    -- load the object
    -- of format "DoSpawnXXX" where XXX is the spawn number

    if (string.starts(msg.name,"DoSpawn")) then

		if (LOCALS["GameStarted"] == true) then

			-- get the spawn number from the rest of the string
			local spawnNum = string.sub(msg.name,8)

			-- get the template out of the spawn data
			local SpawnData = SPAWN_DATA[tonumber(spawnNum)]

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
		RecordPlayerWaveScore(self)
		stopGame(self, false)
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

		local curSpawnNum = IncrementVarAndReturn("CurSpawnNum")

		-- store spawn for use later
		storeObjectByName("spawnObject" .. curSpawnNum, msg.childID)

		-- store who the parent is
		storeParent(self, msg.childID)

		-- store the spawn data in the child (@TODO: Need to Optimize)
		msg.childID:SetVar("SpawnData", SpawnData)

		if (SpawnData.sdMovingPlat == true) then
			msg.childID:SetPathingSpeed{ speed = SpawnData.sdSpeed }
			msg.childID:SetMovingPlatformParams{ wsPlatformPath = SpawnData.sdPath, iStartIndex = 0 }
		else
			-- assign child's waypoint
			msg.childID:SetVar("attached_path",SpawnData.sdPath)
			msg.childID:SetVar("attached_path_start",0)

			-- start child on path
			msg.childID:FollowWaypoints()
			msg.childID:SetPathingSpeed{ speed = SpawnData.sdSpeed }
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
	-- @TODO: Sometimes this teleport works and sometimes it does not.....ghosting distance?
	local player = msg.playerID

	--player:ServerSetUserCtrlCompPause{bPaused = true}

	player:CancelMission{ missionID = 30 }

--	player:Teleport{pos = CONSTANTS["PLAYER_START_POS"],
--	                x = CONSTANTS["PLAYER_START_ROT"].x,
--	                y = CONSTANTS["PLAYER_START_ROT"].y,
--	                z = CONSTANTS["PLAYER_START_ROT"].z,
--	                w = CONSTANTS["PLAYER_START_ROT"].w,
--	                bSetRotation = true}

	if (LOCALS["GameStarted"] == false) then
		-- get the player
		local player = getObjectByName("activityPlayer")

		-- store the player for later use
		if (player == nil) then
			storeObjectByName("activityPlayer", msg.playerID)
		end

		-- put the player in the cannon
		enterCannon(self)
	end

end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

	-- Cannon Object Loaded
	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) then

		-- Override the cannon shooting parameters
		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["CANNON_PLAYER_OFFSET"],
		                                      projectileVelocity = CONSTANTS["CANNON_VELOCITY"],
		                                      cooldown =           CONSTANTS["CANNON_REFIRE_RATE"],
		                                      muzzlePosOffset =    CONSTANTS["CANNON_BARREL_OFFSET"],
		                                      minDistance =        CONSTANTS["CANNON_MIN_DISTANCE"],
		                                      timeLimit =          CONSTANTS["CANNON_TIMEOUT"],
		                                      cameraFOV =          CONSTANTS["CANNON_FOV"],
											  bUseLeaderboards =   CONSTANTS["CANNON_USE_LEADERBOARDS"]}

		-- store the cannon object for use later
		storeObjectByName("cannonObject", msg.objectID)

		if (LOCALS["GameStarted"] == false) then
			-- try to put hte player in the cannon
			enterCannon(self)
		end

	-- check for actors
	elseif ( IsValidActor(msg.templateID) == true ) then

		-- store the actor
		local nextActor = #ACTORS + 1
		ACTORS[nextActor] = msg.objectID:GetID()

	-- check for effects
	elseif ( IsValidEffect(msg.templateID) == true ) then

		-- store the actor
		local nextEffect = #EFFECTS + 1
		EFFECTS[nextEffect] = msg.objectID:GetID()

	end

end


--------------------------------------------------------------
-- Sent from the cannon to get a score for the player
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)

	-- save the score and time for later use
	LOCALS["GameScore"] = msg.fValue1
	LOCALS["GameTime"]  = msg.fValue2
	LOCALS["NumShots"]  = msg.fValue3	-- shots fired
	LOCALS["NumKills"]  = msg.fValue4	-- targets hit
	LOCALS["MaxStreak"] = msg.fValue5	-- max streak

    -- also return the score as the result for the activity
	msg.outActivityRating = msg.fValue1
	return msg

end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

	-- make sure this is the right player
	local player = getObjectByName("activityPlayer")
	if (player:GetID() == msg.sender:GetID()) then

		if msg.identifier == "Shooting_Gallery_Summary" then
			strText = "Retry?"
			-- show the summary message box
			player:DisplayMessageBox{bShow = true,
									 imageID = 2,
									 callbackClient = GAMEOBJ:GetZoneControlID(),
									 text = strText,
									 identifier = "Shooting_Gallery_Retry"}
		else
			-- User wants to retry or is closing the big help to start the game
			if ((msg.iButton == 1 and msg.identifier == "Shooting_Gallery_Retry") or
				(msg.identifier == "SG1")) then

				startGame(self)

			-- User wants to quit
			elseif (msg.iButton == 0 and msg.identifier == "Shooting_Gallery_Retry") then

				-- called before we leave the cannon, so we can trigger a loading screen
				-- as soon as possible. Should still remove player from cannon on server
				RemovePlayerFromZone(self, player)

				-- get the cannon and exit
				local cannon = getObjectByName("cannonObject")
				if (cannon) then
					cannon:RequestActivityExit{userID = player, bUserCancel = false}
				end

			end
		end

	end

end


--------------------------------------------------------------
-- Sent from the spawn objects on death
--------------------------------------------------------------
function onUpdateMissionTask(self, msg)

	-- get this wave number
	local waveNum = tonumber(LOCALS["ThisWave"])

	if (LOCALS["GameStarted"] == true) and (waveNum) and (waveNum > 0) then

		-- get the spawn data (@TODO: Need to Optimize)
		local spawnData = msg.target:GetVar("SpawnData")

		-- spawn the right object
		if (spawnData) and (spawnData.sdRespawn == true) then
			SpawnObject(spawnData.sdnum,spawns[waveNum][spawnData.sdnum],self,false)
		end

		-- check for time adding, disregard if death was SILENT
		if (spawnData.sdTimeScore > 0.0 and msg.taskType ~= "SILENT") then
			AddTimeToWave(self,spawnData.sdTimeScore)
		end

	end

end

--------------------------------------------------------------
-- User is exiting via cancel
--------------------------------------------------------------
function onRequestActivityExit(self, msg)

	if (msg.bUserCancel == true) then
		stopGame(self,msg.bUserCancel)
		RemovePlayerFromZone(self, msg.userID)
	end

end


--------------------------------------------------------------
-- Cannon is firing
--------------------------------------------------------------
function onShootingGalleryFire(self, msg)

	-- play fire animation on actors
	PlaySceneAnimation("fire", false, false)
	PlaySceneEffect("fire")

end