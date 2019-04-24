function onStartup(self)
    self:SetVar('isOn', false)
    self:SetVar('isAlive', true)
    toggleFX(self)
    --self:PlayFXEffect{name = "fanOn", effectID = 495, effectType = "fanOn"}
end

function toggleFX(self, isHit)    
    local groupName = self:GetVar("groupID") or ""
    
    local myGroup = string.gsub(groupName,"%;","")
    local volumeGroup = self:GetObjectsInGroup{ group = myGroup, ignoreSpawners = true  }.objects
    
    if not volumeGroup or not self:GetVar('isAlive') then return end
    
    if self:GetVar('isOn') then
        --print('turnOn fan physics off ')        
        self:PlayAnimation{animationID = 'fan-off', bPlayImmediate = true}
        self:StopFXEffect{name = "fanOn"}
        for i = 1, table.maxn ( volumeGroup ) do
            volumeGroup[i]:SetPhysicsVolumeEffectStatus{bActive = false} 
            self:SetVar('isOn', false)         
            --if not isHit then      
            --    self:GetObjectsInGroup{ group = myGroup .. 'fx', ignoreSpawners = true  }.objects[1]:PlayAnimation{animationID = 'trigger', bPlayImmediate = true}
            --end
        end
    elseif not self:GetVar('isOn') and self:GetVar('isAlive') then      
        --print('turnOn fan physics on ')          
        self:PlayAnimation{animationID = 'fan-on', bPlayImmediate = true}
        --self:PlayFXEffect{name = "fanOn", effectID = 495, effectType = "fanOn"}
        for i = 1, table.maxn ( volumeGroup ) do
            volumeGroup[i]:SetPhysicsVolumeEffectStatus{bActive = true}      
            self:SetVar('isOn', true)     
            --if not isHit then      
            --    self:GetObjectsInGroup{ group = myGroup .. 'fx', ignoreSpawners = true  }.objects[1]:PlayAnimation{animationID = 'idle', bPlayImmediate = true}                        
            --end
        end
    end
end

function onFireEvent( self, msg )
    -- check to make sure there is a message associated with the FireEvent
    --print('Fired Event')
    if not msg.args or not self:GetVar('isAlive') then return end
    local fText = split(msg.args, ',')[1]    
    --print(fText)
    if (fText == 'turnOn' and self:GetVar('isOn')) or (fText == 'turnOff' and not self:GetVar('isOn')) then return end
    toggleFX(self)
end

function onOnHit(self, msg)
    --print('died')    
    if self:GetVar('isOn') then
        toggleFX(self, true)
    end    
    self:SetVar('isAlive', false)
end
