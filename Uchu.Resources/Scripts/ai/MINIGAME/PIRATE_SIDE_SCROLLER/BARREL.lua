-----------------------------------------------------------------
--tells the fuse platform to light up and start moving towards it's destination
-----------------------------------------------------------------

function onDie(self, msg)


   --telling the fuse to start

   local object = self:GetObjectsInGroup{group = "Fuse", ignoreSpawners = true}.objects[1]
   
   if object then
   
      object:GoToWaypoint{iPathIndex = 16, bAllowPathingDirectionChange = false}
      object:PlayFXEffect{name = "firebrick", effectID = 270, effectType = "create"}
      object:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
   end
   
   local object = self:GetObjectsInGroup{group = "MaelstromWall2", ignoreSpawners = true}.objects[1]
   
   if object then
   
      object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
   end
   
   --initiating all the spawner networks
   
   local spawnerObj = LEVEL:GetSpawnerByName("Final_Apes_1")
   
   if spawnerObj then
   
      spawnerObj:SpawnerActivate()
      spawnerObj:SpawnerReset()
   end
   
   local spawnerObj = LEVEL:GetSpawnerByName("Final_Captains_1")
   
   if spawnerObj then
   
      spawnerObj:SpawnerActivate()
      spawnerObj:SpawnerReset()
   end
   
   local spawnerObj = LEVEL:GetSpawnerByName("Final_Pirates_1")
   
   if spawnerObj then
   
      spawnerObj:SpawnerActivate()
      spawnerObj:SpawnerReset()
   end
end