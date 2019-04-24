--------------------------------------------------------------
-- Activity manager: Including this file gives the basic setup 
-- and managment of an activity.
-- mrb... 9/26/09
--------------------------------------------------------------

function SurvivalStartup()
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]
    --print('registering Lua Notification')
    GAMEOBJ:GetZoneControlID():SendLuaNotificationRequest{requestTarget=mgr, messageName="ActivityTimerDone"}
    GAMEOBJ:GetZoneControlID():SendLuaNotificationRequest{requestTarget=mgr, messageName="ActivityTimerUpdate"}
end
----------------------------------------------------------------
-- Sets up an activity of the object, only need to put in variable you want to change
----------------------------------------------------------------
function SetupActivity(nMaxUsers) 
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]
    
    -- set max users to something high
    mgr:SetActivityParams{ modifyMaxUsers = true, maxUsers = nMaxUsers, modifyActivityActive = true,  activityActive = true}
    
    GAMEOBJ:GetZoneControlID():SendLuaNotificationRequest{requestTarget=mgr, messageName="DoCalculateActivityRating"}
end


----------------------------------------------------------------
-- Returns true/false if a player is in the activity
-- takes mgr and a PLAYER object
----------------------------------------------------------------
function IsPlayerInActivity(player)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    -- check if player is in activity
    local existMsg = mgr:ActivityUserExists{ userID = player }
    if (existMsg) then
        return existMsg.bExists
    end
    return false

end

----------------------------------------------------------------
-- Updates players for the activity: addPlayer defaults to true
----------------------------------------------------------------
function UpdatePlayer(player, removePlayer)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]
    
    -- Response to Exit activity dialog and user pressed OK
    if removePlayer and IsPlayerInActivity(player) then
        -- remove the user
        mgr:RemoveActivityUser{ userID = player }
    elseif not removePlayer and not IsPlayerInActivity(player) then
        -- add the new user
        mgr:AddActivityUser{ userID = player }
        
        -- start the activity for the new user
        StartActivity(player, 0)
    end

end

----------------------------------------------------------------
-- Stores the start time for the player in the activity and
-- sends messages to start it
----------------------------------------------------------------
function StartActivity(player, scoreVar)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    mgr:SetActivityUserData{ userID = player, typeIndex = 0, value = tonumber(scoreVar) }
end

----------------------------------------------------------------
-- adds the valueVar to the existing value of the index for the 
-- player in the activity 
----------------------------------------------------------------
function UpdateActivityValue(player, valueIndex, valueVar)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]
    local newValue = mgr:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue + valueVar
    --print(newValue)
    mgr:SetActivityUserData{ userID = player, typeIndex = valueIndex, value = newValue }    
end

----------------------------------------------------------------
-- Stores a vaariable for the player in the activity
----------------------------------------------------------------
function SetActivityValue(player, valueIndex, valueVar)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    mgr:SetActivityUserData{ userID = player, typeIndex = valueIndex, value = tonumber(valueVar) }
end

----------------------------------------------------------------
-- Gets the value of the activity index for the player
----------------------------------------------------------------
function GetActivityValue(player, valueIndex)
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    return mgr:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue
end

----------------------------------------------------------------
-- StopActivity message
----------------------------------------------------------------
function StopActivity(player, scoreVar, value1Var, value2Var, quit )
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    --GAMEOBJ:GetZoneControlID():SendLuaNotificationCancel{requestTarget=mgr, messageName="ActivityTimerDone"}
    --GAMEOBJ:GetZoneControlID():SendLuaNotificationCancel{requestTarget=mgr, messageName="ActivityTimerUpdate"}
    
    -- user is trying to cancel
    if quit then
        
        -- remove the user from activity
        mgr:RemoveActivityUser{ userID = player }        
        
    else    
        -- store the time as activity rating [1]
        mgr:SetActivityUserData{ userID = player, typeIndex = 0, value = scoreVar }
        
        if value1Var ~= nil then            
            mgr:SetActivityUserData{ userID = player, typeIndex = 1, value = value1Var }
        elseif value2Var ~= nil then        
            mgr:SetActivityUserData{ userID = player, typeIndex = 2, value = value2Var }
        end

        -- distribute rewards        
        mgr:DistributeActivityRewards{ userID = player, bAutoAddCurrency = true, bAutoAddItems = true }      
        
        -- Update Leaderboards for this user
        mgr:UpdateActivityLeaderboard{ userID = player }

        -- get the leaderboard data for the user and update summary screen if it exists
        mgr:RequestActivitySummaryLeaderboardData{ user = player, queryType = 7 } 
        
        -- remove the user from activity
        mgr:RemoveActivityUser{ userID = player }    
    end

end

----------------------------------------------------------------
-- GetLeaderboard Data message
----------------------------------------------------------------
function GetLeaderboardData( self, player, activityID )
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]

    -- get the leaderboard data for the user and update summary screen if it exists
    mgr:RequestActivitySummaryLeaderboardData{ user = player, target = self, queryType = 7, gameID = activityID } 
end

----------------------------------------------------------------
-- Start activity timer
----------------------------------------------------------------
function ActivityTimerStart(timerName, updateTime, stopTime)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    
    
    
    if stopTime then    
        mgr:ActivityTimerSet{name = timerName, updateInterval = updateTime, duration = stopTime}
    else
        mgr:ActivityTimerSet{name = timerName, updateInterval = updateTime}
    end
end

----------------------------------------------------------------
-- Reset an activity timer
----------------------------------------------------------------
function ActivityTimerReset(timerName)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    
    
    mgr:ActivityTimerReset{name = timerName}
end

----------------------------------------------------------------
-- Stop activity timer
----------------------------------------------------------------
function ActivityTimerStop(timerName)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    
    --print('** stop activity timer ' .. timerName .. ' now **')
    mgr:ActivityTimerStop{name = timerName}
end

----------------------------------------------------------------
-- Stop activity timer
----------------------------------------------------------------
function ActivityTimerStopAllTimers()    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    
        
    mgr:ActivityTimerStopAllTimers()
end

----------------------------------------------------------------
-- Add time to activity timer
----------------------------------------------------------------
function ActivityTimerAddTime(timerName, addTime)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    
        
    mgr:ActivityTimerModify{naem = timerName, timeToAdd = addTime}
end

----------------------------------------------------------------
-- Get remaining activity timer time
----------------------------------------------------------------
function ActivityTimerGetRemainingTime(timerName)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    

    local timerInfo = mgr:ActivityTimerGet{name = timerName}
    
    return timerInfo.timeRemaining
end

----------------------------------------------------------------
-- Get remaining activity timer time
----------------------------------------------------------------
function ActivityTimerGetCurrentTime(timerName)    
    local mgr = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]    

    local timerInfo = mgr:ActivityTimerGet{name = timerName}
    
    return timerInfo.timeElapsed
end

