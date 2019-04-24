-----------------------------------------------------------------
--script telling the fuse/platform to blow up the barrel when it reaches the final waypoint
-----------------------------------------------------------------



function onArrived(self, msg)
    
    -----------------------------------------------------------------
    --testing to see if on the last waypoint
    -----------------------------------------------------------------
    
    if msg.wayPoint == 16 then
        
        -----------------------------------------------------------------
        --killing the wall inside the log
        -----------------------------------------------------------------
        
        local object = self:GetObjectsInGroup{group = "MaelstromWall", ignoreSpawners = true}.objects[1]
        
        if object then
        
            --object:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
            object:Die()
        end
        
        
        local object = self:GetObjectsInGroup{group = "MaelstromEye", ignoreSpawners = true}.objects[1]
        
        if object then
        
            object:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
            object:PlayFXEffect{name = "explosion", effectID = 1034, effectType = "cast"}
            object:PlayFXEffect{name = "smoke", effectID = 2856, effectType = "fire"}
        end
        
        -----------------------------------------------------------------
        --playing the cannon fire effect
        -----------------------------------------------------------------
        
        local object = self:GetObjectsInGroup{group = "CannonFire", ignoreSpawners = true}.objects[1]
        
        if object then
        
            object:PlayFXEffect{name = "cannonbig", effectID = 71, effectType = "onfire_large"}
        end
        
        -----------------------------------------------------------------
        --deactivating the spawner networks
        -----------------------------------------------------------------
        
        local spawnerObj = LEVEL:GetSpawnerByName("Final_Apes_1")
        
        if spawnerObj then
        
            spawnerObj:SpawnerDeactivate()
            spawnerObj:SpawnerDestroyObjects{bDieSilent = false}
        end
        
        local spawnerObj = LEVEL:GetSpawnerByName("Final_Captains_1")
        
        if spawnerObj then
        
            spawnerObj:SpawnerDeactivate()
            spawnerObj:SpawnerDestroyObjects{bDieSilent = false}
        end
        
        local spawnerObj = LEVEL:GetSpawnerByName("Final_Pirates_1")
        
        if spawnerObj then
        
            spawnerObj:SpawnerDeactivate()
            spawnerObj:SpawnerDestroyObjects{bDieSilent = false}
        end
        
        self:StopFXEffect{name = "firebrick"}
    end
end