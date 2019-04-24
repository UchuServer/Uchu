-----------------------------------------------------------
--script telling the effects to play on the first "cable" in the spider cave
-----------------------------------------------------------

function onStartup(self)
    
    self:SetVar("PipeUp", false)
    self:SetVar("IAmBuilt", false)
end




function onNotifyObject(self, msg)
    
    if msg.name == "PipeFixed" then
        --print("I know the pipe is fixed!")
        self:SetVar("PipeUp", true)
        
        if self:GetVar("IAmBuilt") == true then
        
            self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            
            --[[local object2 = self:GetObjectsInGroup{group = "Teleporter", ignoreSpawners = true}.objects[1]
            
            if object2 then
                
                object2:PlayFXEffect{name = "starkglow", effectID = 935, effectType = "create"}
                object2:PlayFXEffect{name = "blueglowbrick", effectID = 252, effectType = "create"}
                object2:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
            end--]]
            
            local group = self:GetObjectsInGroup{group = "Switch", ignoreSpawners = true}.objects
            
            for i, object in pairs(group) do
            
                if object then
                    
                    object:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
                    object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
                end
            end
            
            --[[local object4 = self:GetObjectsInGroup{group = "teleportSwitch1", ignoreSpawners = true}.objects[1]
            
            if object4 then
                
                object4:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end
            
            local object5 = self:GetObjectsInGroup{group = "teleportSwitch2", ignoreSpawners = true}.objects[1]
            
            if object5 then
                
                object5:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end--]]
        end
    end
end



function onRebuildNotifyState(self, msg)
    
    if msg.iState == 2 then
        
        self:SetVar("IAmBuilt", true)
        
        if self:GetVar("PipeUp") == true then
            
            self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            
            --[[local object2 = self:GetObjectsInGroup{group = "Teleporter", ignoreSpawners = true}.objects[1]
            
            if object2 then
                
                object2:PlayFXEffect{name = "starkglow", effectID = 935, effectType = "create"}
                object2:PlayFXEffect{name = "blueglowbrick", effectID = 252, effectType = "create"}
                object2:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
            end--]]
            
            local group = self:GetObjectsInGroup{group = "Switch", ignoreSpawners = true}.objects
            
            for i, object in pairs(group) do
            
                if object then
                    
                    object:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
                    object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
                end
            end
            
            --[[local object4 = self:GetObjectsInGroup{group = "teleportSwitch1", ignoreSpawners = true}.objects[1]
            
            if object4 then
                
                object4:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end
            
            local object5 = self:GetObjectsInGroup{group = "teleportSwitch2", ignoreSpawners = true}.objects[1]
            
            if object5 then
                
                object5:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end--]]
        end
    end
end