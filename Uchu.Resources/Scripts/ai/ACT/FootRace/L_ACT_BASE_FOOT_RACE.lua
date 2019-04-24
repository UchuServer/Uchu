--------------------------------------------------------------
-- Base Server side Foot Race script 
-- updated mrb... 10/26/10
-- updated rfurino 7/26/11
--------------------------------------------------------------
require('ai/ACT/L_ACT_GENERIC_ACTIVITY_MGR')

function onStartup(self)
    local zControl = GAMEOBJ:GetZoneControlID()
    
    self:AddObjectToGroup{group = "FootRaceStarter"}
    self:SendLuaNotificationRequest{requestTarget = zControl, messageName = "PlayerExit"}
end

function onShutdown(self)
    self:SendLuaNotificationCancel{requestTarget = zControl, messageName = "PlayerExit"}
end

function onFireEventServerSide(self, msg)
    --print('onFireEventServerSide ' .. msg.args)
    local tArgs = split(msg.args, "_")
    local player = false
    
    if tArgs[2] then
        player = GAMEOBJ:GetObjectByID(tArgs[2])
    end
        
    if player then
        if tArgs[1] == "updatePlayer"  then
            UpdatePlayer(self, player) --UpdatePlayer(self, player, removePlayer)       
        elseif IsPlayerInActivity(self, player) then 
            if tArgs[1] == "initialActivityScore" then    
                -- turn on the player flag for is player in foot race
                player:SetFlag{iFlagID = 115, bFlag = true}
                InitialActivityScore(self, player, 1, msg.param1) --InitialActivityScore(self, player, scoreVar)   
                --print('found player ' .. player:GetName().name .. ' ********')
				self:SendLuaNotificationRequest{requestTarget = player, messageName = "PlayerLaunchpadInteract"}
            elseif tArgs[1] == "updatePlayerTrue" then    
				EndActivity(self, player)
            elseif tArgs[1] == "PlayerWon" then      
                if msg.param2 then
                    player:SetFlag{iFlagID = msg.param2, bFlag = true}
                end
                
                -- turn off the player flag for is player in foot race
                player:SetFlag{iFlagID = 115, bFlag = false}
                
                StopActivity(self, player, 0, msg.param1 ) --StopActivity(self, player, scoreVar, value1Var, value2Var, quit )
            end
        end
    elseif tArgs[1] == "setupActivity" then    
        SetupActivity(self, 9999, true)
    end
end

function EndActivity(self, player)
	-- turn off the player flag for is player in foot race
	player:SetFlag{iFlagID = 115, bFlag = false}

	UpdatePlayer(self, player, true) --UpdatePlayer(self, player, removePlayer)
	
    self:SendLuaNotificationCancel{requestTarget = player, messageName = "PlayerLaunchpadInteract"}
end

function notifyPlayerLaunchpadInteract(self, player, msg)

	--msg also contains senderID (the launch pad itself) and targetID (the player id)
	EndActivity(self, player)
	self:NotifyClientObject{name = "stop_timer", reroutID = player}
end

function notifyPlayerExit(self, other, msg)
    if not IsPlayerInActivity(self, msg.playerID) then return end
    
    UpdatePlayer(self, msg.playerID, true) -- if player logs out remove them from the activity
end 

----------------------------------------------------------------
-- When activity is stopped this is needed to update the leaderboard.
----------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)    
    msg.outActivityRating = msg.fValue1
    
    return msg
end