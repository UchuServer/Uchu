--------------------------------------------------------------
-- GF Survival Instance Server Zone Script: Including this 
-- file lets you set the custom variables for the Survival game.
-- updated mrb... 1/7/10
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('BASE_SURVIVAL_SERVER')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables
local gConstants = 
{
    acceptedDelay = 2,          -- how long to wait after all the players have accepted before starting the game.
    startDelay = 2,             -- how long to wait after all the players have accepted before starting the game.
    waveTime = 5,               -- how often to spawn a new wave of mobs
    rewardInterval = 5,         -- how many waves to wait to drop a reward and give the player a gConstants.coolDownTime
    coolDownTime = 10,          -- how long to wait between waves of gConstants.rewardInterval
    startMobSet2 = 5,           -- wave number to start spawning set 2
    startMobSet3 = 6,          -- wave number to start spawning set 3
    unlockNetwork3 = 10,          -- wave number to unlock the 3rd random spawn network
    bUseMobLots = false,
    iLotPhase = 1,
   
    baseMobsStartTierAt = { 8, 18, 28, },   -- wave number to start spawning tier mobs    
    randMobsStartTierAt = { 5, 10, 20, },   -- wave number to start spawning tier mobs
    
    returnZone = 1300,          -- map number the player will return to on exit
    returnLoc = { x = -431.59, y = 315.01, z = 53.03 } -- {x,y,z} location that the player will be teleported to in the returnZone on exit
 
}
--============================================================

-- Mob Sets
local tMobSets = 
{
    -- these will always spawn *************************************************************************
    -- ** format ** tier# = {{Mob1, Mob2, Mob3},}, 
    baseMobSet = -- tMobSets.baseMobSet
    {
            tier1 = { {3, 0, 1}, },
            tier2 = { {3, 0, 2}, },
            tier3 = { {1, 1, 3}, },
    },    
        
    -- randomly pick from these sets to spawn along with the base set **********************************
    -- ** format ** tier# = {{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3},{Mob1, Mob2, Mob3}}, 
    randMobSet = -- tMobSets.randMobSet
    {
            tier1 = { {1, 0, 0},{2, 0, 0},{3, 0, 0},{0, 1, 0},{1, 1, 0},{0, 0, 1},{1, 0, 1},{0, 1, 1},{1, 1, 1} },
            tier2 = { {1, 0, 1},{2, 0, 1},{3, 0, 1},{0, 1, 1},{1, 1, 1},{0, 0, 2},{1, 0, 2},{0, 1, 2},{1, 1, 2} },
            tier3 = { {1, 1, 2},{2, 1, 2},{3, 1, 2},{0, 2, 2},{1, 2, 2},{0, 1, 3},{1, 1, 3},{0, 2, 3},{1, 2, 3} },   
    },
}   

--============================================================
-- Game messages sent to the BASE_SURVIVAL_SERVER.lua file, these
-- must be in this script. Only change to add custom functionality, 
-- but leav e the base*message*(self, msg, newMsg) in the function.
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
-- Game messages sent to the BASE_SURVIVAL_SERVER.lua file, these
-- must be in this script. Only change to add custom functionality, 
-- but leav e the base*message*(self, msg, newMsg) in the function.
--============================================================

----------------------------------------------------------------
-- Received when the script is loaded
----------------------------------------------------------------
function onStartup(self)
    -- send the configured variables to the base script
    setGameVariables(gConstants,tMobSets,tSpawnerNetworks)
    baseStartup(self, newMsg)
end

----------------------------------------------------------------
-- Player is fully loaded and has completed the load handshake process.
----------------------------------------------------------------
function onPlayerReady(self, msg)
    basePlayerReady(self, msg, newMsg)
end

----------------------------------------------------------------
-- Player has loaded into the map
----------------------------------------------------------------
function onPlayerLoaded(self, msg)
    basePlayerLoaded(self, msg, newMsg)
end

----------------------------------------------------------------
-- Player has exited the map
----------------------------------------------------------------
function onPlayerExit(self, msg)
    basePlayerExit(self, msg, newMsg)
end

----------------------------------------------------------------
-- Received a fire event messaged from the client
----------------------------------------------------------------
function onFireEventServerSide(self, msg)   
    baseFireEventServerSide(self, msg, newMsg)
end

----------------------------------------------------------------
-- Received a fire event messaged from someplace on the server
----------------------------------------------------------------
function onFireEvent(self,msg)   
    baseFireEvent(self, msg, newMsg)
end

----------------------------------------------------------------
-- A player had died
----------------------------------------------------------------
function onPlayerDied(self, msg)
    basePlayerDied(self, msg, newMsg)
end

----------------------------------------------------------------
-- A player has respawned
----------------------------------------------------------------
function onPlayerResurrected(self, msg)
    basePlayerResurrected(self, msg, newMsg)
end

----------------------------------------------------------------
-- Received a notify object message 
----------------------------------------------------------------
function onNotifyObject(self, msg)
    baseNotifyObject(self, msg, newMsg)
end

----------------------------------------------------------------
-- This is called when players hit the UI to exit or stop the game.
----------------------------------------------------------------
function onMessageBoxRespond(self,msg)
    baseMessageBoxRespond(self, msg, newMsg)
end

----------------------------------------------------------------
-- Notification that a ui element used.
----------------------------------------------------------------
function onActivityStateChangeRequest(self,msg)
    baseActivityStateChangeRequest(self, msg, newMsg)
end
