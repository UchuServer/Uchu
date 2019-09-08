--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

---------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------.
function onStartup(self)    
--    -- add ui timer
   
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

function onNotifyClientZoneObject(self, msg)    
    if msg.name == "Start_Timer" then        
        -- add ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", "00:00"}} )
        --UI:SendMessage( "ToggleGenericTextField", {{"visible", false }})
    end
    if msg.name == "Update_Timer" then
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", SecondsToClock( msg.param1 ) } })
        --UI:SendMessage( "ToggleGenericTextField", {{"visible", false }})
    end
    if msg.name == "Kill_Timer" then        
        -- remove ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }} )
         UI:SendMessage( "popGameState", {{"state", "Survival" }} )
        --UI:SendMessage( "ToggleGenericTextField", {{"visible", false }})
    end
    if msg.name == "Reset_Timer" then        
        -- remove ui timer
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", "00:00"}} )
    end
    if msg.name == "Wave_Message" then  
        UI:SendMessage( "ToggleGenericTextField", {{"visible", true }, {"text", "Wave: " .. msg.paramStr } })
    end
    if msg.name == "Start_Wave_Message" then  
        GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):ShowActivityCountdown()
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", true }, {"itime", "00:00"}} )
        --UI:SendMessage( "ToggleGenericTextField", {{"visible", true }, {"text", msg.paramStr } })
    end
    
end

----------------------------------------------------------------
-- Server sends us a notification to do help, show dialogs
----------------------------------------------------------------
function onHelp(self, msg)
 
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

    if (msg.iHelpID == 0) then

        player:DisplayMessageBox{bShow = true, 
                                 imageID = 1, 
                                 text = "Click Ok To Defend Against The Endless Horde!!!, cancel to exit.", 
                                 callbackClient = self, 
                                 identifier = "exit_screen"}       
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
    end

end

function onSendActivitySummaryLeaderboardData(self, msg)
    --print('Activity Summary Sent to client Zone')
	if (msg) then
	    self:SetVar("HoldingTopScore", false) 

        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        local  MaxCount = msg.leaderboardData["Result[0].RowCount"]
        
        n = 1
        b = 1
        q = 1
        d = 1
        tableValues = {}
        storedScore = {}
        finalTable = {}
        name_value = 0
        score_value = 0
        name = 0
        score = 0
        
        for i = 0, MaxCount do            
            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].name"]         
            n = n + 1

            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].Score"]	
            n = n + 1		

            if msg.leaderboardData["Result[0].Row["..i.."].Relationship"]  then    
                if msg.leaderboardData["Result[0].Row["..i.."].Relationship"] ~= 0 then 
                    self:SetVar("FoundFriendGuild", true )
                end
            end
        end
     
        self:SetVar("LeaderTable", tableValues) 
        
        leaderboardDataForUI = implode(",",self:GetVar("LeaderTable"))      
        self:SetVar("LeaderString", leaderboardDataForUI)  
        UI:SendMessage("UpdateSurvivalScoreboard", { {"inextbestname",  leaderboardDataForUI} } )
        	
        for j = 1, #tableValues do
            if tableValues[j] == player:GetName().name then
                name = j
                score = (j + 1)     
                
                if name ~= 1 then 
                    name_value = (j - 2)
                    score_value = (j - 1)
                elseif name_value == 3 then
                    name_value = 1
                    score_value = 2
                elseif j == 1 then
                    name_value = 1
                    score_value = 2
                end
                
                
                self:SetVar("FoundPlayer", true )
                break
            end
        end
        
	    self:SetVar("name",name )
	    self:SetVar("score",score )
	    self:SetVar("tableValues",tableValues )
	    self:SetVar("name_value",name_value )
	    self:SetVar("score_value",score_value )
				 
        if  self:GetVar("FoundPlayer") and self:GetVar("FoundFriendGuild") then        
            UI:SendMessage("UpdateSurvivalScoreboard", { {"inextbesttime",  tostring(tableValues[score_value]) }, {"inextbestname", tostring(tableValues[name_value]) } } )
        elseif not self:GetVar("FoundFriendGuild") and self:GetVar("FoundPlayer") then                    
            UI:SendMessage("UpdateSurvivalScoreboard", { {"inextbesttime",  tostring(tableValues[self:GetVar("score")]) }, {"inextbestname", tostring(tableValues[ self:GetVar("name")]) } } )
        end
		
		UI:SendMessage("UpdateSurvivalScoreboard", { {"iplayerName",  tostring(tableValues[ self:GetVar("name")]) } } )
  		--UI:SendMessage("UpdateSurvivalScoreboard", { {"iplayerName",  tostring(tableValues[score_value]) }, {"inextbestname", tostring(tableValues[name_value]) } } )
  		self:SetVar("NextBest", tostring(tableValues[score_value]) )  		
    end

end
