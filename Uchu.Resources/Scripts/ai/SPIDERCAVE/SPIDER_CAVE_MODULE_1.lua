--------------------------------------------------------
--Script on the module to tell the switch that it is built
--------------------------------------------------------

function onStartup(self)
   --print("starting up module 1")
end

function onRebuildNotifyState(self, msg)
   if (msg.iState == 2) then
      --[[local object = self:GetObjectsInGroup{group = "PlatformSwitch1", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
         object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
      end
      local object3 = self:GetObjectsInGroup{group = "PlatformSwitch2", ignoreSpawners = true}.objects[1]
      if object3 then
         object3:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
         object3:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
      end
      local object2 = self:GetObjectsInGroup{group = "Module2", ignoreSpawners = true}.objects[1]
      if object2 then
        --print("playing effect")
         object2:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
      end--]]
      local object = self:GetObjectsInGroup{group = "platform2", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
      end
   end
end