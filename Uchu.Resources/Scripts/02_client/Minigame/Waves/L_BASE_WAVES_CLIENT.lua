--------------------------------------------------------------
-- Generic Survival Instance Client Zone Script: Including this 
-- file gives the custom functions for the Survival game.
--
-- updated mrb... 2/2/11 - Bug fixes
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('ai/L_ACTIVITY_MANAGER')

--//////////////////////////////////////////////////////////////////////////////////
-- local variables
local teamScoreBoard = {}                    
local bFirstTime = true
---------------------------------------------------------------
-- Startup of the object
---------------------------------------------------------------

function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end

function onFinishedLoadingScene(self, msg)
	--print("onFinishedLoadingScene")
	if celebrations then 	
		startCelebration(self, "intro")
	else 
		playerInit(self)
	end
end

function playerInit(self)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if not player:GetPlayerReady().bIsReady then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Try_Init_Again", self )
        
        return
	end	
		
    UI:SendMessage( "pushGameState", {{"state", gameUI.game}})
      
    -- add player to the ui 
    UI:SendMessage( "Update" .. gameUI.summary, {{"user", player }} )    
                        
    --freezePlayer(player)
    
    UI:SendMessage( "ToggleActivityCloseButton", {  {"bShow", true}, 
													{"GameObject", self}, 
													{"MessageName", "toLua"},
													{"senderID", player} } )
                                            
    local tempTable = { nameVar = player:GetName().name,
                        timeVar = 0,
                        scoreVar = 0,
                        bConfirm = false }
                        
    self:SetVar("bInit", true)
    
    teamScoreBoard = checkScoreBoardTable(self, tempTable) 
    showScoreBoard(self) 
    UI:SendMessage( "Update" .. gameUI.summary, {{"countdownTime", " " }} )       
	player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Pre-Game"} 
                    
	freezePlayer(self, true)
	
	-- clear game vars
	self:SetVar("bBeatTopScore", false)
	self:SetVar("bNextWaveBeatTopScore", false)
	self:SetVar("NextBest", false )
	self:SetVar("NextBestName", false)
	self:SetVar("FoundPlayer", false )
end

function checkScoreBoardTable(self, tScoreBoard, bRemoveTable)
    local tTemp = {}    
    local isUsed = false
    
    for k,v in ipairs(teamScoreBoard) do
        if v.nameVar ~= Localize("SURVIVAL_NO_PLAYER") and v.nameVar ~= tScoreBoard.nameVar and v.nameVar ~= '' then            
            table.insert(tTemp,v)
            --print('add old player to used table ' .. v.nameVar)
        end

        if v.nameVar == tScoreBoard.nameVar then
            if not bRemoveTable then            
                for key,val in pairs(tScoreBoard) do
                    if tostring(val) == tostring(-1) then
                        tScoreBoard[key] = v[key]
                    end
                end
            else
                isUsed = true
            end
        end
    end
    
    if not isUsed then
        table.insert(tTemp,tScoreBoard)    
        --print('add new player to used table ' .. tScoreBoard.nameVar) 
    end   
    
    table.sort(tTemp, function(a,b)
                return a.nameVar<b.nameVar
            end)
    
    if not bFirstTime then
        table.sort(tTemp, function(a,b)
                    return a.timeVar>b.timeVar
                end)
    end
    
    self:SetVar("playerNum", table.maxn(tTemp))
        
    while table.maxn(tTemp) < 4 do
        local tempName = ""
        local numPlayers = self:GetNetworkVar("NumberOfPlayers") or 1
        
        if table.maxn(tTemp) < numPlayers then
            tempName = Localize("SURVIVAL_NO_PLAYER")
        end
        
        table.insert(tTemp, {   nameVar =  "",
                                timeVar = 0,
                                scoreVar = 0,
                                bConfirm = false})
    end
    
    --print('***************')
    --for k,v in ipairs(tTemp) do
    --    print('*** player ' .. k .. ' ***')
    --    print(tTemp[k].nameVar .. ' time: ' .. tTemp[k].timeVar .. ' score: ' .. tTemp[k].scoreVar)
    --    print('***************')
    --end
    
    return tTemp
end

function SecondsToClock(sSeconds)
    local nSeconds = tonumber(sSeconds)
    
	if nSeconds == 0 or not nSeconds or bFirstTime then
		return "--"
	else		
        nHours = string.format("%01.f", math.floor(nSeconds/3600));
        nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
        nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
        
        if nHours ~= "0" then
			return nHours..":"..nMins..":"..nSecs
        end
        
        return nMins..":"..nSecs 
    end
end

function numToString(iNum)
    if bFirstTime and (iNum == 0 or iNum == nil) then
       return "--"
    end
    
    return tostring(iNum)
end

function showScoreBoard(self, bUpdateOnly)
    p = teamScoreBoard
   
    if not p or not self:GetVar("bInit") then return end    
        
    local messageName = "Update" .. gameUI.summary
    local messageArgs = {{ "p1_check", p[1].bConfirm }, -- checkmarks
                        { "p2_check",  p[2].bConfirm },
                        { "p3_check",  p[3].bConfirm },
                        { "p4_check",  p[4].bConfirm },
                        { "p1_name", p[1].nameVar }, -- names
                        { "p2_name", p[2].nameVar },
                        { "p3_name", p[3].nameVar },
                        { "p4_name", p[4].nameVar },
                        { "p1_score", SecondsToClock(p[1].timeVar) }, -- times
                        { "p2_score", SecondsToClock(p[2].timeVar) },
                        { "p3_score", SecondsToClock(p[3].timeVar) },
                        { "p4_score", SecondsToClock(p[4].timeVar) },
                        { "p5_score", SecondsToClock(p[1].timeVar) },
                        { "p1_time", numToString(p[1].scoreVar) }, -- score
                        { "p2_time", numToString(p[2].scoreVar) },
                        { "p3_time", numToString(p[3].scoreVar) },
                        { "p4_time", numToString(p[4].scoreVar) },
                        { "p5_time", numToString(p[1].scoreVar)	}}
    
    --for k,v in ipairs(p) do print("**"..p[k].nameVar) end
    if not bUpdateOnly then
        -- Hide timer ui
        UI:SendMessage( "Toggle" .. gameUI.scoreboard, {{"stateChange", "paused"}, {"visible", false }})
        messageName = "Toggle" .. gameUI.summary
        -- insert these bariables when  we are toggling
        table.insert(messageArgs, {"visible", true })        
        table.insert(messageArgs, {"survivalTitle", scoreboard.title})        
        table.insert(messageArgs, {"columnOneTitle", scoreboard.columnOne})        
        table.insert(messageArgs, {"columnTwoTitle", scoreboard.columnTwo})        
    end
    
	-- Show UI
	UI:SendMessage(messageName, messageArgs )            
end

function freezePlayer(self, bFreeze)    
    local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    local eChangeType = "POP"
        
    if bFreeze then
        if playerID:IsDead().bDead then
            --print('frozen')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
            return
        
        elseif not playerID:GetStunned().bCanMove then 
			if self:GetNetworkVar("WatchingIntro") then
				GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
			end
			
            return
        end

        eChangeType = "PUSH"
    end
    
    playerID:SetStunned{ StateChangeType = eChangeType,
                        bCantMove = true, bCantAttack = true, bCantInteract = true }
                        
    
    --print('Player ' .. playerID:GetName().name .. ' ' .. eChangeType .. ' is frozen: ' .. tostring(self:GetVar('frozen')) .. ' ' .. tostring(playerID:GetStunned().bCanMove))
    if playerID:GetStunned().bCanMove and eChangeType == "PUSH" then
        --print(playerID:GetName().name .. ' is still able to move')
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Try_Freeze_Again", self )
    end
end

function onNotifyClientObject(self, msg)
    if msg.name == "ToggleLeaderBoard" and msg.paramObj:GetID() == GAMEOBJ:GetControlledID():GetID() then
		UI:SendMessage( "Update" .. gameUI.scoreboard, {{"stateChange", "paused"}, } ) 
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.param1}, {"visible", true}, {"nextOnly", true} } )
        freezePlayer(self, true)
    end
end

----------------------------------------------------------------
---- variables serialized from the server
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self, msg)    
	local playerID = GAMEOBJ:GetLocalCharID()
    local player = GAMEOBJ:GetObjectByID(playerID)
    
    -- these need to happen first
    for k,v in pairs(msg.tableOfVars) do    
        -- check to see if we have the correct message and deal with it
        if string.starts(k,"Update_ScoreBoard_Players") then  
            local tempPlayer = GAMEOBJ:GetObjectByID(v)               
            
            if tempPlayer:Exists() then                 
                --print("** Update_ScoreBoard_Players " .. tempPlayer:GetName().name)
                local tempTable = { nameVar = tempPlayer:GetName().name,
                                    timeVar = 0,
                                    scoreVar = 0,
                                    bConfirm = false }
                                    
                teamScoreBoard = checkScoreBoardTable(self, tempTable)
                UI:SendMessage("Update" .. gameUI.summary, {{"countdownTime", " " }} )    
            end
        elseif string.starts(k,"PlayerConfirm_ScoreBoard") then     
            local tempPlayer = GAMEOBJ:GetObjectByID(v)         
 
            if tempPlayer:Exists() then                 
                --print("** PlayerConfirm_ScoreBoard " .. tempPlayer:GetName().name)
                local tempTable = { nameVar = tempPlayer:GetName().name,
                                    timeVar = -1,
                                    scoreVar = -1,
                                    bConfirm = true }
                                    
                teamScoreBoard = checkScoreBoardTable(self, tempTable)      
                showScoreBoard(self, true)
            end
        elseif k == "Update_Default_Start_Timer" then
            if self:GetVar("displayDeath") then
                UI:SendMessage( "Update" .. gameUI.scoreboard, {{"iplayerName", Localize"UI_SURVIVAL_START_CLOCK"}, 
																{"iticTime", SecondsToClock( v )} } )   
            end
            
            UI:SendMessage( "Update" .. gameUI.summary, {{"visible", true}, {"countdownTime", v }} )  
        elseif k == "Update_Timer" then
            if self:GetVar("displayDeath") then
                UI:SendMessage( "Update" .. gameUI.scoreboard, {{"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")}, 
																{"iticTime", SecondsToClock( v )} } )   
            else  
				UI:SendMessage( "Update" .. gameUI.scoreboard, {{"iticTime", SecondsToClock( v ) } })
            end
        end
    end
    
    for k,v in pairs(msg.tableOfVars) do
        -- check to see if we have the correct message and deal with it
        if k == "Show_ScoreBoard" then
            if bFirstTime then
                --print("--> Show_ScoreBoard " .. player:GetName().name)    
                showScoreBoard(self)
                
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Pre-Game"}
            else
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_2"}
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_3"}
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Post-Game"}
            end
                    
            freezePlayer(self, true)
            
            -- clear game vars
            self:SetVar("bBeatTopScore", false)
			self:SetVar("bNextWaveBeatTopScore", false)
            self:SetVar("NextBest", false )
            self:SetVar("NextBestName", false)
            self:SetVar("FoundPlayer", false )
        elseif k == "Exit_Waves" then    
            local tempPlayer = GAMEOBJ:GetObjectByID(v)                        
            local tempTable = { nameVar = tempPlayer:GetName().name,
                                timeVar = 0,
                                scoreVar = 0,
                                bConfirm = false}
            
            if player:GetID() == v then     
                break
            end
            
            if table.maxn(teamScoreBoard) > 0 then
                teamScoreBoard = checkScoreBoardTable(self, tempTable, true)
                showScoreBoard(self, true)
            end
        elseif k == "Start_Wave_Message" then
            self:SetVar("displayDeath", false)
            UI:SendMessage("ToggleLeaderboard", { {"id",  v},{"visible", false} } )
            UI:SendMessage( "Toggle" .. gameUI.scoreboard, {{"visible", true },{"stateChange", "paused"}, {"iticTime", "00:00"}, 
															{"countdownText", Localize("UI_COUNTDOWN")}, {"currentWave", 0} } )
            bFirstTime = false
            freezePlayer(self)
            
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Post-Game"}
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Pre-Game"}
            player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_1"}
			CheckPlayerScore(self, 1, 0)
		elseif k == "numRemaining" then         
            UI:SendMessage( "Update" .. gameUI.scoreboard, { {"enemiesRemaining", v}} )
        elseif k == "Wave_Complete.1" then      
			local waveNum = msg.tableOfVars["Wave_Complete.1"]
			local waveTime = msg.tableOfVars["Wave_Complete.2"]
			
            UI:SendMessage( "Update" .. gameUI.scoreboard, { {"stateChange", "waveComplete"}, 
							{"survivalNote", Localize("UI_SG_WAVE") .. " " .. waveNum .. " " .. Localize("UI_WAVES_COMPLETED") .. " \r " .. Localize("UI_RACE_RESULTS_TOTAL_TIME") .. " " .. SecondsToClock( waveTime )} } )
			-- check if the player has beat their LB rank.
			CheckPlayerScore(self, waveNum, waveTime)
        elseif k == "New_Wave" then         
            UI:SendMessage( "Update" .. gameUI.scoreboard, { {"stateChange", "active"}, {"currentWave", v}} )
            self:SetVar("waveNum", v)
        elseif k == "Start_Cool_Down" and v then         
			UI:SendMessage( "Update" .. gameUI.scoreboard, { {"stateChange", "cooldown"}, {"coolDownTime", v}, {"totalCoolDownTime", v} } )
        elseif k == "Start_Timed_Wave.1" then         
			UI:SendMessage( "Update" .. gameUI.scoreboard, { {"stateChange", "timedActive"}, {"coolDownTime", msg.tableOfVars["Start_Timed_Wave.1"]}, 
															{"totalCoolDownTime", msg.tableOfVars["Start_Timed_Wave.1"]}, {"currentWave", msg.tableOfVars["Start_Timed_Wave.2"]} } )
        elseif k == "Update_Cool_Down" then         
			UI:SendMessage( "Update" .. gameUI.scoreboard, { {"coolDownTime", v} } )
			--print('update cool down ' .. v)
        elseif k == "Spawn_Mob" then                   
            if v == "2" then
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_1"}
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_2"}
            elseif v == "3" then
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_2"}
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_3"}
            end
        elseif k == "Clear_Scoreboard" then                 
            UI:SendMessage( "Update" .. gameUI.summary, {  {"countdownTime", " " },
                                                        {"p1_check",  false },
                                                        {"p2_check",  false },
                                                        {"p3_check",  false },
                                                        {"p4_check",  false }} )  
            -- remove ui timer
            UI:SendMessage( "Toggle" .. gameUI.summary, {  {"visible", false },} )    
            teamScoreBoard = {}        
        elseif k == "startCelebration" then							
			if player:Exists() then
				startCelebration(self, v)
			end
        elseif k == "startCinematic" then							
			if player:Exists() then
				player:PlayCinematic{pathName = v}
			end
        end
    end
end

function onNotifyClientZoneObject(self, msg) 
    if msg.name == "Player_Died" then      
		msg.paramObj:DisplayTooltip{ bShow = true, strText = Localize("SWITCH_DEATHCAM"), id = "SwitchCamTooltip"}  
		
		local curWave = self:GetVar("waveNum") or 1
        -- add ui timer
        UI:SendMessage( "Update" .. gameUI.scoreboard, {{"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")},
                                    {"iticTime", SecondsToClock( msg.param1 ) }, 
									{"inextbestticWave",  curWave}, 
                                    {"inextbestticTime",  SecondsToClock(msg.param1) }, 
                                    {"inextbestname", Localize("SURVIVAL_DEAD_RIGHT_SIDE_NAME_PLATE") },
									{"bBeatTopScore", false},
									{"bNextWaveBeatTopScore", false} } )   
        self:SetVar("displayDeath", true) 
        
        if msg.paramStr == "false" then
            UI:SendMessage( "Update" .. gameUI.scoreboard, { {"survivalNote", Localize("SURVIVAL_DEAD_TOOL_TIP_MESSAGE")} } )
        end
        
        UI:SendMessage("pushGameState", { {"state", gameUI.leaderboard } })
    elseif msg.name == "Player_Res" then  				
        freezePlayer(self, true)
    elseif msg.name == "Update_ScoreBoard" then  
        if not msg.paramObj:Exists() then return end
        
        --print('update player ' .. msg.paramObj:GetName().name)
        local tempTable = { nameVar = msg.paramObj:GetName().name,
                            timeVar = msg.param1,
                            scoreVar = tonumber(msg.paramStr),
                            bConfirm = false }
                            
        teamScoreBoard = checkScoreBoardTable(self, tempTable)
    end
end

function ExitBox(self, player)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 	
    
    if player:Exists() then
		local text = Localize("SURVIVAL_EXIT_QUESTION")
		
		if self:GetNetworkVar("wavesStarted") then
			text = Localize("SURVIVAL_PLAYING_EXIT_QUESTION")
		end
		
        -- display exit box
        player:DisplayMessageBox{bShow = true, 
                         imageID = 1, 
                         text = text, 
                         callbackClient = self, 
                         identifier = "Exit_Question"}
    end
end

--------------------------------------------------------------
-- when the celebration is completed, do init
--------------------------------------------------------------
function notifyCelebrationCompleted(self, other, msg)
	-- make sure the player is still there
	if not other:Exists() then return end
	
	-- clear the lua notification
	self:SendLuaNotificationCancel{requestTarget= other, messageName="CelebrationCompleted"}
		
	if self:GetNetworkVar("WatchingIntro") then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Try_Init_Again", self )
	end
end

----------------------------------------------------------------
-- Custom function: starts a celebration based on the name and 
-- sends it to the players specified
----------------------------------------------------------------
function startCelebration(self, name)
	if not celebrations then return end
	
	local celebID = celebrations[name] 
	
	-- if we dont have a celebration with that name then return
	if not celebID then return end
	
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())	
	
	if not player:Exists() then return end
	
	-- set the lua notification to catch that the player has completed the celebration
	self:SendLuaNotificationRequest{requestTarget = player, messageName = "CelebrationCompleted"}
	
	player:StartCelebrationEffect{rerouteID = player, celebrationID = celebID}
end
----------------------------------------------------------------
-- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)       	 
    if msg.identifier == "LeaderboardNext" then
        -- when the player hits the next button on the survival leaderboard
        UI:SendMessage( "popGameState", {{"state", gameUI.leaderboard}})
        self:SetVar("displayDeath", false)   
		UI:SendMessage( "Update" .. gameUI.scoreboard, { {"stateChange", "idle"} } )
        
		GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Post-Game"}
		GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Pre-Game"}
        showScoreBoard(self)
    elseif msg.identifier == "ActivityCloseButtonPressed" and msg.iButton == -1 then  
        -- when the player hits the activity close button, the x in the top right
        ExitBox(self, player)        
    elseif msg.identifier == "Exit" and msg.iButton == 1 then 
        -- when the player hits the exit button on the survival summary UI
        ExitBox(self, player)
    elseif msg.identifier == "Exit_Question" and msg.iButton == 1 then      
        -- when the player hit's the check box on the exit question message box clear all the survival UI elements
        UI:SendMessage( "ToggleActivityCloseButton", {{"bShow", false}} )  
        UI:SendMessage("Toggle" .. gameUI.summary, {{"visible", false }}) 
        UI:SendMessage( "Toggle" .. gameUI.scoreboard, {{"stateChange", "paused"}, {"visible", false }} )
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.identifier},{"visible", false} } )
        UI:SendMessage( "popGameState", {{"state", gameUI.game}} )
        UI:SendMessage( "popGameState", {{"state", gameUI.leaderboard}})
    end
end

function onShutdown(self)
	UI:SendMessage( "Toggle" .. gameUI.scoreboard, {{"stateChange", "idle"}, {"visible", false }} )
end

function onSendActivitySummaryLeaderboardData(self, msg)
	local MaxCount = msg.leaderboardData["Result[0].RowCount"] or 0 
	 
    --print('Activity Summary Sent to client Zone')    
	self:SetVar("bBeatTopScore", false) 
	self:SetVar("bNextWaveBeatTopScore", false)

	if MaxCount ~= 1 then return end

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 		
	local n = 1
	local tableValues = {}
	
	for i = 0, MaxCount do            
		tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].name"]         
		n = n + 1

		tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].Time"]	
		n = n + 1		
		
		tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].Wave"]	
		n = n + 1		

		if msg.leaderboardData["Result[0].Row["..i.."].Relationship"]  then    
			if msg.leaderboardData["Result[0].Row["..i.."].Relationship"] ~= 0 then 
				self:SetVar("FoundFriendGuild", true )
			end
		end
	end
        
	self:SetVar("NextBest", false)  	
	self:SetVar("NextBestName", false)	
	self:SetVar("LeaderTable", tableValues) 
 
	CheckPlayerScore(self, 1, 0)  
end

function CheckPlayerScore(self, waveNum, waveTime)
	if self:GetVar("bBeatTopScore") or self:GetVar("displayDeath") then return end
		
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 	
    local playerName = player:GetName().name
    local tableValues = self:GetVar("LeaderTable") or {}    
    local uiNextName = tableValues[1] or playerName
    local uiNextScore = tableValues[2] or waveTime
    local uiNextWave = tableValues[3] or waveNum
    
    if uiNextScore > 0 then
		if (waveNum == uiNextWave and waveTime <= uiNextScore) or self:GetVar("bNextWaveBeatTopScore") then
			self:SetVar("bBeatTopScore", true)
			UI:SendMessage("Update" .. gameUI.scoreboard, { {"bBeatTopScore", true} } )  
			
			return
		elseif waveNum == uiNextWave then
			self:SetVar("bNextWaveBeatTopScore", true)
			UI:SendMessage("Update" .. gameUI.scoreboard, { {"bNextWaveBeatTopScore", true} } ) 
			
			return
		end
	end
		
	UI:SendMessage("Update" .. gameUI.scoreboard, { {"iplayerName", playerName}, 
								{"inextbestticWave",  uiNextWave}, 
								{"inextbestticTime",  SecondsToClock(uiNextScore) }, 
								{"inextbestname", tostring(uiNextName) } } )   
								 
	self:SetVar("NextBest", math.floor(uiNextScore) )  	
	self:SetVar("NextBestName", uiNextName )	    
end

function onTimerDone(self, msg)
    if msg.name == "Try_Freeze_Again" then 
		if not self:GetNetworkVar("wavesStarted") then
			freezePlayer(self, true)
		end
    elseif msg.name == "Try_Init_Again" then
        playerInit(self)
    end
end

function split(str, pat)
    local t = {}
    -- creates a table of strings based on the passed in pattern   
    string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)

    return t
end 