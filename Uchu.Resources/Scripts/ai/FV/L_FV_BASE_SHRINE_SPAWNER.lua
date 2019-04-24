----------------------------------------------------------------
-- base logic for shrine spawners; when all players 
-- have left the proximity the waitTime starts, when this is 
-- finished a random spawnNumbers will be picked and spawned
-- updated mrb... 12/15/09
----------------------------------------------------------------

----------------------------------------------------------------
-- called when the template onStartUp happens
----------------------------------------------------------------
function baseOnStartUp(self, gVars)
    -- send the configured variables to the base script    
    math.randomseed( os.time() )
	self:SetProximityRadius { radius = gVars.proxRaidus, name = gVars.spawnerNames[1] }
    spawnMobs(self, gVars)
end

function getPlayerNum(self, tObj)
    local playerNum = 0
    
    for k,v in pairs(tObj) do
        if v:BelongsToFaction{factionID = 1}.bIsInFaction then
            playerNum = playerNum + 1
        end
    end
    
    --print('player num ' .. playerNum)
    return playerNum
end
----------------------------------------------------------------
-- called when the template onProximityUpdate happens
----------------------------------------------------------------
function baseOnProximityUpdate(self, msg, gVars)
	if msg.status == "ENTER" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction then
	    --print('In')
	        GAMEOBJ:GetTimer():CancelAllTimers(self)
	elseif msg.status == "LEAVE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction then
	    --print('Out')
        local playerNum = getPlayerNum(self, self:GetProximityObjects{ name = gVars.spawnerNames[1] }.objects)
	    
	    if playerNum < 1 then
            GAMEOBJ:GetTimer():AddTimerWithCancel( gVars.waitTime, "DealyTimer", self )
		elseif playerNum < 2 then
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "CheckTimer", self )
		end
	end
end 

----------------------------------------------------------------
-- called when the template onTimerDone happens
----------------------------------------------------------------
function baseOnTimerDone(self, msg, gVars)
    if msg.name == "DealyTimer" then   
        --print(#self:GetProximityObjects{ name = gVars.spawnerNames[1] }.objects)
        if getPlayerNum(self, self:GetProximityObjects{ name = gVars.spawnerNames[1] }.objects) < 1 then
            --print('spawnMobsNow')
            spawnMobs(self, gVars)
        else        
            --print('timer again')
	        GAMEOBJ:GetTimer():AddTimerWithCancel( gVars.waitTime, "DealyTimer", self )
        end
    elseif msg.name == "CheckTimer" then   
        --print(#self:GetProximityObjects{ name = gVars.spawnerNames[1] }.objects)
        local playerNum = getPlayerNum(self, self:GetProximityObjects{ name = gVars.spawnerNames[1] }.objects)
        
        if playerNum < 1 then       
            GAMEOBJ:GetTimer():AddTimerWithCancel( gVars.waitTime, "DealyTimer", self )
        elseif playerNum < 2 then       
            --print('timer again')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "CheckTimer", self )
        end
    end
end

----------------------------------------------------------------
-- Custom function: Kills all of the mobs on the spawner networks
----------------------------------------------------------------
function killMobs(spawnNetwork)
    --print('in Kill')
    for k,v in pairs(spawnNetwork.spawnerNames) do         
        local spawner = LEVEL:GetSpawnerByName(v)
        
        if spawner then                 
            --print('kill now')
            spawner:SpawnerDestroyObjects()              
            spawner:SpawnerDeactivate()
        end
    end
end

----------------------------------------------------------------
-- Custom function: Decides how to spawne mobs
----------------------------------------------------------------
function spawnMobs(self, tNetwork)        
    --print('spawnMobs')
    killMobs(tNetwork)
    
    local rand = math.random(1, #tNetwork.spawnNumbers)
    
    for i = 1, #tNetwork.spawnNumbers[rand] do 
        local spawner = LEVEL:GetSpawnerByName(tNetwork.spawnerNames[i])
        
        if spawner then
            spawner:SpawnerSetNumToMaintain{uiNum = tNetwork.spawnNumbers[rand][i]}
            
            if not spawner:SpawnerIsActive().bActive then
                --print('activate now ' .. tNetwork.spawnerNames[i] .. ' number to maintain ' .. tNetwork.spawnNumbers[rand][i] )
                spawner:SpawnerActivate()
            end
            
            --print('reset now')
            spawner:SpawnerReset()
        end
    end
end 