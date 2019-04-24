----------------------------------------------------------------
--script for the sentinel module to spawn a quick build once destroyed
----------------------------------------------------------------


function onDie(self)
   
    local spawnerObj = LEVEL:GetSpawnerByName("BossMod2")
   
    if spawnerObj then
        print("spawning QB module 2!")
        spawnerObj:SpawnerActivate()
        --spawnerObj:SpawnerDeactivate()
    end
   
    local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
   
    if object then
        object:NotifyObject{name = "ModuleTwoSmashDown", ObjIDSender = self}
    end
    
    local object = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects[1]
   
    if object then
        object:NotifyObject{name = "ModuleTwoSmashDown", ObjIDSender = self}
    end
end