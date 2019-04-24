local lootLOT = 935     -- LOT of the loot object to spawn
local numToSpawn = 2    -- number of loot objects to spawn
local spawnTime = 1.5  -- how long to wait before spawning another one
local stopLoot = 10	-- length of time the imagination spawns for

function spawnLoot(self)        
    local player = GAMEOBJ:GetObjectByID(self:GetVar('playerID'))
    
    for i = 1, numToSpawn do    
        local newSpawner = GAMEOBJ:GenerateSpawnedID()
        self:DropLoot{owner = player, lootID = newSpawner, itemTemplate = lootLOT, rerouteID = player, sourceObj = self}        
    end
    
    GAMEOBJ:GetTimer():AddTimerWithCancel( spawnTime , "SpawnDelay", self )
end

function onRebuildComplete(self, msg)
    msg.userID:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
    if self:GetVar("bActive") then return end
    
    local playerID = "|" .. msg.userID:GetID()
    
    self:SetVar("playerID", playerID) 
    
    spawnLoot(self)
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( stopLoot , "StopSpawning", self )
	
end

function onTimerDone(self, msg)
    if msg.name == "SpawnDelay" then
        spawnLoot(self)
    elseif msg.name == "StopSpawning" then
		GAMEOBJ:GetTimer():CancelTimer("SpawnDelay", self)
	end
end 