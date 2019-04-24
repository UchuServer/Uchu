local lootLOT = 935     -- LOT of the loot object to spawn
local numToSpawn = 3    -- number of loot objects to spawn
local cooldownTime = 5  -- how long to wait before you can interact with the object again




function onUse(self, msg)

    --print("clicked!")
    
    local player = msg.user
	
    if self:GetVar("bActive") then 
        
        return 
        
    end
    --self:PlayFXEffect{name = "electro_disk", effectType = "create", effectID = 1391}
    
    GAMEOBJ:GetTimer():AddTimerWithCancel( cooldownTime , "InteractionCooldown", self )

    if (player:GetFlag{iFlagID = 66}.bFlag == true) then
    
        msg.user:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
        
        self:SetVar("bActive", true) 

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
        --self:StopFXEffect{name = "electro_disk"}
    end
end 