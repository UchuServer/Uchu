--------------------------------------------------------------
-- Generic Activity manager script, holds a bunch of useful functions
-- for creating activities.
-- updated mrb... 12/08/10
--------------------------------------------------------------

----------------------------------------------------------------
-- Sets up an activity of the object, only need to put in variable you want to change
----------------------------------------------------------------
function SetupActivity(self, nMaxUsers, getActivity)

    if getActivity then        
        self:SetActivityParams{ activityID = self:GetVar('activityID'), modifyMaxUsers = true, maxUsers = nMaxUsers, modifyActivityActive = true,  activityActive = true}    
        return
    end
    
    -- set max users to something high
    self:SetActivityParams{ modifyMaxUsers = true, maxUsers = nMaxUsers, modifyActivityActive = true,  activityActive = true}    
end


----------------------------------------------------------------
-- Returns true/false if a player is in the activity
-- takes self and a PLAYER object
----------------------------------------------------------------
function IsPlayerInActivity(self, player)
    -- check if player is in activity
    local existMsg = self:ActivityUserExists{ userID = player }
    if (existMsg) then
        return existMsg.bExists
    end
    return false

end

----------------------------------------------------------------
-- Updates players for the activity: addPlayer defaults to true
----------------------------------------------------------------
function UpdatePlayer(self, player, removePlayer)
-- Response to Exit activity dialog and user pressed OK
    --print(tostring(removePlayer) .. ' ' .. self:GetActivityID().activityID)
    if removePlayer then
        -- remove the user
        --print('remove user ' .. player:GetName().name)
        self:RemoveActivityUser{ userID = player }
    elseif not removePlayer then
        -- add the new user
        --print('add user ' .. player:GetName().name)
        self:AddActivityUser{ userID = player }
        
        -- start the activity for the new user
        InitialActivityScore(self, player, 0)
    end

end

----------------------------------------------------------------
-- Stores the start time for the player in the activity and
-- sends messages to start it
----------------------------------------------------------------
function InitialActivityScore(self, player, scoreVar)
    self:SetActivityUserData{ userID = player, typeIndex = 0, value = tonumber(scoreVar) }
end

----------------------------------------------------------------
-- adds the valueVar to the existing value of the index for the 
-- player in the activity 
----------------------------------------------------------------
function UpdateActivityValue(self, player, valueIndex, valueVar)
    local newValue = self:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue + valueVar
    --print(newValue)
    self:SetActivityUserData{ userID = player, typeIndex = valueIndex, value = newValue }    
end

----------------------------------------------------------------
-- Stores a vaariable for the player in the activity
----------------------------------------------------------------
function SetActivityValue(self, player, valueIndex, valueVar)
    self:SetActivityUserData{ userID = player, typeIndex = valueIndex, value = tonumber(valueVar) }
end

----------------------------------------------------------------
-- Gets the value of the activity index for the player
----------------------------------------------------------------
function GetActivityValue(self, player, valueIndex)
    return self:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue
end

----------------------------------------------------------------
-- StopActivity message
----------------------------------------------------------------
function StopActivity(self, player, scoreVar, value1Var, value2Var, quit )
    -- user is trying to cancel
    if quit then
        
        -- remove the user from activity
        self:RemoveActivityUser{ userID = player }        
        
    else    
        -- store the time as activity rating [1]
        self:SetActivityUserData{ userID = player, typeIndex = 0, value = scoreVar }
        
        if value1Var ~= nil then            
            self:SetActivityUserData{ userID = player, typeIndex = 1, value = value1Var }
        elseif value2Var ~= nil then        
            self:SetActivityUserData{ userID = player, typeIndex = 2, value = value2Var }
        end

        -- distribute rewards        
        self:DistributeActivityRewards{ userID = player, bAutoAddCurrency = true, bAutoAddItems = true }      
        
        -- Update Leaderboards for this user
        self:UpdateActivityLeaderboard{ userID = player }
        
        local actID = self:GetActivityID().activityID
        -- get the leaderboard data for the user and update summary screen if it exists
        player:RequestActivitySummaryLeaderboardData{target = self, queryType = 1, gameID = actID }
        self:NotifyClientObject{name = "ToggleLeaderBoard", param1 = actID, paramObj = player , rerouteID = player}
        
        -- remove the user from activity
        self:RemoveActivityUser{ userID = player }    
    end
end

----------------------------------------------------------------
-- GetLeaderboard Data message
----------------------------------------------------------------
function GetLeaderboardData(self, player, activityID, EndNumOfResults)
	if EndNumOfResults then
		-- get the leaderboard data for the user and update summary screen if it exists
		player:RequestActivitySummaryLeaderboardData{ user = player, target = self, gameID = activityID, resultsEnd = EndNumOfResults }
	else
		player:RequestActivitySummaryLeaderboardData{ user = player, target = self, gameID = activityID }
	end
end

----------------------------------------------------------------
-- Start activity timer
----------------------------------------------------------------
function ActivityTimerStart(self, timerName, updateTime, stopTime)
    if stopTime then    
        self:ActivityTimerSet{name = timerName, updateInterval = updateTime, duration = stopTime}
    else
        self:ActivityTimerSet{name = timerName, updateInterval = updateTime}
    end
end

----------------------------------------------------------------
-- Reset an activity timer
----------------------------------------------------------------
function ActivityTimerReset(self, timerName)
    self:ActivityTimerReset{name = timerName}
end

----------------------------------------------------------------
-- Stop activity timer
----------------------------------------------------------------
function ActivityTimerStop(self, timerName)
    --print('** stop activity timer ' .. timerName .. ' now **')
    self:ActivityTimerStop{name = timerName}
end

----------------------------------------------------------------
-- Stop activity timer
----------------------------------------------------------------
function ActivityTimerStopAllTimers(self)
    self:ActivityTimerStopAllTimers()
end

----------------------------------------------------------------
-- Add time to activity timer
----------------------------------------------------------------
function ActivityTimerAddTime(self, timerName, addTime)
    self:ActivityTimerModify{name = timerName, timeToAdd = addTime}
end

----------------------------------------------------------------
-- Get remaining activity timer time
----------------------------------------------------------------
function ActivityTimerGetRemainingTime(self, timerName)
    local timerInfo = self:ActivityTimerGet{name = timerName}
    
    return timerInfo.timeRemaining
end

----------------------------------------------------------------
-- Get remaining activity timer time
----------------------------------------------------------------
function ActivityTimerGetCurrentTime(self, timerName)
    local timerInfo = self:ActivityTimerGet{name = timerName}
    
    return timerInfo.timeElapsed
end


function split(str, pat)
   local t = {}
   
   -- splits a string based on the given pattern and returns a table
   string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)
   
   return t
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
                        bCantMove = true, bCantAttack = true, bCantUseItem = true, bCantInteract = true }
                        
    --print('Player ' .. playerID:GetName().name .. ' ' .. eChangeType .. ' is frozen: ' .. tostring(self:GetVar('frozen')) .. ' ' .. tostring(playerID:GetStunned().bCanMove))
    if playerID:GetStunned().bCanMove and eChangeType == "PUSH" then
        print(playerID:GetName().name .. ' is still able to move')
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Try_Freeze_Again", self )
    end
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

function onTimerDone(self, msg)
    if msg.name == "Try_Freeze_Again" then    
        freezePlayer(self, true)
    end
end 

function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end 