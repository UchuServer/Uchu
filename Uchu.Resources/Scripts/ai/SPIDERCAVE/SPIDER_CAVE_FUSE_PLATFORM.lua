-----------------------------------------------------------------
--script telling the fuse/platform to blow up the barrel when it reaches the final waypoint
-----------------------------------------------------------------

function onArrived(self, msg)
   if msg.wayPoint == 1 then
      local object = self:GetObjectsInGroup{group = "Barrel", ignoreSpawners = true}.objects[1]
      if object then
         --object:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
         object:Die()
      end
      local object2 = self:GetObjectsInGroup{group = "SurBoulder", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:Die()
      end
      local spawnerObj = LEVEL:GetSpawnerByName("SurSpiders")
      if spawnerObj then
         spawnerObj:SpawnerDeactivate()
         spawnerObj:SpawnerDestroyObjects()
      end
      --[[local object3 = self:GetObjectsInGroup{group = "SurRock", ignoreSpawners = true}.objects[1]
      if object3 then
         object3:Die()
      end--]]
      self:StopFXEffect{name = "firebrick"}
      self:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
      self:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
   end
end