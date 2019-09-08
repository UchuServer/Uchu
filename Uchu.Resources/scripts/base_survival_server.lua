--------------------------------------------------------------
-- Generic Survival Instance Server Zone Script: requiring this 
-- file gives the custom functions for the Survival game.
-- updated mrb... 12/8/10 - updates for UI bugs
-- updated abeechler ... 4/19/11 - refactored mission update format and checks
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_ACT_GENERIC_ACTIVITY_MGR')

--//////////////////////////////////////////////////////////////////////////////////

local gConstants = {}
local tMobSets = {}
local tSpawnerNetworks = {}
local missionsToUpdate = {}

--============================================================
-- Script only local variables
local gGamestate =
{
    tPlayers = {},          -- players who have entered the game
    tWaitingPlayers = {},   -- players who haven't accepted yet
    iTotalSpawned = 0,      -- total number of spawned mobs
    iWaveNum = 1,           -- current wave number
    iRewardTick = 1,        -- number of rewards given
    iNumberOfPlayers = 0,	-- number of players given from ZoneLoadedInfo
}
--//////////////////////////////////////////////////////////////////////////////////

-- helper function that prints out a variable to the log
function dumpVar(name,var,indent)
	if( indent == nil ) then
		indent = ""
	end
	if( type(var) == "table" ) then
		print( indent .. name .. " is a table with " .. #var .. " entries:" )
		local i,v = next(var)
		while i do
			dumpVar(i,v,indent .. "  ")
			i, v = next(var, i)
		end
	else
		local startOfLine = indent .. name .. " is "
		if( type(var) == "userdata" ) then
			if( type(var.GetID) == "function" ) then
				print( startOfLine .. "an object proxy with ID = " .. var:GetID() )
			else
				print( startOfLine .. "unknown userdata" )
			end
		elseif( var == nil ) then
			print( startOfLine .. "nil" )
	    elseif( var == true ) then
	        print( startOfLine .. "true" )
	    elseif( var == false ) then
	        print( startOfLine .. "false" )
		else
			print( startOfLine .. "a(n) " .. type(var) .. " with value = " .. var )
		end
	end
end

function onZoneLoadedInfo(self, msg)
    self:SetNetworkVar('NumberOfPlayers', msg.maxPlayersSoft)
end

function basePlayerReady(self, msg, newMsg)
    if not self:GetVar('SurvivalStartupComplete') then
        self:SetVar('SurvivalStartupComplete', true)
    end
end

----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------
function baseStartup(self, newMsg)         
    -- Initialize the pseudo random number generator and return 
    math.randomseed( os.time() )
    self:SetVar('playersAccepted', 0)
    self:SetVar('playersReady', false)
    self:MiniGameSetParameters{numTeams = 1, playersPerTeam = 4}
    
--    print('*****************************************************')
--    dumpVar('tSpawnerNetworks', tSpawnerNetworks, ' ')
--    print('*****************************************************')
end

function playerConfirmed(self)
    local playersConfirmed = {}
    
    for k,v in ipairs(gGamestate.tPlayers) do
        local bPass = false
        for key,value in ipairs(gGamestate.tWaitingPlayers) do
            if value == v then
                bPass = true
            end
        end
        
        if not bPass then                        
            table.insert(playersConfirmed, v)
        end
    end
    
    self:SetNetworkVar('PlayerConfirm_ScoreBoard', playersConfirmed)
end

----------------------------------------------------------------
-- Player has loaded into the map
----------------------------------------------------------------
function basePlayerLoaded(self, msg, newMsg)
    -- adding the players to the gGamestate tables
    table.insert(gGamestate.tPlayers, msg.playerID:GetID())    
    table.insert(gGamestate.tWaitingPlayers, msg.playerID:GetID())    
        
    -- adding player to mini game team
    self:MiniGameAddPlayer{playerID = msg.playerID}    
    self:MiniGameSetTeam{playerID = msg.playerID, teamID = 1}
    --print('my team is ' .. self:MiniGameGetTeam{ playerID = msg.playerID}.teamID)
    -- setting up player ui
    self:SetNetworkVar('Define_Player_To_UI', msg.playerID:GetID())
    
    -- freeze the player movement/controls
    if not self:GetNetworkVar('wavesStarted') then
        -- updating the scoreboard for the new players
        
        self:SetNetworkVar('Update_ScoreBoard_Players', gGamestate.tPlayers)
        
        self:SetNetworkVar('Show_ScoreBoard', true)
    end
        
    -- move players to correct spawn locations
    SetPlayerSpawnPoints(self)
    
    msg.playerID:PlayerSetCameraCyclingMode{ cyclingMode = ALLOW_CYCLE_TEAMMATES, bAllowCyclingWhileDeadOnly = true }  
    
    if not self:GetNetworkVar('wavesStarted') then
        playerConfirmed(self) 
    else
        local playerID = msg.playerID
        
        if not playerID then return end
        table.insert(gGamestate.tWaitingPlayers, v)
        UpdatePlayer(self, playerID)        
        --playerID:SetUserCtrlCompPause{bPaused = false}
        GetLeaderboardData(self, playerID, self:GetActivityID().activityID, 50)
        --set player stats to max
        playerID:SetHealth{health = playerID:GetMaxHealth{}.health}
        --print('max health = ' .. playerID:GetMaxHealth{}.health)
        playerID:SetArmor{armor = playerID:GetMaxArmor{}.armor}
        playerID:SetImagination{imagination = playerID:GetMaxImagination{}.imagination}
    end         
end

----------------------------------------------------------------
-- Player has exited the map
----------------------------------------------------------------
function basePlayerExit(self, msg, newMsg)
    local playerNum = 0
    --print('player ' .. msg.playerID:GetName().name .. ' has exited')
        
    for i = 1, table.maxn(gGamestate.tPlayers) do
        if gGamestate.tPlayers[i] == msg.playerID:GetID() then
            playerNum = i
        end
    end
    
    if playerNum ~= 0 then
        table.remove(gGamestate.tPlayers, playerNum)
        -- set player to not auto-respawn
        msg.playerID:SetPlayerAllowedRespawn{dontPromptForRespawn=false}
    end    
                
    playerNum = 0
    
    for k,v in ipairs(gGamestate.tWaitingPlayers) do
        if msg.playerID:GetID() == v then
            playerNum = k
        end
    end
    
    if playerNum ~= 0 then    
        table.remove(gGamestate.tWaitingPlayers, playerNum)
    end
        
    if not self:GetNetworkVar('wavesStarted') then  
        playerConfirmed(self)
        --print('num of players left waiting: ' .. #gGamestate.tWaitingPlayers)
        
        if #gGamestate.tPlayers == 0 then return end
        
        if table.maxn(gGamestate.tWaitingPlayers) == 0 then           
            --print('All players have accepted')        
            ActivityTimerStopAllTimers(self)
            ActivityTimerStart(self, 'AllAcceptedDelay', 1, gConstants.startDelay) --(timerName, updateTime, stopTime)
        elseif #gGamestate.tPlayers > #gGamestate.tWaitingPlayers then
            if not self:GetVar('AcceptedDelayStarted') then
                self:SetVar('AcceptedDelayStarted', true)
                ActivityTimerStart(self, 'AcceptedDelay', 1, gConstants.acceptedDelay ) --(timerName, updateTime, stopTime)
            end
        end        
    else  
        UpdatePlayer(self, msg.playerID, true)
        
        if checkAllPlayersDead() then          
            GameOver(self, msg.playerID)
        end
    end
    
    SetActivityValue(self, msg.playerID, 1, 0)
    local numPlayers = self:GetNetworkVar('NumberOfPlayers')
    
    self:SetNetworkVar('NumberOfPlayers', numPlayers - 1)
end

----------------------------------------------------------------
-- Received a fire event messaged from someplace on the server
----------------------------------------------------------------
function baseFireEvent(self,msg, newMsg)   
    if msg.args == 'start' then
        StartWaves(self)  
    elseif msg.args == 'DeactivateRewards' then
        --print('fireevent DeactivateRewards')
        spawnerResetT(tSpawnerNetworks.rewardNetworks)
    end
end

----------------------------------------------------------------
-- A player had died
----------------------------------------------------------------
function basePlayerDied(self, msg, newMsg)
    -- see if the waves have started, otherwise the player self smashed
    if self:GetNetworkVar('wavesStarted') then     
        local finalTime = ActivityTimerGetCurrentTime(self, 'ClockTick')
        
        SetActivityValue(self, msg.playerID, 1, finalTime)
        self:NotifyClientZoneObject{name = 'Player_Died', paramObj = msg.playerID, rerouteID = msg.playerID, param1 = finalTime, paramStr = tostring(checkAllPlayersDead())} --
        GameOver(self, msg.playerID)
    else
        -- self smash, rez and place
        msg.playerID:Resurrect()
        SetPlayerSpawnPoints(self)
    end
end

----------------------------------------------------------------
-- Received a notify object message 
----------------------------------------------------------------
function baseNotifyObject(self, msg, newMsg)
    local player = msg.ObjIDSender
    
    -- check to make sure the player is in the activity
    if not IsPlayerInActivity(self, player) then return end
    
    -- update kill score
    UpdateActivityValue(self, player, 0, msg.param1)     
end

----------------------------------------------------------------
-- This is called when players hit the UI to exit or stop the game.
----------------------------------------------------------------
function baseMessageBoxRespond(self, msg, newMsg)
    if (msg.identifier == "RePlay" ) then 	
        --print("************* RePlay *************"..msg.sender:GetName().name)		
        PlayerAccepted(self, msg.sender)  
        playerConfirmed(self)
    elseif (msg.identifier == "Exit_Question" ) and msg.iButton == 1 then 		
        --print("************* Exit *************"..msg.sender:GetName().name)
        ResetStats(msg.sender)        
        self:SetNetworkVar('Exit_Waves', msg.sender:GetID())  
        -- send player to a specific location
        msg.sender:TransferToLastNonInstance{ playerID = msg.sender, bUseLastPosition = false, pos_x = 131.83, pos_y = 376, pos_z = -180.31, rot_x = 0, rot_y = -0.268720, rot_z = 0, rot_w = 0.963218}  
    end	
end

-- Custom Functions 
function setGameVariables(passedConstants, passedMobSets, passedSpawnerNetworks, passedMissionsToUpdate)
    --print('updated')
    gConstants = passedConstants
    tMobSets = passedMobSets
    tSpawnerNetworks = passedSpawnerNetworks
    missionsToUpdate = passedMissionsToUpdate
end
----------------------------------------------------------------
-- Custom function: Checks to see if all players have accepted,
-- if they have then the game is started.
----------------------------------------------------------------
function PlayerAccepted(self, playerID)
    local playerNum = 0
    
    for k,v in ipairs(gGamestate.tWaitingPlayers) do
        if playerID:GetID() == v then
            playerNum = k
        end
    end
    
    if playerNum == 0 then return end
    
    table.remove(gGamestate.tWaitingPlayers, playerNum)
    --print('num of players left waiting: ' .. #gGamestate.tWaitingPlayers)
    if table.maxn(gGamestate.tWaitingPlayers) == 0  and #gGamestate.tPlayers >= self:GetNetworkVar('NumberOfPlayers') then           
        --print('All players have accepted')        
        ActivityTimerStopAllTimers(self)
        ActivityTimerStart(self, 'AllAcceptedDelay', 1, gConstants.startDelay) --(timerName, updateTime, stopTime)
    else
        if not self:GetVar('AcceptedDelayStarted') then
            self:SetVar('AcceptedDelayStarted', true)
            ActivityTimerStart(self, 'AcceptedDelay', 1, gConstants.acceptedDelay) --(timerName, updateTime, stopTime)
        end
    end
end

function ResetStats(playerID)
    -- set the player's imag, health and armor to full
    if playerID:Exists() then
        --set player stats to max
        --print('health = ' .. playerID:GetHealth{}.health)
        --print('armor = ' .. playerID:GetArmor{}.armor)
        --print('imagination = ' .. playerID:GetImagination{}.imagination)
        playerID:SetHealth{health = playerID:GetMaxHealth{}.health}
        playerID:SetArmor{armor = playerID:GetMaxArmor{}.armor}
        playerID:SetImagination{imagination = playerID:GetMaxImagination{}.imagination}
        --print('new health = ' .. playerID:GetHealth{}.health)
        --print('new armor = ' .. playerID:GetArmor{}.armor)
        --print('new imagination = ' .. playerID:GetImagination{}.imagination)       
        --print('new imagination = ' .. playerID:GetImagination{}.imagination)    
    end
end

----------------------------------------------------------------
-- Custom function: Starts the game.
----------------------------------------------------------------
function StartWaves(self)    
    SetupActivity(self, 4)  
	self:ActivityStart()
    self:SetVar('playersReady', true)
    self:SetVar('baseMobSetNum', 1)
    self:SetVar('randMobSetNum', 1)
    self:SetVar('AcceptedDelayStarted', false)
    gGamestate.tWaitingPlayers = {}
    
    for k,v in ipairs(gGamestate.tPlayers) do
        local playerID = GAMEOBJ:GetObjectByID(v)   
        
        if not playerID then return end
        
        table.insert(gGamestate.tWaitingPlayers, v)
        UpdatePlayer(self, playerID)        
        GetLeaderboardData(self, playerID, self:GetActivityID().activityID, 50)
        ResetStats(playerID)         
        
        if not self:GetVar('firstTimeDone') then
            --remove the activity cost from the player as they load into the map
            local takeCost = self:ChargeActivityCost{user = playerID}.bSucceeded
            --print('cost taken for: ' .. playerID:GetName().name .. ' = ' .. tostring(takeCost))
        end
    end
    
    self:SetVar('firstTimeDone', true)

    -- needed to get rewards -- taskType = DB name for series of achievments, target = activityID, value1 = what it will evaluate 
    local sTaskType = 'survival_time_team'
    
    if #gGamestate.tPlayers == 1 then
        sTaskType = 'survival_time_solo'
    end
    
    self:SetVar('missionType', sTaskType)
    
    --print('start smashables')
    activateSpawnerNetwork(tSpawnerNetworks.smashNetworks)
    self:SetNetworkVar('wavesStarted', true)
    self:SetNetworkVar('Start_Wave_Message', "Start!")  
end

----------------------------------------------------------------
-- Custom function: Checks to see if all the players are dead,
-- then stops the game.
----------------------------------------------------------------
function checkAllPlayersDead()
    local deadPlayers = 0
    
    for k,v in ipairs(gGamestate.tPlayers) do
        local playerID = GAMEOBJ:GetObjectByID(v)
        
        if not playerID then return end
        
        if playerID:IsDead().bDead then
            deadPlayers = deadPlayers + 1
        end
    end
    
    if deadPlayers == table.maxn(gGamestate.tPlayers) then
        return true
    end
    
    return false
end

function SetPlayerSpawnPoints(self)
    for k,v in ipairs(gGamestate.tPlayers) do           
        local playerID = GAMEOBJ:GetObjectByID(v)
        
        if not playerID then return end
        
        local spawnObj = self:GetObjectsInGroup{ group = 'P' .. k .. '_Spawn', ignoreSpawners = true }.objects[1]
        
        if spawnObj then
            local pos = spawnObj:GetPosition().pos
            local rot = spawnObj:GetRotation()
        
            playerID:Teleport{pos = pos, x = rot.x, y = rot.y, z = rot.z, w = rot.w, bSetRotation = true}       
        end 
    end
end

----------------------------------------------------------------
-- Custom function: Happens when all players have died, this 
-- stops all running processes and resets gGamestate variables
----------------------------------------------------------------
function GameOver(self, player)  
    if not checkAllPlayersDead() then return end
    
    local finalTime = ActivityTimerGetCurrentTime(self, 'ClockTick')
    
    ActivityTimerStopAllTimers(self)
        
    --print('Kill Mobs ***')
    spawnerResetT(tSpawnerNetworks.baseNetworks)
    spawnerResetT(tSpawnerNetworks.randNetworks)
    spawnerResetT(tSpawnerNetworks.rewardNetworks)
        
    for k,v in ipairs(gGamestate.tPlayers) do   
        local playerID = GAMEOBJ:GetObjectByID(v)
        
        if not playerID then return end
        
        local timeVar = GetActivityValue(self, playerID, 1)
        local scoreVar = GetActivityValue(self, playerID, 0)
        
        self:NotifyClientZoneObject{name = 'Update_ScoreBoard', paramObj = playerID, paramStr = tostring(scoreVar), param1 = timeVar}
        
        playerID:Resurrect()
        
        --print('smashed = ' .. smashVar .. ' out of ' .. gGamestate.iTotalSpawned .. ' for ' .. ' score = ' .. scoreVar .. ' @ %' .. math.floor((smashVar/gGamestate.iTotalSpawned)*100) )
       
        local sTaskType = self:GetVar('missionType') or 'survival_time_team'
        
        playerID:UpdateMissionTask{ taskType = sTaskType, value = timeVar, value2 = self:GetActivityID().activityID} --  target = self, 
                                 
        -- Update the missions for the user			
		for missionID,trgtTime in pairs(missionsToUpdate) do
		    -- Determine if we are on the desired mission
		    local missionState = playerID:GetMissionState{missionID = missionID}.missionState
		    
		    -- Are we on the mission?
		    -- Do we satisfy the associated pre-requisite challenge time?
		    if((missionState == 2 or missionState == 10) and (timeVar >= trgtTime)) then
		        -- Update the task
		        playerID:UpdateMissionTask{taskType = "complete", value = missionID, value2 = 1, target = self}
		    end
		end
        
        -- this is to have everyone get their own time at the end of the match
        StopActivity(self, playerID, scoreVar, timeVar)  
        
        --print('***************************')
        --print('send update mission task')
        --print(playerID:GetName().name .. ' ' .. self:GetName().name)
        --print(sTaskType)
        --print(self:GetLOT().objtemplate)
        --print(timeVar)
        --print('***************************')
        
    end
    
    -- this is to have everyone get the same time at the end of the match
    --for k,v in ipairs(gGamestate.tPlayers) do   
    --    local playerID = GAMEOBJ:GetObjectByID(v)
    --    if not playerID then return end
        
    --    StopActivity(self, playerID, scoreVar, finalTime)  
    --end
    
    --print('smashed = ' .. GetActivityValue(self, player, 2) .. ' score = ' .. GetActivityValue(self, player, 1))
        
    -- reset ticks
    gGamestate.iWaveNum = 1                     -- current wave number
    gGamestate.iRewardTick = 1                  -- number of rewards given 
    gGamestate.iTotalSpawned  = 0               -- number of mobs smashed
    self:SetNetworkVar('wavesStarted', false)     
    
    -- set the spawner networks back to the origional LOT's
    if gConstants.bUseMobLots then
        gConstants.iLotPhase = 1                -- put LotPhase back to 1
        updateMobLots(self, tSpawnerNetworks.baseNetworks)         
        updateMobLots(self, tSpawnerNetworks.randNetworks)   
    end        
    
    SetPlayerSpawnPoints(self)
	
end

function basePlayerResurrected(self, msg, newMsg)
    self:SetNetworkVar('Show_ScoreBoard', true)
end

----------------------------------------------------------------
-- Custom function: Gets a random number that is not the old number
----------------------------------------------------------------
function newRand(oldNum, maxRand)
    if maxRand == 1 then
        return '01', '01'
    end
    
    local randNum = math.random(1, maxRand)
    
    if randNum < 10 then
        randNum = '0' .. randNum
    end
        
    while randNum == oldNum do
        --print('found same')
        randNum = math.random(1, maxRand)
        
        if randNum < 10 then
            randNum = '0' .. randNum
        end
    end
    
    return randNum
end

----------------------------------------------------------------
-- Custom function: Starts a spawner network
----------------------------------------------------------------
function activateSpawnerNetwork(spawnNetwork)
    for k,v in ipairs(spawnNetwork) do 
        for i = 1, v.spawnerNum do 
            --print('activateSpawnerNetwork: ' .. v.spawnerName .. '0' .. i)
            local spawner = LEVEL:GetSpawnerByName(v.spawnerName[i] .. v.spawnerNum)
            
            if spawner then
            --print('activateSpawnerNetwork --> ' .. v.spawnerName .. '0' .. i)
                if not spawner:SpawnerIsActive().bActive then
                    --print('activate now')
                    spawner:SpawnerActivate()
                end
                --print('reset now')
                spawner:SpawnerReset()
            end
        end
    end
end

----------------------------------------------------------------
-- Custom function: Resets a spawner network
----------------------------------------------------------------
function spawnerResetT(spawnNetwork, bMaintainSpawnNum, iNumToMaintain)
    local TotalSpawned = 0
    
    --dumpVar('resetTable', spawnNetwork, ' ')   
    for k,v in ipairs(spawnNetwork) do 
        for i = 1, table.maxn(v.spawnerName) do         
            local spawner = LEVEL:GetSpawnerByName(v.spawnerName[i] .. v.spawnerNum)
            
            if spawner then
                --print('reset: ' .. v.spawnerName[i] .. v.spawnerNum)
                local numSpawned = spawner:SpawnerGetTotalSpawned()
                
                if numSpawned then
                    --track total mobs spawned
                    TotalSpawned = TotalSpawned + numSpawned.iSpawned        
                end        
                
                v.bIsActive = false

                if not bMaintainSpawnNum then                    
                    spawner:SpawnerDestroyObjects()
                end
                
                if iNumToMaintain then
                    spawner:SpawnerSetNumToMaintain{uiNum = iNumToMaintain}
                    --print('set spawn number to ' .. iNumToMaintain)
                end
                
                spawner:SpawnerDeactivate()
            end
        end
    end
    
    if TotalSpawned > gGamestate.iTotalSpawned then
       gGamestate.iTotalSpawned = TotalSpawned
    end 
end

----------------------------------------------------------------
-- Custom function: Spawns mobs on a spawner network, Now...
----------------------------------------------------------------
function spawnNow(spawner, spawnNum)
    if spawner then
        --print('*** Spawn Now!!')
        if not spawner:SpawnerIsActive().bActive then
            spawner:SpawnerSetNumToMaintain{uiNum = spawnNum}
            spawner:SpawnerActivate()
        else
            spawner:SpawnerSetNumToMaintain{uiNum = spawnNum}
        end
        
		spawner:SpawnerReset()
    end
end

----------------------------------------------------------------
-- Custom function: Returns a random spawn set from tMobSets or false
----------------------------------------------------------------
function getRandomSet(setName, setNum)
    local randNum = math.random(1, #tMobSets[setName]['tier' .. setNum])
    local randSet = tMobSets[setName]['tier' .. setNum][randNum]
    
    --dumpVar('** ' .. setName .. ' using: ' .. randNum, randSet)
    if randSet then return randSet end
    
    return false
end

----------------------------------------------------------------
-- Custom function: Returns a random spawner number from the given 
-- spawner table or false
----------------------------------------------------------------
function getRandomSpawnerNum(tSpawner)
    
    local randNum = 0
    local bValid = false
    
    while not bValid do
        randNum = 0
        for k,v in ipairs(tSpawner) do
            if v.bIsLocked == false then
                randNum = randNum + 1
            end
        end
            
        randNum = math.random(1, randNum)
        
        if randNum == 1 then
            bValid = true
        elseif not tSpawner[randNum].bIsActive then
            bValid = true
            tSpawner[randNum].bIsActive = true
        end
    end
    if randNum ~= 0 then return randNum end
    
    return false
end

----------------------------------------------------------------
-- Custom function: Update the spawner in the specified way
----------------------------------------------------------------
function updateSpawner(self, tSpawner, spawnNum)
    if not tSpawner then return end  
    
            
    if spawnNum then
        --print('Spawner: ' .. tSpawner.spawnerName[1] .. tSpawner.spawnerNum .. ' spawn: ' .. spawnNum)
        local spawner = LEVEL:GetSpawnerByName(tSpawner.spawnerName[1] .. tSpawner.spawnerNum)
        spawnNow(spawner, spawnNum)
        return
    end
    
    local newSet = getRandomSet(tSpawner.set, self:GetVar(tSpawner.set .. 'Num'))
    
    if newSet then 
        local newSpawner = getRandomSpawnerNum(tSpawner)
        
        for k,v in ipairs(newSet) do 
            if v ~= 0 then
                --print('Spawner: ' .. tSpawner[newSpawner].spawnerName[k] .. tSpawner[newSpawner].spawnerNum .. ' spawn: ' .. tSpawner[newSpawner].spawnerNum)            
                local spawner = LEVEL:GetSpawnerByName(tSpawner[newSpawner].spawnerName[k] .. tSpawner[newSpawner].spawnerNum)            
                
                spawnNow(spawner, v)
            end  
        end
    end
end

----------------------------------------------------------------
-- Custom function: Update the objects that are being spawned on
-- a spawner network based on the LotPhase of the game.
----------------------------------------------------------------
function updateMobLots(self, tSpawner)
    local iPhase = gConstants.iLotPhase
    
    -- search through the given table to find the spanwer network
    for k,v in ipairs(tSpawner) do
        for i,name in ipairs(v.spawnerName) do
            if name ~= nil then
                local spawner = LEVEL:GetSpawnerByName(name .. v.spawnerNum)  
                local tempName = split(name, "_")
                local lotName = tempName[2]
                
                if not lotName then
                    lotName = tempName[1]
                end
                
                if spawner then
                    -- Update the spawn set template ID based on the new LOT
                    spawner:SpawnerSetSpawnTemplateID{iObjTemplate = tMobSets.mobLots[lotName][iPhase]}
                    --print('swapped ' .. name .. v.spawnerNum .. ' for: ' .. tMobSets.mobLots[lotName][iPhase])
                end
            end
        end
    end
end

----------------------------------------------------------------
-- Custom function: splits a string based on patern and returns a table
----------------------------------------------------------------
function split(str, pat)
   local t = {}  -- NOTE: use {n = 0} in Lua-5.0
   local fpat = "(.-)" .. pat
   local last_end = 1
   local s, e, cap = str:find(fpat, 1)
   
   while s do
      if s ~= 1 or cap ~= "" then
	 table.insert(t,cap)
      end
      last_end = e+1
      s, e, cap = str:find(fpat, last_end)
   end
   
   if last_end <= #str then
      cap = str:sub(last_end)
      table.insert(t, cap)
   end
   
   return t
end

----------------------------------------------------------------
-- Custom function: Decides how to spawne mobs
----------------------------------------------------------------
function spawnMobs(self)        
    if not self:GetNetworkVar('wavesStarted') then return end
    
    gGamestate.iWaveNum = gGamestate.iWaveNum + 1 
    
    local spawnNum = gGamestate.iWaveNum
    
    if spawnNum > gConstants.rewardInterval then
        spawnNum = spawnNum - (gGamestate.iRewardTick-1)
    end
          
    for k,v in ipairs(gConstants.baseMobsStartTierAt) do
        if spawnNum == v then        
            --print('************** Base Tier ' .. v .. ' **************')            
            self:SetVar('baseMobSetNum', k)   
        end 
    end
    
    for k,v in ipairs(gConstants.randMobsStartTierAt) do
        if spawnNum == v then        
            self:SetVar('randMobSetNum', k)   
            --print('************** Random Tier ' .. v .. ' **************')
        end 
    end
    
    if gGamestate.iWaveNum == gConstants.unlockNetwork3 then        
        tSpawnerNetworks.randNetworks[3].bIsLocked = false   
    end
    
    spawnerResetT(tSpawnerNetworks.baseNetworks, true, 0)
    spawnerResetT(tSpawnerNetworks.randNetworks, true, 0)
        
    updateSpawner(self, tSpawnerNetworks.baseNetworks)
    --print('**** Spawn Tier' .. self:GetVar('baseMobSetNum') .. ' BaseMobs @ ' .. spawnNum .. '****')
    
    if spawnNum >= gConstants.startMobSet2 then -- gGamestate.iWaveNum >= gConstants.startMobSet2 and 
        if spawnNum == gConstants.startMobSet2 then
            --print('************** Start Random Mobs Set 2 **************')            
            self:SetNetworkVar('Spawn_Mob', "2")
        end
        --print('**** Spawn Tier' .. self:GetVar('randMobSetNum') .. ' RandMobs1 @ ' .. spawnNum .. '****')
        updateSpawner(self, tSpawnerNetworks.randNetworks)
    end
    
    if spawnNum >= gConstants.startMobSet3 then --gGamestate.iWaveNum >= gConstants.startMobSet3 and 
        if spawnNum == gConstants.startMobSet3 then
            --print('************** Start Random Mobs Set 3 **************')
            self:SetNetworkVar('Spawn_Mob', "3")
        end
        --print('**** Spawn Tier' .. self:GetVar('randMobSetNum') .. ' RandMobs2 ' .. spawnNum .. '****')
        updateSpawner(self, tSpawnerNetworks.randNetworks)
    end
    
    -- check if we need to update the spawner network template ID, if we have reached the end of our spawn pattern and we want to use lots
    if gConstants.bUseMobLots and gConstants.iLotPhase < #tSpawnerNetworks.baseNetworks[1].spawnerName then
        if spawnNum >= gConstants.baseMobsStartTierAt[#gConstants.baseMobsStartTierAt] then            
            gConstants.iLotPhase = gConstants.iLotPhase + 1        
            gGamestate.iWaveNum = 1
            updateMobLots(self, tSpawnerNetworks.baseNetworks)         
            updateMobLots(self, tSpawnerNetworks.randNetworks)
            --print('updating to next phase ' .. gConstants.iLotPhase)
        end
    end
end


----------------------------------------------------------------
-- When activity is stopped this is needed to update the leaderboard.
----------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)
    -- get the time for the player    
    --print('Score = ' .. msg.fValue1)
    --print('Time = ' .. msg.fValue2)
    
    msg.outActivityRating = msg.fValue2
    
    return msg
end

-- activity timers 
----------------------------------------------------------------
-- When ActivityTimerUpdate is sent, basically when a timer hits it updateInterval.
----------------------------------------------------------------
function onActivityTimerUpdate(self, msg)
    if msg.name == "AcceptedDelay" then
        --print('update delay timer to ' .. math.ceil(msg.timeRemaining))
        self:SetNetworkVar('Update_Default_Start_Timer', math.ceil(msg.timeRemaining))                      
    elseif msg.name == "ClockTick" then
        self:SetNetworkVar('Update_Timer', msg.timeElapsed)                            
    elseif msg.name == "SpawnTick" and not self:GetVar('isCoolDown') then
        spawnMobs(self)                           
    end    
end

----------------------------------------------------------------
-- When ActivityTimerDone is sent, basically when the activity timer has reached it's duration.
----------------------------------------------------------------
function onActivityTimerDone(self, msg)
    if msg.name == "AcceptedDelay" then --or msg.name == "AllAcceptedDelay"       
        --print('update delay timer to 0')
        self:SetNetworkVar('Update_Default_Start_Timer', 0)                      
        ActivityTimerStart(self, 'AllAcceptedDelay', 1, 1)
    elseif msg.name == "AllAcceptedDelay" then --or msg.name == "AllAcceptedDelay"       
        --print('accepted delay *******************************')         
        self:SetNetworkVar('Clear_Scoreboard', true)                      
        ActivityTimerStart(self, 'StartDelay', 3, 3) --(timerName, updateTime, stopTime)  
        StartWaves(self)    
    elseif msg.name == "StartDelay" then        
        --print('adding in timers *******************************')        
        ActivityTimerStart(self, 'ClockTick', 1) --(timerName, updateTime, stopTime)
        ActivityTimerStart(self, 'SpawnTick', gConstants.waveTime) --(timerName, updateTime, stopTime)        
        spawnMobs(self)
        ActivityTimerStart(self, 'CoolDownStart', (gConstants.rewardInterval*gConstants.waveTime), (gConstants.rewardInterval*gConstants.waveTime)) --(timerName, updateTime, stopTime)       
        ActivityTimerStart(self, 'PlaySpawnSound', 3, 3) --(timerName, updateTime, stopTime)          
    elseif msg.name == "CoolDownStart" then
        --print('cool down start timer *******************************')
        self:SetVar('isCoolDown', true)
        ActivityTimerStop(self, 'SpawnTick')
        ActivityTimerStart(self, 'CoolDownStop', gConstants.coolDownTime, gConstants.coolDownTime)      
        
        --print('**** Reward ****')
        updateSpawner(self, tSpawnerNetworks.rewardNetworks[1], 1)
        gGamestate.iRewardTick = gGamestate.iRewardTick + 1
        
        --print('stopping clock tick')       
        ActivityTimerStart(self, 'CoolDownTick', 1, gConstants.coolDownTime) --(timerName, updateTime, stopTime)         
        
        spawnerResetT(tSpawnerNetworks.baseNetworks, true, 0)
        spawnerResetT(tSpawnerNetworks.randNetworks, true, 0)
    elseif msg.name == "CoolDownStop" then       
        --print('cool down stop timer *******************************')
        self:SetVar('isCoolDown', false)
        ActivityTimerStart(self, 'SpawnTick', gConstants.waveTime) --(timerName, updateTime, stopTime)        
        ActivityTimerStart(self, 'CoolDownStart', (gConstants.rewardInterval*gConstants.waveTime), (gConstants.rewardInterval*gConstants.waveTime)) --(timerName, updateTime, stopTime)        

        spawnMobs(self)         
        ActivityTimerStart(self, 'PlaySpawnSound', 3, 3) --(timerName, updateTime, stopTime)          
    elseif msg.name == "PlaySpawnSound" then
        -- play war horn sound
        for k,v in ipairs(gGamestate.tPlayers) do      
            GAMEOBJ:GetObjectByID(v):PlayNDAudioEmitter{m_NDAudioEventGUID = '{ca36045d-89df-4e96-a317-1e152d226b69}'}             
        end      
    end
end


function baseActivityStateChangeRequest(self, msg, newMsg)
end
