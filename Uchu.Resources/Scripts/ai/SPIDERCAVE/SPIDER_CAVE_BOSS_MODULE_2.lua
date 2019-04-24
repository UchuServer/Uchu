----------------------------------------------------------
--script for one of the spider den modules
----------------------------------------------------------



function onRebuildNotifyState(self, msg)
    
    if msg.iState == 4 then
        
        local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
        
        if object then
            object:NotifyObject{name = "ModuleTwoDown", ObjIDSender = self}
        end
    
    elseif msg.iState == 2 then
        
        local object2 = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
        
        if object2 then
            object2:NotifyObject{name = "ModuleTwoUp", ObjIDSender = self}
        end
    end
end