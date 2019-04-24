---------------------------------------------------------------------
--spawns x amount of imagination once the gong is hit depending on the items equipped
---------------------------------------------------------------------



local lootLOT = 935     -- LOT of the loot object to spawn
local numToSpawn = 2    -- number of loot objects to spawn
local numToSpawnWithHammer = 12    -- number of loot objects to spawn when the hammer is equipped
local cooldownTime = 5  -- how long to wait before you can interact with the object again




function onOnHit(self, msg)

    --print("hit!")
    
    local player = msg.attacker
    
    self:SetHealth{health = 9999}
	
    if self:GetVar("bActive") then 
        
        return 
        
    end
    
    GAMEOBJ:GetTimer():AddTimerWithCancel( cooldownTime , "InteractionCooldown", self )
        
    self:SetVar("bActive", true) 
    
    ---------------------------------------------------------------------
    --check to see if the player has the hammer equipped
    ---------------------------------------------------------------------
    
    if player:CheckPrecondition{ PreconditionID = 96,iPreconditionType = 0 }.bPass then

        player:UpdateMissionTask{taskType = "complete", value = 847, value2 = 1, target = self}
        
        for i = 1, numToSpawnWithHammer do
            
            --print("spawning powerup for hammer")
            
            local newSpawner = GAMEOBJ:GenerateSpawnedID()
            self:DropLoot{owner = player, lootID = newSpawner, itemTemplate = lootLOT, --[[bUsePosition = true,--]] rerouteID = player, sourceObj = self}       
			self:PlayFXEffect{effectType = "cast" }
            
        end
    
    ---------------------------------------------------------------------
    --spawn a regular amount if they don't have the hammer
    ---------------------------------------------------------------------
    
    else
        
        for i = 1, numToSpawn do
            
            --print("spawning powerup")
            
            local newSpawner = GAMEOBJ:GenerateSpawnedID()
            self:DropLoot{owner = player, lootID = newSpawner, itemTemplate = lootLOT, --[[bUsePosition = true,--]] rerouteID = player, sourceObj = self}       
			self:PlayFXEffect{effectType = "cast" }
            
        end
    end
end




function onTimerDone(self, msg)

    --print("ready to spawn")
    
    if msg.name == "InteractionCooldown" then
        
        self:SetVar("bActive", false)
        
    end
end 