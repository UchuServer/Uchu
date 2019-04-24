--------------------------------------------------------------
-- template for setting up shrine spawners; when all players 
-- have left the proximity the waitTime starts, when this is 
-- finished a random spawnNumbers will be picked and spawned
-- updated mrb... 12/15/09
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('ai/FV/L_FV_BASE_SHRINE_SPAWNER')

--============================================================

-- template specific local variables
local gVars = 
{       
    -- name of spawner networks from HF
    spawnerNames = {'Area_1_Mob_A', 'Area_1_Mob_B','Area_1_Mob_C'},         
    -- how many of each spawnerNames to spawn; spawnerNames[2] corresponds to spawnNumbers[random][2]
    spawnNumbers = { {3, 0, 0},{1, 1, 1},{0, 3, 0},{0, 1, 2},{0, 0, 3} },   
    -- this is the size of the proximity sphere
    proxRaidus = 50,    
    -- how long to wait after all players/pets have left the proxRaidus before spawning in new mobs
    waitTime = 1,       
}
    
--============================================================
-- happens when the script is loaded
function onStartup(self)
    baseOnStartUp(self, gVars)
end

-- happens when the proximity is updated
function onProximityUpdate(self, msg)
    baseOnProximityUpdate(self, msg, gVars)
end 

-- timers...
function onTimerDone(self, msg)
    baseOnTimerDone(self, msg, gVars)
end 