----------------------------------------------------------------
--script for the sentinel module to spawn a quick build once destroyed
----------------------------------------------------------------

function onDie(self)
   
    local spawnerObj = LEVEL:GetSpawnerByName("BossCoilMod")
   
    if spawnerObj then
        print("spawning QB module COIL!")
        spawnerObj:SpawnerActivate()
        --spawnerObj:SpawnerDeactivate()
    end
   
    local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
   
    if object then
        object:NotifyObject{name = "ModuleCoilSmashDown", ObjIDSender = self}
    end
    
    local object = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects[1]
   
    if object then
        object:NotifyObject{name = "ModuleCoilSmashDown", ObjIDSender = self}
    end
end