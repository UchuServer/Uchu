--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_ACTIVITY_MANAGER')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables
local waveTime = 5                             -- how often to spawn a new wave of mobs
local rewardInterval = 5                        -- how many waves to wait to drop a reward and give the player a coolDownTime
local coolDownTime = 10                         -- how long to wait between waves of rewardInterval
local startTier2Waves = 6                       -- wave number to start spawning tier 2 mobs
local startTier3Waves = 12                      -- wave number to start spawning tier 3 mobs
local displayCoolDown = false

-- Spawn Networks 
local spawnerNetworks = {
--**********************************tier1
    {spawnerName = 'hordespawner',        -- name of the tier 1 spawner network without the #'s from HF
    spawnerNum = 2,
    lastRand = 0}
}

-- Reward Networks
local rewardNetworks = {
--********************************Reward1
    {spawnerName = 'Rewards_',              -- name of the reward 1 spawner network without the #'s from HF
    spawnerNum = 2,
    lastRand = 0},} 
--***************************************

-- Script only local variables
local timerTick = 0         -- tick variable
local coolDownTick = 10     -- cool down tick variable
local waveNum = 1           -- current wave number
local rewardTick = 1        -- number of rewards given
local tPlayers = {}
local totalSpawned = 0
--//////////////////////////////////////////////////////////////////////////////////

----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------.
function onStartup(self)     
    -- Initialize the pseudo random number generator and return 
    --self:SetVar('isCoolDown', false)  
    math.randomseed( os.time() )
    self:SetVar('playersAccepted', 0)
    self:SetVar('playersReady', false)
end

function onPlayerLoaded(self, msg)
    table.insert(tPlayers, msg.playerID:GetID())
    msg.playerID:SetUserCtrlCompPause{bPaused = true}
    
    self:NotifyClientZoneObject{name = 'Reset_Timer'}
	self:Help{rerouteID = msg.playerID, iHelpID = 0}
    playerLocationTransfer(self)
end

function onPlayerExit(self, msg)
    local playerNum = 0
    for i = 1, table.maxn(tPlayers) do
        if tPlayers[i] == msg.playerID:GetID() then
            playerNum = i
        end
    end
    
    if playerNum ~= 0 then
        table.remove(tPlayers, playerNum)
    end
end

function onFireEventServerSide(self, msg)   
    if msg.args == 'start' then
        --print('start')
        PlayerAccepted(self)
    elseif msg.args == 'exit' then        
        --print('exit')
        for k,v in ipairs(tPlayers) do
            GAMEOBJ:GetObjectByID(v):TransferToZone{ zoneID = 22, pos_x = 125, pos_y = 376, pos_z = -175  } --, rot_x = 0, rot_y = 0, rot_z = 0, rot_w = 0
        end
    end
end

function onFireEvent(self,msg)   
    if msg.args == 'start' then
        StartWaves(self)  
    elseif msg.args == 'front' and self:GetVar('playersReady') then
        playerLocationTransfer(self)
    elseif msg.args == 'back' then
        playerLocationTransfer(self, true)
    end
end

function onPlayerDied(self, msg)
    --print('player died ***********')
    GameOver(self, msg.playerID)
end

function onNotifyObject(self, msg)
    local player = msg.ObjIDSender
    --if not IsPlayerInActivity(player) then return end
    --print('Scored - ' .. msg.name .. ' - ' .. msg.param1)
    -- If the Quickbuild is done    
    if (msg.name == "built") then
       
        local hordeSpawner = LEVEL:GetSpawnerByName("hordespawner")
		if hordeSpawner then
			hordeSpawner:SpawnerActivate()
		end
    end
    
    if (msg.name == "qbdead") then
        local hordeSpawner = LEVEL:GetSpawnerByName("hordespawner")
		if hordeSpawner then
			hordeSpawner:SpawnerDeactivate()
		end
    end
    
    -- update smash count
    UpdateActivityValue(player, 1, 1)    
    -- update kill score
    UpdateActivityValue(player, 2, msg.param1)     
end

function playerLocationTransfer(self, bToBack)
    local playerCount = self:GetVar('playerCountFront')
    
    if not playerCount then 
        playerCount = 1
        self:SetVar('spawnFrontOnly', true)
        print('firstTime')
        print('# of players: ' .. playerCount)
        self:SetVar('playerCountFront', playerCount)
        return
    end
         
    if bToBack and playerCount > 0 then        
        playerCount = playerCount - 1   
        self:SetVar('playerCountFront', playerCount)
        
        if playerCount == 0 then
            print('reset spawns to back ** spawnBackOnly ' .. playerCount)
            self:SetVar('spawnBackOnly', true)
            self:SetVar('spawnFrontOnly', false)
        end
    elseif not bToBack and playerCount < table.maxn(tPlayers) then
        playerCount = playerCount + 1 
        self:SetVar('playerCountFront', playerCount)
        print('# of players in front' .. playerCount)
        
        if playerCount == table.maxn(tPlayers) and not bToBack then
            print('reset spawns to front ** spawnFrontOnly ' .. playerCount)
            self:SetVar('spawnFrontOnly', true)
            self:SetVar('spawnBackOnly', false)
        end
    end
    
    if playerCount > 0 and playerCount < table.maxn(tPlayers) then
        print('spawnBoth ' .. playerCount)   
        self:SetVar('spawnFrontOnly', false)
        self:SetVar('spawnBackOnly', false)
    else        
        --spawnMobs(self) 
    end
    
    print('# of players in table: ' .. tostring(table.maxn(tPlayers)) .. ' players in front: ' .. playerCount)    
end

function PlayerAccepted(self)
    self:SetVar('playersAccepted', self:GetVar('playersAccepted') + 1)
    
    if self:GetVar('playersAccepted') >= table.maxn(tPlayers) then
        print('All players have accepted')
        StartWaves(self)  
    end
end
function StartWaves(self)    
    SetupActivity(4)    
    self:SetVar('playersReady', true)
    self:SetVar('playersAccepted', 0)
    for k,v in ipairs(tPlayers) do
        UpdatePlayer(GAMEOBJ:GetObjectByID(v))        
        GAMEOBJ:GetObjectByID(v):SetUserCtrlCompPause{bPaused = false}
        GetLeaderboardData(self, GAMEOBJ:GetObjectByID(v), 5)
        -- start the music
        GAMEOBJ:GetObjectByID(v):ActivateNDAudioMusicCue{m_NDAudioMusicCueName = 'AG_Horde'}
        -- set music to cooldown the music
        --GAMEOBJ:GetObjectByID(v):SetNDAudioMusicParameter{m_NDAudioMusicParameterName = 'Intensity', m_Value = 3.0}
    end
    
    --self:NotifyClientZoneObject{name = 'Start_Timer', param1 = timerTick}
    self:SetVar('wavesStarted', true)
    if displayCoolDown then            
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CoolDownTick", self )
    else       
        self:NotifyClientZoneObject{name = 'Start_Wave_Message', paramStr = "Start!"}
        GAMEOBJ:GetTimer():AddTimerWithCancel(3, "StartDelay", self )
    end
end

-- exit
function GameOver(self, player)  
    GAMEOBJ:GetTimer():CancelAllTimers(self) 
    KillMobs(self)
    for k,v in ipairs(tPlayers) do   
        local smashVar = GetActivityValue(GAMEOBJ:GetObjectByID(v), 2)--/totalSpawned
        local scoreVar = GetActivityValue(GAMEOBJ:GetObjectByID(v), 1)
 --       StopActivity(GAMEOBJ:GetObjectByID(v), timerTick, smashVar, scoreVar)
        --print('smashed = ' .. GetActivityValue(GAMEOBJ:GetObjectByID(v), 2) .. ' out of ' .. totalSpawned .. ' for ' .. smashVar .. ' score = ' .. scoreVar)
        -- stop the music
        GAMEOBJ:GetObjectByID(v):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = 'AG_Horde'}
    end
    
    -- reset ticks
    timerTick = 0                   -- tick variable
    coolDownTick = coolDownTime     -- cool down tick variable
    waveNum = 1                     -- current wave number
    rewardTick = 1                  -- number of rewards given    
    self:SetVar('wavesStarted', false)
end

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
    
    return randNum, randNum
end

function spawnerResetT(spawnNetwork, bMaintainSpawnNum)
    for k,v in ipairs(spawnNetwork) do 
        for i = 1, v.spawnerNum do 
            --print('kill: ' .. v.spawnerName .. '0' .. i)
            local spawner = LEVEL:GetSpawnerByName(v.spawnerName .. '0' .. i)
            if spawner then
                --track total mobs spawned
                totalSpawned = totalSpawned + spawner:SpawnerGetTotalSpawned().iSpawned
                
                if not bMaintainSpawnNum then
                    spawner:SpawnerDestroyObjects()
                    spawner:SpawnerSetNumToMaintain{uiNum = 1}
                end
                spawner:SpawnerDeactivate()
            end
        end
    end
end



function KillMobs(self)
    --print('Kill Mobs ***')
    spawnerResetT(spawnerNetworks)
    spawnerResetT(rewardNetworks)
end
--[[
function spawnMobs(self)        
    if not self:GetVar('wavesStarted') then return end
    
    local spawnNum = waveNum
    if spawnNum > rewardInterval then
        spawnNum = spawnNum - (rewardTick-1)
    end
       
    updateSpawner(self, spawnerNetworks[1], spawnNum)
    
    if waveNum >= startTier2Waves and spawnNum - startTier2Waves > 0 then
        updateSpawner(self, spawnerNetworks[2], spawnNum - startTier2Waves)
    end
    if waveNum >= startTier3Waves and spawnNum - startTier3Waves > 0 then
        updateSpawner(self, spawnerNetworks[3], spawnNum - startTier3Waves)
    end
end
--]]
--[[
function waveReward(self)
    --print('Cool Down')
    updateSpawner(self, rewardNetworks[1], 1)
    rewardTick = rewardTick + 1
end
--]]

function onPlayerResurrected(self, msg)
    --print('resurrected')      
    self:NotifyClientZoneObject{name = 'Reset_Timer'}
    
    for i = 1, table.maxn(tPlayers) do
        self:Help{rerouteID = GAMEOBJ:GetObjectByID(tPlayers[i]), iHelpID = 0}
    end
end

-- timers...
function onTimerDone(self, msg)
    if msg.name == "StartDelay" then        
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "ClockTick", self )
    end
    
    if msg.name == "ClockTick" then
        timerTick = timerTick + 1
        --print('Wave: ' .. waveNum .. ' Tick: ' .. timerTick .. ' : ' .. ((waveTime * (waveNum-1)) + (coolDownTime * (rewardTick-1))))
                
        if timerTick >= ((waveTime * (waveNum-1)) + (coolDownTime * (rewardTick-1))) or timerTick == 1 then    
           -- spawnMobs(self)
            --print('spawn number: ' .. waveNum .. ' @ ' .. timerTick)          
            
            if waveNum == rewardInterval * rewardTick then     
                -- set music to cooldown the music
                for k,v in ipairs(tPlayers) do
                    GAMEOBJ:GetObjectByID(v):SetNDAudioMusicParameter{m_NDAudioMusicParameterName = 'Intensity', m_Value = 0.0}
                end               
                
               -- waveReward(self)
                --print('stopping clock tick')
                GAMEOBJ:GetTimer():CancelTimer( "ClockTick", self )                
                GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CoolDownTick", self )
                spawnerResetT(spawnerNetworks, true)     
                --waveNum = waveNum + 1      
                return      
            end
            
            waveNum = waveNum + 1 
        end
        --print('clock tick starting clock tick')
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "ClockTick", self )
        self:NotifyClientZoneObject{name = 'Update_Timer', param1 = timerTick}
    end    
    
    if msg.name == "CoolDownTick" then          
        --print('cooldown ' .. coolDownTick)
        coolDownTick = coolDownTick - 1        
 
        if not displayCoolDown then   
            timerTick = timerTick + 1
            self:NotifyClientZoneObject{name = 'Update_Timer', param1 = timerTick}
        end
        
        if coolDownTick <= 1 then      
            --self:SetVar('isCoolDown', false)  
            --print('cool down starting clock tick')
            GAMEOBJ:GetTimer():AddTimerWithCancel(1, "ClockTick", self )
                        
            coolDownTick = coolDownTime 
            
            local iValue = 1.0
            if rewardTick == 2 then
                iValue = 2.0
            elseif rewardTick > 2 then
                iValue = 3.0
            end
            
            -- set music to cooldown the music
            for k,v in ipairs(tPlayers) do
                GAMEOBJ:GetObjectByID(v):SetNDAudioMusicParameter{m_NDAudioMusicParameterName = 'Intensity', m_Value = iValue}
            end
            
            return
        end
                            
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CoolDownTick", self )
        if displayCoolDown then
            if coolDownTick > coolDownTime/2  and coolDownTick > 0 then
                self:NotifyClientZoneObject{name = 'Wave_Message', paramStr = rewardTick}
            elseif coolDownTick == 0 then        
                self:NotifyClientZoneObject{name = 'Start_Wave_Message', paramStr = "Start!"}
            else
                self:NotifyClientZoneObject{name = 'Update_Timer', param1 = coolDownTick}
            end
        end
    end
end
