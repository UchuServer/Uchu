--------------------------------------------------------------
-- Battle instance Activators single and multiple player
-- created by Trent
-- updated mrb... 5/4/10
--------------------------------------------------------------

--Config Data

--[[

	MissionA  =  Mission#
	MissionB  =  Mission#
	PlayerMax =  Max # of players
	PlayerMin =  Min # of players
	Zone      =  Testmap to this Zone

--]]


     

--------------------------------------------------------------
-- Sent when the script is started.
--------------------------------------------------------------
function onStartup(self,msg)
    -- set up the proximity radius
    self:SetProximityRadius{radius = 20, name = "BattleActivator"}
    
     local tVars = {
     releaseVersion = 200 , -- which version release # the content should be made available for Beta 1
     misID = self:GetVar("MissionA"),
     missionState = 2,
     UI_Type = "AG_Survival_01",
     failText = Localize("MINIGAME_LOBBY_AG_SUVIVAL_FAIL_MESSAGE"),}
    
    
     baseSetVars(self, tVars)
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    -- send the correct UI message 
    if (msg.status == "ENTER") then
 
    end
      
end 
--------------------------------------------------------------
-- Spawn Temp FX Rings
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

		local Markpos = self:GetPosition().pos 
		local Markrot = self:GetRotation()
		
		RESMGR:LoadObject { objectTemplate = 10068 , x = Markpos.x ,
		y = Markpos.y - 4 , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self }; 

		RESMGR:LoadObject { objectTemplate = 10068 , x = Markpos.x ,
		y = Markpos.y - 8 , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self }; 


end

--------------------------------------------------------------
-- ON CLIENT USE check Missions
--------------------------------------------------------------

function onClientUse(self,msg)                      

    local player = msg.user
	local missionStateA = player:GetMissionState{missionID = self:GetVar("MissionA")}.missionState
	
    if missionStateA == 2 or missionStateA == 8 then

		SendClientUse(self,msg)
	end
	
	
end






--------------------------------------------------------------
-- Description: Base Client script for Shooting Gallery NPC 
-- in GF area. Lets client know the object can be interacted with
-- players are stored as a table with the playerID as a key and 
-- the readyState as a value. 
--
-- ALSO: Add L_BASE_DRAG_INTERACT_INSTANCER_SERVER.lua 
-- to server side to make instancer use drag to interact
-- and double click functionality (racing)
--
-- updated mrb... 4/26/10
--------------------------------------------------------------
local tVars = {}
local tLobby = {}

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)
    local verInfo = msg.objIDUser:GetVersioningInfo()
    
    -- check to see if the instancer is supposed to be blocked for beta
    if not verInfo.bIsInternal and verInfo.iMajorRelease < 1 and verInfo.iVersionRelease < tVars.releaseVersion then
        msg.objIDUser:DisplayTooltip { bShow = true, strText = self:GetVar('ActivityTable').ActivityName .. " " .. Localize("MINIGAME_LOBBY_BETA_CLOSED_MESSAGE"), iTime = 3000 }
            
        msg.bCanUse = false
    else
        msg.bCanUse = checkChatServer(msg.objIDUser)
    end
    
    -- if this is an item interaction and they havent build a car/modular obj do this stuff
    if tVars.itemType then
	    if msg.objIDUser:GetInvItemCount{iObjTemplate = tVars.itemType}.itemCount < 1 then 
            msg.HasReasonFromScript = true
            msg.Script_IconID = 3140
            msg.Script_Reason = Localize("MINIGAME_LOBBY_RACE_BUILD_A_CAR_FAIL") -- needs localization
            msg.Script_Failed_Requirement = true
            msg.bCanUse = false
        end
    end
    
    return msg
end

function onGetInteractionDetails(self, msg)
    local instanceType = split(tVars.UI_Type, "_")[2]
    
    if instanceType == "Race" then
        msg.TextDetails = Localize("MINIGAME_LOBBY_RACE_DRAG_A_CAR_INTERACT") -- needs localization
    end

    return msg
end

function baseSetVars(self, table)
    tVars = table
    
    local actNum = self:GetActivityID().activityID -- 5 --  
    
    -- save out the activity table
    self:SetVar('ActivityTable', GAMEOBJ:GetDBTable{table='Activities',keyname='ActivityID',key=actNum})    
    -- we notify the game object system so it can predownload our zone assets
    GAMEOBJ:NotifyOfZoneTransferObject( self, actNum, 0 )
        
    local instanceIcon = 78
    local instanceType = split(tVars.UI_Type, "_")[2]
    
    if instanceType == "Survival" then
        instanceIcon = 79
    elseif instanceType == "SG" then
        instanceIcon = 81
    end
    
    -- set up the two proximity radius for icon display
    self:SetProximityRadius{iconID = instanceIcon, radius = 35, name = "Icon_Display_Distance"}
    self:SetProximityRadius{radius = self:GetVar("interaction_distance"), name = "Interaction_Distance"}
end

function checkChatServer(player)
    -- check to see if the chat server is down
    if not player:GetServerState().bChatServerOnline then
        player:DisplayTooltip { bShow = true, strText = Localize("MINIGAME_LOBBY_CHAT_SERVER_DOWN"), iTime = 3000 }
        
        return false
    end
    
    return true
end

function showFirstScreen(self, player)   
    -- if help window is open or chat server is down dont showFirstScreen 
    if self:GetVar("helpOpen") or not checkChatServer(player) then return end    

    -- check if the this object has been unlocked.
    if tVars.misID and player:GetMissionState{missionID = tVars.misID}.missionState < tVars.missionState then
        player:DisplayTooltip { bShow = true, strText = tVars.failText, iTime = 3000 }
        
        return 
    end
		            
    -- send message to the UI to open the lobby		            
    UI:SendMessage("ToggleInstanceEnter", {{"user", player}, {"callbackObj", self}, {"HelpVisible", "show" }, {"type", tVars.UI_Type}} )
    self:SetVar("helpOpen", true)
end

function SendClientUse(self, msg)
    local player = GAMEOBJ:GetControlledID()
    
    -- check if this is the local palyer or not
    if player:GetID() ~= msg.user:GetID() or self:GetVar('LobbyOpen') then return end    
    
    -- see if this is an item interaction or a normal interaction
    if tVars.itemType then
        local modObj = player:GetFirstInventoryItemByLOT{ iObjTemplate = tVars.itemType, inventoryType = 5 }.itemID
        
        -- if we have an obj use it to interact or display the fail text
        if modObj then
            itemInteract(self, tVars.itemType, modObj)
        else            
            player:DisplayTooltip { bShow = true, strText = tVars.failItem, iTime = 3000 }
        end
    else        
        showFirstScreen(self, player)
    end
end

function checkDoubleClickInteract(self, playerID, objID)
    -- do we have the required variables passed to us?
    if not playerID or not objID then return end
    
    local bFail = true -- check var
    
    -- see if the player is within the client side interaction proximity
    for k,v in ipairs(self:GetProximityObjects{name = "Interaction_Distance"}.objects) do
        if v:GetID() == playerID then
            bFail = false
            break
        end
    end
    
    -- if not then return out of the function
    if bFail then return end
    
    local player = GAMEOBJ:GetControlledID()
    
    -- check to make sure we have the correct player and that the object double clicked was the correct LOT
    if playerID ~= player:GetID() or GAMEOBJ:GetObjectByID(objID):GetLOT().objtemplate ~= tVars.itemType then return end
    
    -- find the first LOT in the players inventory
    local modObj = player:GetFirstInventoryItemByLOT{ iObjTemplate = tVars.itemType, inventoryType = 5 }.itemID
    
    -- if we have an obj then sen the interaction
    if modObj then     
        -- set the player interaction
        player:SetPlayerInteraction{interaction = self}
        -- send message to use the obj as the drag interaction
        itemInteract(self, tVars.itemType, modObj)      
    end
end

function onScriptNetworkVarUpdate(self, msg)    
    local player = GAMEOBJ:GetControlledID()
    
    -- check to see if we have the correct message and deal with it
    if msg.tableOfVars["bPassedCheck.1"] == player:GetID() then 
        checkDoubleClickInteract(self, msg.tableOfVars["bPassedCheck.1"], msg.tableOfVars["bPassedCheck.2"])
    end
end

function onUseItemOnClient(self,msg)
    -- if we aren't an item interaction return out of this function
	if not tVars.itemType then
		return
	end
	
    -- set the player interaction
    msg.playerID:SetPlayerInteraction{interaction = self}
    itemInteract(self, msg.itemLOT, msg.itemToUse)
end

function itemInteract(self, itemLOT, itemToUse)
    -- check to see if the itemLOT matches the interaction item in tVars
	if itemLOT == tVars.itemType then
	    -- open the help window
		showFirstScreen(self, GAMEOBJ:GetControlledID())
		-- Store the item they dropped on us
		self:SetVar(GAMEOBJ:GetControlledID():GetID(), itemToUse:GetID())
	else
        GAMEOBJ:GetControlledID():DisplayTooltip { bShow = true, strText = tVars.failItem, iTime = 3000 }
	end
end

function onTerminateInteraction(self,msg)
    -- check to see if the help window is open
    if not self:GetVar("helpOpen") then return end
    
    local player = GAMEOBJ:GetControlledID()
    
    -- see if the player fails the mission state check
    if tVars.misID and player:GetMissionState{missionID = tVars.misID}.missionState < tVars.missionState then
        return
    end
    
    -- close the UI window because the interaction was terminated should return iButton -1
    UI:SendMessage("ToggleInstanceEnter", {{"user", player}, {"callbackObj", self}, {"HelpVisible", "hide"}})
    self:SetVar("helpOpen", false)
end

function onMessageBoxRespond(self, msg)	
    -- when the player hit's ok or hits enter transfer to new zone
    local player = GAMEOBJ:GetControlledID()
    
    -- handle UI button responses
    if msg.iButton == 1 then
        if msg.identifier == "PlayButton" and not self:GetVar('LobbyOpen')then	 
            self:SetVar("helpOpen", false)     
            EnterLobby(self, player)            
	    elseif msg.identifier == "LobbyReady" then
            player:MatchRequest{type = 1, value = 1} -- type: 1 = REQUEST_READY; value = (1 = ready, 0 = notready)            
        elseif msg.identifier == "LobbyUnready" then
            player:MatchRequest{type = 1, value = 0}           
        end	    
    elseif msg.iButton == -1 then
        if msg.identifier == "CloseButton" then            
            self:SetVar("helpOpen", false)
            player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
        end
    end
    
    if msg.identifier == "LobbyExit" and msg.iButton ~= -1 then         
        LeaveLobby(self,player)
	end
end

function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    -- set the pick type
    if ( myPriority > msg.fCurrentPickTypePriority ) then
        msg.fCurrentPickTypePriority = myPriority
        msg.ePickType = 14    -- Interactive pick type
    end

    return msg
end 

function notifyServerStateNotify(self, other, msg)
    -- check to see if the chat server is online
    if not bChatServerOnline then        
        -- close the UI window because the interaction was terminated should return iButton -1
        GAMEOBJ:GetTimer():CancelAllTimers(self)    
        UI:SendMessage("ToggleInstanceLobby", {{"visible", false}})    
        LeaveLobby(self,other)  
        
        other:DisplayTooltip { bShow = true, strText = Localize("MINIGAME_LOBBY_CHAT_SERVER_DOWN"), iTime = 3000 }
    end
end

function notifyDie(self, other, msg)
    -- if the player dies leave the lobby
    LeaveLobby(self,other)   
end

function notifyMatchUpdate(self,other,msg) 
    -- check to make sure we were sent a msg and the lobby is open
    if not msg or not self:GetVar('LobbyOpen') then return end
    
    if msg.data.player then
        if msg.type == 0 then
            --print('MatchUpdate - ' .. msg.type .. ' = ' .. GAMEOBJ:GetObjectByID(msg.data.player):GetName().name)
            local tempTable = self:GetVar('tLobbyPlayers')
            local tempTable2 = self:GetVar('tLobbyPlayerNames')
            local bAdd = true
            
            -- create a tempTable if there isn't tLobbyPlayers in GetVar
            if not tempTable then
                tempTable = {}
            end
            
            -- create a tempTable2 if there isn't tLobbyPlayerNames in GetVar
            if not tempTable2 then
                tempTable2 = {}
            end
            
            -- add the player sent to tempTable
            if tempTable[msg.data.player] ~= nil then
            	bAdd = false
            end
            
            -- add all this data, save the vars and update the lobby
            if bAdd then
                tempTable[msg.data.player]  = 0
                tempTable2[msg.data.player] = msg.data.playerName
                self:SetVar('tLobbyPlayers', tempTable)   
                self:SetVar('tLobbyPlayerNames', tempTable2)   
                UpdateLobby(self)
            end     
        elseif msg.type == 1 then
            LeaveLobby(self,GAMEOBJ:GetObjectByID(msg.data.player))
        elseif msg.type == 5 then -- player ready
        	ReadyInstance(self, msg.data.player, true)
        	UpdateLobby(self)
        elseif msg.type == 6 then -- player not ready
        	ReadyInstance(self, msg.data.player, false)
        	UpdateLobby(self)       
        end
    elseif msg.data.time then -- update the time and UI
        local player = GAMEOBJ:GetControlledID()
        local countdownTime = math.floor(msg.data.time)
        
        GAMEOBJ:GetTimer():CancelAllTimers(self)    
        
        if msg.type == 2 then
            UI:SendMessage("UpdateInstanceLobby", {{"user", player}, {"callbackObj", self}, {"countdownTime", "--"}, {"type", tVars.UI_Type}} )  
        else
            if countdownTime > 0 then
                UI:SendMessage("UpdateInstanceLobby", {{"user", player}, {"callbackObj", self}, {"countdownTime", countdownTime}, {"type", tVars.UI_Type}} )        
                self:SetVar('CountdownTick', countdownTime - 1)
                GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self ) 
            else
                self:SetVar('CountdownTick', countdownTime)
                GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self )
                UI:SendMessage("UpdateInstanceLobby", {{"user", player}, {"callbackObj", self}, {"countdownTime", self:GetVar('ActivityTable').waitTime}, {"type", tVars.UI_Type}} )        
            end
        end
    end
end

function UpdateLobby(self)    
    local player = GAMEOBJ:GetControlledID()
    local playerID = player:GetID()
    local actTable = self:GetVar('ActivityTable')            
    local tPlayers = self:GetVar('tLobbyPlayers')
    local tPlayerNames = self:GetVar('tLobbyPlayerNames')
    local minigameVars = {{"user", player}, {"callbackObj", self},{"activityName", actTable.ActivityName}, {"type", tVars.UI_Type},}
    
    -- if tPlayers doens't exist then create a new table with the nessessary variables
    if not tPlayers then            
        tPlayers = {}  
        tPlayers[playerID] = 0
        self:SetVar('tLobbyPlayers', tPlayers)
        tPlayerNames = {}  
        tPlayerNames[playerID] = player:GetName().name
        self:SetVar('tLobbyPlayerNames', tPlayerNames)
    end    
            
    local count = 1
    
    -- loop through all the players in the tPlayers and update the settings
    for k,v in pairs(tPlayers) do 
        local name = tPlayerNames[k]
        
        -- Use Unknown Player if we cannot find the player name
        if name ~= nil and name ~= "" then
        	table.insert(minigameVars, {"p".. count .. "_name", name})
        else
        	table.insert(minigameVars, {"p".. count .. "_name", Localize("MINGAME_LOBBY_UNKNOWN_PLAYER")}) --shouldn't ever get this case
        end
        
        -- if the player is in the ready state then check the box
        if v == 0 then
            table.insert(minigameVars, {"p"..count.."_check", false})
        else
            table.insert(minigameVars, {"p"..count.."_check", true})
        end
        
        count = count + 1
    end
    
    for i = count, actTable.maxTeamSize*actTable.maxTeams do
        table.insert(minigameVars, {"p".. i .. "_name", Localize("MINIGAME_LOBBY_WAITING_FOR_PLAYER") .. "..."})
        table.insert(minigameVars, {"p".. i .."_check", false})
    end
    
    -- send message to UI to update the lobby
    UI:SendMessage("UpdateInstanceLobby", minigameVars )       
end

function freezePlayer(self, bFreeze)
    local playerID = GAMEOBJ:GetControlledID()  
    local eChangeType = "POP"
    local bVisible = true
    
    -- freeze and hide the player if bFreeze is true
    if bFreeze then    
        eChangeType = "PUSH"
        bVisible = false
    end
        
    -- update the player stunned/visible state
    playerID:SetVisible{visible = bVisible, fadeTime = 2}
    playerID:SetStunned{ StateChangeType =  eChangeType,
                                            bCantMove = true,
                                            bCantAttack = true,
                                            bCantInteract = true }                                                                                           
end

function EnterLobby(self, player)
    -- check to see if the chat server is down
    if not checkChatServer(player) then return end
    
    local actTable = self:GetVar('ActivityTable')    
    local playerID = player:GetID()
    local item = self:GetVar(playerID)
    
    -- check for item interaction and send appropriate requests
    if not item then 
    	player:MatchRequest{type = 0, value = actTable.ActivityID } -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)        
    else
    	player:MatchRequest{type = 0, value = actTable.ActivityID, activator=self, playerChoices={droppedItem=item} } -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)        
    end
    
    -- freeze and hide player
    freezePlayer(self, true)
    
    -- if max num of players is 1 transfer straight into mini game
    if actTable.maxTeamSize == 1 and actTable.maxTeams == 1 then
        player:DisplayTooltip { bShow = true, strText = Localize("MINIGAME_LOBBY_WAIT_MESSAGE_START") .. " " .. self:GetVar('ActivityTable').ActivityName .. " " .. Localize("MINIGAME_LOBBY_WAIT_MESSAGE_END"), iTime = 100000 }
    else
        -- send Lua notifications, open and update lobby
        self:SetVar('LobbyOpen', true)
        self:SendLuaNotificationRequest{requestTarget=player, messageName="MatchUpdate"}
        self:SendLuaNotificationRequest{requestTarget=player, messageName="ServerStateNotify"}
        self:SendLuaNotificationRequest{requestTarget=player, messageName="Die"}
        UI:SendMessage("ToggleInstanceLobby", {{"visible", true}, {"type", tVars.UI_Type}, {"minPlayersRequired", (actTable.minTeamSize * actTable.minTeams)}})
        UpdateLobby(self)
    end
end

function ReadyInstance(self, playerID, ready)
    local tempTable = self:GetVar('tLobbyPlayers')
    
    -- make a new table if it didn't exist
    if not tempTable then
        tempTable = {}
    end

    local findplayer = tempTable[playerID]
    
    -- if there is a player then set the appropriate ready state
    if findplayer ~= nil then
        if not ready then
            tempTable[playerID] = 0
        else
            tempTable[playerID] = ready
        end
        
        self:SetVar('tLobbyPlayers', tempTable)
    end
end 

function LeaveLobby(self,player)   
    -- if this is the local player then reset everything back to beginning state
    if player:GetID() == GAMEOBJ:GetControlledID():GetID() then        
		GAMEOBJ:GetTimer():CancelAllTimers(self)        
        player:MatchRequest{type = 0, value = 0} -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)   
        self:SetVar('LobbyOpen', false) 
        self:SetVar("helpOpen", false)
        self:SetVar('tLobbyPlayers', {})  
        self:SetVar('tLobbyPlayerNames', {})  
        UI:SendMessage("UpdateInstanceLobby", {{"user", GAMEOBJ:GetControlledID()}, {"callbackObj", self}, {"countdownTime", self:GetVar('ActivityTable').waitTime}, {"type", tVars.UI_Type}} )
        UI:SendMessage("ToggleInstanceLobby", {{"visible", false}})
        self:SendLuaNotificationCancel{requestTarget=player, messageName="MatchUpdate"}
        self:SendLuaNotificationCancel{requestTarget=player, messageName="ServerStateNotify"}
        self:SendLuaNotificationCancel{requestTarget=player, messageName="Die"}
        freezePlayer(self)
        player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
    else
        -- not local player so just update the lobby and remove the player from the table
        removePlayerFromLobby(self,player)
        UpdateLobby(self)
    end
end 

function removePlayerFromLobby(self,player)
    local tempTable = self:GetVar('tLobbyPlayers')
    local tempTable2 = self:GetVar('tLobbyPlayerNames')
    local actTable = self:GetVar('ActivityTable')
    local iRemove = 0
        
    -- clear the player from the tables
    tempTable[player:GetID()] = nil
    tempTable2[player:GetID()] = nil
    
    -- update the tables
    self:SetVar('tLobbyPlayers', tempTable)
    self:SetVar('tLobbyPlayerNames', tempTable2)
end

function split(str, pat)
   local t = {}
   
   -- splits a string based on the given pattern and returns a table
   string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)
   
   return t
end   

function onTimerDone(self, msg) -- todo: need to break out the timers into start and wait, so if there is network lag it wont close ui on first timer; or a bool
    if ( msg.name == "StartTimer" ) then
        local countdownTime = self:GetVar('CountdownTick')
        
        -- update the UI with the new time
        UI:SendMessage("UpdateInstanceLobby", {{"user", GAMEOBJ:GetControlledID()}, {"callbackObj", self}, {"countdownTime", countdownTime}, {"type", tVars.UI_Type}} )
        
        if countdownTime > 0 then -- start another 1 sec timer
            self:SetVar('CountdownTick', countdownTime - 1)
            GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self )   
        else -- all done close out the lobby UI and pop up the waiting message
            local player = GAMEOBJ:GetControlledID()
            
            GAMEOBJ:GetTimer():CancelAllTimers(self)    
            UI:SendMessage("ToggleInstanceLobby", {{"visible", false}})
            --UI:SendMessage("MiniGameLobby", {{"user", player}, {"callbackObj", self}, {"countdownTime", self:GetVar('ActivityTable').waitTime}, {"LobbyVisible", false}} )
            player:DisplayTooltip { bShow = true, strText = Localize("MINIGAME_LOBBY_WAIT_MESSAGE_START") .. " " .. self:GetVar('ActivityTable').ActivityName .. " " .. Localize("MINIGAME_LOBBY_WAIT_MESSAGE_END"), iTime = 100000 }
        end            
    end
end 