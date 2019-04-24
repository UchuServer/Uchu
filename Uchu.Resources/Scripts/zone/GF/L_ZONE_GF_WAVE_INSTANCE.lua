--------------------------------------------------------------
-- Generic Survival Instance Server Zone Script: Including this 
-- file gives the custom functions for the Survival game.
-- updated mrb... 9/26/09
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('ai/L_ACTIVITY_MANAGER')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables
local gConstants = 
{
    acceptedDelay = 2,          -- how long to wait after all the players have accepted before starting the game.
    waveTime = 5,               -- how often to spawn a new wave of mobs
    rewardInterval = 5,         -- how many waves to wait to drop a reward and give the player a gConstants.coolDownTime
    coolDownTime = 10,          -- how long to wait between waves of gConstants.rewardInterval
    startMobSet2 = 5,           -- wave number to start spawning set 2
    startMobSet3 = 15,          -- wave number to start spawning set 3
    unlockMobSet3 = 5,
    baseMobsStartTier2 = 10,    -- wave number to start spawning tier 2 mobs
    baseMobsStartTier3 = 20,    -- wave number to start spawning tier 3 mobs
    randMobsStartTier2 = 6,     -- wave number to start spawning tier 2 mobs
    randMobsStartTier3 = 12,    -- wave number to start spawning tier 3 mobs
}
--============================================================

-- Mob Sets
local tMobSets = 
{
    -- these will always spawn *************************************************************************
    -- ** format ** tier# = {{Mob1, Mob2, Mob3},}, 
    baseMobSet = -- tMobSets.baseMobSet
    {
            tier1 = { {5, 0, 0}, },
            tier2 = { {0, 5, 0}, },
            tier3 = { {0, 0, 5}, },
    },    
        
    -- randomly pick from these sets to spawn along with the base set **********************************
    -- ** format ** tier# = {{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3}}, 
    randMobSet = -- tMobSets.randMobSet
    {
            tier1 = { {5, 0, 0},{5, 0, 0},{5, 0, 0},{5, 0, 0},{4, 1, 0} },
            tier2 = { {0, 5, 0},{0, 5, 0},{0, 5, 0},{1, 4, 0},{0, 4, 1} },
            tier3 = { {0, 2, 3},{0, 1, 4},{0, 1, 4},{0, 1, 4},{0, 0, 5} },   
    },
}    
--============================================================

-- Spawn Networks 
local tSpawnerNetworks = 
{
    baseNetworks = -- tSpawnerNetworks.baseNetworks
    {   set = 'baseMobSet',
        {
            spawnerName = {'Base_MobA', 'Base_MobB','Base_MobC'}, -- name of base spawner network without from HF
            spawnerNum = '', 
            bIsLocked = false,
            bIsActive = false,
        },
    },
    --***************************************

    randNetworks = -- tSpawnerNetworks.randNetworks
    {   set = 'randMobSet',
        --********************************** Spawner 1
        {   
            spawnerName = {'MobA_','MobB_','MobC_'},        -- name of the MobA spawner networks without the #'s from HF
            spawnerNum = '01',                              -- number of MobB spawner networks  
            bIsLocked = false,
            bIsActive = false,
        },                    
        --********************************** Spawner 2
        {   
            spawnerName = {'MobA_','MobB_','MobC_'},        -- name of the MobA spawner networks without the #'s from HF
            spawnerNum = '02',                              -- number of MobB spawner networks  
            bIsLocked = false,
            bIsActive = false,
        },       
        --********************************** Spawner 3
        {   
            spawnerName = {'MobA_','MobB_','MobC_'},        -- name of the MobA spawner networks without the #'s from HF
            spawnerNum = '03',                              -- number of MobB spawner networks  
            bIsLocked = true,
            bIsActive = false,
        },
    },
    --***************************************
  
    -- Reward Networks
    rewardNetworks = -- tSpawnerNetworks.rewardNetworks
    {
        {
            --******************************** Rewards 1
            spawnerName = {'Rewards_'},             -- name of the reward 1 spawner network without the #'s from HF
            spawnerNum = '01',    
            bIsLocked = false,
        },
    },
    -- Smashable Networks
    smashNetworks = -- tSpawnerNetworks.smashNetworks
    {
        {
            --******************************** Smashables 1
            spawnerName = {'Smash_'},               -- name of the reward 1 spawner network without the #'s from HF
            spawnerNum = '01',    
            bIsLocked = false,
        },
    } ,
}
--============================================================

-- Script only local variables
local gGamestate =
{
    tPlayers = {},          -- players who have entered the game
    tWaitingPlayers = {},   -- players who haven't accepted yet
    iTotalSpawned = 0,      -- total number of spawned mobs
    iWaveNum = 1,           -- current wave number
    iRewardTick = 1,        -- number of rewards given
}
--//////////////////////////////////////////////////////////////////////////////////


-- helper
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

----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------
function onStartup(self)     
    -- Initialize the pseudo random number generator and return 
    math.randomseed( os.time() )
    self:SetVar('playersAccepted', 0)
    self:SetVar('playersReady', false)
    
--    print('*****************************************************')
--    dumpVar('tSpawnerNetworks', tSpawnerNetworks, ' ')
--    print('*****************************************************')
end

----------------------------------------------------------------
-- Player has loaded into the map
----------------------------------------------------------------
function onPlayerLoaded(self, msg)
    -- adding the players to the gGamestate tables
    table.insert(gGamestate.tPlayers, msg.playerID:GetID())    
    table.insert(gGamestate.tWaitingPlayers, msg.playerID:GetID())
    
    -- freeze the player movement/controls
    msg.playerID:SetUserCtrlCompPause{bPaused = true}
    
    -- updating the scoreboard for the new players
    for k,v in ipairs(gGamestate.tPlayers) do
        self:NotifyClientZoneObject{name = 'Update_ScoreBoard', paramObj = GAMEOBJ:GetObjectByID(v), paramStr = "0", param1 = 0, param2 = 0}
    end
    
    self:NotifyClientZoneObject{name = 'Define_Player_To_UI', paramObj = msg.playerID, rerouteID = msg.playerID}    
    self:NotifyClientZoneObject{name = 'Show_ScoreBoard'}
end

----------------------------------------------------------------
-- Player has exited the map
----------------------------------------------------------------
function onPlayerExit(self, msg)
    local playerNum = 0
    
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
end

----------------------------------------------------------------
-- Received a fire event messaged from the client
----------------------------------------------------------------
function onFireEventServerSide(self, msg)   
    if msg.args == 'start' then
        --print('start')
        PlayerAccepted(self)
    elseif msg.args == 'exit' then        
        --print('exit')
        for k,v in ipairs(gGamestate.tPlayers) do
            GAMEOBJ:GetObjectByID(v):TransferToZone{ zoneID = 1100, pos_x = 125, pos_y = 376, pos_z = -175  } --, rot_x = 0, rot_y = 0, rot_z = 0, rot_w = 0
        end
    end
end

----------------------------------------------------------------
-- Received a fire event messaged from someplace on the server
----------------------------------------------------------------
function onFireEvent(self,msg)   
    if msg.args == 'start' then
        StartWaves(self)  
    end
end

----------------------------------------------------------------
-- A player had died
----------------------------------------------------------------
function onPlayerDied(self, msg)
    local finalTime = ActivityTimerGetCurrentTime('ClockTick')
    
    SetActivityValue(msg.playerID, 0, finalTime)
    GameOver(self, msg.playerID)
end

----------------------------------------------------------------
-- Received a notify object message 
----------------------------------------------------------------
function onNotifyObject(self, msg)
    local player = msg.ObjIDSender
    
    -- check to make sure the player is in the activity
    if not IsPlayerInActivity(player) then return end
    
    -- update smash count
    UpdateActivityValue(player, 1, 1)    
    -- update kill score
    UpdateActivityValue(player, 2, msg.param1)     
end

----------------------------------------------------------------
-- This is called when players hit the UI to exit or stop the game.
----------------------------------------------------------------
function onMessageBoxRespond(self,msg)
    if (msg.identifier == "RePlay" ) then 	
        print("************* RePlay *************"..msg.sender:GetName().name)		
        self:NotifyClientZoneObject{name = 'PlayerConfirm_ScoreBoard', paramObj = msg.sender}     
        PlayerAccepted(self, msg.sender)
    elseif (msg.identifier == "Exit" ) then 		
        print("************* Exit *************"..msg.sender:GetName().name)
        self:NotifyClientZoneObject{name = 'Exit_Waves'}
        -- exit level
        msg.sender:TransferToZone{ zoneID = 1100, pos_x = 125, pos_y = 376, pos_z = -175  } --, rot_x = 0, rot_y = 0, rot_z = 0, rot_w = 0
    end	
end

-- Custom Functions 
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
    
    if table.maxn(gGamestate.tWaitingPlayers) == 0 then           
        --print('All players have accepted')        
        ActivityTimerStart('AcceptedDelay', gConstants.acceptedDelay, gConstants.acceptedDelay) --(timerName, updateTime, stopTime)          
    end
end

----------------------------------------------------------------
-- Custom function: Starts the game.
----------------------------------------------------------------
function StartWaves(self)    
    SetupActivity(4)    
    self:SetVar('playersReady', true)
    self:SetVar('baseMobSetNum', 1)
    self:SetVar('randMobSetNum', 1)
    
    for k,v in ipairs(gGamestate.tPlayers) do
        local playerID = GAMEOBJ:GetObjectByID(v)   
        
        if not playerID then return end
        table.insert(gGamestate.tWaitingPlayers, v)
        UpdatePlayer(playerID)        
        playerID:SetUserCtrlCompPause{bPaused = false}
        GetLeaderboardData(self, playerID, 5)        
        -- start the music
        playerID:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = 'AG_Horde'}
        --set player stats to max
        playerID:SetHealth{health = playerID:GetMaxHealth{}.health}
        playerID:SetArmor{armor = playerID:GetMaxArmor{}.armor}
        playerID:SetImagination{imagination = playerID:GetMaxImagination{}.imagination}
    end
    --print('start smashables')
    activateSpawnerNetwork(tSpawnerNetworks.smashNetworks)
    self:SetVar('wavesStarted', true)

    self:NotifyClientZoneObject{name = 'Start_Wave_Message', paramStr = "Start!"}
    ActivityTimerStart('StartDelay', 3, 3) --(timerName, updateTime, stopTime)  
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

----------------------------------------------------------------
-- Custom function: Happens when all players have died, this 
-- stops all running processes and resets gGamestate variables
----------------------------------------------------------------
function GameOver(self, player)  
    if not checkAllPlayersDead() then return end
    
    local finalTime = ActivityTimerGetCurrentTime('ClockTick')
    
    ActivityTimerStop('StartDelay')    
    ActivityTimerStop('CoolDownTick')  
    ActivityTimerStop('ClockTick')
        
    --print('Kill Mobs ***')
    spawnerResetT(tSpawnerNetworks.baseNetworks)
    spawnerResetT(tSpawnerNetworks.randNetworks)
    spawnerResetT(tSpawnerNetworks.rewardNetworks)
    
    for k,v in ipairs(gGamestate.tPlayers) do   
        local playerID = GAMEOBJ:GetObjectByID(v)
        
        if not playerID then return end
        
        local timeVar = GetActivityValue(playerID, 0)
        local smashVar = GetActivityValue(playerID, 1)--/gGamestate.iTotalSpawned
        local scoreVar = GetActivityValue(playerID, 2)
        local respawnPoint = self:GetObjectsInGroup{ group = 'P' .. k .. '_Spawn', ignoreSpawners = true }.objects[1]:GetPosition().pos
        
        self:NotifyClientZoneObject{name = 'Update_ScoreBoard', paramObj = playerID, paramStr = tostring(scoreVar), param1 = timeVar, param2 = smashVar}
        
        playerID:Teleport{pos = respawnPoint}      
        playerID:Resurrect()
        playerID:SetUserCtrlCompPause{bPaused = true}
        
        --print('smashed = ' .. smashVar .. ' out of ' .. gGamestate.iTotalSpawned .. ' for ' .. ' score = ' .. scoreVar)
        -- stop the music
        playerID:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = 'AG_Horde'}
        -- needed to get rewards
        playerID:UpdateMissionTask{ taskType = "performact_time", target = GAMEOBJ:GetZoneControlID():GetObjectsInGroup{ group = 'instance_manager', ignoreSpawners = true }.objects[1]:GetActivityID().activityID, value = 392, value2 = timeVar }
    end
    
    for k,v in ipairs(gGamestate.tPlayers) do   
        local playerID = GAMEOBJ:GetObjectByID(v)
        if not playerID then return end
        
        StopActivity(playerID, finalTime, smashVar, scoreVar)  
    end
    
    --print('smashed = ' .. GetActivityValue(player, 2) .. ' score = ' .. GetActivityValue(player, 1))
    
    self:NotifyClientZoneObject{name = 'Show_ScoreBoard', paramObj = playerID}
    
    -- reset ticks
    gGamestate.iWaveNum = 1                     -- current wave number
    gGamestate.iRewardTick = 1                  -- number of rewards given    
    self:SetVar('wavesStarted', false)    
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
    --dumpVar('resetTable', spawnNetwork, ' ')   
    for k,v in ipairs(spawnNetwork) do 
        for i = 1, table.maxn(v.spawnerName) do         
            local spawner = LEVEL:GetSpawnerByName(v.spawnerName[i] .. v.spawnerNum)
            
            if spawner then               
                --print('reset: ' .. v.spawnerName[i] .. v.spawnerNum)
                --track total mobs spawned
                gGamestate.iTotalSpawned = gGamestate.iTotalSpawned + spawner:SpawnerGetTotalSpawned().iSpawned                
                
                v.bIsActive = false

                if not bMaintainSpawnNum then                    
                    spawner:SpawnerDestroyObjects()
                    spawner:SpawnerSetNumToMaintain{uiNum = numToMaintain}
                end
                
                if iNumToMaintain then
                    spawner:SpawnerSetNumToMaintain{uiNum = iNumToMaintain}
                end
                
                spawner:SpawnerDeactivate()
            end
        end
    end
end

----------------------------------------------------------------
-- Custom function: Spawns mobs on a spawner network, Now...
----------------------------------------------------------------
function spawnNow(spawner, spawnNum)
    --print('* inside spawnNow')
    if spawner then
        --print('*** Spawn Now!!')
        if not spawner:SpawnerIsActive().bActive then
            spawner:SpawnerSetNumToMaintain{uiNum = spawnNum}
            spawner:SpawnerActivate()
        else
            spawner:SpawnerSetNumToMaintain{uiNum = spawnNum}
            spawner:SpawnerReset()
        end
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
-- Custom function: Decides how to spawne mobs
----------------------------------------------------------------
function spawnMobs(self)        
    if not self:GetVar('wavesStarted') then return end
    
    local spawnNum = gGamestate.iWaveNum
    
    if spawnNum > gConstants.rewardInterval then
        spawnNum = spawnNum - (gGamestate.iRewardTick-1)
    end
    
    if gGamestate.iWaveNum == gConstants.baseMobsStartTier2 then        
        self:SetVar('baseMobSetNum', 2)     
    elseif gGamestate.iWaveNum == gConstants.baseMobsStartTier3 then        
        self:SetVar('baseMobSetNum', 3)     
    end 
    
    if gGamestate.iWaveNum == gConstants.randMobsStartTier2 then        
        self:SetVar('randMobSetNum', 2)     
    elseif gGamestate.iWaveNum == gConstants.randMobsStartTier3 then        
        self:SetVar('randMobSetNum', 3)     
    end
    
    if gGamestate.iWaveNum == gConstants.unlockMobSet3 then        
        tSpawnerNetworks.randNetworks[3].bIsLocked = false   
    end
    
    updateSpawner(self, tSpawnerNetworks.baseNetworks)
    --print('**** Spawn Tier' .. self:GetVar('baseMobSetNum') .. ' BaseMobs @ ' .. spawnNum .. '****')
    
    if gGamestate.iWaveNum >= gConstants.startMobSet2 and spawnNum > gConstants.startMobSet2 then
        --print('**** Spawn Tier' .. self:GetVar('randMobSetNum') .. ' RandMobs1 @ ' .. spawnNum .. '****')
        updateSpawner(self, tSpawnerNetworks.randNetworks)
    end
    
    if gGamestate.iWaveNum >= gConstants.startMobSet3 and spawnNum > gConstants.startMobSet3 then
        --print('**** Spawn Tier' .. self:GetVar('randMobSetNum') .. ' RandMobs2 ' .. spawnNum .. '****')
        updateSpawner(self, tSpawnerNetworks.randNetworks)
    end
end

-- Notify messages from Activity Manager
----------------------------------------------------------------
-- notify from activity mng: When activity is stopped this is 
-- needed to update the leaderboard.
----------------------------------------------------------------
function notifyDoCalculateActivityRating(self,other,msg)
    -- get the time for the player    
    print('Time = ' .. msg.fValue1)
    print('Smash = ' .. msg.fValue2)
    print('Score = ' .. msg.fValue3)
    
    msg.outActivityRating = msg.fValue1
    
    self:SendLuaNotificationCancel{requestTarget=cage, messageName="Die"}
    return msg
end

-- activity timers 
----------------------------------------------------------------
-- notify from activity mng: When ActivityTimerUpdate is sent, 
-- basically when a timer hits it updateInterval.
----------------------------------------------------------------
function notifyActivityTimerUpdate(self, other, msg)
    if msg.name == "ClockTick" then
        self:NotifyClientZoneObject{name = 'Update_Timer', param1 = msg.timeElapsed}
        --print('Wave: ' .. gGamestate.iWaveNum .. ' Tick: ' .. msg.timeElapsed} .. ' : ' .. ((gConstants.waveTime * (gGamestate.iWaveNum-1)) + (gConstants.coolDownTime * (gGamestate.iRewardTick-1))))
                
        if msg.timeElapsed >= ((gConstants.waveTime * (gGamestate.iWaveNum-1)) + (gConstants.coolDownTime * (gGamestate.iRewardTick-1))) or msg.timeElapsed == 1 then    
            spawnMobs(self)
            --print('spawn number: ' .. gGamestate.iWaveNum .. ' @ ' .. msg.timeElapsed)          
            
            if gGamestate.iWaveNum == gConstants.rewardInterval * gGamestate.iRewardTick then     
                -- set music to cooldown the music
                for k,v in ipairs(gGamestate.tPlayers) do
                    GAMEOBJ:GetObjectByID(v):SetNDAudioMusicParameter{m_NDAudioMusicParameterName = 'Intensity', m_Value = 0.0}
                end               
                
                --print('**** Reward ****')
                updateSpawner(self, tSpawnerNetworks.rewardNetworks[1], 1)
                gGamestate.iRewardTick = gGamestate.iRewardTick + 1
                
                --print('stopping clock tick')       
                ActivityTimerStart('CoolDownTick', 1, gConstants.coolDownTime) --(timerName, updateTime, stopTime)         
                
                spawnerResetT(tSpawnerNetworks.baseNetworks, true, 0)
                spawnerResetT(tSpawnerNetworks.randNetworks, true, 0)
                
                return      
            end
            
            gGamestate.iWaveNum = gGamestate.iWaveNum + 1 
        end                            
    end    
end

----------------------------------------------------------------
-- notify from activity mng: When ActivityTimerDone is sent, 
-- basically when the activity timer has reached it's duration.
----------------------------------------------------------------
function notifyActivityTimerDone(self, other, msg)
    if msg.name == "AcceptedDelay" then                
        self:NotifyClientZoneObject{name = 'Kill_ScoreBoard'}
        self:NotifyClientZoneObject{name = 'Reset_Timer'}    
        StartWaves(self)    
    elseif msg.name == "StartDelay" then        
        ActivityTimerStart('ClockTick', 1) --(timerName, updateTime, stopTime)
    elseif msg.name == "CoolDownTick" then                                
        local iValue = 1.0
        if gGamestate.iRewardTick == 2 then
            iValue = 2.0
        elseif gGamestate.iRewardTick > 2 then
            iValue = 3.0
        end
        
        -- set music to cooldown the music
        for k,v in ipairs(gGamestate.tPlayers) do
            GAMEOBJ:GetObjectByID(v):SetNDAudioMusicParameter{m_NDAudioMusicParameterName = 'Intensity', m_Value = iValue}
        end
    end
end
