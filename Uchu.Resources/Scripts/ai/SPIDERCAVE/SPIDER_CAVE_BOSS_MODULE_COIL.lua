----------------------------------------------------------------------
--Script for the coil module to power the coil/turret and the switches
----------------------------------------------------------------------




function onRebuildNotifyState(self, msg)
    
    ------------------------------------------------------
    --if the module is destroyed then turn off the effects and disable the switches--
    ------------------------------------------------------
    
    if msg.iState == 4 then
    
        local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
        
        if object then
            object:NotifyObject{name = "ModuleCoilDown", ObjIDSender = self}
        end
    
    elseif msg.iState == 2 then
        
        local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
        
        if object then
            object:NotifyObject{name = "ModuleCoilUp", ObjIDSender = self}
        end
    end
end