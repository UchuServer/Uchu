
--------------------------------------------------------------
-- Generic Survival Instance Client Zone Script: Including this 
-- file gives the custom functions for the Survival game.
-- Updated mrb... 1/7/10
-- Updated stc... 5/19/10 - changed UI message structure to work with the game state system
--------------------------------------------------------------
-- Localize("SURVIVAL_WAITING_FOR_PLAYER")
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('ai/L_ACTIVITY_MANAGER')

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
                    
--local tMedals = { {name = "Outwit", score  = 10}, {name = "Outplay", score = 20} } -- testing
---------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------.

function onCharacterUnserialized(self, msg)
    UI:SendMessage( "pushGameState", {{"state", "Survival"}})
end

function checkScoreBoardTable(self, tScoreBoard, bRemoveTable)
    local t = teamScoreBoard
    local tTemp = {}    
    local isUsed = false
    
    for k,v in ipairs(teamScoreBoard) do
        if v.nameVar ~= Localize("SURVIVAL_NO_PLAYER") and v.nameVar ~= tScoreBoard.nameVar and v.nameVar ~= '' then            
            table.insert(tTemp,v)
            --print('add old player to used table ' .. v.nameVar)
        end
        if v.nameVar == tScoreBoard.nameVar and bRemoveTable then
            isUsed = true
        end
    end
    
    if not isUsed then
        table.insert(tTemp,tScoreBoard)    
        --print('add new player to used table ' .. tScoreBoard.nameVar) 
    end   
    
    table.sort(tTemp, function(a,b)
                return a.nameVar<b.nameVar
            end)
    table.sort(tTemp, function(a,b)
                return a.smashVar>b.smashVar
            end)
    table.sort(tTemp, function(a,b)
                return a.timeVar>b.timeVar
            end)       
            
    self:SetVar("playerNum", table.maxn(tTemp))
        
    while table.maxn(tTemp) < 4 do
        --tVaule = tValue + 1
        table.insert(tTemp, {   nameVar = Localize("SURVIVAL_NO_PLAYER"),
                            timeVar = 0,
                            scoreVar = 0,
                            smashVar = 0})
    end
    
    --print('***************')
    --for k,v in ipairs(t) do
    --    print('*** player ' .. k .. ' ***')
    --    print(t[k].nameVar .. ' time: ' .. t[k].timeVar .. ' score: ' .. t[k].scoreVar .. ' smash: ' .. t[k].smashVar)
    --    print('***************')
    --end
    
    return tTemp
end

function SecondsToClock(sSeconds)
    local nSeconds = tonumber(sSeconds)
        if nSeconds == 0 or nSeconds == nil then
            return "00:00"; --return "00:00:00";
        else
        nHours = string.format("%02.f", math.floor(nSeconds/3600));
        nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
        nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
        return nMins..":"..nSecs --return nHours..":"..nMins..":"..nSecs
    end
end

function numToString(iNum)
    if iNum == 0 or iNum == nil then
        return "**"
    end
    
    return tostring(iNum)
end

function showScoreBoard(self)
    p = teamScoreBoard
   
    if not p then print('missing scoreboard data') return end    
    
    --for k,v in ipairs(p) do print(p[k].nameVar) end
        	
	-- Hide timer ui
    UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }})
    
	-- Show UI
	UI:SendMessage("ToggleSurvivalSummary", { { "visible", true },
                                { "p1_check",  false }, -- checkmarks
                                { "p2_check",  false },
                                { "p3_check",  false },
                                { "p4_check",  false },
                                { "p1_name", p[1].nameVar }, -- names
                                { "p2_name", p[2].nameVar },
                                { "p3_name", p[3].nameVar },
                                { "p4_name", p[4].nameVar },
                                { "p1_time", SecondsToClock(p[1].timeVar) }, -- times
                                { "p2_time", SecondsToClock(p[2].timeVar) },
                                { "p3_time", SecondsToClock(p[3].timeVar) },
                                { "p4_time", SecondsToClock(p[4].timeVar) },
                                { "p5_time", SecondsToClock(p[1].timeVar) },
                                { "p1_smash", numToString(p[1].smashVar) }, -- smash
                                { "p2_smash", numToString(p[2].smashVar) },
                                { "p3_smash", numToString(p[3].smashVar) },
                                { "p4_smash", numToString(p[4].smashVar) },
                                { "p5_smash", numToString(p[1].smashVar + p[3].smashVar + p[2].smashVar + p[4].smashVar) },
                                { "p1_score", numToString(p[1].scoreVar) }, -- score
                                { "p2_score", numToString(p[2].scoreVar) },
                                { "p3_score", numToString(p[3].scoreVar) },
                                { "p4_score", numToString(p[4].scoreVar) },
                                { "p5_score", numToString(p[1].scoreVar + p[3].scoreVar + p[2].scoreVar + p[4].scoreVar) }} )                                   
            
end

function PlayerConfirm(self, playerID)
    local playerNum = ""    
    for k,v in ipairs(teamScoreBoard) do
        --print('my name is ' .. teamScoreBoard[k].nameVar)
        if playerID:GetName().name == teamScoreBoard[k].nameVar then
            playerNum = tostring(k)
        end
    end
    
    UI:SendMessage("UpdateSurvivalSummary", {{"p" .. playerNum .. "_check",  true }} )
end

function setTeamUI(self)
    local num = #self:MiniGameGetTeamPlayers{teamID = 1}.objects    
    --print('set team ui')
    
    if num > 1 then
        for i = 1,  #self:MiniGameGetTeamPlayers{teamID = 1}.objects do
            local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[i]
            if i == 1 then
                player:TeamAddPlayerMsg{name =  self:MiniGameGetTeamPlayers{teamID = 1}.objects[1], bIsLeader = true}
            end
            player:TeamAddPlayerMsg{name =  self:MiniGameGetTeamPlayers{teamID = 1}.objects[1]}
            player:RequestTeamUIUpdate{}
        end        
    end
end

function freezePlayer(self, bFreeze)    
    local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    local eChangeType = "POP"
    
    if bFreeze then
        if playerID:IsDead().bDead then
            --print('frozen')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
            return
        end

        eChangeType = "PUSH"
    end
    
    playerID:SetStunned{ StateChangeType = eChangeType,
                        bCantMove = true, bCantAttack = true, bCantInteract = true }
                        
    
    --print('Player ' .. playerID:GetName().name .. ' ' .. eChangeType .. ' is frozen: ' .. tostring(self:GetVar('frozen')) .. ' ' .. tostring(playerID:GetStunned().bCanMove))
    if playerID:GetStunned().bCanMove and eChangeType == "PUSH" then
        print(playerID:GetName().name .. ' is still able to move')
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Try_Freeze_Again", self )
    end
end

function onNotifyClientObject(self, msg)
    if msg.name == "ToggleLeaderBoard" and msg.paramObj:GetID() == GAMEOBJ:GetControlledID():GetID() then
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.param1}, {"visible", true} } )    
    end
end

function onNotifyClientZoneObject(self, msg)    
    --if msg.name ~= "Update_Default_Start_Timer" then print('notify ' .. msg.name) end
    
    if msg.name == "Start_Timer" then        
        -- add ui timer        
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", "00:00"}} )
    elseif msg.name == "Player_Died" then        
        -- add ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, 
                                    {"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")}, 
                                    {"itime", SecondsToClock( msg.param1 ) }, 
                                    {"inextbesttime",  SecondsToClock(msg.param1) }, 
                                    {"inextbestname", Localize("SURVIVAL_DEAD_RIGHT_SIDE_NAME_PLATE") } } )   
        self:SetVar("displayDeath", true) 
        
        if msg.paramStr == "false" then
            msg.paramObj:DisplayTooltip { bShow = true, strText = Localize("SURVIVAL_DEAD_TOOL_TIP_MESSAGE"), iTime = 3000 }
            --print('playerdied *****************')
        end
    elseif msg.name == "Update_Timer" then
        if self:GetVar("displayDeath") then
            UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, 
                                        {"iplayerName", Localize("SURVIVAL_DEAD_LEFT_SIDE_NAME_PLATE")}, 
                                        {"itime", SecondsToClock( msg.param1 ) } } )   
        else
            if self:GetVar("HoldingTopScore") then
                UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, 
                                            {"itime", SecondsToClock( msg.param1 ) }, 
                                            {"inextbesttime",  SecondsToClock(msg.param1) }, 
                                            {"inextbestname", msg.paramObj:GetName().name } } )   
            else        
                UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, 
                                {"itime", SecondsToClock( msg.param1 ) } })
                checkNextHighScore(self, msg.param1)
            end
        end
    elseif msg.name == "Update_Default_Start_Timer" then
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
        UI:SendMessage( "UpdateSurvivalSummary", {{"visible", true}, {"countdownTime", msg.param1 }} )  
    elseif msg.name == "Kill_Default_Start_Timer" then
        --print('Kill Default Start Timer')
        UI:SendMessage( "UpdateSurvivalSummary", {{"countdownTime", " " }} )  
    elseif msg.name == "Kill_Timer" then        
        -- remove ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
    elseif msg.name == "Reset_Timer" then        
        -- remove ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
        UI:SendMessage( "UpdateSurvivalSummary", {{"p1_check",  false },
                                    {"p2_check",  false },
                                    {"p3_check",  false },
                                    {"p4_check",  false }} )
        
	    --teamScoreBoard = {}
    elseif msg.name == "Wave_Message" then  
        UI:SendMessage( "ToggleGenericTextField", {{"visible", true }, {"text", "Wave: " .. msg.paramStr } })
    elseif msg.name == "Start_Wave_Message" then      
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.paramStr},{"visible", false} } )
        GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):ShowActivityCountdown()
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", "00:00"}} )
        setTeamUI(self)
        freezePlayer(self)
        --print(GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetName().name)
    elseif msg.name == "Define_Player_To_UI" then     
        local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())      
        -- add player to the ui 
        UI:SendMessage( "UpdateSurvivalSummary", {{"user", msg.paramObj }} )    
        -- set player to auto-respawn
        playerID:SetPlayerAllowedRespawn{dontPromptForRespawn=true}  
    elseif msg.name == "Update_ScoreBoard" then  
        --print('update player')
        local tempTable = { nameVar = msg.paramObj:GetName().name,
                            timeVar = msg.param1,
                            scoreVar = tonumber(msg.paramStr),
                            smashVar = msg.param2}
                            
        teamScoreBoard = checkScoreBoardTable(self, tempTable)
    elseif msg.name == "Show_ScoreBoard" then  
        local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
         
        --print("Show_ScoreBoard " .. playerID:GetName().name)
        playerID:RequestTeamUIUpdate{} 
        self:SetVar("displayDeath", false)
        showScoreBoard(self)                
        
        if msg.paramStr == playerID:GetID() then
            freezePlayer(self, true)
        end
        
        UI:SendMessage( "ToggleActivityCloseButton", {{"bShow", false}} )    
        -- clear game vars
        self:SetVar("HoldingTopScore", false)
        self:SetVar("NextBest", false )  	
        self:SetVar("NextBestName", false)	
        self:SetVar("bShowedPlayer", false)
        self:SetVar("FoundPlayer", false )
    elseif msg.name == "PlayerConfirm_ScoreBoard" then  
        PlayerConfirm(self, msg.paramObj)
    elseif msg.name == "Kill_ScoreBoard" then        
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        
        UI:SendMessage("ToggleSurvivalSummary", {{"visible", false }}) 
        UI:SendMessage("EmbedUI", {{"sgembed", true }}) -- closes the leaderboard
	    teamScoreBoard = {}
        UI:SendMessage( "ToggleActivityCloseButton", {  {"bShow", true}, 
                                                        {"GameObject", self}, 
                                                        {"MessageName", "toLua"}, 
                                                        {"senderID", player} } )
    elseif msg.name == "Exit_Waves" then                
        local tempTable = { nameVar = msg.paramObj:GetName().name,
                            timeVar = msg.param1,
                            scoreVar = tonumber(msg.paramStr),
                            smashVar = msg.param2}
                            
        UI:SendMessage( "UpdateSurvivalSummary", {{"countdownTime", " " }} )  
        
	    if GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetName().name == tempTable.nameVar then    
                UI:SendMessage("ToggleSurvivalSummary", {{"visible", false }}) 
                UI:SendMessage("EmbedUI", {{"sgembed", true }}) -- closes the leaderboard
                UI:SendMessage( "popGameState", {{"state", "Survival"}} )
                UI:SendMessage( "ToggleActivityCloseButton", {{"bShow", false}} )    
                UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
	        return
	    end
	    
	    --print('remove player ' .. tempTable.nameVar)
        msg.paramObj:TeamRemovePlayerMsg{name =  'SurvivalTeam'}       
        teamScoreBoard = checkScoreBoardTable(self, tempTable, true)
        showScoreBoard(self)
    end
end

----------------------------------------------------------------
-- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)        
    -- Response to Exit activity dialog and user pressed OK
    if msg.identifier == "exit_screen" and msg.iButton == 1 then
        -- restart waves
        self:FireEventServerSide{senderID = msg.sender, args = 'start'}

    -- Response to Start activity dialog and ok is pressed and player is not in activity
    elseif msg.identifier == "exit_screen" and msg.iButton == 0 then  
        -- remove ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
        -- exit instance
        self:FireEventServerSide{senderID = msg.sender, args = 'exit'}  
    -- Response to Start activity dialog and ok is pressed and player is not in activity    
    elseif (msg.identifier == "Exit" ) and msg.iButton == 1 then 	
        UI:SendMessage( "ToggleActivityCloseButton", {{"bShow", false}} )    
    elseif msg.identifier == "ActivityCloseButtonPressed" and msg.iButton == -1 then  
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 	
        -- display exit box
        player:DisplayMessageBox{bShow = true, 
                         imageID = 1, 
                         text = Localize("SURVIVAL_EXIT_QUESTION"), 
                         callbackClient = self, 
                         identifier = "Exit"}
    end
end

function onSendActivitySummaryLeaderboardData(self, msg)
    --print('Activity Summary Sent to client Zone')    
    if (msg) then
	    self:SetVar("HoldingTopScore", false) 

        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 		
        local MaxCount = msg.leaderboardData["Result[0].RowCount"]        
        local n = 1
        local tableValues = {}
        
        for i = 0, MaxCount do            
            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].name"]         
            n = n + 1

            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].Time"]	
            n = n + 1		

            if msg.leaderboardData["Result[0].Row["..i.."].Relationship"]  then    
                if msg.leaderboardData["Result[0].Row["..i.."].Relationship"] ~= 0 then 
                    self:SetVar("FoundFriendGuild", true )
                end
            end
        end
     
        self:SetVar("bShowedPlayer", false)          
        self:SetVar("NextBest", false)  	
        self:SetVar("NextBestName", false)	
        self:SetVar("LeaderTable", tableValues)         
        setNextHighScore(self, iScore)
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
    
    for testName = 1, #tableValues do            
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
            self:SetVar("FoundPlayer", true )
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
    
    if iScore >= math.floor(highestScore) or not self:GetVar("FoundPlayer") then
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
        
    UI:SendMessage("UpdateSurvivalScoreboard", { {"iplayerName", player:GetName().name}, 
                                {"inextbesttime",  SecondsToClock(uiNextScore) }, 
                                {"inextbestname", tostring(uiNextName) } } )        
    self:SetVar("NextBest", math.floor(uiNextScore) )  	
    self:SetVar("NextBestName", uiNextName )	
    self:SetVar("bShowedPlayer", true)
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
    end
end 