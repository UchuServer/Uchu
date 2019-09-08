--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_START_POS"] = {x = -15.711255, y = 276.552267, z = -13.312634}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}
CONSTANTS["NPC_START_POS"] = { x = 156.0, y = 270.06, z = 145.0 }

-- cannon constants
CONSTANTS["CANNON_TEMPLATEID"] = 1864
CONSTANTS["CANNON_PLAYER_OFFSET"] = {x = 0, y = 0, z = 0}
CONSTANTS["CANNON_VELOCITY"] = 100.0
CONSTANTS["CANNON_MIN_DISTANCE"] = 30.0
CONSTANTS["CANNON_REFIRE_RATE"] = 800.0
CONSTANTS["CANNON_BARREL_OFFSET"] = {x = 0, y = 4.3, z = 9}
CONSTANTS["CANNON_TIMEOUT"] = 60.0

-- Other constants
CONSTANTS["FLOATING_TARGET"] = 2565
CONSTANTS["FLOATING_PATH_NAME"] = "TestPath"
CONSTANTS["FLOATING_TARGET_SPAWN_TIME"] = 5.0


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
	self:SetVar("GameStarted", true)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	
	-- spawn floating thing
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FLOATING_TARGET_SPAWN_TIME"], "SpawnFloater",self ) 		
end

--------------------------------------------------------------
-- handle all the game shutdown data
--------------------------------------------------------------
function DoGameShutdown(self)

	self:SetVar("GameStarted", false)
	
	-- cancel all timers
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	
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
		local strText = ""
		if (score and time) then
			strText = "Your Score is " .. score .. ".<BR><BR>Retry?"
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
-- Game Message Handlers
--------------------------------------------------------------

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

	-- set game state
	self:SetVar("GameStarted", false)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if msg.name == "SpawnFloater" then

		RESMGR:LoadObject { objectTemplate = CONSTANTS["FLOATING_TARGET"], 
							bIsSmashable = true, 
							x = 32, 
							y = 290, 
							z = 32,
							owner = self }    
		                        
		-- spawn more
		GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FLOATING_TARGET_SPAWN_TIME"], "SpawnFloater",self ) 				                        
		
    end
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	
	if msg.templateID == CONSTANTS["FLOATING_TARGET"] then
	
		-- set the path
		msg.childID:SetMovingPlatformParams{ wsPlatformPath = "TestPath", iStartIndex = 0 }
		
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
	
	player:Teleport{pos = CONSTANTS["PLAYER_START_POS"], 
	                x = CONSTANTS["PLAYER_START_ROT"].x, 
	                y = CONSTANTS["PLAYER_START_ROT"].y, 
	                z = CONSTANTS["PLAYER_START_ROT"].z, 
	                w = CONSTANTS["PLAYER_START_ROT"].w, 
	                bSetRotation = true}

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
	if (msg.templateID == CONSTANTS["CANNON_TEMPLATEID"]) then

		-- Override the cannon shooting parameters
		msg.objectID:SetShootingGalleryParams{playerPosOffset =    CONSTANTS["CANNON_PLAYER_OFFSET"],
		                                      projectileVelocity = CONSTANTS["CANNON_VELOCITY"], 
		                                      cooldown =           CONSTANTS["CANNON_REFIRE_RATE"], 
		                                      muzzlePosOffset =    CONSTANTS["CANNON_BARREL_OFFSET"], 
		                                      minDistance =        CONSTANTS["CANNON_MIN_DISTANCE"], 
		                                      timeLimit =          CONSTANTS["CANNON_TIMEOUT"]}
		
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
			player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true }
		
		end

	end
end
