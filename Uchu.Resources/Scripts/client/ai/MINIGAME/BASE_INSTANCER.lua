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
-- updated mrb... 9/7/11 -- updated for taking damage
--------------------------------------------------------------
local tLobby = {}

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)    
	local tVars = self:GetVar("tVars")    

	msg.bCanUse = checkChatServer(msg.objIDUser)
    
    -- if this is an item interaction and they havent build a car/modular obj do this stuff
    if tVars.itemType then
        if not checkInProximity(self, msg.objIDUser:GetID()) then 
            msg.bCanUse = false
        elseif msg.objIDUser:GetInvItemCount{iObjTemplate = tVars.itemType, bCheckItemDefaultInv = true}.itemCount < 1 then 
            msg.HasReasonFromScript = true
            msg.Script_IconID = 3140
            msg.Script_Reason = Localize("MINIGAME_LOBBY_RACE_BUILD_A_CAR_FAIL") -- needs localization
            msg.Script_Failed_Requirement = true
            msg.bCanUse = false
        end
    end
    
    local preConVar = self:GetVar("CheckPrecondition")
    
    if preConVar and preConVar ~= "" then             
        local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
        
        if not check.bPass then 
            msg.HasReasonFromScript = true
            msg.Script_IconID = check.IconID
            msg.Script_Reason = check.FailedReason
            msg.Script_Failed_Requirement = true
            msg.bCanUse = false
        end
    end
        
    return msg
end

function onProximityUpdate(self, msg)
	local tVars = self:GetVar("tVars")
	
    -- if the player is using a racing instancer and they entered into the interaction range then set lua notification
    if msg.name == "Interaction_Distance" and tVars.itemType then
        if msg.status == "ENTER" then
            self:SendLuaNotificationRequest{requestTarget=GAMEOBJ:GetControlledID(), messageName="RequestUseItemOn"}        
        else
            self:SendLuaNotificationCancel{requestTarget=GAMEOBJ:GetControlledID(), messageName="RequestUseItemOn"}        
        end
    end
end

function notifyRequestUseItemOn(self, other, msg)
	local tVars = self:GetVar("tVars")
	
    if tVars.itemType ~= msg.itemToUse:GetLOT().objtemplate then return end
    
    -- set the object that was double clicked
    if other:GetID() == GAMEOBJ:GetControlledID():GetID() then
        self:SetVar("UseObjectID", "|" .. msg.itemToUse:GetID())
        --print(msg.itemToUse:GetName().name)
    end
end

function onGetInteractionDetails(self, msg)
	local tVars = self:GetVar("tVars")
    local instanceType = split(tVars.UI_Type, "_")[2]    
    
    if instanceType == "Race" then
        msg.TextDetails = Localize("MINIGAME_LOBBY_RACE_DRAG_A_CAR_INTERACT") 
    end
    
    return msg
end

function baseSetVars(self, table)
    self:SetVar("tVars", table)
    
    local actNum = self:GetActivityID().activityID -- 5 --  
    local actTable = GAMEOBJ:GetDBTable{table='Activities',keyname='ActivityID',key=actNum}
    
    -- save out the activity table
    self:SetVar('ActivityTable', actTable)
    
    local costLOT = actTable.optionalCostLOT or 0
    local costCount = actTable.optionalCostCount or 0
    
    self:SetVar('ActCost', {LOT = costLOT, Count = costCount})    
    
    -- we notify the game object system so it can predownload our zone assets
    GAMEOBJ:NotifyOfZoneTransferObject( self, actNum, 0 )
        
    local instanceIcon = 78
    local overrideIcon = self:GetVar("iconID") 
    
    if overrideIcon then 
        instanceIcon = overrideIcon
    else
        local instanceType = split(table.UI_Type, "_")[2]
        
        if instanceType == "Survival" or instanceType == "Waves" then
            instanceIcon = 79
        elseif instanceType == "Dragon" then
            instanceIcon = 91
        elseif instanceType == "SG" then
            instanceIcon = 81
        elseif instanceType == "Prototype" then
            instanceIcon = 98
        end
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

function checkCost(self, player)
    local actCost = self:GetVar('ActCost') -- {LOT = actTable.optionalCostLOT, Count = actTable.optionalCostCount})    
    local itemCheck = player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 0}.itemCount -- default
    
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 2}.itemCount -- brick
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 5}.itemCount -- model
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 12}.itemCount -- quest
    
    if itemCheck < actCost.Count then 
        return false
    end
    
    return true
end

function showFirstScreen(self, player)   
    -- if help window is open or chat server is down dont showFirstScreen 
    if self:GetVar("helpOpen") or not checkChatServer(player) then return end    
	
	local tVars = self:GetVar("tVars")
	
    -- check if the this object has been unlocked.
    if tVars.misID and player:GetMissionState{missionID = tVars.misID}.missionState < tVars.missionState then
        player:DisplayTooltip { bShow = true, strText = tVars.failText, iTime = 3000 }
        
        return 
    end
		                  
    local actCost = self:GetVar('ActCost') -- {LOT = actTable.optionalCostLOT, Count = actTable.optionalCostCount})
    local uiContext = { {"user", player}, 
                        {"callbackObj", self}, 
                        {"HelpVisible", "show" }, 
                        {"type", tVars.UI_Type}, 
                        {"bHasCost", true } }
                        
    if actCost.LOT then
        if actCost.LOT > 0 then
            uiContext[5][2] = checkCost(self, player)
            table.insert(uiContext, {"optionalCostLOT", actCost.LOT})
            table.insert(uiContext, {"optionalCostCount", actCost.Count})
        end
    end
    
    -- send message to the UI to open the lobby		            
    UI:SendMessage("pushGameState", { {"state", "Lobby"}, {"context", uiContext} }) 

    self:SetVar("helpOpen", true)
    
    -- set lua notifications
	self:SendLuaNotificationRequest{requestTarget=player, messageName="ServerStateNotify"}
	self:SendLuaNotificationRequest{requestTarget=player, messageName="HitOrHealResult"}
	self:SendLuaNotificationRequest{requestTarget=player, messageName="Die"}
end

function onClientUse(self, msg)
    local player = GAMEOBJ:GetControlledID()
    
    -- check if this is the local palyer or not
    if player:GetID() ~= msg.user:GetID() or self:GetVar('LobbyOpen') then return end    
    
    if player:GetFlag{iFlagID = 115}.bFlag then        
        -- if the player is in a foot race then ask if they want to quit
        player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = "FOOT_RACE_STOP_QUESTION", identifier = "FootRaceCancel"}            
    else
		local tVars = self:GetVar("tVars")
		
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
end

function checkInProximity(self, playerID)
    -- do we have the required variables passed to us?
    if not playerID then return end
    
    -- see if the player is within the client side interaction proximity
    for k,v in ipairs(self:GetProximityObjects{name = "Interaction_Distance"}.objects) do
        if v:GetID() == playerID then
            return true
        end
    end
    
    -- if not then return false
    return false
end

function checkDoubleClickInteract(self, playerID, objID)
    -- do we have the required variables passed to us?
    if not playerID or not objID or not checkInProximity(self, playerID) then return end
    
    local player = GAMEOBJ:GetControlledID()
	local tVars = self:GetVar("tVars")
    
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
	local tVars = self:GetVar("tVars")
	
	if not tVars.itemType then
		return
	end
	
    -- set the player interaction
    msg.playerID:SetPlayerInteraction{interaction = self}
    itemInteract(self, msg.itemLOT, msg.itemToUse)
end

function itemInteract(self, itemLOT, itemToUse)
    local player = GAMEOBJ:GetControlledID()
	local tVars = self:GetVar("tVars")
    
    -- check to see if the itemLOT matches the interaction item in tVars
    if itemLOT == tVars.itemType then
        if player:GetFlag{iFlagID = 115}.bFlag then        
            -- if the player is in a foot race then ask if they want to quit
            player:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = "FOOT_RACE_STOP_QUESTION", identifier = "FootRaceCancel"}            
        else
            -- open the help window
            showFirstScreen(self, player)
        end
        
        -- Store the item they dropped on us
        self:SetVar(player:GetID(), itemToUse:GetID())
    else
        player:DisplayTooltip { bShow = true, strText = tVars.failItem, iTime = 3000 }
    end
end

function onTerminateInteraction(self,msg)    
    local player = GAMEOBJ:GetControlledID()
    
    player:DisplayMessageBox{bShow = false, imageID = 1, callbackClient = self, text = "FOOT_RACE_STOP_QUESTION", identifier = "FootRaceCancel"}            
        
    -- check to see if the help window is open
    if not self:GetVar("helpOpen") then return end
    
	local tVars = self:GetVar("tVars")
	
    -- see if the player fails the mission state check
    if tVars.misID and player:GetMissionState{missionID = tVars.misID}.missionState < tVars.missionState then
        return
    end
    
	ClearInstancer(self)
end

function ClearInstancer(self, optPlayer)
    local player = optPlayer or GAMEOBJ:GetControlledID()

    if self:GetVar("helpOpen") then
		-- close the UI window because the interaction was terminated should return iButton -1
		UI:SendMessage("ToggleInstanceEnter", {{"visible", false}})
		self:SetVar("helpOpen", false)
    end
    
	if self:GetVar('LobbyOpen') then 
		LeaveLobby(self,player)  
	end
	
	self:SendLuaNotificationCancel{requestTarget=player, messageName="ServerStateNotify"}
	self:SendLuaNotificationCancel{requestTarget=player, messageName="HitOrHealResult"}
	self:SendLuaNotificationCancel{requestTarget=player, messageName="Die"}
	UI:SendMessage( "popGameState", {{"state", "Lobby"}} )
end

function onMessageBoxRespond(self, msg)	
    -- when the player hit's ok or hits enter transfer to new zone
    local player = GAMEOBJ:GetControlledID()
    
    --print(msg.identifier .. " : " .. msg.iButton)
    
    -- handle UI button responses
    if msg.iButton == 1 then
        if msg.identifier == "PlayButton" and not self:GetVar('LobbyOpen')then	 
            self:SetVar("helpOpen", false)     
            EnterLobby(self, player)            
	    elseif msg.identifier == "LobbyReady" then
            player:MatchRequest{type = 1, value = 1} -- type: 1 = REQUEST_READY; value = (1 = ready, 0 = notready)            
        elseif msg.identifier == "LobbyUnready" then
            player:MatchRequest{type = 1, value = 0}    
        elseif msg.identifier == "FootRaceCancel" then    
            -- player is in a foot race and said they want to quit
            -- find the race starters in the zone
            local tFootRaceStarters = self:GetObjectsInGroup{ group = 'FootRaceStarter', ignoreSpawners = true }.objects
            
            -- check the race starters to see if the player is in it's activity
            for k,v in ipairs(tFootRaceStarters) do
                -- remove the user
                if v:ActivityUserExists{ userID = player }.bExists then
                    v:NotifyClientObject{name = "stop_timer" , rerouteID = player}
                    -- done
                    break
                end
            end
            
            -- show the mini-game start screen
            showFirstScreen(self, player)
                    
            -- turn off the player flag for is player in foot race
            player:SetFlag{iFlagID = 115, bFlag = false}       
        end	    
    elseif msg.iButton == -1 then
        if msg.identifier == "CloseButton" then            
            self:SetVar("helpOpen", false)
            player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}           
        end
    elseif msg.iButton == 0 then
        if msg.identifier == "FootRaceCancel" then    
            player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}     
        end
    end
    
    if msg.identifier == "LobbyExit" and msg.iButton ~= -1 then         
        --LeaveLobby(self,player)
        ClearInstancer(self,player)
    elseif msg.identifier == "InstanceEnterLeaderboardButton" then
        local actTable = self:GetVar('ActivityTable')
                    
        ClearInstancer(self, player)
        UI:SendMessage("ToggleLeaderboard", { {"id",  actTable.ActivityID}, {"visible", true}, {"hideReplay", true} } )          
    end    
end

function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    -- set the pick type
    if ( myPriority > msg.fCurrentPickTypePriority ) then
        msg.fCurrentPickTypePriority = myPriority
        
        if self:GetVar("bTransfering") then
			msg.ePickType = -1
		else
			msg.ePickType = 14    -- Interactive pick type
		end
    end

    return msg
end 

function notifyServerStateNotify(self, other, msg)
    -- check to see if the chat server is online
    if not bChatServerOnline then        
        -- close the UI window because the interaction was terminated should return iButton -1
        GAMEOBJ:GetTimer():CancelAllTimers(self)  
        
        ClearInstancer(self, other)
        
        other:DisplayTooltip { bShow = true, strText = Localize("MINIGAME_LOBBY_CHAT_SERVER_DOWN"), iTime = 3000 }
    end
end

function notifyDie(self, other, msg)
	ClearInstancer(self, other)
end

function notifyHitOrHealResult(self, other, msg)
	if msg.receiver:GetID() ~= GAMEOBJ:GetControlledID():GetID() or (msg.lifeDamageDealt == 0 and msg.armorDamageDealt == 0) then return end
	
	ClearInstancer(self, other)
end

function notifyMatchUpdate(self,other,msg) 
	--print("**** Type = " .. msg.type)
	--print("** " .. tostring(self:GetVar('LobbyOpen')))
    -- check to make sure we were sent a msg and the lobby is open
    if not msg or not self:GetVar('LobbyOpen') then return end
    
    if msg.data.player then
        if msg.type == 0 then
			--print('**** - ' .. msg.type .. ' = ' .. msg.data.playerName .. " Added")
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
            if tempTable[msg.data.player:GetID()] ~= nil then
            	bAdd = false
            end
            
            -- add all this data, save the vars and update the lobby
            if bAdd then
                tempTable[msg.data.player:GetID()]  = 0
                tempTable2[msg.data.player:GetID()] =  msg.data.playerName
                self:SetVar('tLobbyPlayers', tempTable)   
                self:SetVar('tLobbyPlayerNames', tempTable2)   
                UpdateLobby(self)
            end     
        elseif msg.type == 1 then        
			--print('**** - ' .. msg.type .. ' = ' .. msg.data.playerName .. " Removed")
            local actTable = self:GetVar('ActivityTable')
            local NumOfPlayers = self:GetVar("NumberOfPlayers") or 1   
            
            -- check and see if there are not enough players to start
            if (NumOfPlayers - 1) < (actTable.minTeamSize * actTable.minTeams) then
                GAMEOBJ:GetTimer():CancelAllTimers(self)                
            end
            
            LeaveLobby(self, msg.data.player)
        elseif msg.type == 5 then -- player ready
			--print('**** - ' .. msg.type .. ' = ' .. msg.data.player:GetName().name .. " Ready")
        	ReadyInstance(self, msg.data.player:GetID(), true)
        	UpdateLobby(self)
        elseif msg.type == 6 then -- player not ready
			--print('**** - ' .. msg.type .. ' = ' .. msg.data.player:GetName().name .. " not Ready")
        	ReadyInstance(self, msg.data.player:GetID(), false)
        	UpdateLobby(self)       
        end
    elseif msg.data.time then -- update the time and UI
        local player = GAMEOBJ:GetControlledID()
        local countdownTime = math.floor(msg.data.time)
        
        GAMEOBJ:GetTimer():CancelAllTimers(self)    
        
        if msg.type == 2 then            
            if not self:GetVar("bOpenedLobbyOnce") then
                --print('first time')
                self:SetVar("bOpenedLobbyOnce", true)
            else
                --print('lost a player')
                self:SetVar("resetWaitingForPlayers", true)
                UI:SendMessage("UpdateInstanceLobby", {{"user", player}, {"callbackObj", self}, {"resetWaitingForPlayers", true}} )  
            end
            
            UpdateLobby(self)
        else
            if countdownTime > 0 then                  
                if self:GetVar("resetWaitingForPlayers") then
                    self:SetVar("resetWaitingForPlayers", false)
                    UI:SendMessage("UpdateInstanceLobby", {{"user", player}, {"callbackObj", self}, {"resetWaitingForPlayers", false}} )  
                end
                
                self:SetVar('CountdownTick', countdownTime - 1)
                GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self ) 
                UpdateLobby(self, countdownTime)
            else
                self:SetVar('CountdownTick', countdownTime)
                GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self )
                UpdateLobby(self, self:GetVar('ActivityTable').waitTime)
            end
        end
    end
end

function UpdateLobby(self, optionalTime)    
	if not self:GetVar('LobbyOpen') then return end
	
    local player = GAMEOBJ:GetControlledID()
    local playerID = player:GetID()
    local actTable = self:GetVar('ActivityTable')
    local tPlayers = self:GetVar('tLobbyPlayers')
    local tPlayerNames = self:GetVar('tLobbyPlayerNames')
    local minigameVars = {{"user", player}, {"callbackObj", self}}
    
    -- if tPlayers doens't exist then create a new table with the nessessary variables
    if not tPlayers then            
        tPlayers = {}  
        tPlayers[playerID] = 0
        self:SetVar('tLobbyPlayers', tPlayers)
        tPlayerNames = {}  
        tPlayerNames[playerID] = player:GetDisplayName().name
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
        
    local playersNeeded = (actTable.minTeamSize*actTable.minTeams) - (count - 1)
    
    if playersNeeded >= 0 then
        table.insert(minigameVars, {"updatePlayersNeeded", playersNeeded})
    end
    
    if optionalTime then
        table.insert(minigameVars, {"countdownTime", optionalTime})
    end
    
    self:SetVar("NumberOfPlayers", count - 1)
    
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
    if not checkChatServer(player) or self:GetVar('LobbyOpen') then return end
    
    local actTable = self:GetVar('ActivityTable')    
    local playerID = player:GetID()
    local item = self:GetVar("UseObjectID")
    
    -- if they didn't double-click a car, use their default
    if item == nil or item == 0 then
		item = self:GetVar(playerID)
    end
    
    -- check for item interaction and send appropriate requests
    if not item then 
    	player:MatchRequest{type = 0, value = actTable.ActivityID } -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)        
    else
    	player:MatchRequest{type = 0, value = actTable.ActivityID, activator=self, playerChoices={droppedItem=item} } -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)        
    end
    
    -- freeze and hide player
    freezePlayer(self, true)
    
    local actNameKey = Localize("Activities_".. actTable.ActivityID .. "_ActivityName")
    
    -- if max num of players is 1 transfer straight into mini game
    if actTable.maxTeamSize == 1 and actTable.maxTeams == 1 then
        player:DisplayTooltip { bShow = true, NoRevive = true, strText = Localize("MINIGAME_LOBBY_WAIT_MESSAGE_START") .. " " .. actNameKey .. " " .. Localize("MINIGAME_LOBBY_WAIT_MESSAGE_END"), iTime = 100000 }
    else
		local tVars = self:GetVar("tVars")
	
        -- send Lua notifications, open and update lobby
        self:SetVar('LobbyOpen', true)
        self:SendLuaNotificationRequest{requestTarget=player, messageName="MatchUpdate"}
        UI:SendMessage("ToggleInstanceLobby", {{"visible", true}, {"callbackObj", self}, {"type", tVars.UI_Type}, {"minPlayersRequired", (actTable.minTeamSize * actTable.minTeams)},{"activityName", actNameKey}})
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
	if self:GetVar("bTransfering") then return end
        
    -- if this is the local player then reset everything back to beginning state
    if player:GetID() == GAMEOBJ:GetControlledID():GetID() then        
		GAMEOBJ:GetTimer():CancelAllTimers(self)        
        player:MatchRequest{type = 0, value = 0} -- type: 0 = REQUEST_JOIN; value = (0 = exit, any other # = activityID)   
        self:SetVar('LobbyOpen', false) 
        self:SetVar("helpOpen", false)
        self:SetVar('tLobbyPlayers', {})  
        self:SetVar('tLobbyPlayerNames', {})  
        UI:SendMessage("UpdateInstanceLobby", {{"user", GAMEOBJ:GetControlledID()}, {"callbackObj", self}, {"countdownTime", self:GetVar('ActivityTable').waitTime}} )
        UI:SendMessage("ToggleInstanceLobby", {{"visible", false}})
		self:SendLuaNotificationCancel{requestTarget=player, messageName="MatchUpdate"}
        freezePlayer(self)
        player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
        self:SetVar("bOpenedLobbyOnce", false)
    else
        -- not local player so just update the lobby and remove the player from the table
        removePlayerFromLobby(self,player)
        UpdateLobby(self)
    end
end 

function removePlayerFromLobby(self,player)
    local tempTable = self:GetVar('tLobbyPlayers')
    local tempTable2 = self:GetVar('tLobbyPlayerNames')
        
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
	if not self:GetVar('LobbyOpen') then return end
	
    if ( msg.name == "StartTimer" ) then
        local countdownTime = self:GetVar('CountdownTick')
        
        -- update the UI with the new time
        UI:SendMessage("UpdateInstanceLobby", {{"user", GAMEOBJ:GetControlledID()}, {"callbackObj", self}, {"countdownTime", countdownTime}} )
        
        if countdownTime > 0 then -- start another 1 sec timer
            self:SetVar('CountdownTick', countdownTime - 1)
            GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartTimer", self )
            if countdownTime == 1 then			
				self:SetVar("bTransfering", true)
				self:RequestPickTypeUpdate()
			end
		else -- all done close out the lobby UI and pop up the waiting message
            local player = GAMEOBJ:GetControlledID()
            local actNameKey = Localize("Activities_".. self:GetVar('ActivityTable').ActivityID .. "_ActivityName")
            
            GAMEOBJ:GetTimer():CancelAllTimers(self)    
            ClearInstancer(self, player)
            player:DisplayTooltip { bShow = true, NoRevive = true, strText = Localize("MINIGAME_LOBBY_WAIT_MESSAGE_START") .. " " .. actNameKey .. " " .. Localize("MINIGAME_LOBBY_WAIT_MESSAGE_END"), iTime = 100000 }
            if self:GetVar("nofade") == 1 then
				player:SetFlag{iFlagID = 120, bFlag = true}
            end
        end            
    end
end 
