--------------------------------------------------
-- Nexus Tower Target Client script
--
-- updated - 4/7/11 - mrb... updated icon dist
--------------------------------------------------
local preConVar = "166"
local UI_Type = "NT_Challenge_01"
local iconDist = 80

function onStartup(self)
    local actNum = self:GetActivityID().activityID -- 5 --  
    local actTable = GAMEOBJ:GetDBTable{table='Activities',keyname='ActivityID',key=actNum}
    
    -- save out the activity table
    self:SetVar('ActivityTable', actTable)
    
    local costLOT = actTable.optionalCostLOT or 3039
    local costCount = actTable.optionalCostCount or 1
    
    self:SetVar('ActCost', {LOT = costLOT, Count = costCount})
            
    -- set up the proximity radius for icon display
    toggleIcon(self, true)
end

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)    
	if self:GetVar("bUIOpen") or self:GetNetworkVar("bInUse") then		
		msg.bCanUse = false
	else	
		local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
			
		if not check.bPass then 
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = check.IconID
				msg.Script_Reason = check.FailedReason
				msg.Script_Failed_Requirement = true
			end
			
			msg.bCanUse = false
		end    
    end

    return msg
end

function onTerminateInteraction(self, msg)            
    if msg.type == "fromInteraction" then return end
	
	-- close the UI
	closeUI(self, false)
end

function onRenderComponentReady(self, msg)
	if not self:GetNetworkVar("bInUse") then return end
	
	local curTime = self:GetNetworkVar("update_time") or "--"
	local curScore = self:GetNetworkVar("totalDmg") or 0
	
	-- open the scoreboard GUI
	UI:SendMessage( "ArmoryScoreboard", {{"visible", true }, {"time", curTime}, {"score", curScore}}, self)		
	
	local totalTime = self:GetNetworkVar("totalTime") or 30
	--set the max time for the GUI
	UI:SendMessage( "SetTotalTime", {{"totalTime", totalTime}}, self)
end

function onScriptNetworkVarUpdate(self,msg) 
    for k,v in pairs(msg.tableOfVars) do
        if k == "bInUse" then
			local playerID = GAMEOBJ:GetLocalCharID()
            local player = GAMEOBJ:GetObjectByID(playerID)   
            
			if self:GetVar("bUIOpen") then
				closeUI(self, false)
			end
            
            -- update the pick type and terminate the interaction
			self:RequestPickTypeUpdate()
			
			if player:GetPlayerInteraction().interaction:GetID() == self:GetID() then
				player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
			end
		elseif k == "toggle" then
            if v then
				-- open the scoreboard GUI
                UI:SendMessage( "ArmoryScoreboard", {{"visible", true }, {"time", "--"}, {"score", 0}}, self)
            else
				-- close the scoreboard GUI
                UI:SendMessage( "ArmoryScoreboard", {{"visible", false }}, self)
                self:SetVar("lastTotal", 0)
            end	
		elseif k == "update_time" then
			-- update the time GUI
			UI:SendMessage( "ArmoryScoreboard", {{"time", v}}, self)
        elseif k == "totalDmg" and v then	
			-- update the score GUI
			UI:SendMessage( "ArmoryScoreboard", {{"score", v}}, self)	
            
            local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())   
            
            -- display the floating text
            if player:Exists() then
                local tSize = {x = 0.11, y = 0.16}
                local tStart = self:GetPosition().pos
                tStart.y = tStart.y + 4.5    
                local lastTotal = self:GetVar("lastTotal") or 0
                local dmgText = tostring(v - lastTotal)                           

                player:RenderFloatingText{  ni3WorldCoord = tStart, ni2ElementSize = tSize, 
                                            fFloatAmount = 0.1,  uiTextureHeight = 200, uiTextureWidth = 200,
                                            i64Sender = self, fStartFade = 1.0, 
                                            fTotalFade = 1.25, wsText = dmgText, 
                                            uiFloatSpeed = 4, iFontSize = 4, 
                                            niTextColor = {r=255 ,g=255 ,b=255 ,a=0} }
            end
            
            self:SetVar("lastTotal", v)
        elseif k == "totalTime" and v then
        	--set the max time for the GUI
        	UI:SendMessage( "SetTotalTime", {{"totalTime", v}}, self)
        end			
    end
end

function onNotifyClientObject(self, msg)
	if msg.name == "UI_Open" then
		-- show the mini-game start screen
		showFirstScreen(self, msg.paramObj)
	elseif msg.name == "UI_Close" then
		closeUI(self, msg.param1)
	end	
end

function closeUI(self, bUpdatePickType)
	if not self:GetVar("bUIOpen") then return end
	
	self:SetVar("bUIOpen", false)
	
	-- close the UI window because the interaction was terminated should return iButton -1
	UI:SendMessage("ToggleInstanceEnter", {{"visible", false}})
	UI:SendMessage( "popGameState", {{"state", "Lobby"}} )
	
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
            
	player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
	
	if bUpdatePickType then
		-- update the pick type
		self:RequestPickTypeUpdate()
	end
end

function checkCost(self, player)
    local actCost = self:GetVar('ActCost')
    
    if actCost == 0 then return true end
    
    local itemCheck = player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 0}.itemCount -- default
    
    if itemCheck >= actCost.Count then return true end
    
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 2}.itemCount -- brick
    
    if itemCheck >= actCost.Count then return true end
    
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 5}.itemCount -- model
    
    if itemCheck >= actCost.Count then return true end
    
    itemCheck = itemCheck + player:GetInvItemCount{iObjTemplate = actCost.LOT, eInvType = 12}.itemCount -- quest
    
    return itemCheck >= actCost.Count
end

function showFirstScreen(self, player)   
    -- if help window is open or chat server is down dont showFirstScreen 
    if self:GetVar("bUIOpen") then return end    
    
	self:SetVar("bUIOpen", true)
	-- update the pick type
	self:RequestPickTypeUpdate()
			                  
    local actCost = self:GetVar('ActCost')
    local uiContext = { {"user", player}, 
                        {"callbackObj", self}, 
                        {"HelpVisible", "show" }, 
                        {"type", UI_Type}, 
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
end

function onClientUse(self, msg)
    if msg.user:GetFlag{iFlagID = 115}.bFlag then        
        -- if the player is in a foot race then ask if they want to quit
        msg.user:DisplayMessageBox{bShow = true, imageID = 1, callbackClient = self, text = "FOOT_RACE_STOP_QUESTION", identifier = "FootRaceCancel"}
	end	
end

function toggleIcon(self, bOn)
	if bOn and not self:GetNetworkVar("bInUse") then
		self:SetProximityRadius{iconID = 100, radius = iconDist, name = "Icon_Display_Distance"}	
	else
		self:UnsetProximityRadius{name = "Icon_Display_Distance"}	
	end
end

function onMessageBoxRespond(self, msg)	
    -- when the player hit's ok or hits enter transfer to new zone
    local player = msg.sender
    
	if msg.identifier == "CloseButton" then
		closeUI(self, true)
    end
    
    if msg.identifier ~= "FootRaceCancel" then return end
    
	if msg.iButton == 1 then
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
		
		-- turn off the player flag for is player in foot race
		player:SetFlag{iFlagID = 115, bFlag = false}      		
		-- show the mini-game start screen
		showFirstScreen(self, player)
	elseif msg.iButton == 0 then
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
	end
end 

function onGetPriorityPickListType(self, msg)
	local myPriority = 0.8
	
    if myPriority > msg.fCurrentPickTypePriority  and not self:GetNetworkVar("bInUse") then
        msg.fCurrentPickTypePriority = myPriority
        msg.ePickType = 14   
        toggleIcon(self, true)
    else   		
        msg.ePickType = -1 
        toggleIcon(self, false)
    end

    return msg
end 