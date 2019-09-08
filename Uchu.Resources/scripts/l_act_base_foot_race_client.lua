--------------------------------------------------------------
-- Base Foot Race script that handlese the client side logic
--
-- updated 1/5/11 mrb... added overhead icon
--------------------------------------------------------------
require('L_ACT_GENERIC_ACTIVITY_MGR')
require('L_AG_NPC_NO_INTERACT')

local gVars = {}
local tGoalpostVars = {} 

----------------------------------------------------------------
-- Called when the script starts up; setup activity and randomseed
----------------------------------------------------------------
function baseStartup(self)
    self:FireEventServerSide{senderID = self, args = "setupActivity"} -- send message to the server script to set up the activity
    math.randomseed( os.time() ) -- get random seed for randomizing numbers    
	
	-- only set up the chat bubble if this object is an NPC
	if self:GetIsNPC().bIsNPC then
		--set the vars for interaction. NOTE: any/all of thses are optional
		SetProximityDistance(self, 30)
		
		self:AddObjectToGroup{group = "FootRaceStarter"}
		
		-- Proximity speech
		AddInteraction(self, "proximityText", Localize("CHAT_FOOTRACE_A"))  
		AddInteraction(self, "proximityText", Localize("CHAT_FOOTRACE_B"))
		AddInteraction(self, "proximityText", Localize("CHAT_FOOTRACE_C"))  
		AddInteraction(self, "proximityAnim", "prox")
	end
	
    -- set up the two proximity radius for icon display
    self:SetProximityRadius{iconID = 63, radius = 40, name = "Icon_Display_Distance"}
end

----------------------------------------------------------------
-- called when the script is shut down; clears out the UI
----------------------------------------------------------------
function baseShutdown(self)
    -- turn of the generic UI timer if the script shuts down
    -- UI:SendMessage( "UdpateFootRaceScoreboard",  {{"visible", false }} )
    
    --we might want to throw the leaderboard up in this fail case so users can return to gameplay mode
    --from my understanding of the scripts this will shutdown when the race ends - which would be when we want the leaderboards anyway
    
end

----------------------------------------------------------------
-- sets the local constants for this script
----------------------------------------------------------------
function setLocalVars(self, var1, var2)
    for k,v in pairs(var1) do
        self:SetVar(tostring(k), v)
    end    
    
    self:SetVar("tGoalpostVars", var2)
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 


----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)
    local preConVar = self:GetVar("CheckPrecondition")
    
    -- if we dont have CheckPreconditions set in HF then return out of this function
    if not preConVar then return msg end
    
    -- TODO: need new tech to check all preconditions or need to redo and/or logic in Lua
    local tPreconditions = split(preConVar, ";")
    
    for k,v in ipairs(tPreconditions) do
        local check = msg.objIDUser:CheckPrecondition{PreconditionID = v}
        
        if not check.bPass then 
            msg.HasReasonFromScript = true
            msg.Script_IconID = check.IconID
            msg.Script_Reason = check.FailedReason -- needs localization
            msg.Script_Failed_Requirement = true
            msg.bCanUse = false
            break
        end
    end
    
    return msg
end

----------------------------------------------
-- Message sent from some server object
----------------------------------------------
function onNotifyClientObject(self, msg)
    if msg.name == "ToggleLeaderBoard" then
        UI:SendMessage("pushGameState", {{"state", "TimedRaceLeaderboard"}})
        UI:SendMessage("ToggleLeaderboard", { {"id",  msg.param1}, {"visible", true}, {"hideReplay", true} } )    
    elseif msg.name == "stop_timer" then              
        UI:SendMessage("UpdateFootRaceScoreboard",  {{"visible", false }} )              
        StopFootRace(self, GAMEOBJ:GetControlledID())    
    end
end

----------------------------------------------
-- Message sent from some client object
----------------------------------------------
function onNotifyObject(self, msg)      
    if msg.name == "stop_timer" then              
        UI:SendMessage("UpdateFootRaceScoreboard",  {{"visible", false }} )              
        StopFootRace(self, GAMEOBJ:GetControlledID())    
    end
end

----------------------------------------------
-- sent when the local player interacts with the object
----------------------------------------------
function baseClientUse(self, msg)    
    local playerID = GAMEOBJ:GetControlledID():GetID()
    -- check to see if we are the correct player
    if playerID ~= msg.user:GetID() or self:GetVar('isInUse') then return end
        
	-- Clear out any other gamestates before opening the help tooltip
    UI:SendMessage( "pushGameState", {{"state", "gameplay"}} )
    
    local player = GAMEOBJ:GetObjectByID(playerID)
    local displayText = self:GetVar("sStopGoalText") or Localize("FOOT_RACE_START_QUESTION")
    local boxIdentifier = "Foot_Race_Start"
    
    -- tell the object to do what it's supposed to, then turn off the interaction icon
    toggleActivatorIcon(self, true)   
        
    -- if player is in activity ask to stop other wise ask to start the foot race
    if IsPlayerInActivity(self, player) then  
        displayText = self:GetVar("sStopGoalText") or Localize("FOOT_RACE_STOP_QUESTION")
        boxIdentifier = "Foot_Race_Stop"
    elseif player:GetFlag{iFlagID = 115}.bFlag  then                
        displayText = self:GetVar("sStopGoalText") or Localize("FOOT_RACE_STOP_QUESTION")
        boxIdentifier = "Foot_Race_Cancel"
    end
    
    player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = displayText, identifier = boxIdentifier}
	self:SetVar("boxIdentifier", boxIdentifier)
end 

----------------------------------------------
-- sent when the local player terminates an interacts with the object
----------------------------------------------
function baseTerminateInteraction(self, msg)    
	if self:GetVar("boxIdentifier") then
		msg.ObjIDTerminator:DisplayMessageBox{bShow = false, identifier = self:GetVar("boxIdentifier")}
	end
    
    if msg.type ~= "user" then
        -- tell the object to do what it's supposed to, then turn off the interaction icon
        toggleActivatorIcon(self)   
    end
end

----------------------------------------------
-- sent when the player responds to the message box
----------------------------------------------
function baseMessageBoxRespond(self, msg)
    if not msg then return end
    
    local player = GAMEOBJ:GetControlledID()
    
    if msg.identifier == "Foot_Race_Stop" then
        if msg.iButton == 1 then
            ----------------------------------------------------------------------------------------------------------------------
            --You respond in the positive to "I would like to stop playing the foot race"
            ----------------------------------------------------------------------------------------------------------------------            
            UI:SendMessage("UpdateFootRaceScoreboard",  {{"visible", false }} )              
            StopFootRace(self, player)
        end
        -- tell the object to do what it's supposed to, then turn off the interaction icon
        player:TerminateInteraction{ type="fromInteraction", ObjIDTerminator=self }
        toggleActivatorIcon(self)   
        
        -- turn off the player flag for is player in foot race
        player:SetFlag{iFlagID = 115, bFlag = false}
    elseif msg.identifier == "Foot_Race_Start" then
        if msg.iButton == 1 and not IsPlayerInActivity(self, player) then
            -- add the new user
            self:FireEventServerSide{senderID = player, args = "updatePlayer_" .. player:GetID()}            
            -- start the activity for the new user
            self:FireEventServerSide{senderID = player, args = "initialActivityScore_" .. player:GetID(), param1 = self:GetVar("startTime")}
            StartFootRace(self, player)
        else
            player:TerminateInteraction{ type="fromInteraction", ObjIDTerminator=self }
            toggleActivatorIcon(self)
        end
    elseif msg.identifier == "Foot_Race_Cancel" then 
        -- check to see if the player wants to stop the other foot race they are currently in    
        local tFootRaceStarters = self:GetObjectsInGroup{ group = 'FootRaceStarter', ignoreSpawners = true }.objects
        local starterObj = false
        
        for k,v in ipairs(tFootRaceStarters) do
            -- remove the user
            if IsPlayerInActivity(v, player) then
                starterObj = v
                
                break
            end
        end
                    
        if msg.iButton == 1 then
            -- player hit yes so ask if they want to start this foot race
            local displayText = self:GetVar("sStopGoalText") or Localize("FOOT_RACE_START_QUESTION")
            local boxIdentifier = "Foot_Race_Start"
            
            if starterObj then
                starterObj:NotifyObject{name = "stop_timer" , rerouteID = player}
            end
            
            player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = displayText, identifier = boxIdentifier}
                    
            -- turn off the player flag for is player in foot race
            player:SetFlag{iFlagID = 115, bFlag = false}
        else
            -- player hit the X so clean up
            player:TerminateInteraction{ type="fromInteraction", ObjIDTerminator=self }     
            toggleActivatorIcon(self)       
        end
    end
    
end

----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide)
    local player = GAMEOBJ:GetControlledID()
    
    if not bHide then -- show the icon, cancel notification, set isInUse to false
        bHide = false
        self:SetVar('isInUse', false)
    else -- hide the icon, request notification, set isInUse to true
        self:SetVar('isInUse', true)
    end

    -- request the interaction update
    self:RequestPickTypeUpdate()
end 

----------------------------------------------
-- get a random obsticle based on the 
-- self:GetVar("obsticleLot") table and return it 
----------------------------------------------
function getRandObsticles(self)
    local obsticleTable = self:GetVar("obsticleLot")
    
    if not obsticleTable then return {} end
    
    local obsticlePoints = {}    
    local totalNodes = self:GetVar("total_spawner_nodes")
    
    -- if we want a finishline remove 1 from the total nodes
    if self:GetVar("finalGoalObsticleLot") > 0 then
        totalNodes = totalNodes - 1
    end
    
    -- run through the total nodes and obsticle Lot table, then make a table of the resulting random objects/points.
    for i = 1, self:GetVar("numOfObsticles") do
        local pass = false
        
        while not pass do
            local rand = math.random(1,totalNodes)                
            
            if not obsticlePoints[rand] then
                obsticlePoints[rand] = obsticleTable[math.random(1,#obsticleTable)]
                pass = true
            end
        end
    end    

    return obsticlePoints    
end

----------------------------------------------
-- spawn in the goals and start the activity
----------------------------------------------
function StartFootRace(self, player)
    if self:GetVar('bRaceStarted') then return end
    
    self:SetVar('bRaceStarted', true)
    
    local path = LEVEL:GetPathWaypoints(self:GetVar("pathName"))     
    self:SetVar('total_spawner_nodes', #path)    
    local randObsticles = getRandObsticles(self)
    
    for k,v in ipairs(path) do -- set up the config data for each goal and load the object
        local config = {    {"custom_script_client", "scripts/ai/ACT/FootRace/L_ACT_FOOT_RACE_GOAL.lua" },
                            {"groupID", "Goals;Goals_" .. k}, 
                            {"total_spawner_nodes", self:GetVar("total_spawner_nodes")}, 
                            {"node_number", k},
                            {"tGoalpostVars", self:GetVar("tGoalpostVars")},
                            {"renderDisabled", true}, }
                            
        if randObsticles[k] then -- insert an obsticle in to the config data if appropriate
            table.insert(config, {"spawnObsticle", randObsticles[k]})
        end
        
        if k == #path and self:GetVar("finalGoalObsticleLot") > 0 then -- insert the finish line in to the config data if appropriate
            table.insert(config, {"finalGoalObsticleLot", self:GetVar("finalGoalObsticleLot")})
        end
        
        RESMGR:LoadObject{ objectTemplate = tostring(self:GetVar("goalLot")), x= v.pos.x, y= v.pos.y , z= v.pos.z, rw = v.rot.w, 
                                            rx = v.rot.x, ry = v.rot.y, rz = v.rot.z, owner = self, configData = config,
											bDelayedLoad = true}  
    end
    
    freezePlayer(self, true) --freezePlayer(self, bFreeze) 
    player:ShowActivityCountdown()
    ActivityTimerStart(self, "Start_Timer_Delay", -1, 3) --ActivityTimerStart(self, timerName, updateTime, stopTime)
end

----------------------------------------------
-- fire event sent from another client object
----------------------------------------------
function baseFireEvent(self, msg)
    if not msg then return end
    
    local tArgs = split(msg.args, "_") -- seperate out the objID and the message name
    local player = false
    local curTime = ActivityTimerGetRemainingTime(self, "Foot_Race_Timer")
    local displayText = false
    
    -- if we have an objID then get the lwoobj
    if tArgs[2] then
        -- if we dont have the local player return
        if tArgs[2] ~= GAMEOBJ:GetControlledID():GetID() then 
            return
        else
            player = GAMEOBJ:GetObjectByID(tArgs[2])
        end
    end
    
    -- send the correct message to the server side script and display the correct message based on tArgs[1]
    if curTime then
        if tArgs[1] == "PlayerHitGoal" then            
            ActivityTimerAddTime(self, "Foot_Race_Timer", self:GetVar("addTime")) --ActivityTimerAddTime(self, timerName, addTime)  
            UI:SendMessage( "UpdateFootRaceScoreboard",  {{"time", curTime + self:GetVar("addTime")}} ) 
        elseif tArgs[1] == "PlayerHitFirstGoal" then
           --[[
            if self:GetVar("sFirstGoalText") then
                displayText = self:GetVar("sFirstGoalText")
            else
                displayText = Localize("FOOT_RACE_FIRST_GOAL")            
            end
           ]]--
            
            ActivityTimerAddTime(self, "Foot_Race_Timer", self:GetVar("addTime")) --ActivityTimerAddTime(self, timerName, addTime)  
            UI:SendMessage( "UpdateFootRaceScoreboard",  {{"time", curTime + self:GetVar("addTime")}} ) 
        elseif tArgs[1] == "PlayerWon" and player then
            ----------------------------------------------------------------------------------------------------------------------
            --pretty sure that this is the case where the player finishes the race and the leaderboard will get called
            --at this point the UI should be informed to stop updating the clock / what the final time is
            ----------------------------------------------------------------------------------------------------------------------
            if ActivityTimerGetRemainingTime(self, "Foot_Race_Timer") < 1 then
                return
            end
            
            ActivityTimerStopAllTimers(self)
            
            if self:GetVar("sFinalGoalText") then
                displayText = self:GetVar("sFinalGoalText") .. ' ' .. SecondsToClock(curTime)
            else 
                displayText = Localize("FOOT_RACE_FINAL_GOAL") .. ' ' .. SecondsToClock(curTime)
            end
            
            freezePlayer(self, true) --freezePlayer(self, bFreeze) 
            player:PlayAnimation{animationID = "happy2", bPlayImmediate = true}
            
            local animTime = player:GetAnimationTime{animationID = "happy2"}.time
            self:SetVar("finalTime", curTime)
            UI:SendMessage( "UpdateFootRaceScoreboard",  {{"time", curTime}, {"raceComplete", true}} ) 
            --ActivityTimerStart(self, timerName, updateTime, stopTime)
            ActivityTimerStart(self, "Finish_Anim_Timer", animTime, animTime) 
        end
    end    
            
    if displayText then        
        UI:SendMessage( "UpdateFootRaceScoreboard",  {{"message", displayText}} )         
    end
end

----------------------------------------------
-- Shuts down the foot race 
----------------------------------------------
function StopFootRace(self, sender)
    local player = GAMEOBJ:GetControlledID()
    local killObjs = self:GetObjectsInGroup{ group = 'Goals', ignoreSpawners = true }.objects
    
    if sender then 
        player = sender
    end
    
    if IsPlayerInActivity(self, player) then
        -- remove the user
        self:FireEventServerSide{senderID = self, args = "updatePlayerTrue_" .. player:GetID()}  
    end
    
    ActivityTimerStopAllTimers(self)
        
    for k,v in ipairs(killObjs) do
        GAMEOBJ:DeleteObject( v )
    end    
    
    self:SetVar('bRaceStarted', false)
end

----------------------------------------------
-- called when an activity timer is updated
----------------------------------------------
function baseActivityTimerUpdate(self, msg)
    -- update the ui with the current time
    if msg.name == "Foot_Race_Timer" then
        UI:SendMessage( "UpdateFootRaceScoreboard",  {{"time", msg.timeRemaining}} )
    end
end

----------------------------------------------
-- called when an activity timer is finished
----------------------------------------------
function baseActivityTimerDone(self, msg)
    
    if msg.name == "Start_Timer_Delay" then
        ----------------------------------------------------------------------------------------------------------------------
        --The flow from accepting the footrace eventually gets to this statement after countdowns and animations
        ----------------------------------------------------------------------------------------------------------------------        
        ActivityTimerStart(self, "Foot_Race_Timer", 0.20, self:GetVar("startTime")) --ActivityTimerStart(self, timerName, updateTime, stopTime)
        ActivityTimerStart(self, "Foot_Race_Reset_Icon", 2, 2) --ActivityTimerStart(self, timerName, updateTime, stopTime)
        UI:SendMessage( "ToggleFootRaceScoreboard",  {{"visible", true }, {"time", self:GetVar("startTime") }} )
        freezePlayer(self) --freezePlayer(self, bFreeze) 
    elseif msg.name == "Foot_Race_Reset_Icon" then
        -- tell the object to do what it's supposed to, then turn off the interaction icon
        toggleActivatorIcon(self)   
    elseif msg.name == "Foot_Race_Timer" then
        ----------------------------------------------------------------------------------------------------------------------
        --If the foot race timer dies, you failed to finish the footrace, no leaderboard displays, footrace widget needs to die
        ----------------------------------------------------------------------------------------------------------------------
        UI:SendMessage("UpdateFootRaceScoreboard",  {{"visible", false }} )        
        GAMEOBJ:GetControlledID():DisplayTooltip{ bShow = true, strText = Localize("FOOT_RACE_FAIL"), iTime = 1 }
        StopFootRace(self)
    elseif msg.name == "Finish_Anim_Timer" then
        ----------------------------------------------------------------------------------------------------------------------
        --we reach here when the player successfully finished the footrace and the celebration animation ends   
        ----------------------------------------------------------------------------------------------------------------------
        local player = GAMEOBJ:GetControlledID()
        
        freezePlayer(self)
        
        if self:GetVar("completedFireEventGroup") then
            for k,v in ipairs(self:GetObjectsInGroup{ group = self:GetVar("completedFireEventGroup"), ignoreSpawners = true}.objects) do
                v:FireEventServerSide{senderID = self, args = "Foot_Race_Completed", param1 = player:GetID()}
            end
        end
        
        self:FireEventServerSide{senderID = self, args = "PlayerWon_" .. player:GetID(), param1 = self:GetVar("finalTime"), param2 = self:GetVar("completedSetFlagNum")}          
        self:SetVar("finalTime", 0)
            
        StopFootRace(self, player)
    end
end
