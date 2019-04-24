--------------------------------------------------------------
-- (CLIENT SIDE) Obstacle Course Starter NPC
-- Handles client side dialogs/messages and input
--
-- updated mrb... 7/21/11 - added toggleCollision
--------------------------------------------------------------

--//////////////////////////////////////////////////////////////////////////////////
local preConVar = 190
local finishLineFlag = 33
local timerTick = 0
local COUNTDOWN_TIME = 2.0
--//////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Make this object interactable (must register for picking)
------------------------------------------------------------------------------------------------------------
function onGetPriorityPickListType(self, msg)
	local myPriority = 0.8
	
    if ( myPriority > msg.fCurrentPickTypePriority ) then
		msg.fCurrentPickTypePriority = myPriority
       
		if not self:GetVar("active") then
			msg.ePickType = -1
		else
			msg.ePickType = 14    -- Interactive pick type 
		end
    end

    return msg
end

-- Catch when the local player comes within ghosting distance
----------------------------------------------
function onScopeChanged(self, msg)
    -- Has the player entered ghosting range?
    if not msg.bEnteredScope then return end
    
	-- Obtain the local player object
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then
		-- Subscribe to a zone control object notification alerting the script
		-- when the local player object is ready
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
		
		return
	end
	
	-- check if the player flag has been set
	local bFlag = player:GetFlag{iFlagID = finishLineFlag}.bFlag 
	    
	self:SetVisible{visible = bFlag, fadeTime = 0.0}
	toggleCollision(self, bFlag)
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
	-- Obtain the local player object
	local player = GAMEOBJ:GetControlledID()
	-- check if the player flag has been set
	local bFlag = player:GetFlag{iFlagID = finishLineFlag}.bFlag 
	    
	self:SetVisible{visible = bFlag, fadeTime = 0.0}
	toggleCollision(self, bFlag)
	
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function onVisibilityChanged(self, msg)
	-- set the collision and proximity based on the visibility of the object
	self:SetVar("active", msg.isVisible)
	self:RequestPickTypeUpdate()
	
	toggleCollision(self, msg.isVisible)
end

function toggleCollision(self, bOn)	
	if bOn then
		self:SetProximityRadius{name = "icon", radius = 40, iconID = 63}
		self:SetCollisionGroupToOriginal()
	else
		self:SetCollisionGroup{colGroup = 16}
	end
end

function onCheckUseRequirements(self, msg)
	-- We have a valid list of preconditions to check
	local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}

	if not check.bPass then 
		-- Failed the precondition check
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_IconID = check.IconID
			msg.Script_Reason = check.FailedReason
			msg.Script_Failed_Requirement = true
		end
		
		msg.bCanUse = false		
	end

    return msg
end

function onClientUse(self, msg)
	-- Clear out any other gamestates before opening the help tooltip
    UI:SendMessage( "pushGameState", {{"state", "gameplay"}} )
end

----------------------------------------------------------------
-- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)
    --gets the player and ends the player's cinematic if one is playing
    local player = GAMEOBJ:GetControlledID()
    
    if not player:Exists() then return end

	--player responded to the race messagebox
    if msg.identifier == "player_dialog_start_course" or msg.identifier == "player_dialog_cancel_course" then        
        if msg.identifier == "player_dialog_cancel_course" or msg.iButton == 0 then
            freezePlayer(self)
        end
        
        self:SetVar("start_open", false)
    elseif msg.identifier == "Exit" then
		player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Post-Monument"}
	end
    
	player:EndCinematic{ leadOut = 1.0 }
	player:SetVisible{visible = true, fadeTime = 0.5}
	player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
end

function SecondsToClock(sSeconds)
    local nSeconds = tonumber(sSeconds)
    
    if nSeconds == 0 then
        return "00:00"; --return "00:00:00";
    else
        
        nHours = string.format("%02.f", math.floor(nSeconds/3600));
        nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
        nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
        
        return nMins..":"..nSecs --return nHours..":"..nMins..":"..nSecs
    end
end

function shutdownUI(self)
    local player = GAMEOBJ:GetControlledID()
    
    if player:Exists() then
        -- close any tooltip or messagebox
        player:DisplayTooltip{ bShow = false, strText = "..." }
        player:DisplayMessageBox{bShow = false, imageID = 1, text = Localize("UI_OBSTACLE_COURSE_EXIT"), callbackClient = self, identifier = "player_dialog_cancel_course"}
        player:DisplayMessageBox{bShow = false, imageID = 4, text = Localize("UI_OBSTACLE_COURSE_START"), callbackClient = self, identifier = "player_dialog_start_course"}
        
        UI:SendMessage("ToggleFootRace", { {"visible", false}, {"callbackClient", self}, {"identifier", "player_dialog_start_course"} })
	end
	
	-- reset game state
    UI:SendMessage("UpdateFootRaceScoreboard", {{"visible", false }})
    GAMEOBJ:GetTimer():CancelAllTimers( self )
    self:SetVar('FinalTime', 0)
    timerTick = 0
end

function onNotifyClientObject(self, msg)    
    -- player is trying to exit the activity
    if msg.name == "exit" then
        local player = GAMEOBJ:GetControlledID()
		-- show the exit message box	
		player:DisplayMessageBox{bShow = true, 
								 imageID = 1, 
								 text = Localize("UI_OBSTACLE_COURSE_EXIT"), 
								 callbackClient = self, 
								 identifier = "player_dialog_cancel_course"}								  
    -- player is trying to start the activity
    elseif msg.name == "start" then
        -- show the start message box
        UI:SendMessage("ToggleFootRace", { {"visible", true}, 
                                        {"text", Localize("UI_OBSTACLE_COURSE_START")}, 
                                        {"callbackClient", self}, 
                                        {"identifier", "player_dialog_start_course"} })
								 
        freezePlayer(self, true)
		self:SetVar("start_open", true)						
    elseif msg.name == "start_timer" then
        local player = GAMEOBJ:GetControlledID()
        
        player:ShowActivityCountdown()
        GAMEOBJ:GetTimer():AddTimerWithCancel( COUNTDOWN_TIME, "CourseGo", self )
    elseif msg.name == "cancel_timer" then
        UI:DisplayToolTip{strDialogText = Localize("UI_OBSTACLE_COURSE_OUT_OF_RANGE"), 
                          strImageName = "", 
                          bShow = true, 
                          iTime = 5000}
        ----------------------------------------------------------------------------------------------------------------------
        --You respond in the positive to "I would like to stop playing the foot race"
        ----------------------------------------------------------------------------------------------------------------------            
        UI:SendMessage("UpdateFootRaceScoreboard",  {{"visible", false }} )
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        timerTick = 0          
    elseif msg.name == "stop_timer" then
        GAMEOBJ:GetTimer():CancelAllTimers( self ) 
        self:SetVar('FinalTime', msg.param2)
	    UI:SendMessage( "UpdateFootRaceScoreboard",  {{"raceComplete", true}, {"message", Localize("UI_OBSTACLE_COURSE_FINISH") .. " " .. Localize("UI_OBSTACLE_COURSE_TIME") .. " " .. SecondsToClock(self:GetVar('FinalTime'))}} )	
        timerTick = 0 
	elseif msg.name == "ToggleLeaderBoard" and msg.paramObj:GetID() == GAMEOBJ:GetControlledID():GetID() then
		UI:SendMessage("pushGameState", {{"state", "TimedRaceLeaderboard"}})
		UI:SendMessage("ToggleLeaderboard", { {"id",  msg.param1}, {"visible", true}, {"hideReplay", true}, {"callbackObject", self} } )
		GAMEOBJ:GetControlledID():ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Post-Monument"}
	elseif msg.name == "out_of_bounds" then
        shutdownUI(self)
        freezePlayer(self)
	end
end

function onTerminateInteraction(self, msg)
    local player = GAMEOBJ:GetControlledID()
    
    if not player:Exists() then return end
    
    -- close the message box
	player:DisplayMessageBox{bShow = false} 
	
	-- close the race gui and release player
	if self:GetVar("start_open") then
		shutdownUI(self)
		freezePlayer(self)
	end
end

function freezePlayer(self, bFreeze)    
    local player = GAMEOBJ:GetControlledID()
    
    if not player:Exists() then return end
    
    local eChangeType = "POP"
    
    if bFreeze then
        if self:GetVar('frozen') and player:IsDead().bDead then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
            
            return
        end
        
        self:SetVar('frozen', true)
        eChangeType = "PUSH"
    else
        self:SetVar('frozen', false)
    end
    
    --print('Player ' .. eChangeType .. ' is frozen: ' .. tostring(self:GetVar('frozen')))
    player:SetStunned{ StateChangeType = eChangeType,
                        bCantMove = true, bCantAttack = true, bCantInteract = true, bDontTerminateInteract = true }
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
    if (msg.name == "CourseGo") then
        freezePlayer(self)
        UI:SendMessage( "ToggleFootRaceScoreboard", {{"visible", true }, {"time", 0 } })
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "ClockTick", self )
    elseif msg.name == "ClockTick" then
        timerTick = timerTick + 1
        UI:SendMessage( "UpdateFootRaceScoreboard", {{"time", timerTick } })
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "ClockTick", self )
    elseif msg.name == "Try_Freeze_Again" then
        freezePlayer(self, true)
    end
end 