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
    spawnerNames = {'temple_three_mob_a', 'temple_three_mob_b','temple_three_mob_c'},         
    -- how many of each spawnerNames to spawn; spawnerNames[2] corresponds to spawnNumbers[random][2]
    spawnNumbers = { {4, 0, 0},{4, 0, 0},{2, 0, 1},{3, 1, 1},{4, 0, 0},{3, 1, 0},{3, 0, 0},{4, 0, 0},{3, 1, 0},{2, 0, 0},{4, 0, 0},{3, 0, 0},{5, 0, 0},{4, 0, 0},{4, 0, 0},{0, 2, 0},{4, 0, 0},{3, 0, 0},{3, 1, 1},{0, 0, 1} },   
    -- this is the size of the proximity sphere
    proxRaidus = 200,    
    -- how long to wait after all players/pets have left the proxRaidus before spawning in new mobs
    waitTime = 1,       
}
    
--============================================================
-- happens when the script is loaded
function onStartup(self)
	print('startup')
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