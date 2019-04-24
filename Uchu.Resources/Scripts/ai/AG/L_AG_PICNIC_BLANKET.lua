local lootLOT = 935     -- LOT of the loot object to spawn
local numToSpawn = 3    -- number of loot objects to spawn
local cooldownTime = 5  -- how long to wait before you can interact with the object again

function onUse(self, msg)
    msg.user:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
    if self:GetVar("bActive") then return end
    
    local player = msg.user
    
    self:SetVar("bActive", true) 
    
    for i = 1, numToSpawn do    
        local newSpawner = GAMEOBJ:GenerateSpawnedID()
        self:DropLoot{owner = player, lootID = newSpawner, itemTemplate = lootLOT, rerouteID = player, sourceObj = self}        
    end
    
    GAMEOBJ:GetTimer():AddTimerWithCancel( cooldownTime , "InteractionCooldown", self )
end

function onTimerDone(self, msg)
    if msg.name == "InteractionCooldown" then
        self:SetVar("bActive", false) 
   end
end 