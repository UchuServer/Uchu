--------------------------------------------------------------
-- Generic Survival Instance Client Zone Script: Including this 
-- file gives the custom functions for the Survival game.
-- updated mrb... 12/8/10 - updates for UI bugs
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('L_ACTIVITY_MANAGER')

--//////////////////////////////////////////////////////////////////////////////////
-- local variables
local teamScoreBoard = {}
local tMedalsSolo = {   {name = Localize("Missions_467_name"), score  = 300},   -- 467; Survivor
                        {name = Localize("Missions_468_name"), score = 600} }   -- 468; Ultimate Survivor
local tMedals = {   {name = Localize("Missions_392_name"), score = 60},         -- 392; Outwit
                    {name = Localize("Missions_393_name"), score = 180},        -- 393; Outplay
                    {name = Localize("Missions_394_name"), score = 300},        -- 394; Outlast
                    {name = Localize("Missions_395_name"), score = 420},        -- 395; Survivalist
                    {name = Localize("Missions_396_name"), score = 480},        -- 396; Be Prepared!
                    {name = Localize("Missions_397_name"), score = 540},        -- 397; Paramedics!
                    {name = Localize("Missions_398_name"), score = 600},   }    -- 398; Unstoppable!
                    
                    
local bFirstTime = true
local checkMission = 479 	-- earn your sentinel badge
local checkMissionTime = 60	-- earn your sentinel badge time
---------------------------------------------------------------
-- Startup of the object
---------------------------------------------------------------

function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end

function playerInit(self)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    UI:SendMessage( "pushGameState", {{"state", "Survival"}})
    -- set player to auto-respawn
    player:SetPlayerAllowedRespawn{dontPromptForRespawn=true}  
    -- add player to the ui 
    UI:SendMessage( "UpdateSurvivalSummary", {{"user", player }} )    
                        
    freezePlayer(player)
    
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
    UI:SendMessage( "UpdateSurvivalSummary", {{"countdownTime", " " }} )        
end

function onPlayerReady(self)
    --print('onPlayerReady ' .. tostring(GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetPlayerReady().bIsReady))
    
    if not GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetPlayerReady().bIsReady then
        --print('Try_Ready_Again')
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Ready_Again", self )     
    else
        playerInit(self)
    end
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
        
        if table.maxn(tTemp) < self:GetNetworkVar("NumberOfPlayers") then
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
    
	if nSeconds == 0 or not nSeconds then
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
        
    local messageName = "UpdateSurvivalSummary"
    local messageArgs = {{ "p1_check", p[1].bConfirm }, -- checkmarks
                        { "p2_check",  p[2].bConfirm },
                        { "p3_check",  p[3].bConfirm },
                        { "p4_check",  p[4].bConfirm },
                        { "p1_name", p[1].nameVar }, -- names
                        { "p2_name", p[2].nameVar },
                        { "p3_name", p[3].nameVar },
                        { "p4_name", p[4].nameVar },
                        { "p1_time", SecondsToClock(p[1].timeVar) }, -- times
                        { "p2_time", SecondsToClock(p[2].timeVar) },
                        { "p3_time", SecondsToClock(p[3].timeVar) },
                        { "p4_time", SecondsToClock(p[4].timeVar) },
                        { "p5_time", SecondsToClock(p[1].timeVar) },
                        { "p1_score", numToString(p[1].scoreVar) }, -- score
                        { "p2_score", numToString(p[2].scoreVar) },
                        { "p3_score", numToString(p[3].scoreVar) },
                        { "p4_score", numToString(p[4].scoreVar) },
                        { "p5_score", numToString(p[1].scoreVar + p[3].scoreVar + p[2].scoreVar + p[4].scoreVar) }}
    
    --for k,v in ipairs(p) do print("**"..p[k].nameVar) end
    if not bUpdateOnly then
        -- Hide timer ui
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }})
        messageName = "ToggleSurvivalSummary"
        table.insert(messageArgs, { "visible", true })        
    end
    
	-- Show UI
	UI:SendMessage(messageName, messageArgs )            
end

function freezePlayer(self, bFreeze)    
    local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    local eChangeType = "POP"
        
    if bFreeze and not self:GetNetworkVar('wavesStarted') then
        if playerID:IsDead().bDead then
            --print('frozen')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
            return
        
        elseif not playerID:GetStunned().bCanMove then 
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
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.param1}, {"visible", true}, {"nextOnly", true} } )    
    end
end

----------------------------------------------------------------
---- variables serialized from the server
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self, msg)    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
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
                UI:SendMessage( "UpdateSurvivalSummary", {{"countdownTime", " " }} )    
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
                UI:SendMessage( "UpdateSurvivalScoreboard", {{"iplayerName", "Next Round In"}, 
                                            {"itime", SecondsToClock( v )} } )   
            end
            
            if v == 0 then
                UI:SendMessage( "UpdateSurvivalSummary", {  {"visible", true}, 
                                                            {"countdownTime", v },
                                                            { "p1_check", true }, -- checkmarks
                                                            { "p2_check", true },
                                                            { "p3_check", true },
                                                            { "p4_check", true } } )              
            else
                UI:SendMessage( "UpdateSurvivalSummary", {{"visible", true}, {"countdownTime", v }} )  
            end
        elseif k == "Update_Timer" then
            if self:GetVar("displayDeath") then
                UI:SendMessage( "UpdateSurvivalScoreboard", {{"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")}, 
                                            {"itime", SecondsToClock( v )} } )   
            else            
                if not self:GetVar("bShowedPlayer") then     
                    setNextHighScore(self)
                end
                
                if self:GetVar("HoldingTopScore") then
                    UI:SendMessage( "UpdateSurvivalScoreboard", {{"itime", SecondsToClock( v ) }, 
                                                {"inextbesttime",  SecondsToClock(v) },
                                                {"inextbestname", player:GetName().name } } )   
                else        
                    UI:SendMessage( "UpdateSurvivalScoreboard", {{"itime", SecondsToClock( v ) } })
                    checkNextHighScore(self, v)
                end
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
            self:SetVar("HoldingTopScore", false)
            self:SetVar("NextBest", false )  	
            self:SetVar("NextBestName", false)	
            self:SetVar("bShowedPlayer", false)
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
            
            if #teamScoreBoard > 0 then
                teamScoreBoard = checkScoreBoardTable(self, tempTable, true)
                showScoreBoard(self, true)
            end
        elseif k == "Start_Wave_Message" then
            self:SetVar("displayDeath", false)
            UI:SendMessage("ToggleLeaderboard", { {"id",  v},{"visible", false} } )
            player:ShowActivityCountdown()
            UI:SendMessage( "UpdateSurvivalScoreboard", {{"itime", "00:00"},{"iplayerName", player:GetName().name}} )
            bFirstTime = false
            freezePlayer(self)
            
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Post-Game"}
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_Pre-Game"}
            player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_1"}
        elseif k == "Spawn_Mob" then             
            if v == "2" then
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_1"}
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_2"}
            elseif v == "3" then
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_2"}
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Survival_3"}
            end
        elseif k == "Clear_Scoreboard" then                 
            UI:SendMessage( "UpdateSurvivalSummary", {  {"countdownTime", " " },
                                                        {"p1_check",  false },
                                                        {"p2_check",  false },
                                                        {"p3_check",  false },
                                                        {"p4_check",  false }} )  
            -- remove ui timer
            UI:SendMessage( "ToggleSurvivalSummary", {  {"visible", false },} )
			-- add ui timer        
			--print('showScoreBoard in Clear Scoreboard ****')
			
			if not self:GetVar("bShowedPlayer") then   
				UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true },
															{"itime", "00:00"},
															{"iplayerName", ""}, 
															{"inextbesttime",  "" }, 
															{"inextbestname", ""} } ) 
				self:SetVar("bShowedPlayer", true)
			end
			
			teamScoreBoard = {}         
        end
    end
end

function onNotifyClientZoneObject(self, msg) 
    if msg.name == "Player_Died" then    		
        -- add ui timer
        UI:SendMessage( "UpdateSurvivalScoreboard", {{"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")}, 
                                    {"itime", SecondsToClock( msg.param1 ) }, 
                                    {"inextbesttime",  SecondsToClock(msg.param1) }, 
                                    {"inextbestname", Localize("SURVIVAL_DEAD_RIGHT_SIDE_NAME_PLATE") } } )   
        self:SetVar("displayDeath", true) 
        
        -- display tool tip if there are other players still alive
        if msg.paramStr == "false" then
            UI:SendMessage( "UpdateSurvivalScoreboard", { {"survivalNote", Localize("SWITCH_DEATHCAM")} } )           
        end
        
        UI:SendMessage("pushGameState", { {"state", "SurvivalLeaderboard" } })
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
			text = Localize("SURVIVAL_PLAYING_EXIT_QUESTION") -- "Your time will not be recorded if you exit before the game is over. Exit anyway?"
		end
		
        -- display exit box
        player:DisplayMessageBox{bShow = true, 
                         imageID = 1, 
                         text = text, 
                         callbackClient = self, 
                         identifier = "Exit_Question"}
    end
end
----------------------------------------------------------------
-- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)        
    if msg.identifier == "LeaderboardNext" then
        -- when the player hits the next button on the survival leaderboard
        UI:SendMessage( "popGameState", {{"state", "SurvivalLeaderboard"}})
        self:SetVar("displayDeath", false)
        
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
        UI:SendMessage("ToggleSurvivalSummary", {{"visible", false }}) 
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.identifier},{"visible", false} } )
        UI:SendMessage( "popGameState", {{"state", "Survival"}} )
        UI:SendMessage( "popGameState", {{"state", "SurvivalLeaderboard"}})
    end
end

function onSendActivitySummaryLeaderboardData(self, msg)
    --print('Activity Summary Sent to client Zone')    
    if (msg) then
	    self:SetVar("HoldingTopScore", false) 

        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 		
        local MaxCount = msg.leaderboardData["Result[0].RowCount"] or 0
        local tableValues = {}
        
        for i = 0, MaxCount do       
			local name = msg.leaderboardData["Result[0].Row["..i.."].name"] 
			
			table.insert(tableValues, name)
			table.insert(tableValues, msg.leaderboardData["Result[0].Row["..i.."].Time"])

			if msg.leaderboardData["Result[0].Row["..i.."].Relationship"]  then    
				if msg.leaderboardData["Result[0].Row["..i.."].Relationship"] ~= 0 then 
					self:SetVar("FoundFriendGuild", true )
				end
			end			
			
			-- found the player so dont record the other values
			if name == player:GetName().name then 				
				self:SetVar("FoundPlayer", true )
				
				break 
			end
        end
     
        self:SetVar("bShowedPlayer", false)          
        self:SetVar("NextBest", false)  	
        self:SetVar("NextBestName", false)	
        self:SetVar("LeaderTable", tableValues)         
        setNextHighScore(self)
    end
end

function setNextHighScore(self, iScore)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 			
    local name_value = 0
    local score_value = 0
    local name = 0    
    local score = 0
    local tableValues = self:GetVar("LeaderTable") or {}
    local curName = self:GetVar("NextBestName")
    local playerScore = 0
     
    if not curName then
        curName = player:GetName().name
    end
    
    for testName = 1, table.maxn(tableValues) do            
        if tableValues[testName] == curName then
            name = testName
            score = (testName + 1)     
            
            if name ~= 1 then 
                name_value = (testName - 2)
                score_value = (testName - 1)
            elseif name_value == 3 then
                name_value = 1
                score_value = 2
            elseif name_value == 1 then
                name_value = 1
                score_value = 2
            end
        end            
        
		if tableValues[testName] == player:GetName().name then
			playerScore = tableValues[(testName + 1)]
		end
	end        
        
    local uiName = tableValues[name] or player:GetName().name
    local uiScore = tableValues[score] or playerScore
    local uiNextName = tableValues[name_value] or player:GetName().name
    local uiNextScore = tableValues[score_value] or playerScore
                        
    if not self:GetVar("bShowedPlayer") then     
        uiNextName = uiName
        uiNextScore = uiScore		   
    end
    
    if not iScore then 
        iScore = 0
    end
            
    local tempMedals = tMedals
    
    if self:GetVar("playerNum") == 1 then
        tempMedals = tMedalsSolo
    end
    
    local highestScore = tableValues[2] or 0
    local missionCheck = player:GetMissionState{missionID = checkMission}.missionState < 4 and iScore < checkMissionTime
    
    if missionCheck then    
		uiNextName = Localize("Missions_479_name")
		uiNextScore = checkMissionTime
    elseif iScore >= math.floor(highestScore) or not self:GetVar("FoundPlayer") then
        local pass = false
        
        for k,v in ipairs(tempMedals) do
            if iScore < v.score then
                uiNextName = v.name
                uiNextScore = v.score
                pass = true
                break
            end
        end
        
        if not pass then
            self:SetVar("HoldingTopScore", true) 
            uiNextName = player:GetName().name
            uiNextScore = iScore      
        end      
    end    
    
    if not self:GetVar("displayDeath") then    
		local messageName = "UpdateSurvivalScoreboard"
		
		if not self:GetVar("bShowedPlayer") then
			messageName = "ToggleSurvivalScoreboard"
			self:SetVar("bShowedPlayer", true)
		end
		
        UI:SendMessage(messageName, { 	{"visible", true },
										{"iplayerName", player:GetName().name}, 
										{"inextbesttime",  SecondsToClock(uiNextScore) or "" }, 
										{"inextbestname", tostring(uiNextName) or ""} } ) 
		--print("name: " .. uiNextName .. " time: " .. uiNextScore)
    end
    
    --print("** update nextBest")
    self:SetVar("NextBest", math.floor(uiNextScore) )
    self:SetVar("NextBestName", uiNextName )
end

function checkNextHighScore(self, iScore)	
    if not iScore or not self:GetVar('NextBest') then return end
    
    if iScore >= self:GetVar('NextBest') then        
        setNextHighScore(self, iScore)
    end
end

function onTimerDone(self, msg)
	if msg.name == "Try_Freeze_Again" then    
        freezePlayer(self, true)
    elseif msg.name == "Try_Ready_Again" then
        PlayerReady(self)
    end
end 
